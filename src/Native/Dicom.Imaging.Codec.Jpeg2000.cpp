// Copyright (c) 2012-2019 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

#include <iostream>
#include <new>

#if defined(_WIN32)
#define EXPORT_OpenJPEG  __declspec(dllexport)
extern "C"{
#include "OpenJPEG/openjpeg.h"
#include "OpenJPEG/j2k.h"
}

#elif defined(__linux__)
#define EXPORT_OpenJPEG extern 
extern "C"{
#include "./Linux64/OpenJPEG/openjpeg.h"
#include "./Linux64/OpenJPEG/j2k.h"
# include <string.h>
}

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_OpenJPEG extern
		extern "C"{
        #include "./MacOS/OpenJPEG/openjpeg.h"
        #include "./MacOS/OpenJPEG/j2k.h"
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

EXPORT_OpenJPEG opj_cinfo_t* Opj_create_compress(OPJ_CODEC_FORMAT format){
    try
    {
        return opj_create_compress(format);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG opj_event_mgr_t* Opj_set_event_mgr(opj_common_ptr cinfo, opj_event_mgr_t* e, void* context){
    try
    {
        return opj_set_event_mgr(cinfo, e, context);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG opj_image_t* Opj_image_create(int numcmpts, opj_image_cmptparm_t* cmptparms, OPJ_COLOR_SPACE clrspc){
    try
    {
        return opj_image_create(numcmpts, cmptparms, clrspc);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG void Opj_setup_encoder(opj_cinfo_t* cinfo, opj_cparameters_t* parameters, opj_image_t* image){
    try
    {
        opj_setup_encoder(cinfo, parameters, image);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG opj_cio_t* Opj_cio_open(opj_common_ptr cinfo , unsigned char* buffer, int length){
    try
    {
        return opj_cio_open(cinfo, buffer, length);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG int Opj_encode(opj_cinfo_t* cinfo, opj_cio_t* cio, opj_image_t* image, char* index){
    try
    {
        return opj_encode(cinfo, cio, image, index);
    }
    catch(const std::exception& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG void Opj_cio_close(opj_cio_t* cio){
    try
    {
        opj_cio_close(cio);
    }
    catch(const std::exception& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG void Opj_image_destroy(opj_image_t* image){
    try
    {
        opj_image_destroy(image);
    }
    catch(const std::exception& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG void Opj_destroy_compress(opj_cinfo_t* cinfo){
    try
    {
        opj_destroy_compress(cinfo);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG int Cio_tell(opj_cio_t* cio){
    try
    {
        return cio_tell(cio);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

//Decode OpenJPEG

EXPORT_OpenJPEG opj_dinfo_t* Opj_create_decompress(OPJ_CODEC_FORMAT format){
    try
    {
        return opj_create_decompress(format);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG void Opj_setup_decoder(opj_dinfo_t* dinfo, opj_dparameters_t* parameters){
    try
    {
        opj_setup_decoder(dinfo, parameters);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG opj_image_t* Opj_decode(opj_dinfo_t* dinfo, opj_cio_t* cio){
    try
    {
        return opj_decode(dinfo, cio);
    }
    catch(const std::bad_alloc& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG void Opj_destroy_decompress(opj_dinfo_t* dinfo){
    try
    {
        opj_destroy_decompress(dinfo);
    }
    catch(const std::exception& e)
    {
        std::cerr << e.what() << '\n';
    }
    
}

EXPORT_OpenJPEG void Opj_set_default_decode_parameters(opj_dparameters_t *parameters){
    opj_set_default_decoder_parameters(parameters);
}

EXPORT_OpenJPEG void Memset(void * prt, int value ,size_t num){
    memset(prt, value, num);
}

#ifdef __cplusplus
}
#endif

}//Codec
}//Imaging
}//Dicom