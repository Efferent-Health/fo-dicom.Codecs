cmake_minimum_required(VERSION 2.8)
project(Dicom.Codec)

if (CMAKE_COMPILER_IS_GNUCC)
    # the following is optimization micromanagement that made better code for x86
    # SET(CMAKE_CXX_FLAGS "-D NDEBUG -O3 -Wall -Wextra -pedantic -fvisibility=hidden -fomit-frame-pointer -momit-leaf-frame-pointer -fweb  -ftracer" )

    # Define GNU C++ defines for both Debug and Release
    set (CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++14 -Wall -Wextra -pedantic")

    # Define specific Debug settings.
    set (CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -g")

    # Define specific Release settings.
    set (CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} -D NDEBUG -O3")
ENDIF ()

    # Define specific Debug settings.
    set (CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -g")

    # Define specific Release settings.
    set (CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} -D NDEBUG -O3")

if(NOT CMAKE_BUILD_TYPE)

  set(CMAKE_BUILD_TYPE Release)
  
endif(NOT CMAKE_BUILD_TYPE)

INCLUDE_REGULAR_EXPRESSION("^.*$")

# Defines the source code for the library
option (BUILD_SHARED_LIBS "Build CharLS with shared libraries." OFF)
option (BUILD_TESTING     "Build tests"                         ON)


SET(CROSSPLATFORM_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../../Dicom.Imaging.Codec.JpegLS.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../../Dicom.Imaging.Codec.Jpeg2000.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../../Dicom.Imaging.Codec.Jpeg_8.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../../Dicom.Imaging.Codec.Jpeg_12.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../../Dicom.Imaging.Codec.Jpeg_16.cpp
)

#CharLS Library
SET(CHARLS_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/interface.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegls.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegmarkersegment.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegstreamreader.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegstreamwriter.cpp	
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/charls.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/publictypes.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/colortransform.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/context.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/contextrunmode.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/decoderstrategy.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/defaulttraits.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/encoderstrategy.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jlscodecfactory.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegimagedatasegment.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegmarkercode.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegmarkersegment.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegsegment.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegstreamreader.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/jpegstreamwriter.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/lookuptable.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/losslesstraits.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/processline.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/scan.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../CharLS/util.h
 )

#libijg8 library
SET(LIBIJG8_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jaricom.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jccoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jccolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcdctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcdiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jchuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcinit.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jclhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jclossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jclossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcodec.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcomapi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcparam.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcprepct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jcshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jctrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdatadst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdatasrc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdcoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdcolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jddctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jddiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdinput.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdlhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdlossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdlossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdmerge.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdpostct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jdtrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jerror.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jfdctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jfdctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jfdctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jidctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jidctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jidctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jidctred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jmemmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jmemnobs.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jquant1.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jquant2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg8/jutils.c
)

#libijg12 library
SET(LIBIJG12_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jaricom.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jccoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jccolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcdctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcdiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jchuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcinit.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jclhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jclossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jclossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcodec.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcomapi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcparam.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcprepct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jcshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jctrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdatadst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdatasrc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdcoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdcolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jddctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jddiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdinput.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdlhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdlossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdlossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdmerge.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdpostct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jdtrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jerror.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jfdctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jfdctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jfdctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jidctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jidctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jidctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jidctred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jmemmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jmemnobs.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jquant1.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jquant2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg12/jutils.c
)

#libijg12 library
SET(LIBIJG16_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jaricom.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jccoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jccolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcdctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcdiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jchuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcinit.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jclhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jclossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jclossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcodec.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcomapi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcparam.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcprepct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jcshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jctrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdapistd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdatadst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdatasrc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdcoefct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdcolor.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jddctmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jddiffct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdinput.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdlhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdlossls.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdlossy.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdmainct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdmerge.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdpostct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdpred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdsample.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdscale.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdshuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jdtrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jerror.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jfdctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jfdctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jfdctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jidctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jidctfst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jidctint.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jidctred.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jmemmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jmemnobs.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jquant1.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jquant2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../libijg16/jutils.c
)

#OpenJPEG Library
SET(OPENJPEG_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/bio.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/cio.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/dwt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/event.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/image.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/j2k.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/j2k_lib.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/jp2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/jpt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/mct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/mqc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/openjpeg.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/pi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/raw.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/t1.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/t2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/tcd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/tgt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/cidx_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/phix_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/ppix_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/thix_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/tpix_manager.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../OpenJPEG/openjpeg.h
)

add_library(fo-dicom.Codecs.Native-linux64 SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${LIBIJG16_SRCS} ${LIBIJG12_SRCS} ${LIBIJG8_SRCS} ${CHARLS_SRCS})

#Setting properties for shared o dynamic library 
set_target_properties(Dicom.Native-linux64
  PROPERTIES
  LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
  RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
  PREFIX ""
)

