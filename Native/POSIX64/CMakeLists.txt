cmake_minimum_required(VERSION 3.25.2)
project(Dicom.Native)

if(CMAKE_COMPILER_IS_GNUCC)
  # the following is optimization micromanagement that made better code for x86
  # SET(CMAKE_CXX_FLAGS "-D NDEBUG -O3 -Wall -Wextra -pedantic -fvisibility=hidden -fomit-frame-pointer -momit-leaf-frame-pointer -fweb  -ftracer" )

  # Define GNU C++ defines for both Debug and Release
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++14 -Wall -Wextra -pedantic")
  # Define specific Debug settings.
  set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -g")
  # Define specific Release settings.
  set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} -D NDEBUG -O3")

else()
  set(CMAKE_CXX_STANDARD 17)

  if(CMAKE_CXX_COMPILER_ID MATCHES "Clang")
    # Define clang C++ defines for both Debug and Release
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++14 -Wall -Wextra")
    # Define specific Debug settings.
    set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -g")
    # Define specific Release settings.
    set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} -D NDEBUG -O3")
  endif()
endif()

## if compile ARM64
set(ARCH_FLAG CACHE STRING "")

if("${ARCH_FLAG}" STREQUAL "arm64")
  if(UNIX AND NOT APPLE)
    set(CMAKE_C_COMPILER "/usr/bin/arm-linux-gnueabi-gcc")
    set(CMAKE_CXX_COMPILER "/usr/bin/arm-linux-gnueabi-g++")
  else()
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -arch arm64")
  endif()

  set(CMAKE_OSX_ARCHITECTURES "arm64")
  set(CMAKE_HOST_SYSTEM_PROCESSOR "arm64")

endif()

if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release)  
endif(NOT CMAKE_BUILD_TYPE)

INCLUDE_REGULAR_EXPRESSION("^.*$")

# Defines the source code for the library
option (BUILD_SHARED_LIBS "Build CharLS with shared libraries." OFF)
option (BUILD_TESTING     "Build tests"                         ON)


SET(CROSSPLATFORM_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.JpegLS.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.Jpeg2000.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.Jpeg_8.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.Jpeg_12.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.Jpeg_16.cpp
)

#CharLS Library
SET(CHARLS_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/interface.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegls.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegmarkersegment.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegstreamreader.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegstreamwriter.cpp	
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/publictypes.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/colortransform.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/context.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/contextrunmode.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/decoderstrategy.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/defaulttraits.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/encoderstrategy.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jlscodecfactory.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegimagedatasegment.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegmarkercode.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegmarkersegment.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegsegment.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegstreamreader.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegstreamwriter.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/lookuptable.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/losslesstraits.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/processline.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/scan.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/util.h
)

#libijg8 library
SET(LIBIJG8_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jaricom.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jccoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jccolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcdctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcdiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jchuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcinit.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jclhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jclossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jclossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcodec.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcomapi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcparam.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcprepct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jcshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jctrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdatadst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdatasrc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdcoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdcolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jddctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jddiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdinput.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdlhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdlossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdlossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdmerge.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdpostct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jdtrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jerror.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jfdctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jfdctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jfdctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jidctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jidctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jidctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jidctred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jmemmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jmemnobs.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jquant1.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jquant2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jutils.c
)

#libijg12 library
SET(LIBIJG12_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jaricom.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jccoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jccolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcdctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcdiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jchuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcinit.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jclhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jclossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jclossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcodec.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcomapi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcparam.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcprepct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jcshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jctrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdatadst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdatasrc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdcoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdcolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jddctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jddiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdinput.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdlhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdlossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdlossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdmerge.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdpostct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jdtrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jerror.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jfdctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jfdctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jfdctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jidctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jidctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jidctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jidctred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jmemmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jmemnobs.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jquant1.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jquant2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jutils.c
)

#libijg12 library
SET(LIBIJG16_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jaricom.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jccoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jccolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcdctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcdiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jchuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcinit.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jclhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jclossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jclossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcodec.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcomapi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcparam.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcprepct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jcshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jctrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdatadst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdatasrc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdcoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdcolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jddctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jddiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdinput.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdlhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdlossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdlossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdmerge.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdpostct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jdtrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jerror.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jfdctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jfdctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jfdctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jidctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jidctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jidctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jidctred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jmemmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jmemnobs.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jquant1.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jquant2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jutils.c
)

#OpenJPEG Library
SET(OPENJPEG_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/bio.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/cio.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/dwt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/event.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/image.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/j2k.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/j2k_lib.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/jp2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/jpt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/mct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/mqc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/openjpeg.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/pi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/raw.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/t1.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/t2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/tcd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/tgt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/cidx_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/phix_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/ppix_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/thix_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/tpix_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/openjpeg.h
)

if(UNIX AND NOT APPLE)
  if("${ARCH_FLAG}" STREQUAL "arm64")
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${LIBIJG16_SRCS} ${LIBIJG12_SRCS} ${LIBIJG8_SRCS} ${CHARLS_SRCS})
    #Setting properties for shared or dynamic library 
    set_target_properties(Dicom.Native
      PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-linux-arm64"
      RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-linux-arm64"
      PREFIX ""
    )
  else()
  add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${LIBIJG16_SRCS} ${LIBIJG12_SRCS} ${LIBIJG8_SRCS} ${CHARLS_SRCS})
  #Setting properties for shared or dynamic library 
  set_target_properties(Dicom.Native
    PROPERTIES
    LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-linux-x64"
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-linux-x64"
    PREFIX ""
  ) 
  endif()
elseif(UNIX)
  if("${ARCH_FLAG}" STREQUAL "arm64")
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${LIBIJG16_SRCS} ${LIBIJG12_SRCS} ${LIBIJG8_SRCS} ${CHARLS_SRCS})
    #Setting properties for shared or dynamic library 
    set_target_properties(Dicom.Native
      PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-arm64"
      RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-arm64"
      PREFIX ""
    )
  else()
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${LIBIJG16_SRCS} ${LIBIJG12_SRCS} ${LIBIJG8_SRCS} ${CHARLS_SRCS})
    #Setting properties for shared or dynamic library 
    set_target_properties(Dicom.Native
      PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-x64"
      RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-x64"
      PREFIX ""
    )
  endif()
endif()