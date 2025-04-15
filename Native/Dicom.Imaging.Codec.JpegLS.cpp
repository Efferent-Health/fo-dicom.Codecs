// Copyright (c) 2012-2025 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

#include <iostream>
#include <new>

#include "./Common/CharLS/charls/charls_jpegls_decoder.h"
#include "./Common/CharLS/charls/charls_jpegls_encoder.h"

#if defined (_WIN32)
#define EXPORT_Charls __declspec(dllexport)

#elif defined(__linux__)
#define EXPORT_Charls extern 

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_Charls extern
    #endif

#endif

namespace Dicom {
namespace Imaging {
namespace Codec {

#ifdef __cplusplus
extern "C" {

#else

#include <stddef.h>

#endif

EXPORT_Charls CharlsApiResultType JpegLSEncode(void* destination, size_t destinationLength, size_t* bytesWritten, void* source, size_t sourceLength, JlsParameters* obj, char* errorMessage)
{
    return JpegLsEncode(destination, destinationLength, bytesWritten, source, sourceLength, obj, errorMessage); 
}

EXPORT_Charls CharlsApiResultType JpegLSDecode(void * destination, int destinationLength, void* source, size_t sourceLength, JlsParameters* obj, char* errorMessage)
{
    return JpegLsDecode(destination, destinationLength, source, sourceLength, obj, errorMessage);     
}

#ifdef __cplusplus
}
#endif

}//Codec
}//Imaging
}//Dicom