// Copyright (c) 2012-2021 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

#include <iostream>
#include <new>

// Twelve and four first values for JPEG2000 buffer image (j2k and jp2 decode formats)
/*----------------------------------------------------------------------------------*/
#define JP2_RFC3745_MAGIC "\x00\x00\x00\x0c\x6a\x50\x20\x20\x0d\x0a\x87\x0a"
#define JP2_MAGIC "\x0d\x0a\x87\x0a"
#define J2K_CODESTREAM_MAGIC "\xff\x4f\xff\x51"

#if defined(_WIN32)
#define EXPORT_OpenJPEG  __declspec(dllexport)
extern "C"{
#include "./Common/OpenJPEG/openjpeg.h"
#include "./Common/OpenJPEG/openjpeg2to1wrapper.h"
#include "./Common/OpenJPEG/opj_includes.h"
#include "./Common/OpenJPEG/j2k.h"
}

#elif defined(__linux__)
#define EXPORT_OpenJPEG extern 
extern "C"{
#include "./Common/OpenJPEG/openjpeg.h"
#include "./Common/OpenJPEG/j2k.h"
#include <string.h>
}

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_OpenJPEG extern
		extern "C"{
        #include "./Common/OpenJPEG/openjpeg.h"
        #include "./Common/OpenJPEG/j2k.h"
		}
    #endif

#endif


namespace Dicom {
namespace Imaging {
namespace Codec {

#ifdef __cplusplus
extern "C" {
#endif

    typedef struct {
        const uint8_t* pabyData;
        size_t         nCurPos;
        size_t         nLength;
    } MemFile;
    static OPJ_SIZE_T ReadCallback(void* pBuffer, OPJ_SIZE_T nBytes,
        void* pUserData)
    {
        MemFile* memFile = (MemFile*)pUserData;
        //printf("want to read %d bytes at %d\n", (int)memFile->nCurPos, (int)nBytes);
        if (memFile->nCurPos >= memFile->nLength) {
            return -1;
        }
        if (memFile->nCurPos + nBytes >= memFile->nLength) {
            size_t nToRead = memFile->nLength - memFile->nCurPos;
            memcpy(pBuffer, memFile->pabyData + memFile->nCurPos, nToRead);
            memFile->nCurPos = memFile->nLength;
            return nToRead;
        }
        if (nBytes == 0) {
            return -1;
        }
        memcpy(pBuffer, memFile->pabyData + memFile->nCurPos, nBytes);
        memFile->nCurPos += nBytes;
        return nBytes;
    }

    static OPJ_SIZE_T WriteCallback(void* pBuffer, OPJ_SIZE_T nBytes,
        void* pUserData)
    {
        MemFile* memFile = (MemFile*)pUserData;
        if (memFile->nCurPos >= memFile->nLength) {
            return -1;
        }
        if (memFile->nCurPos + nBytes >= memFile->nLength) {
            size_t nToWrite = memFile->nLength - memFile->nCurPos;
            memcpy((void *)(memFile->pabyData + memFile->nCurPos), pBuffer, nToWrite);
            memFile->nCurPos = memFile->nLength;
            return nToWrite;
        }
        if (nBytes == 0) {
            return -1;
        }
        memcpy((void *)(memFile->pabyData + memFile->nCurPos), pBuffer, nBytes);
        memFile->nCurPos += nBytes;
        return nBytes;
    }

    static OPJ_BOOL SeekCallback(OPJ_OFF_T nBytes, void* pUserData)
    {
        MemFile* memFile = (MemFile*)pUserData;
        //printf("seek to %d\n", (int)nBytes);
        memFile->nCurPos = nBytes;
        return OPJ_TRUE;
    }

    static OPJ_OFF_T SkipCallback(OPJ_OFF_T nBytes, void* pUserData)
    {
        MemFile* memFile = (MemFile*)pUserData;
        memFile->nCurPos += nBytes;
        return nBytes;
    }

    static void FreeCallback(void* pUserData)
    {
        MemFile* memFile = (MemFile*)pUserData;
        opj_free(memFile);
    }

//Encode OpenJPEG

EXPORT_OpenJPEG opj_cinfo_t* Opj_create_compress(OPJ_CODEC_FORMAT format)
{
    opj_codec_t* realCodec = opj_create_compress(format);
 
    opj_cinfo_t* cinfo = (opj_cinfo_t*)opj_calloc(1, sizeof(opj_cinfo_t));
    if (!cinfo) return NULL;
    cinfo->is_decompressor = OPJ_FALSE;
    switch (format) {
    case OPJ_CODEC_J2K:
        /* get a J2K coder handle */
        cinfo->j2k_handle = (void*)realCodec;
        if (!cinfo->j2k_handle) {
            opj_free(cinfo);
            return NULL;
        }
        break;
    case OPJ_CODEC_JP2:
        /* get a JP2 coder handle */
        cinfo->jp2_handle = (void*)realCodec;
        if (!cinfo->jp2_handle) {
            opj_free(cinfo);
            return NULL;
        }
        break;
    case OPJ_CODEC_JPT:
    case OPJ_CODEC_UNKNOWN:
    default:
        opj_free(cinfo);
        return NULL;
    }
    return cinfo;
}

EXPORT_OpenJPEG opj_event_mgr_t* Opj_set_event_mgr(opj_common_ptr cinfo, opj_event_mgr_t* e, void* context)
{
    return opj_set_event_mgr(cinfo, e, context);  
}

EXPORT_OpenJPEG opj_image_t* Opj_image_create(int numcmpts, opj_image_cmptparm_t* cmptparms, OPJ_COLOR_SPACE clrspc)
{
    return opj_image_create(numcmpts, cmptparms, clrspc);  
}

EXPORT_OpenJPEG void Opj_setup_encoder(opj_cinfo_t* cinfo, opj_cparameters_t* parameters, opj_image_t* image)
{
    if (cinfo && parameters && image) {
        switch (cinfo->codec_format) {
        case OPJ_CODEC_J2K:
            opj_setup_encoder((opj_codec_t*)cinfo->j2k_handle, parameters, image);
            break;
        case OPJ_CODEC_JP2:
            opj_setup_encoder((opj_codec_t*)cinfo->jp2_handle, parameters, image);
            break;
        case OPJ_CODEC_JPT:
        case OPJ_CODEC_UNKNOWN:
        default:
            break;
        }
    }
}

EXPORT_OpenJPEG opj_stream_t* Opj_cio_open(opj_common_ptr cinfo, unsigned char* buffer, int length)
{
    opj_stream_t* pStream = opj_stream_create(length, cinfo->is_decompressor);
    MemFile* memFile = (MemFile*)opj_malloc(sizeof(MemFile));
    memFile->pabyData = buffer;
    memFile->nLength = length;
    memFile->nCurPos = 0;
    opj_stream_set_user_data_length(pStream, length);
    if (!cinfo->is_decompressor)
        opj_stream_set_write_function(pStream, WriteCallback);
    else
        opj_stream_set_read_function(pStream, ReadCallback);
    opj_stream_set_seek_function(pStream, SeekCallback);
    opj_stream_set_skip_function(pStream, SkipCallback);
    opj_stream_set_user_data(pStream, memFile, FreeCallback);

    return pStream;
}

EXPORT_OpenJPEG int Opj_encode(opj_cinfo_t* cinfo, opj_stream_t* cio, opj_image_t* image, char* index)
{
    OPJ_BOOL bSuccess;
    if (cinfo && cio && image) {
        switch (cinfo->codec_format) {
        case OPJ_CODEC_J2K:
            bSuccess = opj_start_compress((opj_codec_t*)cinfo->j2k_handle, image, cio);
            if (!bSuccess) {
                opj_stream_destroy(cio);
                opj_destroy_codec((opj_codec_t*)cinfo->j2k_handle);
                opj_image_destroy(image);
                return 0;
            }
            bSuccess = opj_encode((opj_codec_t*)cinfo->j2k_handle, cio);
            bSuccess = opj_end_compress((opj_codec_t*)cinfo->j2k_handle, cio);
            return bSuccess;
        case OPJ_CODEC_JP2:
            return opj_start_compress((opj_codec_t*)cinfo->jp2_handle, image, cio);
        case OPJ_CODEC_JPT:
        case OPJ_CODEC_UNKNOWN:
        default:
            break;
        }
    }
    return OPJ_FALSE;
}

EXPORT_OpenJPEG void Opj_cio_close(opj_stream_t* cio)
{
    opj_stream_destroy(cio);
}

EXPORT_OpenJPEG void Opj_image_destroy(opj_image_t* image)
{
    opj_image_destroy(image);  
}

EXPORT_OpenJPEG void Opj_destroy_compress(opj_cinfo_t* cinfo)
{
    if (cinfo) {
        /* destroy the codec */
        switch (cinfo->codec_format) {
        case OPJ_CODEC_J2K:
            opj_destroy_codec((opj_codec_t*)cinfo->j2k_handle);
            break;
        case OPJ_CODEC_JP2:
            opj_destroy_codec((opj_codec_t*)cinfo->jp2_handle);
            break;
        case OPJ_CODEC_JPT:
        case OPJ_CODEC_UNKNOWN:
        default:
            break;
        }
        /* destroy the decompressor */
        opj_free(cinfo);
    }
}

EXPORT_OpenJPEG OPJ_OFF_T Cio_tell(opj_stream_t* cio)
{
    return opj_stream_tell((opj_stream_private_t*)cio);
}

//Decode OpenJPEG

EXPORT_OpenJPEG opj_dinfo_t* Opj_create_decompress(OPJ_CODEC_FORMAT format)
{
    opj_dinfo_t* dinfo = (opj_dinfo_t*)opj_calloc(1, sizeof(opj_dinfo_t));
    if (!dinfo) return NULL;
    dinfo->is_decompressor = OPJ_TRUE;
    switch (format) {
    case OPJ_CODEC_J2K:
    case OPJ_CODEC_JPT:
        /* get a J2K decoder handle */
        dinfo->j2k_handle = (void*)opj_create_decompress(format);
        if (!dinfo->j2k_handle) {
            opj_free(dinfo);
            return NULL;
        }
        break;
    case OPJ_CODEC_JP2:
        /* get a JP2 decoder handle */
        dinfo->jp2_handle = (void*)opj_create_decompress(format);
        if (!dinfo->jp2_handle) {
            opj_free(dinfo);
            return NULL;
        }
        break;
    case OPJ_CODEC_UNKNOWN:
    default:
        opj_free(dinfo);
        return NULL;
    }

    dinfo->codec_format = format;

    return dinfo;
}

EXPORT_OpenJPEG void Opj_setup_decoder(opj_dinfo_t* dinfo, opj_dparameters_t* parameters)
{
    if (dinfo && parameters) {
        switch (dinfo->codec_format) {
        case OPJ_CODEC_J2K:
        case OPJ_CODEC_JPT:
            opj_setup_decoder((opj_codec_t*)dinfo->j2k_handle, parameters);
            break;
        case OPJ_CODEC_JP2:
            opj_setup_decoder((opj_codec_t*)dinfo->jp2_handle, parameters);
            break;
        case OPJ_CODEC_UNKNOWN:
        default:
            break;
        }
    }
}

EXPORT_OpenJPEG opj_image_t* Opj_decode(opj_dinfo_t* dinfo, opj_stream_t* cio)
{
    OPJ_BOOL bSuccess;
    opj_image_t* pImage;
    if (dinfo && cio) {
        switch (dinfo->codec_format) {
        case OPJ_CODEC_J2K:
            if (!opj_read_header(cio, (opj_codec_t*)dinfo->j2k_handle, &pImage))
            {
                fprintf(stderr, "ERROR -> failed to read the header\n");
                opj_stream_destroy(cio);
                opj_destroy_codec((opj_codec_t*)dinfo->j2k_handle);
                return NULL;
            }
            bSuccess = opj_decode((opj_codec_t*)dinfo->j2k_handle, cio, pImage);
            bSuccess = opj_end_decompress((opj_codec_t*)dinfo->j2k_handle, cio);
            return pImage;
        //case OPJ_CODEC_JPT:
        //    return opj_decode((opj_codec_t*)dinfo->j2k_handle, cio, NULL);
        //case OPJ_CODEC_JP2:
        //    return opj_decode((opj_codec_t*)dinfo->jp2_handle, cio, NULL);
        case OPJ_CODEC_UNKNOWN:
        default:
            break;
        }
    }
    return OPJ_FALSE;
}

EXPORT_OpenJPEG void Opj_destroy_decompress(opj_dinfo_t* dinfo)
{
    if (dinfo) {
        /* destroy the codec */
        switch (dinfo->codec_format) {
        case OPJ_CODEC_J2K:
        case OPJ_CODEC_JPT:
            opj_destroy_codec((opj_codec_t*)dinfo->j2k_handle);
            break;
        case OPJ_CODEC_JP2:
            opj_destroy_codec((opj_codec_t*)dinfo->jp2_handle);
            break;
        case OPJ_CODEC_UNKNOWN:
        default:
            break;
        }
        /* destroy the decompressor */
        opj_free(dinfo);
    }
}

EXPORT_OpenJPEG void Opj_set_default_decode_parameters(opj_dparameters_t* parameters)
{
    opj_set_default_decoder_parameters(parameters);
}

EXPORT_OpenJPEG void Memset(void * prt, int value ,size_t num)
{
    memset(prt, value, num);
}

EXPORT_OpenJPEG OPJ_CODEC_FORMAT GetCodecFormat(unsigned char* buffer)
{   
    unsigned char buf12[12];

    OPJ_CODEC_FORMAT opj_buffer_format;

    //Copying 12 first values from image buffer
    memcpy(buf12, buffer, sizeof(unsigned char)*12);

    //Comparing 12 or 4 first values from image buffer to get the JPEG2000 decode format
    if(memcmp(buf12, JP2_RFC3745_MAGIC, 12) == 0 || memcmp(buf12, JP2_MAGIC, 4) == 0)
    {   
        opj_buffer_format = OPJ_CODEC_FORMAT::OPJ_CODEC_JP2;

        return opj_buffer_format;
    }
    else if (memcmp(buf12, J2K_CODESTREAM_MAGIC, 4) == 0)
    {   
        opj_buffer_format = OPJ_CODEC_FORMAT::OPJ_CODEC_J2K;

        return opj_buffer_format;
    }
    else
    {   
        opj_buffer_format = OPJ_CODEC_FORMAT::OPJ_CODEC_UNKNOWN;
    }

    return opj_buffer_format;
}

#ifdef __cplusplus
}
#endif

}//Codec
}//Imaging
}//Dicom