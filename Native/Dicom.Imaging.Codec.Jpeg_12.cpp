// Copyright (c) 2012-2025 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

#include <iostream>
#include <new>

#if defined(_WIN32)
#define EXPORT_libijg12  __declspec(dllexport)
extern "C" {
#include "stdio.h"
#include "string.h"
#include "setjmp.h"
#include "./Common/libijg12/jpeglib12.h"
#include "./Common/libijg12/jerror12.h"
#include "./Common/libijg12/jpegint12.h"
}

#elif defined(__linux__)
#define EXPORT_libijg12  extern
extern "C" {
#include "stdio.h"
#include "string.h"
#include "setjmp.h"
#include "./Common/libijg12/jpeglib12.h"
#include "./Common/libijg12/jerror12.h"
#include "./Common/libijg12/jpegint12.h"
} 

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_libijg12 extern
		extern "C"{
		#include "stdio.h"
		#include "string.h"
		#include "setjmp.h"
		#include "./Common/libijg12/jpeglib12.h"
		#include "./Common/libijg12/jerror12.h"
		#include "./Common/libijg12/jpegint12.h"
		}
    #endif

#endif

namespace Dicom {
	namespace Imaging {
		namespace Codec {

#ifdef __cplusplus
			extern "C" {
#endif
				//Encode JPEG_12

				EXPORT_libijg12  jpeg_error_mgr*  jpeg_std_error_12(struct jpeg_error_mgr * err)
				{
					return jpeg12_std_error(err);	
				}

				EXPORT_libijg12 void  jpeg_create_compress_12(j_compress_ptr cinfo)
				{	
					jpeg_create_compress(cinfo);					
				}

				EXPORT_libijg12 void  jpeg_set_defaults_12(j_compress_ptr cinfo)
				{
					jpeg_set_defaults(cinfo);	
				}

				EXPORT_libijg12 void  jpeg_set_quality_12(j_compress_ptr cinfo, int quality, int b)
				{
					jpeg_set_quality(cinfo, quality, b);					
				}

				EXPORT_libijg12 void  jpeg_simple_progression_12(j_compress_ptr cinfo) 
				{
					jpeg_simple_progression(cinfo);
				}

				EXPORT_libijg12 void  jpeg_simple_lossless_12(j_compress_ptr cinfo, int predictor, int point_transform)
				{
					jpeg_simple_lossless(cinfo, predictor, point_transform);	
				}

				EXPORT_libijg12 void  jpeg_set_colorspace_12(j_compress_ptr cinfo, J_COLOR_SPACE in_color_space)
				{
					jpeg_set_colorspace(cinfo, in_color_space);	
				}

				EXPORT_libijg12 void  jpeg_start_compress_12(j_compress_ptr cinfo, int b)
				{
					jpeg_start_compress(cinfo, b);	
				}

				EXPORT_libijg12 unsigned int jpeg_write_scanlines_12(j_compress_ptr cinfo, JSAMPARRAY scanlines, JDIMENSION num_lines)
				{
					return jpeg_write_scanlines(cinfo, scanlines, num_lines);			
				}

				EXPORT_libijg12 void  jpeg_finish_compress_12(j_compress_ptr cinfo) 
				{
					jpeg_finish_compress(cinfo);	
				}

				EXPORT_libijg12 void  jpeg_destroy_compress_12(j_compress_ptr cinfo)
				{
					jpeg_destroy_compress(cinfo);				
				}

				//Decode JPEG_12

				EXPORT_libijg12 void jpeg_create_decompress_12(j_decompress_ptr dinfo)
				{
					jpeg_create_decompress(dinfo);
				}

				EXPORT_libijg12 int jpeg_read_header_12(j_decompress_ptr dinfo, boolean require_image)
				{
					return jpeg_read_header(dinfo, require_image);
				}

				EXPORT_libijg12 void jpeg_calc_output_dimensions_12(j_decompress_ptr dinfo)
				{
					jpeg_calc_output_dimensions(dinfo);	
				}

				EXPORT_libijg12 int jpeg_start_decompress_12(j_decompress_ptr dinfo) 
				{	
					return jpeg_start_decompress(dinfo);	
				}

				EXPORT_libijg12 unsigned int jpeg_read_scanlines_12(j_decompress_ptr dinfo, JSAMPARRAY scanlines,
					JDIMENSION max_lines)
				{
					return jpeg_read_scanlines(dinfo, scanlines, max_lines);
				}

				EXPORT_libijg12 void jpeg_destroy_decompress_12(j_decompress_ptr dinfo)
				{
					jpeg_destroy_decompress(dinfo);
				}

				EXPORT_libijg12 boolean jpeg_resync_to_restart_12(j_decompress_ptr dinfo, int desired )
				{
					return jpeg_resync_to_restart(dinfo, desired);
				}

#ifdef __cplusplus
			}
#endif

		}//Codec
	}//Imaging
}//Dicom
