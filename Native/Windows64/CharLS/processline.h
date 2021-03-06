//
// (C) Jan de Vaan 2007-2010, all rights reserved. See the accompanying "License.txt" for licensed use.
//
#ifndef CHARLS_PROCESSLINE
#define CHARLS_PROCESSLINE

#include "util.h"
#include "publictypes.h"
#include <vector>
#include <sstream>
#include <cstring>
#include <algorithm>


//
// This file defines the ProcessLine base class, its derivitives and helper functions.
// During coding/decoding, CharLS process one line at a time. The different Processline implementations
// convert the uncompressed format to and from the internal format for encoding.
// Conversions include color transforms, line interleaved vs sample interleaved, masking out unused bits,
// accounting for line padding etc.
// This mechanism could be used to encode/decode images as they are received.
//

class ProcessLine
{
public:
    virtual ~ProcessLine() = default;
    virtual void NewLineDecoded(const void* pSrc, int pixelCount, int sourceStride) = 0;
    virtual void NewLineRequested(void* pDest, int pixelCount, int destStride) = 0;
};


class PostProcesSingleComponent : public ProcessLine
{
public:
    PostProcesSingleComponent(void* rawData, const JlsParameters& params, size_t bytesPerPixel) :
        _rawData(static_cast<uint8_t*>(rawData)),
        _bytesPerPixel(bytesPerPixel),
        _bytesPerLine(params.stride)
    {
    }

    void NewLineRequested(void* dest, int pixelCount, int /*byteStride*/) override
    {
        std::memcpy(dest, _rawData, pixelCount * _bytesPerPixel);
        _rawData += _bytesPerLine;
    }

    void NewLineDecoded(const void* pSrc, int pixelCount, int /*sourceStride*/) override
    {
        std::memcpy(_rawData, pSrc, pixelCount * _bytesPerPixel);
        _rawData += _bytesPerLine;
    }

private:
    uint8_t* _rawData;
    size_t _bytesPerPixel;
    size_t _bytesPerLine;
};


inline void ByteSwap(unsigned char* data, int count)
{
    if (count & 1)
    {
        std::ostringstream message;
        message << "An odd number of bytes (" << count << ") cannot be swapped.";
        throw charls_error(charls::ApiResult::InvalidJlsParameters, message.str());
    }

    auto data32 = reinterpret_cast<unsigned int*>(data);
    for(auto i = 0; i < count / 4; i++)
    {
        const auto value = data32[i];
        data32[i] = ((value >> 8) & 0x00FF00FF) | ((value & 0x00FF00FF) << 8);
    }

    if ((count % 4) != 0)
    {
        std::swap(data[count-2], data[count-1]);
    }
}

class PostProcesSingleStream : public ProcessLine
{
public:
    PostProcesSingleStream(std::basic_streambuf<char>* rawData, const JlsParameters& params, size_t bytesPerPixel) :
        _rawData(rawData),
        _bytesPerPixel(bytesPerPixel),
        _bytesPerLine(params.stride)
    {
    }

    void NewLineRequested(void* dest, int pixelCount, int /*destStride*/) override
    {
        auto bytesToRead = pixelCount * _bytesPerPixel;
        while (bytesToRead != 0)
        {
            const auto bytesRead = _rawData->sgetn(static_cast<char*>(dest), bytesToRead);
            if (bytesRead == 0)
                throw charls_error(charls::ApiResult::UncompressedBufferTooSmall);

            bytesToRead = static_cast<std::size_t>(bytesToRead - bytesRead);
        }

        if (_bytesPerPixel == 2)
        {
            ByteSwap(static_cast<unsigned char*>(dest), 2 * pixelCount);
        }

        if (_bytesPerLine - pixelCount * _bytesPerPixel > 0)
        {
            _rawData->pubseekoff(static_cast<std::streamoff>(_bytesPerLine - bytesToRead), std::ios_base::cur);
        }
    }

    void NewLineDecoded(const void* pSrc, int pixelCount, int /*sourceStride*/) override
    {
        const auto bytesToWrite = pixelCount * _bytesPerPixel;
        const auto bytesWritten = static_cast<size_t>(_rawData->sputn(static_cast<const char*>(pSrc), bytesToWrite));
        if (bytesWritten != bytesToWrite)
            throw charls_error(charls::ApiResult::UncompressedBufferTooSmall);
    }

private:
    std::basic_streambuf<char>* _rawData;
    size_t _bytesPerPixel;
    size_t _bytesPerLine;
};


template<typename TRANSFORM, typename SAMPLE>
void TransformLineToQuad(const SAMPLE* ptypeInput, int32_t pixelStrideIn, Quad<SAMPLE>* pbyteBuffer, int32_t pixelStride, TRANSFORM& transform)
{
    const int cpixel = std::min(pixelStride, pixelStrideIn);
    Quad<SAMPLE>* ptypeBuffer = pbyteBuffer;

    for (auto x = 0; x < cpixel; ++x)
    {
        Quad<SAMPLE> pixel(transform(ptypeInput[x], ptypeInput[x + pixelStrideIn], ptypeInput[x + 2*pixelStrideIn]), ptypeInput[x + 3 * pixelStrideIn]);
        ptypeBuffer[x] = pixel;
    }
}


template<typename TRANSFORM, typename SAMPLE>
void TransformQuadToLine(const Quad<SAMPLE>* pbyteInput, int32_t pixelStrideIn, SAMPLE* ptypeBuffer, int32_t pixelStride, TRANSFORM& transform)
{
    const auto cpixel = std::min(pixelStride, pixelStrideIn);
    const Quad<SAMPLE>* ptypeBufferIn = pbyteInput;

    for (auto x = 0; x < cpixel; ++x)
    {
        const Quad<SAMPLE> color = ptypeBufferIn[x];
        Quad<SAMPLE> colorTranformed(transform(color.v1, color.v2, color.v3), color.v4);

        ptypeBuffer[x] = colorTranformed.v1;
        ptypeBuffer[x + pixelStride] = colorTranformed.v2;
        ptypeBuffer[x + 2 * pixelStride] = colorTranformed.v3;
        ptypeBuffer[x + 3 * pixelStride] = colorTranformed.v4;
    }
}


template<typename SAMPLE>
void TransformRgbToBgr(SAMPLE* pDest, int samplesPerPixel, int pixelCount)
{
    for (auto i = 0; i < pixelCount; ++i)
    {
        std::swap(pDest[0], pDest[2]);
        pDest += samplesPerPixel;
    }
}


template<typename TRANSFORM, typename SAMPLE>
void TransformLine(Triplet<SAMPLE>* pDest, const Triplet<SAMPLE>* pSrc, int pixelCount, TRANSFORM& transform)
{
    for (auto i = 0; i < pixelCount; ++i)
    {
        pDest[i] = transform(pSrc[i].v1, pSrc[i].v2, pSrc[i].v3);
    }
}


template<typename TRANSFORM, typename SAMPLE>
void TransformLineToTriplet(const SAMPLE* ptypeInput, int32_t pixelStrideIn, Triplet<SAMPLE>* pbyteBuffer, int32_t pixelStride, TRANSFORM& transform)
{
    const auto cpixel = std::min(pixelStride, pixelStrideIn);
    Triplet<SAMPLE>* ptypeBuffer = pbyteBuffer;

    for (auto x = 0; x < cpixel; ++x)
    {
        ptypeBuffer[x] = transform(ptypeInput[x], ptypeInput[x + pixelStrideIn], ptypeInput[x + 2*pixelStrideIn]);
    }
}


template<typename TRANSFORM, typename SAMPLE>
void TransformTripletToLine(const Triplet<SAMPLE>* pbyteInput, int32_t pixelStrideIn, SAMPLE* ptypeBuffer, int32_t pixelStride, TRANSFORM& transform)
{
    const auto cpixel = std::min(pixelStride, pixelStrideIn);
    const Triplet<SAMPLE>* ptypeBufferIn = pbyteInput;

    for (auto x = 0; x < cpixel; ++x)
    {
        const Triplet<SAMPLE> color = ptypeBufferIn[x];
        Triplet<SAMPLE> colorTranformed = transform(color.v1, color.v2, color.v3);

        ptypeBuffer[x] = colorTranformed.v1;
        ptypeBuffer[x + pixelStride] = colorTranformed.v2;
        ptypeBuffer[x + 2 *pixelStride] = colorTranformed.v3;
    }
}


template<typename TRANSFORM>
class ProcessTransformed : public ProcessLine
{
    typedef typename TRANSFORM::SAMPLE SAMPLE;

public:
    ProcessTransformed(ByteStreamInfo rawStream, const JlsParameters& info, TRANSFORM transform) :
        _params(info),
        _templine(info.width * info.components),
        _buffer(info.width * info.components * sizeof(SAMPLE)),
        _transform(transform),
        _inverseTransform(transform),
        _rawPixels(rawStream)
    {
    }

    void NewLineRequested(void* dest, int pixelCount, int destStride) override
    {
        if (!_rawPixels.rawStream)
        {
            Transform(_rawPixels.rawData, dest, pixelCount, destStride);
            _rawPixels.rawData += _params.stride;
            return;
        }

        Transform(_rawPixels.rawStream, dest, pixelCount, destStride);
    }

    void Transform(std::basic_streambuf<char>* rawStream, void* dest, int pixelCount, int destStride)
    {
        std::streamsize bytesToRead = pixelCount * _params.components * sizeof(SAMPLE);
        while (bytesToRead != 0)
        {
            const auto read = rawStream->sgetn(reinterpret_cast<char*>(_buffer.data()), bytesToRead);
            if (read == 0)
            {
                std::ostringstream message;
                message << "No more bytes available in input buffer, still neededing " << read;
                throw charls_error(charls::ApiResult::UncompressedBufferTooSmall, message.str());
            }

            bytesToRead -= read;
        }
        Transform(_buffer.data(), dest, pixelCount, destStride);
    }

    void Transform(const void* source, void* dest, int pixelCount, int destStride)
    {
        if (_params.outputBgr)
        {
            memcpy(_templine.data(), source, sizeof(Triplet<SAMPLE>) * pixelCount);
            TransformRgbToBgr(_templine.data(), _params.components, pixelCount);
            source = _templine.data();
        }

        if (_params.components == 3)
        {
            if (_params.interleaveMode == charls::InterleaveMode::Sample)
            {
                TransformLine(static_cast<Triplet<SAMPLE>*>(dest), static_cast<const Triplet<SAMPLE>*>(source), pixelCount, _transform);
            }
            else
            {
                TransformTripletToLine(static_cast<const Triplet<SAMPLE>*>(source), pixelCount, static_cast<SAMPLE*>(dest), destStride, _transform);
            }
        }
        else if (_params.components == 4 && _params.interleaveMode == charls::InterleaveMode::Line)
        {
            TransformQuadToLine(static_cast<const Quad<SAMPLE>*>(source), pixelCount, static_cast<SAMPLE*>(dest), destStride, _transform);
        }
    }

    void DecodeTransform(const void* pSrc, void* rawData, int pixelCount, int byteStride)
    {
        if (_params.components == 3)
        {
            if (_params.interleaveMode == charls::InterleaveMode::Sample)
            {
                TransformLine(static_cast<Triplet<SAMPLE>*>(rawData), static_cast<const Triplet<SAMPLE>*>(pSrc), pixelCount, _inverseTransform);
            }
            else
            {
                TransformLineToTriplet(static_cast<const SAMPLE*>(pSrc), byteStride, static_cast<Triplet<SAMPLE>*>(rawData), pixelCount, _inverseTransform);
            }
        }
        else if (_params.components == 4 && _params.interleaveMode == charls::InterleaveMode::Line)
        {
            TransformLineToQuad(static_cast<const SAMPLE*>(pSrc), byteStride, static_cast<Quad<SAMPLE>*>(rawData), pixelCount, _inverseTransform);
        }

        if (_params.outputBgr)
        {
            TransformRgbToBgr(static_cast<SAMPLE*>(rawData), _params.components, pixelCount);
        }
    }

    void NewLineDecoded(const void* pSrc, int pixelCount, int sourceStride) override
    {
        if (_rawPixels.rawStream)
        {
            const std::streamsize bytesToWrite = pixelCount * _params.components * sizeof(SAMPLE);
            DecodeTransform(pSrc, _buffer.data(), pixelCount, sourceStride);

            const auto bytesWritten = _rawPixels.rawStream->sputn(reinterpret_cast<char*>(_buffer.data()), bytesToWrite);
            if (bytesWritten != bytesToWrite)
                throw charls_error(charls::ApiResult::UncompressedBufferTooSmall);
        }
        else
        {
            DecodeTransform(pSrc, _rawPixels.rawData, pixelCount, sourceStride);
            _rawPixels.rawData += _params.stride;
        }
    }

private:
    const JlsParameters& _params;
    std::vector<SAMPLE> _templine;
    std::vector<uint8_t> _buffer;
    TRANSFORM _transform;
    typename TRANSFORM::INVERSE _inverseTransform;
    ByteStreamInfo _rawPixels;
};


#endif
