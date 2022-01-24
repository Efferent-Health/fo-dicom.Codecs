// Copyright (c) 2012-2021 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

#include <iostream>
#include <new>

#if defined(_WIN32)
#define EXPORT_libijg16  __declspec(dllexport)
extern "C" {
#include "stdio.h"
#include "string.h"
#include "setjmp.h"
#include "./Common/libijg16/jpeglib16.h"
#include "./Common/libijg16/jerror16.h"
#include "./Common/libijg16/jpegint16.h"
}
#elif defined(__linux__)
#define EXPORT_libijg16  extern 
extern "C" {
#include "stdio.h"
#include "string.h"
#include "setjmp.h"
#include "./Common/libijg16/jpeglib16.h"
#include "./Common/libijg16/jerror16.h"
#include "./Common/libijg16/jpegint16.h"
}

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_libijg16 extern
		extern "C"{
		#include "stdio.h"
		#include "stdlib.h"
		#include "string.h"
		#include "setjmp.h"
		#include "./Common/libijg16/jpeglib16.h"
		#include "./Common/libijg16/jerror16.h"
		#include "./Common/libijg16/jpegint16.h"
		}
    #endif

#endif



namespace Dicom {
	namespace Imaging {
		namespace Codec {

#ifdef __cplusplus
			extern "C" {
#endif
				//Encode JPEG_16

				EXPORT_libijg16 jpeg_error_mgr* jpeg_std_error_16(struct jpeg_error_mgr * err)
				{
					return jpeg16_std_error(err);	
				}

				EXPORT_libijg16 void jpeg_create_compress_16(j_compress_ptr cinfo)
				{
					jpeg_create_compress(cinfo);
				}

				EXPORT_libijg16 void jpeg_set_defaults_16(j_compress_ptr cinfo) 
				{
					jpeg_set_defaults(cinfo);
				}

				EXPORT_libijg16 void jpeg_set_quality_16(j_compress_ptr cinfo, int quality, bool b)
				{
					jpeg_set_quality(cinfo, quality, b);
				}

				EXPORT_libijg16 void jpeg_simple_progression_16(j_compress_ptr cinfo)
				{
					jpeg_simple_progression(cinfo);
				}

				EXPORT_libijg16 void jpeg_simple_lossless_16(j_compress_ptr cinfo, int predictor, int point_transform)
				{
					jpeg_simple_lossless(cinfo, predictor, point_transform);
				}

				EXPORT_libijg16 void jpeg_set_colorspace_16(j_compress_ptr cinfo, J_COLOR_SPACE in_color_space) 
				{
					jpeg_set_colorspace(cinfo, in_color_space);
				}

				EXPORT_libijg16 void jpeg_start_compress_16(j_compress_ptr cinfo, bool b)
				{
					jpeg_start_compress(cinfo, b);
				}

				EXPORT_libijg16 void jpeg_write_scanlines_16(j_compress_ptr cinfo, JSAMPARRAY scanlines, JDIMENSION num_lines) 
				{
					jpeg_write_scanlines(cinfo, scanlines, num_lines);					
				}

				EXPORT_libijg16 void jpeg_finish_compress_16(j_compress_ptr cinfo) 
				{
					jpeg_finish_compress(cinfo);
				}

				EXPORT_libijg16 void jpeg_destroy_compress_16(j_compress_ptr cinfo)
				{
					jpeg_destroy_compress(cinfo);
				}

				//Decode JPEG_16

				EXPORT_libijg16 void jpeg_create_decompress_16(j_decompress_ptr dinfo) 
				{
					jpeg_create_decompress(dinfo);
				}

				EXPORT_libijg16 int jpeg_read_header_16(j_decompress_ptr dinfo, boolean require_image)
				{
					return jpeg_read_header(dinfo, require_image);
				}

				EXPORT_libijg16 void jpeg_calc_output_dimensions_16(j_decompress_ptr dinfo) 
				{
					jpeg_calc_output_dimensions(dinfo);
				}

				EXPORT_libijg16 int jpeg_start_decompress_16(j_decompress_ptr dinfo)
				{
					return jpeg_start_decompress(dinfo);
				} 

				EXPORT_libijg16 unsigned int jpeg_read_scanlines_16(j_decompress_ptr dinfo, JSAMPARRAY scanlines,
					JDIMENSION max_lines) 
				{
					return jpeg_read_scanlines(dinfo, scanlines, max_lines);
				}

				EXPORT_libijg16 void jpeg_destroy_decompress_16(j_decompress_ptr dinfo) 
				{
					jpeg_destroy_decompress(dinfo);
				}

				EXPORT_libijg16 boolean jpeg_resync_to_restart_16(j_decompress_ptr dinfo, int desired) 
				{
					return jpeg_resync_to_restart(dinfo, desired);
				}

				EXPORT_libijg16 void format_message_16(j_common_ptr cinfo, char* buffer)
				{
					(*cinfo->err->format_message)((jpeg_common_struct *)cinfo, buffer);
				}

#ifdef __cplusplus
			}
#endif

		}//Codec
	}//Imaging
}//Dicom
