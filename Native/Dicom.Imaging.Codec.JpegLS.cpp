// Copyright (c) 2012-2026 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

#include <iostream>
#include <new>

#include "./Common/CharLS/charls/charls_jpegls_decoder.h"
#include "./Common/CharLS/charls/charls_jpegls_encoder.h"

#if defined (_WIN32)
#define EXPORT_Charls __declspec(dllexport)

#elif defined(__linux__)
#define EXPORT_Charls extern 

#elif defined(__APPLE__)
#include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #define EXPORT_Charls extern
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

EXPORT_Charls CharlsApiResultType JpegLSDecode(void * destination, size_t destinationLength, void* source, size_t sourceLength, JlsParameters* obj, char* errorMessage)
{
    return JpegLsDecode(destination, destinationLength, source, sourceLength, obj, errorMessage);     
}


//Charls methods for encoding

EXPORT_Charls void * Charls_jpegls_encoder_create()
{
    return charls_jpegls_encoder_create();
}

EXPORT_Charls void Charls_jpegls_encoder_destroy(const charls_jpegls_encoder *encoder)
{
    charls_jpegls_encoder_destroy(encoder);
}

EXPORT_Charls charls_jpegls_errc Charls_jpegls_encoder_set_frame_info(charls_jpegls_encoder* encoder, charls_frame_info* frameinfo)
{
    return charls_jpegls_encoder_set_frame_info(encoder, frameinfo);
}

EXPORT_Charls charls::jpegls_errc Charls_jpegls_encoder_set_near_lossless(charls_jpegls_encoder* encoder, const int32_t near_lossless)
{
    return charls_jpegls_encoder_set_near_lossless(encoder, near_lossless);
}

EXPORT_Charls charls_jpegls_errc Charls_jpegls_encoder_encode_from_buffer(charls_jpegls_encoder* encoder, const void* source_buffer,
                                    const size_t source_size_bytes, const uint32_t stride)
{
    return charls_jpegls_encoder_encode_from_buffer(encoder, source_buffer, source_size_bytes, stride);
}

EXPORT_Charls charls::jpegls_errc Charls_jpegls_encoder_get_estimated_destination_size(const charls_jpegls_encoder* encoder, size_t* size_in_bytes)
{
    return charls_jpegls_encoder_get_estimated_destination_size(encoder, size_in_bytes);
}

EXPORT_Charls charls::jpegls_errc Charls_jpegls_encoder_set_destination_buffer(charls_jpegls_encoder* encoder, void* destination_buffer, const size_t destination_size_bytes)
{ 
    return charls_jpegls_encoder_set_destination_buffer(encoder, destination_buffer, destination_size_bytes);
}

EXPORT_Charls charls::jpegls_errc Charls_jpegls_encoder_set_interleave_mode(charls_jpegls_encoder* encoder, const charls_interleave_mode interleave_mode)
{
    return charls_jpegls_encoder_set_interleave_mode(encoder, interleave_mode);
}

EXPORT_Charls charls::jpegls_errc Charls_jpegls_encoder_get_bytes_written(const charls_jpegls_encoder* encoder, size_t* bytes_written)
{
    return charls_jpegls_encoder_get_bytes_written(encoder, bytes_written);
}

//Charls methods for decoding

EXPORT_Charls void * Charls_jpegls_decoder_create()
{
    return charls_jpegls_decoder_create();
}

EXPORT_Charls charls::jpegls_errc Charls_jpegls_decoder_set_source_buffer(charls_jpegls_decoder * decoder, const void * source_buffer, const size_t source_size_bytes)
{
    return charls_jpegls_decoder_set_source_buffer(decoder, source_buffer, source_size_bytes);
}

EXPORT_Charls charls::jpegls_errc Charls_jpegls_decoder_read_header(charls_jpegls_decoder * decoder)
{
    return charls_jpegls_decoder_read_header(decoder);
}

EXPORT_Charls charls::jpegls_errc Charls_jpegls_decoder_get_destination_size(const charls_jpegls_decoder* decoder, const uint32_t stride, size_t* destination_size_bytes)
{
    return charls_jpegls_decoder_get_destination_size(decoder, stride, destination_size_bytes);
} 

EXPORT_Charls charls::jpegls_errc Charls_jpegls_decoder_decode_to_buffer(charls_jpegls_decoder* decoder, void* destination_buffer,
            const size_t destination_size_bytes, const uint32_t stride)
{
    return charls_jpegls_decoder_decode_to_buffer(decoder, destination_buffer, destination_size_bytes, stride);
}

EXPORT_Charls void Charls_jpegls_decoder_destroy(const charls_jpegls_decoder *decode)
{
    Charls_jpegls_decoder_destroy(decode);
}


#ifdef __cplusplus
}
#endif

}//Codec
}//Imaging
}//Dicom