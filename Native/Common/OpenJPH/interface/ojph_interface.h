#pragma once

#ifdef __cplusplus
    #include <cstdint>
    #include <cstddef>
#else
    #include <stdint.h>
#endif

#include "../common/ojph_arch.h"
#include "../common/ojph_file.h"
#include "../common/ojph_mem.h"
#include "../common/ojph_params.h"
#include "../codestream/ojph_params_local.h"
#include "../common/ojph_codestream.h"

struct Htj2k_outdata
{
    unsigned char* j2c_buffer;
    size_t size_outbuffer;
};

struct Decoded_outdata
{
    unsigned char* raw_buffer;
    size_t size_outbuffer;
};

struct Ipoint
{
    Ipoint(ui32 x = 0, ui32 y = 0) : x(x), y(y) {}
    ui32 x;
    ui32 y;
};

struct Isize
{
    Isize(ui32 width = 0, ui32 height = 0) : width(width), height(height) {}
    ui32 width;
    ui32 height;
};

struct Frameinfo
{
    /// <summary>
    /// Width of the image, range [1, 65535].
    /// </summary>
    uint16_t width{0};

    /// <summary>
    /// Height of the image, range [1, 65535].
    /// </summary>
    uint16_t height{0};

    /// <summary>
    /// Number of bits per sample, range [2, 16]
    /// </summary>
    uint8_t bitsPerSample{0};

    /// <summary>
    /// Number of components contained in the frame, range [1, 255]
    /// </summary>
    uint8_t componentCount{0};

    /// <summary>
    /// true if signed, false if unsigned
    /// </summary>
    bool isSigned{false};

    /// <summary>
    /// true if color transform is used, false if not
    /// </summary>
    bool isUsingColorTransform{false};

    /// <summary>
    /// true if lossless, false is lossy
    /// </summary>
    bool isReversible{true};
};

void HTJpeg2000EncodeStream(Htj2k_outdata* htj2k_outinfo, const unsigned char* source, size_t sourceLength, const struct Frameinfo* finfo, ojph::PROGRESSION_ORDER progression_order);
void HTJpeg2000DecodeStream(Decoded_outdata* raw_outinfo, const unsigned char* source, size_t sourceLength);