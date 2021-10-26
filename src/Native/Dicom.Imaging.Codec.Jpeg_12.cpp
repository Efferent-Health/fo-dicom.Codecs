// Copyright (c) 2012-2021 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

#include <iostream>
#include <new>

#if defined(_WIN32)
#define EXPORT_libijg12  __declspec(dllexport)
extern "C" {
#include "stdio.h"
#include "string.h"
#include "setjmp.h"
#include "libijg12/jpeglib12.h"
#include "libijg12/jerror12.h"
#include "libijg12/jpegint12.h"
}

#elif defined(__linux__)
#define EXPORT_libijg12  extern
extern "C" {
#include "stdio.h"
#include "string.h"
#include "setjmp.h"
#include "./Linux64/libijg12/jpeglib12.h"
#include "./Linux64/libijg12/jerror12.h"
#include "./Linux64/libijg12/jpegint12.h"
} 

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_libijg12 extern
		extern "C"{
		#include "stdio.h"
		#include "string.h"
		#include "setjmp.h"
		#include "./MacOS/libijg12/jpeglib12.h"
		#include "./MacOS/libijg12/jerror12.h"
		#include "./MacOS/libijg12/jpegint12.h"
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

				EXPORT_libijg12  jpeg_error_mgr*  jpeg_std_error_12(struct jpeg_error_mgr * err) {
					try
					{
						return jpeg12_std_error(err);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 void  jpeg_create_compress_12(j_compress_ptr cinfo) {
					try
					{
						jpeg_create_compress(cinfo);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 void  jpeg_set_defaults_12(j_compress_ptr cinfo) {
					try
					{
						jpeg_set_defaults(cinfo);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}				
					
				}

				EXPORT_libijg12 void  jpeg_set_quality_12(j_compress_ptr cinfo, int quality, int b) {
					try
					{
						jpeg_set_quality(cinfo, quality, b);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 void  jpeg_simple_progression_12(j_compress_ptr cinfo) {
					try
					{
						jpeg_simple_progression(cinfo);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}

				}

				EXPORT_libijg12 void  jpeg_simple_lossless_12(j_compress_ptr cinfo, int predictor, int point_transform) {
					try
					{
						jpeg_simple_lossless(cinfo, predictor, point_transform);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 void  jpeg_set_colorspace_12(j_compress_ptr cinfo, J_COLOR_SPACE in_color_space) {
					try
					{
						jpeg_set_colorspace(cinfo, in_color_space);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 void  jpeg_start_compress_12(j_compress_ptr cinfo, int b) {
					try
					{
						jpeg_start_compress(cinfo, b);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 unsigned int jpeg_write_scanlines_12(j_compress_ptr cinfo, JSAMPARRAY scanlines, JDIMENSION num_lines) {
					try
					{
						return jpeg_write_scanlines(cinfo, scanlines, num_lines);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 void  jpeg_finish_compress_12(j_compress_ptr cinfo) {
					try
					{
						jpeg_finish_compress(cinfo);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 void  jpeg_destroy_compress_12(j_compress_ptr cinfo) {
					try
					{
						jpeg_destroy_compress(cinfo);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
									
				}

				//Decode JPEG_12

				EXPORT_libijg12 void jpeg_create_decompress_12(j_decompress_ptr dinfo) {
					try
					{
						jpeg_create_decompress(dinfo);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 int jpeg_read_header_12(j_decompress_ptr dinfo, boolean require_image) {
					try
					{
						return jpeg_read_header(dinfo, require_image);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 void jpeg_calc_output_dimensions_12(j_decompress_ptr dinfo) {
					try
					{
						jpeg_calc_output_dimensions(dinfo);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 int jpeg_start_decompress_12(j_decompress_ptr dinfo) {
					try
					{
						return jpeg_start_decompress(dinfo);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 unsigned int jpeg_read_scanlines_12(j_decompress_ptr dinfo, JSAMPARRAY scanlines,
					JDIMENSION max_lines) {
						try
						{
							return jpeg_read_scanlines(dinfo, scanlines, max_lines);
						}
						catch(const std::bad_alloc& e)
						{
							std::cerr << e.what() << '\n';
						}
						
				}

				EXPORT_libijg12 void jpeg_destroy_decompress_12(j_decompress_ptr dinfo) {
					try
					{
						jpeg_destroy_decompress(dinfo);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 boolean jpeg_resync_to_restart_12(j_decompress_ptr dinfo, int desired ) {
					try
					{
						return jpeg_resync_to_restart(dinfo, desired);
					}
					catch(const std::bad_alloc& e)
					{
						std::cerr << e.what() << '\n';
					}
					
				}

				EXPORT_libijg12 void format_message_12(j_common_ptr cinfo, char* buffer){
					(*cinfo->err->format_message)((jpeg_common_struct *)cinfo, buffer);
				}

#ifdef __cplusplus
			}
#endif

		}//Codec
	}//Imaging
}//Dicom
