// Copyright (c) 2012-2023 fo-dicom contributors.
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
#include "./Common/OpenJPEG/j2k.h"
#include "./Common/OpenJPEG/opj_includes.h"
}

#elif defined(__linux__)
#define EXPORT_OpenJPEG extern
extern "C"{
#include "./Common/OpenJPEG/openjpeg.h"
#include "./Common/OpenJPEG/opj_includes.h"
#include <string.h>
}

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_OpenJPEG extern
		extern "C"{
        #include "./Common/OpenJPEG/openjpeg.h"
        #include "./Common/OpenJPEG/j2k.h"
        #include "./Common/OpenJPEG/opj_includes.h"
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
    
static OPJ_SIZE_T ReadCallback(void* pBuffer, OPJ_SIZE_T nBytes, void* pUserData)
{
    MemFile* memFile = (MemFile*)pUserData;
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

static OPJ_SIZE_T WriteCallback(void* pBuffer, OPJ_SIZE_T nBytes, void* pUserData)
{
    MemFile* memFile = (MemFile*)pUserData;
    if (memFile->nCurPos >= memFile->nLength) 
    {
        return -1;
    }
        
    if (memFile->nCurPos + nBytes >= memFile->nLength) 
    {
        size_t nToWrite = memFile->nLength - memFile->nCurPos;
        memcpy((void *)(memFile->pabyData + memFile->nCurPos), pBuffer, nToWrite);
        memFile->nCurPos = memFile->nLength;
        return nToWrite;
    }
        
    if (nBytes == 0) 
    {
        return -1;
    }
    
    memcpy((void *)(memFile->pabyData + memFile->nCurPos), pBuffer, nBytes);
    memFile->nCurPos += nBytes;
    return nBytes;
}

static OPJ_BOOL SeekCallback(OPJ_OFF_T nBytes, void* pUserData)
{
    MemFile* memFile = (MemFile*)pUserData;
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

/**
sample error debug callback expecting no client object
*/
static void error_callback(const char *msg, void *client_data)
{
    (void)client_data;
    //fprintf(stdout, "[ERROR] %s", msg);
}
    
/**
sample warning debug callback expecting no client object
*/
static void warning_callback(const char *msg, void *client_data)
{
    (void)client_data;
    //fprintf(stdout, "[WARNING] %s", msg);
}

/**
sample debug callback expecting no client object
*/
static void info_callback(const char *msg, void *client_data)
{
    (void)client_data;
    //fprintf(stdout, "[INFO] %s", msg);
}

/* CALLING COMPRESSION FUNCTIONS */

EXPORT_OpenJPEG opj_codec_t* Opj_create_compress(OPJ_CODEC_FORMAT format)
{
    opj_codec_t* codec = opj_create_compress(format);
    
    /* catch events using our callbacks and give a local context */
    opj_set_info_handler(codec, info_callback, 00);
    opj_set_warning_handler(codec, warning_callback, 00);
    opj_set_error_handler(codec, error_callback, 00);
    
    return codec;
}

EXPORT_OpenJPEG opj_image_t* Opj_image_create(OPJ_UINT32 numcmpts, opj_image_cmptparm_t* cmptparms, OPJ_COLOR_SPACE clrspc)
{
    return opj_image_create(numcmpts, cmptparms, clrspc);  
}

EXPORT_OpenJPEG void Opj_setup_encoder(opj_codec_t* codec, opj_cparameters_t* parameters, opj_image_t* image)
{   
    opj_setup_encoder(codec, parameters, image);
}

EXPORT_OpenJPEG opj_stream_t* Opj_create_stream(unsigned char* buffer, size_t length, bool isDecompressor)
{
    opj_stream_t* pStream = opj_stream_create(length, isDecompressor);
    MemFile* memFile = (MemFile*)opj_malloc(sizeof(MemFile));
    memFile->pabyData = buffer;
    memFile->nLength = length;
    memFile->nCurPos = 0;

    opj_stream_set_user_data_length(pStream, length);

    if (!isDecompressor)
        opj_stream_set_write_function(pStream, WriteCallback);
    else
        opj_stream_set_read_function(pStream, ReadCallback);

    opj_stream_set_seek_function(pStream, SeekCallback);
    opj_stream_set_skip_function(pStream, SkipCallback);
    opj_stream_set_user_data(pStream, memFile, FreeCallback);

    return pStream;   
}

EXPORT_OpenJPEG OPJ_BOOL Opj_encode(opj_codec_t* codec, opj_stream_t* stream, opj_image_t* image, char* index)
{
    OPJ_BOOL bSuccess;

    int num_threads = opj_get_num_cpus();
    opj_codec_set_threads(codec, num_threads);

    bSuccess = opj_start_compress(codec, image, stream);
        
    if (!bSuccess) 
    {
        opj_stream_destroy(stream);
        opj_destroy_codec(codec);
        opj_image_destroy(image);
        return OPJ_FALSE;
    }
        
    bSuccess = opj_encode(codec, stream);
    bSuccess = opj_end_compress(codec, stream);
    return bSuccess;
}

EXPORT_OpenJPEG void Opj_stream_close(opj_stream_t* stream)
{
    opj_stream_destroy(stream);
}

EXPORT_OpenJPEG void Opj_image_destroy(opj_image_t* image)
{
    opj_image_destroy(image);  
}

EXPORT_OpenJPEG void Opj_destroy_compress(opj_codec_t* codec)
{
    if (codec) 
       opj_destroy_codec(codec);
}

EXPORT_OpenJPEG int Opj_stream_tell(opj_stream_t* stream)
{
    return opj_stream_tell((opj_stream_private_t*)stream);  
}

/* CALLING DECOMPRESSION FUNCTIONS */

EXPORT_OpenJPEG opj_codec_t* Opj_create_decompress(OPJ_CODEC_FORMAT format)
{   
    opj_codec_t* codec = opj_create_decompress(format);

    /* catch events using our callbacks and give a local context */
    opj_set_info_handler(codec, info_callback, 00);
    opj_set_warning_handler(codec, warning_callback, 00);
    opj_set_error_handler(codec, error_callback, 00);
    
    return codec;  
}

EXPORT_OpenJPEG void Opj_setup_decoder(opj_codec_t* codec, opj_dparameters_t* parameters)
{
    opj_setup_decoder(codec, parameters);  
}

EXPORT_OpenJPEG opj_image_t* Opj_decode(opj_codec_t* codec, opj_stream_t* stream)
{
    OPJ_BOOL bSuccess;
    opj_image_t* pImage;
    
    int num_threads = opj_get_num_cpus();
    opj_codec_set_threads(codec, num_threads);

    if (!opj_read_header(stream, codec, &pImage))
    {
        opj_stream_destroy(stream);
        opj_destroy_codec(codec);
        return NULL;
    }

    bSuccess = opj_decode(codec, stream, pImage);
    bSuccess = opj_end_decompress(codec, stream);
    return pImage;
}


EXPORT_OpenJPEG void Opj_destroy_decompress(opj_codec_t* codec)
{
    if (codec) 
       opj_destroy_codec(codec);
}

EXPORT_OpenJPEG void Opj_set_default_decode_parameters(opj_dparameters_t *parameters)
{
    opj_set_default_decoder_parameters(parameters);
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