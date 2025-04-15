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
    set(CMAKE_C_COMPILER "/usr/bin/aarch64-linux-gnu-gcc")
    set(CMAKE_CXX_COMPILER "/usr/bin/aarch64-linux-gnu-g++")
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
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.HtJpeg2000.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.Jpeg_8.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.Jpeg_12.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.Jpeg_16.cpp
)

#CharLS Library
SET(CHARLS_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls_jpegls_decoder.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls_jpegls_encoder.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpeg_stream_reader.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpeg_stream_writer.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegls_error.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegls.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/validate_spiff_header.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/version.cpp

  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls/annotations.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls/api_abi.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls/charls_jpegls_decoder.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls/charls_jpegls_encoder.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls/charls.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls/jpegls_error.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls/public_types.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls/validate_spiff_header.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/charls/version.h

  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/byte_span.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/coding_parameters.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/color_transform.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/conditional_static_cast.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/constants.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/context_regular_mode.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/context_run_mode.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/decoder_strategy.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/default_traits.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/encoder_strategy.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jls_codec_factory.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpeg_marker_code.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpeg_stream_reader.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpeg_stream_writer.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegls_preset_coding_parameters.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/jpegls_preset_parameters_type.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/lookup_table.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/lossless_traits.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/process_line.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/scan.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/CharLS/util.h
)

#libijg8 library
SET(LIBIJG8_SRCS
#  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg8/jaricom.c
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
#  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg12/jaricom.c
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
#  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libijg16/jaricom.c
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
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/thread.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/thread.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/bio.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/bio.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/cio.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/cio.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/dwt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/dwt.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/event.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/event.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/ht_dec.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/image.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/image.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/invert.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/invert.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/j2k.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/j2k.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/jp2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/jp2.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/mct.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/mct.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/mqc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/mqc.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/mqc_inl.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/openjpeg.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/openjpeg.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/opj_clock.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/opj_clock.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/pi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/pi.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/t1.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/t1.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/t2.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/t2.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/tcd.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/tcd.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/tgt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/tgt.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/function_list.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/function_list.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/opj_codec.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/opj_includes.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/opj_intmath.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/opj_malloc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/opj_malloc.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/sparse_array.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/sparse_array.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/opj_config.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPEG/opj_config_private.h
)

#OpenJPH Library
include_directories(../Common/OpenJPH/common/)

SET(OPJH_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_bitbuffer_read.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_bitbuffer_write.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codeblock.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codeblock_fun.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_local.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_params_local.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_precinct.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_resolution.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_subband.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_tile.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_tile_comp.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codeblock.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codeblock_fun.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_gen.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_local.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_params.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_precinct.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_resolution.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_subband.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_tile.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_tile_comp.cpp

  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_common.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_decoder.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_encoder.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/table0.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/table1.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_common.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_decoder32.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_decoder64.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_encoder.cpp

  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_arch.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_arg.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_base.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_codestream.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_defs.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_file.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_mem.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_message.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_params.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/common/ojph_version.h

  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/others/ojph_arch.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/others/ojph_file.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/others/ojph_mem.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/others/ojph_message.cpp

  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour_local.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_local.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour.cpp 
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform.cpp

  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/interface/encoded_buffer.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/interface/ojph_interface.h
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/interface/ojph_interface.cpp
)

SET(OPJH_INTEL_SIMD_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_sse.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_sse2.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_avx.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_avx2.cpp
  
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_decoder_avx2.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_decoder_ssse3.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_encoder_avx2.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_encoder_avx512.cpp

  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour_avx2.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour_avx.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour_sse.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour_sse2.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_sse.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_sse2.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_avx2.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_avx.cpp
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_avx512.cpp
)

set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_avx.cpp PROPERTIES COMPILE_FLAGS -mavx)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_avx2.cpp PROPERTIES COMPILE_FLAGS -mavx2)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_sse.cpp PROPERTIES COMPILE_FLAGS -msse)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/codestream/ojph_codestream_sse2.cpp PROPERTIES COMPILE_FLAGS -msse2)

set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_decoder_ssse3.cpp PROPERTIES COMPILE_FLAGS -mssse3)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_decoder_avx2.cpp PROPERTIES COMPILE_FLAGS -mavx2)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_encoder_avx2.cpp PROPERTIES COMPILE_FLAGS -mavx2)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/coding/ojph_block_encoder_avx512.cpp PROPERTIES COMPILE_FLAGS -mavx512cd)

set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour_avx.cpp PROPERTIES COMPILE_FLAGS -mavx)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour_avx2.cpp PROPERTIES COMPILE_FLAGS -mavx2)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour_sse.cpp PROPERTIES COMPILE_FLAGS -msse)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_colour_sse2.cpp PROPERTIES COMPILE_FLAGS -msse2)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_avx.cpp PROPERTIES COMPILE_FLAGS -mavx)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_avx2.cpp PROPERTIES COMPILE_FLAGS -mavx2)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_sse.cpp PROPERTIES COMPILE_FLAGS -msse)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_sse2.cpp PROPERTIES COMPILE_FLAGS -msse2)
set_source_files_properties(${CMAKE_CURRENT_SOURCE_DIR}/../Common/OpenJPH/transform/ojph_transform_avx512.cpp PROPERTIES COMPILE_FLAGS -mavx512cd)

if(UNIX AND NOT APPLE)
  if("${ARCH_FLAG}" STREQUAL "arm64")
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${LIBIJG16_SRCS} ${LIBIJG12_SRCS} ${LIBIJG8_SRCS} ${CHARLS_SRCS} ${OPJH_SRCS})
    #Setting properties for shared or dynamic library 
    set_target_properties(Dicom.Native
      PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-linux-arm64"
      RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-linux-arm64"
      PREFIX ""
    )
  else()
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${LIBIJG16_SRCS} ${LIBIJG12_SRCS} ${LIBIJG8_SRCS} ${CHARLS_SRCS} ${OPJH_SRCS} ${OPJH_INTEL_SIMD_SRCS})
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
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${LIBIJG16_SRCS} ${LIBIJG12_SRCS} ${LIBIJG8_SRCS} ${CHARLS_SRCS} ${OPJH_SRCS})
    #Setting properties for shared or dynamic library 
    set_target_properties(Dicom.Native
      PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-arm64"
      RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-arm64"
      PREFIX ""
    )
  else()
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${LIBIJG16_SRCS} ${LIBIJG12_SRCS} ${LIBIJG8_SRCS} ${CHARLS_SRCS} ${OPJH_SRCS} ${OPJH_INTEL_SIMD_SRCS})
    #Setting properties for shared or dynamic library 
    set_target_properties(Dicom.Native
      PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-x64"
      RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-x64"
      PREFIX ""
    )
  endif()
endif()