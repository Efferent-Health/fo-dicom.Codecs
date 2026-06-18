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
  ${CMAKE_CURRENT_SOURCE_DIR}/../Dicom.Imaging.Codec.Jpeg.cpp

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

#libjpeg-turbo library
SET(TURBO_SRCS
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jcapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jchuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jcicc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jcinit.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jclhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jcmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jcmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jcomapi.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jcparam.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jcphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jctrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdapimin.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdatadst.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdatasrc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdicc.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdinput.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdlhuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdmarker.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdmaster.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdphuff.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdtrans.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jerror.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jfdctflt.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jmemmgr.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jmemnobs.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jpeg_nbits.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jaricom.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jcarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/jdarith.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcapistd-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcapistd-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcapistd-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jccoefct-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jccoefct-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jccolor-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jccolor-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jccolor-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcdctmgr-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcdctmgr-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcdiffct-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcdiffct-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcdiffct-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jclossls-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jclossls-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jclossls-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcmainct-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcmainct-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcmainct-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcprepct-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcprepct-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcprepct-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcsample-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcsample-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jcsample-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdapistd-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdapistd-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdapistd-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdcoefct-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdcoefct-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdcolor-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdcolor-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdcolor-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jddctmgr-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jddctmgr-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jddiffct-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jddiffct-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jddiffct-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdlossls-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdlossls-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdlossls-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdmainct-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdmainct-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdmainct-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdmerge-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdmerge-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdpostct-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdpostct-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdpostct-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdsample-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdsample-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jdsample-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jfdctfst-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jfdctfst-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jfdctint-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jfdctint-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jidctflt-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jidctflt-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jidctfst-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jidctfst-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jidctint-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jidctint-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jidctred-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jidctred-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jquant1-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jquant1-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jquant2-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jquant2-8.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jutils-12.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jutils-16.c
  ${CMAKE_CURRENT_SOURCE_DIR}/../Common/libjpeg-turbo/wrapper/jutils-8.c
)

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
include_directories(../Common/libjpeg-turbo/)

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
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${TURBO_SRCS} ${CHARLS_SRCS} ${OPJH_SRCS})
    #Setting properties for shared or dynamic library 
    set_target_properties(Dicom.Native
      PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-linux-arm64"
      RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-linux-arm64"
      PREFIX ""
    )
  else()
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${TURBO_SRCS} ${CHARLS_SRCS} ${OPJH_SRCS} ${OPJH_INTEL_SIMD_SRCS})
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
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${TURBO_SRCS} ${CHARLS_SRCS} ${OPJH_SRCS})
    #Setting properties for shared or dynamic library 
    set_target_properties(Dicom.Native
      PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-arm64"
      RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-arm64"
      PREFIX ""
    )
  else()
    add_library(Dicom.Native SHARED ${CROSSPLATFORM_SRCS} ${OPENJPEG_SRCS} ${TURBO_SRCS} ${CHARLS_SRCS} ${OPJH_SRCS} ${OPJH_INTEL_SIMD_SRCS})
    #Setting properties for shared or dynamic library 
    set_target_properties(Dicom.Native
      PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-x64"
      RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib-osx-x64"
      PREFIX ""
    )
  endif()
endif()