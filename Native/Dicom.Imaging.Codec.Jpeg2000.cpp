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

//Encode OpenJPEG

EXPORT_OpenJPEG opj_cinfo_t* Opj_create_compress(OPJ_CODEC_FORMAT format)
{
    return opj_create_compress(format);
 
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
    opj_setup_encoder(cinfo, parameters, image);  
}

EXPORT_OpenJPEG opj_cio_t* Opj_cio_open(opj_common_ptr cinfo , unsigned char* buffer, int length)
{
    return opj_cio_open(cinfo, buffer, length);   
}

EXPORT_OpenJPEG int Opj_encode(opj_cinfo_t* cinfo, opj_cio_t* cio, opj_image_t* image, char* index)
{
    return opj_encode(cinfo, cio, image, index);   
}

EXPORT_OpenJPEG void Opj_cio_close(opj_cio_t* cio)
{
    opj_cio_close(cio);  
}

EXPORT_OpenJPEG void Opj_image_destroy(opj_image_t* image)
{
    opj_image_destroy(image);  
}

EXPORT_OpenJPEG void Opj_destroy_compress(opj_cinfo_t* cinfo)
{
    opj_destroy_compress(cinfo);   
}

EXPORT_OpenJPEG int Cio_tell(opj_cio_t* cio)
{
    return cio_tell(cio);    
}

//Decode OpenJPEG

EXPORT_OpenJPEG opj_dinfo_t* Opj_create_decompress(OPJ_CODEC_FORMAT format)
{
    return opj_create_decompress(format);   
}

EXPORT_OpenJPEG void Opj_setup_decoder(opj_dinfo_t* dinfo, opj_dparameters_t* parameters)
{
    opj_setup_decoder(dinfo, parameters);  
}

EXPORT_OpenJPEG opj_image_t* Opj_decode(opj_dinfo_t* dinfo, opj_cio_t* cio)
{
    return opj_decode(dinfo, cio);  
}

EXPORT_OpenJPEG void Opj_destroy_decompress(opj_dinfo_t* dinfo)
{
    opj_destroy_decompress(dinfo); 
}

EXPORT_OpenJPEG void Opj_set_default_decode_parameters(opj_dparameters_t *parameters)
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
        opj_buffer_format = OPJ_CODEC_FORMAT::CODEC_JP2;

        return opj_buffer_format;
    }
    else if (memcmp(buf12, J2K_CODESTREAM_MAGIC, 4) == 0)
    {   
        opj_buffer_format = OPJ_CODEC_FORMAT::CODEC_J2K;

        return opj_buffer_format;
    }
    else
    {   
        opj_buffer_format = OPJ_CODEC_FORMAT::CODEC_UNKNOWN;
    }

    return opj_buffer_format;
}

#ifdef __cplusplus
}
#endif

}//Codec
}//Imaging
}//Dicom