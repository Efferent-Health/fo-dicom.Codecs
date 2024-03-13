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

#include "./Common/OpenJPH/common/ojph_arg.h"
#include "./Common/OpenJPH/common/ojph_mem.h"
#include "./Common/OpenJPH/common/ojph_file.h"
#include "./Common/OpenJPH/common/ojph_codestream.h"
#include "./Common/OpenJPH/common/ojph_params.h"
#include "./Common/OpenJPH/common/ojph_message.h"

namespace Dicom {
namespace Imaging {
namespace Codec {

#ifdef __cplusplus
extern "C" {
#endif




#ifdef __cplusplus
}
#endif

}
}
}