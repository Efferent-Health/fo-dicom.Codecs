// Copyright (c) 2012-2026 fo-dicom contributors.
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
#include <stdint.h>
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

                typedef struct
                {
                    struct jpeg_destination_mgr pub; /* public fields */

                    FILE *outfile;  /* target stream */
                    JOCTET *buffer; /* start of buffer */
                } my_destination_mgr;

                typedef my_destination_mgr *my_dest_ptr;

                METHODDEF(void)
                init_destination(j_compress_ptr cinfo)
                {
                    my_dest_ptr dest = (my_dest_ptr)cinfo->dest;

                    /* Allocate the output buffer --- it will be released when done with image */
                    dest->buffer = (JOCTET *)(*cinfo->mem->alloc_small)((j_common_ptr)cinfo, JPOOL_IMAGE,
                                                                        4096 * sizeof(JOCTET));

                    dest->pub.next_output_byte = dest->buffer;
                    dest->pub.free_in_buffer = 4096;
                }

                METHODDEF(void)
                init_mem_destination(j_compress_ptr cinfo)
                {
                    /* no work necessary here */
                }

                typedef struct
                {
                    struct jpeg_destination_mgr pub; /* public fields */

                    unsigned char **outbuffer; /* target buffer */
                    unsigned long *outsize;
                    unsigned char *newbuffer; /* newly allocated buffer */
                    JOCTET *buffer;           /* start of buffer */
                    size_t bufsize;
                } my_mem_destination_mgr;

                typedef my_mem_destination_mgr *my_mem_dest_ptr;

                
                boolean empty_mem_output_buffer(j_compress_ptr cinfo)
                {
                    size_t nextsize;
                    JOCTET *nextbuffer;
                    my_mem_dest_ptr dest = (my_mem_dest_ptr)cinfo->dest;

                    /* Try to allocate new buffer with double size
                     *
                     * NOTE: The following check isn't actually necessary.  On 64-bit systems,
                     * the maximum theoretical JPEG size is
                     * 65500 * 65500 * cinfo->num_components * sizeof(DCTELEM) bytes, which is of
                     * course much less than 8 exabytes (SIZE_MAX / 2).  On 32-bit systems,
                     * malloc() will never return a buffer >= 2 GB, so the malloc() call will
                     * fail before 32-bit integer overflow/wraparound can occur.  The sole
                     * purpose of this code is to shut up automated code analysis tools.
                     */
                    if (dest->bufsize > SIZE_MAX / 2)
                        ERREXIT1(cinfo, JERR_OUT_OF_MEMORY, 13);
                    nextsize = dest->bufsize * 2;
                    nextbuffer = (JOCTET *)malloc(nextsize);

                    if (nextbuffer == NULL)
                        ERREXIT1(cinfo, JERR_OUT_OF_MEMORY, 12);

                    memcpy(nextbuffer, dest->buffer, dest->bufsize);

                    free(dest->newbuffer);

                    dest->newbuffer = nextbuffer;

                    dest->pub.next_output_byte = nextbuffer + dest->bufsize;
                    dest->pub.free_in_buffer = dest->bufsize;

                    dest->buffer = nextbuffer;
                    dest->bufsize = nextsize;

                    return TRUE;
                }


                void term_mem_destination(j_compress_ptr cinfo)
                {
                    my_mem_dest_ptr dest = (my_mem_dest_ptr)cinfo->dest;

                    *dest->outbuffer = dest->buffer;
                    *dest->outsize = (unsigned long)(dest->bufsize - dest->pub.free_in_buffer);
                }

                void jpeg_memory_destination(j_compress_ptr cinfo, unsigned char **outbuffer,
                                    unsigned long *outsize)
                {
                    my_mem_dest_ptr dest;

                    if (outbuffer == NULL || outsize == NULL) /* sanity check */
                        ERREXIT(cinfo, JERR_BUFFER_SIZE);

                    /* The destination object is made permanent so that multiple JPEG images
                     * can be written to the same buffer without re-executing jpeg_mem_dest.
                     */
                    if (cinfo->dest == NULL)
                    { /* first time for this JPEG object? */
                        cinfo->dest = (struct jpeg_destination_mgr *)(*cinfo->mem->alloc_small)((j_common_ptr)cinfo, JPOOL_PERMANENT,
                                                                                                sizeof(my_mem_destination_mgr));
                    }
                    else if (cinfo->dest->init_destination != init_mem_destination)
                    {
                        /* It is unsafe to reuse the existing destination manager unless it was
                         * created by this function.
                         */
                        ERREXIT(cinfo, JERR_BUFFER_SIZE);
                    }

                    dest = (my_mem_dest_ptr)cinfo->dest;
                    dest->pub.init_destination = init_mem_destination;
                    dest->pub.empty_output_buffer = empty_mem_output_buffer;
                    dest->pub.term_destination = term_mem_destination;
                    dest->outbuffer = outbuffer;
                    dest->outsize = outsize;
                    dest->newbuffer = NULL;

                    if (*outbuffer == NULL && *outsize == 0)
                    {
                        /* Allocate initial buffer */
                        dest->newbuffer = *outbuffer = (unsigned char *)malloc(4096);
                        if (dest->newbuffer == NULL)
                            ERREXIT1(cinfo, JERR_OUT_OF_MEMORY, 10);
                        *outsize = 4096;
                    }
                    else if (*outbuffer != NULL && *outsize == 0)
                    {
                        /* Reference initial buffer */
                        dest->newbuffer = *outbuffer;
                        if (dest->newbuffer == NULL)
                            ERREXIT1(cinfo, JERR_OUT_OF_MEMORY, 10);
                    }

                    dest->pub.next_output_byte = dest->buffer = *outbuffer;
                    dest->pub.free_in_buffer = dest->bufsize = *outsize;
                }

                // Encode
                // Returns 0 on success, non-zero on failure. On success *out_buffer points
                // to a libjpeg-allocated buffer of *out_size bytes that the caller must
                // release with DicomJpegFreeBuffer.
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
                    unsigned char *out_buffer,
                    unsigned int *out_size,
                    int *out_jpegColorSpace,
                    char *errorMessage,
                    unsigned int errorMessageSize)
                {
                    struct jpeg_compress_struct cinfo;
                    dicom_jpeg_error_mgr jerr;
                    unsigned char *encoded = out_buffer;
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
                    jpeg_memory_destination(&cinfo, &encoded, &encodedSize);

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
                    *out_size = static_cast<unsigned int>(encodedSize);

                    jpeg_destroy_compress(&cinfo);
                    return 0;
                }

                // Decode
                // Returns 0 on success, non-zero on failure. On success *out_pixels points
                // to a malloc-allocated buffer of *out_size bytes that the caller must
                // release with DicomJpegFreeBuffer.
                EXPORT_Jpeg int DicomJpegDecode(
                    const unsigned char *jpegData,
                    unsigned int jpegSize,
                    int convertColorSpaceToRGB,
                    int isSigned,
                    unsigned char *out_pixels,
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
                        if (out_pixels != nullptr)
                            free(out_pixels);
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

                    if (out_pixels == nullptr)
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
                        unsigned char *row = out_pixels + (size_t)dinfo.output_scanline * rowSize;
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

                            free(out_pixels);
                            jpeg_destroy_decompress(&dinfo);
                            return 3;
                        }
                    }

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

                // Read the data precision (bits per sample) from a JPEG SOF marker without
                // performing a full decode. Returns 0 on success.
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

#ifdef __cplusplus
            }
#endif

        } // namespace Codec
    } // namespace Imaging
} // namespace Dicom