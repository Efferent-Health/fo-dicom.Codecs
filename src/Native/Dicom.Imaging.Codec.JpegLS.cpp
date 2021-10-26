// Copyright (c) 2012-2021 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

#include <iostream>
#include <new>

#if defined (_WIN32)
#include "CharLS/charls.h"
#define EXPORT_Charls __declspec(dllexport)

#elif defined(__linux__)
#include "./Linux64/CharLS/charls.h"
#define EXPORT_Charls extern 

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_Charls extern
		#include "./MacOS/CharLS/charls.h"
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
    return JpegLsDecode(destination, destinationLength, source, sourceLength, obj, errorMessage );     
}

#ifdef __cplusplus
}
#endif

}//Codec
}//Imaging
}//Dicom