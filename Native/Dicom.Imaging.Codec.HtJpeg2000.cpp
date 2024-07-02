#include <iostream>
#include <new>

#if defined(_WIN32)
#define EXPORT_OpenJPH  __declspec(dllexport)

#elif defined(__linux__)
#define EXPORT_OpenJPH extern 

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_OpenJPH extern
    #endif
#endif

#include "./Common/OpenJPH/interface/ojph_interface.h"

namespace Dicom {
namespace Imaging {
namespace Codec {

#ifdef __cplusplus

extern "C" {
#endif

EXPORT_OpenJPH void InvokeHTJ2KEncode(struct Htj2k_outdata* j2c_outinfo, const unsigned char *source, size_t sourceLength, const struct Frameinfo *finfo, ojph::PROGRESSION_ORDER progression_order)
{
    HTJpeg2000EncodeStream(j2c_outinfo, source, sourceLength, finfo, progression_order);
}

EXPORT_OpenJPH void InvokeHTJ2KDecode(struct Decoded_outdata* raw_outinfo, const unsigned char* source, size_t sourceLength)
{
    HTJpeg2000DecodeStream(raw_outinfo, source, sourceLength);
}


#ifdef __cplusplus
}
#endif

}//Codec
}//Imaging
}//Dicom