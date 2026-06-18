// Copyright (c) 2012-2025 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).
//
// High-level JPEG codec glue over libjpeg-turbo 3.x.
//
// This replaces the previous per-precision IJG 6b glue
// (Dicom.Imaging.Codec.Jpeg_8/_12/_16.cpp) and its three separately compiled
// libijg8/12/16 libraries. libjpeg-turbo supports run-time selection of data
// precision (8/12/16 bits) from a single library, including the lossless
// process required by the DICOM JPEG Lossless transfer syntaxes (.57/.70).
//
// The native side now owns all of the per-field codec orchestration that used
// to live in managed code, and exposes a small, stable buffer-in / buffer-out
// ABI so the managed layer no longer needs to mirror libjpeg's internal
// structures.

#include <cstring>
#include <csetjmp>
#include <cstdlib>

extern "C"
{
// jpeglib.h references FILE (jpeg_stdio_src/dest) and therefore requires
// <stdio.h> to have been included beforehand.
#include <stdio.h>
#include "./Common/libjpeg-turbo/jpeglib.h"
#include "./Common/libjpeg-turbo/jerror.h"
}

#if defined(_WIN32)
#define EXPORT_Jpeg __declspec(dllexport)
#elif defined(__linux__)
#define EXPORT_Jpeg extern
#elif defined(__APPLE__)
#include "TargetConditionals.h"
#ifdef TARGET_OS_MAC
#define EXPORT_Jpeg extern
#endif
#endif

namespace Dicom
{
namespace Imaging
{
namespace Codec
{

#ifdef __cplusplus
extern "C"
{
#endif

    // Keep these enum values in sync with the managed DicomJpegCodec.
    // Mode values mirror the managed JpegMode enum.
    enum DicomJpegModeNative
    {
        DJ_MODE_BASELINE = 0,
        DJ_MODE_SEQUENTIAL = 1,
        DJ_MODE_SPECTRAL_SELECTION = 2,
        DJ_MODE_PROGRESSIVE = 3,
        DJ_MODE_LOSSLESS = 4
    };

    // Sample factor values mirror the managed DicomJpegSampleFactor enum.
    enum DicomJpegSampleFactorNative
    {
        DJ_SF_444 = 0,
        DJ_SF_422 = 1,
        DJ_SF_UNKNOWN = 2
    };

    // setjmp-based error handling so a libjpeg ERREXIT does not abort the
    // process. The error message is copied out for the managed layer.
    struct dicom_jpeg_error_mgr
    {
        struct jpeg_error_mgr pub;
        jmp_buf setjmp_buffer;
        char *message_out;
        size_t message_out_size;
    };

    static void dicom_jpeg_error_exit(j_common_ptr cinfo)
    {
        dicom_jpeg_error_mgr *err = reinterpret_cast<dicom_jpeg_error_mgr *>(cinfo->err);
        if (err->message_out != nullptr && err->message_out_size > 0)
        {
            char buffer[JMSG_LENGTH_MAX];
            (*cinfo->err->format_message)(cinfo, buffer);
            std::strncpy(err->message_out, buffer, err->message_out_size - 1);
            err->message_out[err->message_out_size - 1] = '\0';
        }
        longjmp(err->setjmp_buffer, 1);
    }

    // Suppress libjpeg's default stderr emission of warnings/trace messages.
    static void dicom_jpeg_emit_message(j_common_ptr /*cinfo*/, int /*msg_level*/)
    {
    }

    static void dicom_jpeg_output_message(j_common_ptr /*cinfo*/)
    {
    }

    //=========================================================================
    // Encode
    //
    // Returns 0 on success, non-zero on failure. On success *out_buffer points
    // to a libjpeg-allocated buffer of *out_size bytes that the caller must
    // release with DicomJpegFreeBuffer.
    //=========================================================================
    EXPORT_Jpeg int DicomJpegEncode(
        const unsigned char *pixelData,
        unsigned int width,
        unsigned int height,
        int inputComponents,
        int inColorSpace,
        int mode,
        int dataPrecision,
        int quality,
        int smoothingFactor,
        int predictor,
        int pointTransform,
        int sampleFactor,
        unsigned int rowStride,
        unsigned char **out_buffer,
        unsigned int *out_size,
        int *out_jpegColorSpace,
        char *errorMessage,
        unsigned int errorMessageSize)
    {
        struct jpeg_compress_struct cinfo;
        dicom_jpeg_error_mgr jerr;
        unsigned char *encoded = nullptr;
        unsigned long encodedSize = 0;

        std::memset(&cinfo, 0, sizeof(cinfo));

        cinfo.err = jpeg_std_error(&jerr.pub);
        jerr.pub.error_exit = dicom_jpeg_error_exit;
        jerr.pub.emit_message = dicom_jpeg_emit_message;
        jerr.pub.output_message = dicom_jpeg_output_message;
        jerr.message_out = errorMessage;
        jerr.message_out_size = errorMessageSize;

        if (setjmp(jerr.setjmp_buffer))
        {
            jpeg_destroy_compress(&cinfo);
            if (encoded != nullptr)
                free(encoded);
            return 1;
        }

        jpeg_create_compress(&cinfo);
        jpeg_mem_dest(&cinfo, &encoded, &encodedSize);

        cinfo.image_width = width;
        cinfo.image_height = height;
        cinfo.input_components = inputComponents;
        cinfo.in_color_space = static_cast<J_COLOR_SPACE>(inColorSpace);

        // data_precision must be set before jpeg_set_defaults: the defaults
        // (quant tables, etc.) are computed from it.
        cinfo.data_precision = dataPrecision;

        jpeg_set_defaults(&cinfo);

        cinfo.optimize_coding = TRUE;

        if (mode == DJ_MODE_LOSSLESS)
        {
            jpeg_enable_lossless(&cinfo, predictor, pointTransform);
        }
        else if (mode == DJ_MODE_PROGRESSIVE)
        {
            jpeg_set_quality(&cinfo, quality, FALSE);
            jpeg_simple_progression(&cinfo);
        }
        else
        {
            // Baseline / Sequential / (SpectralSelection treated as sequential).
            jpeg_set_quality(&cinfo, quality, FALSE);
        }

        cinfo.smoothing_factor = smoothingFactor;

        // Sampling factors.
        if (mode == DJ_MODE_LOSSLESS)
        {
            // A lossless process must not apply a lossy RGB->YCbCr conversion.
            // jpeg_set_defaults defaults RGB input to a YCbCr JPEG colorspace;
            // force the JPEG colorspace back to the input colorspace so libjpeg
            // does not error with JERR_CONVERSION_NOTIMPL (and so the stored
            // samples are truly lossless).
            jpeg_set_colorspace(&cinfo, cinfo.in_color_space);
            for (int i = 0; i < cinfo.num_components; i++)
            {
                cinfo.comp_info[i].h_samp_factor = 1;
                cinfo.comp_info[i].v_samp_factor = 1;
            }
        }
        else if (cinfo.jpeg_color_space == JCS_YCbCr && sampleFactor != DJ_SF_UNKNOWN)
        {
            // Component 0 (luma) carries the subsampling; chroma stays 1x1.
            if (sampleFactor == DJ_SF_422)
            {
                cinfo.comp_info[0].h_samp_factor = 2;
                cinfo.comp_info[0].v_samp_factor = 1;
            }
            else // DJ_SF_444
            {
                cinfo.comp_info[0].h_samp_factor = 1;
                cinfo.comp_info[0].v_samp_factor = 1;
            }
            for (int i = 1; i < cinfo.num_components; i++)
            {
                cinfo.comp_info[i].h_samp_factor = 1;
                cinfo.comp_info[i].v_samp_factor = 1;
            }
        }
        else
        {
            if (sampleFactor == DJ_SF_UNKNOWN)
                jpeg_set_colorspace(&cinfo, cinfo.in_color_space);
            for (int i = 0; i < cinfo.num_components; i++)
            {
                cinfo.comp_info[i].h_samp_factor = 1;
                cinfo.comp_info[i].v_samp_factor = 1;
            }
        }

        jpeg_start_compress(&cinfo, TRUE);

        // Feed scanlines, dispatching on precision.
        while (cinfo.next_scanline < cinfo.image_height)
        {
            const unsigned char *row = pixelData + (size_t)cinfo.next_scanline * rowStride;
            if (dataPrecision <= 8)
            {
                JSAMPROW rowPtr = const_cast<JSAMPROW>(reinterpret_cast<const JSAMPLE *>(row));
                jpeg_write_scanlines(&cinfo, &rowPtr, 1);
            }
            else if (dataPrecision <= 12)
            {
                J12SAMPROW rowPtr = const_cast<J12SAMPROW>(reinterpret_cast<const J12SAMPLE *>(row));
                jpeg12_write_scanlines(&cinfo, &rowPtr, 1);
            }
            else
            {
                J16SAMPROW rowPtr = const_cast<J16SAMPROW>(reinterpret_cast<const J16SAMPLE *>(row));
                jpeg16_write_scanlines(&cinfo, &rowPtr, 1);
            }
        }

        jpeg_finish_compress(&cinfo);

        // Report the JPEG colorspace actually selected so the managed layer can
        // set the resulting photometric interpretation (e.g. RGB input encoded
        // as YCbCr for a lossy process).
        *out_jpegColorSpace = static_cast<int>(cinfo.jpeg_color_space);
        *out_buffer = encoded;
        *out_size = static_cast<unsigned int>(encodedSize);

        jpeg_destroy_compress(&cinfo);
        return 0;
    }

    //=========================================================================
    // Decode
    //
    // Returns 0 on success, non-zero on failure. On success *out_pixels points
    // to a malloc-allocated buffer of *out_size bytes that the caller must
    // release with DicomJpegFreeBuffer.
    //=========================================================================
    EXPORT_Jpeg int DicomJpegDecode(
        const unsigned char *jpegData,
        unsigned int jpegSize,
        int convertColorSpaceToRGB,
        int isSigned,
        unsigned char **out_pixels,
        unsigned int *out_size,
        unsigned int *out_width,
        unsigned int *out_height,
        int *out_components,
        int *out_precision,
        int *out_colorSpace,
        unsigned int *out_rowSize,
        char *errorMessage,
        unsigned int errorMessageSize)
    {
        struct jpeg_decompress_struct dinfo;
        dicom_jpeg_error_mgr jerr;
        unsigned char *pixels = nullptr;

        std::memset(&dinfo, 0, sizeof(dinfo));

        dinfo.err = jpeg_std_error(&jerr.pub);
        jerr.pub.error_exit = dicom_jpeg_error_exit;
        jerr.pub.emit_message = dicom_jpeg_emit_message;
        jerr.pub.output_message = dicom_jpeg_output_message;
        jerr.message_out = errorMessage;
        jerr.message_out_size = errorMessageSize;

        if (setjmp(jerr.setjmp_buffer))
        {
            jpeg_destroy_decompress(&dinfo);
            if (pixels != nullptr)
                free(pixels);
            return 1;
        }

        jpeg_create_decompress(&dinfo);
        jpeg_mem_src(&dinfo, jpegData, jpegSize);

        jpeg_read_header(&dinfo, TRUE);

        const bool willConvert = convertColorSpaceToRGB &&
            (dinfo.jpeg_color_space == JCS_YCbCr || dinfo.jpeg_color_space == JCS_RGB);

        if (willConvert)
        {
            // Reject colorspace conversion of signed pixel data (matches the
            // managed contract; signed YCbCr/RGB conversion is not defined).
            if (isSigned)
            {
                if (errorMessage != nullptr && errorMessageSize > 0)
                {
                    std::strncpy(errorMessage,
                        "JPEG codec unable to perform colorspace conversion on signed pixel data",
                        errorMessageSize - 1);
                    errorMessage[errorMessageSize - 1] = '\0';
                }
                jpeg_destroy_decompress(&dinfo);
                return 5;
            }
            dinfo.out_color_space = JCS_RGB;
        }
        else
        {
            // Pass component data through unchanged. libjpeg's deconverter only
            // permits a null conversion when out_color_space == jpeg_color_space,
            // so both must be set to JCS_UNKNOWN (otherwise grayscale and other
            // non-converted images error with JERR_CONVERSION_NOTIMPL).
            dinfo.jpeg_color_space = JCS_UNKNOWN;
            dinfo.out_color_space = JCS_UNKNOWN;
        }

        jpeg_calc_output_dimensions(&dinfo);
        jpeg_start_decompress(&dinfo);

        const int precision = dinfo.data_precision;
        const int bytesPerSample = (precision + 7) / 8;
        const size_t rowSize = (size_t)dinfo.output_width * dinfo.output_components * bytesPerSample;
        const size_t frameSize = rowSize * dinfo.output_height;

        // Guard against a crafted codestream whose decoded frame size exceeds
        // what the caller can represent / allocate (Int32). Computed with size_t
        // (64-bit on all targets) from the codestream's own dimensions and data
        // precision, so it cannot silently wrap and produce an undersized buffer
        // that jpeg_read_scanlines would then overrun.
        if (frameSize > 0x7FFFFFFFu)
        {
            if (errorMessage != nullptr && errorMessageSize > 0)
            {
                std::strncpy(errorMessage,
                    "Decoded JPEG frame size exceeds the supported maximum",
                    errorMessageSize - 1);
                errorMessage[errorMessageSize - 1] = '\0';
            }
            jpeg_destroy_decompress(&dinfo);
            return 4;
        }

        pixels = static_cast<unsigned char *>(malloc(frameSize));
        if (pixels == nullptr)
        {
            if (errorMessage != nullptr && errorMessageSize > 0)
            {
                std::strncpy(errorMessage, "Out of memory allocating decode buffer", errorMessageSize - 1);
                errorMessage[errorMessageSize - 1] = '\0';
            }
            jpeg_destroy_decompress(&dinfo);
            return 2;
        }

        while (dinfo.output_scanline < dinfo.output_height)
        {
            unsigned char *row = pixels + (size_t)dinfo.output_scanline * rowSize;
            JDIMENSION read = 0;
            if (precision <= 8)
            {
                JSAMPROW rowPtr = reinterpret_cast<JSAMPLE *>(row);
                read = jpeg_read_scanlines(&dinfo, &rowPtr, 1);
            }
            else if (precision <= 12)
            {
                J12SAMPROW rowPtr = reinterpret_cast<J12SAMPLE *>(row);
                read = jpeg12_read_scanlines(&dinfo, &rowPtr, 1);
            }
            else
            {
                J16SAMPROW rowPtr = reinterpret_cast<J16SAMPLE *>(row);
                read = jpeg16_read_scanlines(&dinfo, &rowPtr, 1);
            }
            if (read == 0)
            {
                if (errorMessage != nullptr && errorMessageSize > 0)
                {
                    std::strncpy(errorMessage, "jpeg_read_scanlines returned 0 (suspended)", errorMessageSize - 1);
                    errorMessage[errorMessageSize - 1] = '\0';
                }
                free(pixels);
                jpeg_destroy_decompress(&dinfo);
                return 3;
            }
        }

        *out_pixels = pixels;
        *out_size = static_cast<unsigned int>(frameSize);
        *out_width = dinfo.output_width;
        *out_height = dinfo.output_height;
        *out_components = dinfo.output_components;
        *out_precision = precision;
        *out_colorSpace = static_cast<int>(dinfo.out_color_space);
        *out_rowSize = static_cast<unsigned int>(rowSize);

        jpeg_finish_decompress(&dinfo);
        jpeg_destroy_decompress(&dinfo);
        return 0;
    }

    //=========================================================================
    // Read the data precision (bits per sample) from a JPEG SOF marker without
    // performing a full decode. Returns 0 on success.
    //=========================================================================
    EXPORT_Jpeg int DicomJpegReadPrecision(
        const unsigned char *jpegData,
        unsigned int jpegSize,
        int *out_precision,
        char *errorMessage,
        unsigned int errorMessageSize)
    {
        struct jpeg_decompress_struct dinfo;
        dicom_jpeg_error_mgr jerr;

        std::memset(&dinfo, 0, sizeof(dinfo));

        dinfo.err = jpeg_std_error(&jerr.pub);
        jerr.pub.error_exit = dicom_jpeg_error_exit;
        jerr.pub.emit_message = dicom_jpeg_emit_message;
        jerr.pub.output_message = dicom_jpeg_output_message;
        jerr.message_out = errorMessage;
        jerr.message_out_size = errorMessageSize;

        if (setjmp(jerr.setjmp_buffer))
        {
            jpeg_destroy_decompress(&dinfo);
            return 1;
        }

        jpeg_create_decompress(&dinfo);
        jpeg_mem_src(&dinfo, jpegData, jpegSize);
        jpeg_read_header(&dinfo, TRUE);

        *out_precision = dinfo.data_precision;

        jpeg_destroy_decompress(&dinfo);
        return 0;
    }

    //=========================================================================
    // Release a buffer returned by DicomJpegEncode / DicomJpegDecode.
    //=========================================================================
    EXPORT_Jpeg void DicomJpegFreeBuffer(unsigned char *buffer)
    {
        if (buffer != nullptr)
            free(buffer);
    }

#ifdef __cplusplus
}
#endif

} // namespace Codec
} // namespace Imaging
} // namespace Dicom
