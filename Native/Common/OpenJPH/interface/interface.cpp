#include <iostream>
#include <new>
#include <climits>

#include "interface.h"
#include "encoded_buffer.h"

#include <vector>

using namespace ojph;
using namespace std;

void HTJpeg2000EncodeStream(Htj2k_outdata *j2c_outinfo, const unsigned char *source, size_t sourceLength, const struct Frameinfo *finfo, ojph::PROGRESSION_ORDER progression_order)
{
    size_t decompositions_ = 5;
    bool request_tlm_marker_ = true;
    bool set_tilepart_divisions_at_components_ = false;
    bool set_tilepart_divisions_at_resolutions_ = true;
    float quantizationStep_ = -1.0f;
    size_t progressionOrder_ = progression_order; // RPCL

    std::vector<Ipoint> downSamples_;
    Ipoint imageOffset_;
    Isize tileSize_;
    Ipoint tileOffset_;
    Isize blockDimensions_ = Isize(64, 64);
    std::vector<Isize> precincts_;
    precincts_.resize(0);

    const size_t bytesPerPixelInfo = (finfo->bitsPerSample + 8 - 1) / 8;
    const size_t decodedSizeInfo = finfo->width * finfo->height * finfo->componentCount * bytesPerPixelInfo;
    downSamples_.resize(finfo->componentCount);
    for (int c = 0; c < finfo->componentCount; ++c)
    {
        downSamples_[c].x = 1;
        downSamples_[c].y = 1;
    }

    // Set source data
    std::vector<uint8_t> rawdata;
    rawdata.resize(sourceLength);
    rawdata.insert(rawdata.begin(), source, source + decodedSizeInfo);

    encoded_buffer encoder;
    encoder.open();

    // Setup image size parameters
    ojph::codestream codestream;
    ojph::param_siz siz = codestream.access_siz();
    siz.set_image_extent(ojph::point(finfo->width, finfo->height));
    int num_comps = finfo->componentCount;

    siz.set_num_components(num_comps);

    for (int c = 0; c < num_comps; ++c)
    {
        siz.set_component(c, ojph::point(downSamples_[c].x, downSamples_[c].y), finfo->bitsPerSample, finfo->isSigned);
    }

    siz.set_image_offset(point(imageOffset_.x, imageOffset_.y));
    siz.set_tile_size(ojph::size(tileSize_.width, tileSize_.height));
    siz.set_tile_offset(point(tileOffset_.x, tileOffset_.y));

    // Setup encoding parameters
    ojph::param_cod cod = codestream.access_cod();
    cod.set_num_decomposition(decompositions_);
    cod.set_block_dims(blockDimensions_.width, blockDimensions_.height);
    std::vector<ojph::size> precincts;
    precincts.resize(precincts_.size());

    for (size_t i = 0; i < precincts_.size(); i++)
    {
        precincts[i].w = precincts_[i].width;
        precincts[i].h = precincts_[i].height;
    }
    cod.set_precinct_size(precincts_.size(), precincts.data());

    if (progressionOrder_ >= 0)
    {
        const char *progOrders[] = {"LRCP", "RLCP", "RPCL", "PCRL", "CPRL"};
        switch (progressionOrder_)
        {
            case 0:
                cod.set_progression_order(progOrders[0]);
                break;
            case 1:
                cod.set_progression_order(progOrders[1]);
                break;
            case 2:
                cod.set_progression_order(progOrders[2]);
                break;
            case 3:
                cod.set_progression_order(progOrders[3]);
                break;
            case 4:
                cod.set_progression_order(progOrders[4]);
                break;
            default:
                break;
        }
    }
     
    cod.set_color_transform(finfo->isUsingColorTransform);
    bool lossless = finfo->isReversible;

    cod.set_reversible(lossless);
    if (!lossless)
    {
        codestream.access_qcd().set_irrev_quant(quantizationStep_);
    }

    codestream.set_tilepart_divisions(set_tilepart_divisions_at_resolutions_, set_tilepart_divisions_at_components_);
    codestream.request_tlm_marker(request_tlm_marker_);
    codestream.set_planar(finfo->isUsingColorTransform == false);

    codestream.write_headers(&encoder);

    // Encode the image
    const size_t bytesPerPixel = finfo->bitsPerSample / 8;
    ojph::ui32 next_comp;
    ojph::line_buf *cur_line = codestream.exchange(NULL, next_comp);

    siz = codestream.access_siz();
    int height = siz.get_image_extent().y - siz.get_image_offset().y;

    for (size_t y = 0; y < height; y++)
    {
        for (size_t c = 0; c < siz.get_num_components(); c++)
        {
            int *dp = cur_line->i32;

            if (finfo->bitsPerSample <= 8)
            {
                uint8_t *pIn = (uint8_t *)(source + (y * finfo->width * bytesPerPixel * siz.get_num_components()) + c);
                for (size_t x = 0; x < finfo->width; x++)
                {
                    *dp++ = *pIn;
                    pIn += siz.get_num_components();
                }
            }
            else
            {
                if (finfo->isSigned)
                {
                    int16_t *pIn = (int16_t *)(source + (y * finfo->width * bytesPerPixel));
                    for (size_t x = 0; x < finfo->width; x++)
                    {
                        *dp++ = *pIn++;
                    }
                }
                else
                {
                    uint16_t *pIn = (uint16_t *)(source + (y * finfo->width * bytesPerPixel));
                    for (size_t x = 0; x < finfo->width; x++)
                    {
                        *dp++ = *pIn++;
                    }
                }
            }

            cur_line = codestream.exchange(cur_line, next_comp);
        }
    }

    codestream.flush();

    j2c_outinfo->size_outbuffer = encoder.getBuffer().size();
    j2c_outinfo->j2c_buffer = new unsigned char[j2c_outinfo->size_outbuffer];

    memcpy(j2c_outinfo->j2c_buffer, encoder.getBuffer().data(), sizeof(unsigned char) * (int)j2c_outinfo->size_outbuffer);

    // cleanup
    codestream.close();
    encoder.close();
}

void HTJpeg2000DecodeStream(Decoded_outdata *raw_outinfo, const unsigned char *source, size_t sourceLength)
{
    ojph::codestream codestream;
    ojph::mem_infile mem_file;
    mem_file.open((ui8*)source, sourceLength);

    // read headers
    codestream.enable_resilience();
    codestream.read_headers(&mem_file);

    // getting and setting decoding parameters
    Frameinfo frameInfo;
    ojph::param_siz siz = codestream.access_siz();
    frameInfo.width = siz.get_image_extent().x - siz.get_image_offset().x;
    frameInfo.height = siz.get_image_extent().y - siz.get_image_offset().y;
    frameInfo.componentCount = siz.get_num_components();
    frameInfo.bitsPerSample = siz.get_bit_depth(0);
    frameInfo.isSigned = siz.is_signed(0);

    std::vector<Ipoint> downSamples;
    downSamples.resize(frameInfo.componentCount);

    for (size_t i = 0; i < frameInfo.componentCount; i++)
    {
        downSamples[i].x = siz.get_downsampling(i).x;
        downSamples[i].y = siz.get_downsampling(i).y;
    }

    Ipoint imageOffset;
    imageOffset.x = siz.get_image_offset().x;
    imageOffset.y = siz.get_image_offset().y;

    Isize tileSize;
    tileSize.width = siz.get_tile_size().w;
    tileSize.height = siz.get_tile_size().h;

    Ipoint tileOffset;
    tileOffset.x = siz.get_tile_offset().x;
    tileOffset.y = siz.get_tile_offset().y;

    ojph::param_cod cod = codestream.access_cod();
    size_t numDecompositions = cod.get_num_decompositions();
    bool isReversible = cod.is_reversible();
    int progressionOrder = cod.get_progression_order();

    Isize blockDimensions;
    blockDimensions.width = cod.get_block_dims().w;
    blockDimensions.height = cod.get_block_dims().h;

    std::vector<Isize> precincts;
    precincts.resize(numDecompositions);
    for (size_t i = 0; i < numDecompositions; i++)
    {
        precincts[i].width = cod.get_precinct_size(i).w;
        precincts[i].height = cod.get_precinct_size(i).h;
    }

    int numLayers_ = cod.get_num_layers();
    frameInfo.isUsingColorTransform = cod.is_using_color_transform();

    // calculate the resolution at the requested decomposition level and allocate destination buffer
    int decompositionLevel = 0;
    //Isize sizeAtDecompositionLevel = calculateSizeAtDecompositionLevel(decompositionLevel, frameInfo);
    Isize sizeAtDecompositionLevel(frameInfo.width, frameInfo.height);

    int resolutionLevel = numDecompositions - decompositionLevel;

    const size_t bytesPerPixel = (frameInfo.bitsPerSample + 8 - 1) / 8;
    const size_t destinationSize = sizeAtDecompositionLevel.width * sizeAtDecompositionLevel.height * frameInfo.componentCount * bytesPerPixel;
    std::vector<uint8_t> decoded_buffer;
    decoded_buffer.resize(destinationSize);

    // set the level to read to and reconstruction level to the specified decompositionLevel
    codestream.restrict_input_resolution(decompositionLevel, decompositionLevel);

    if (frameInfo.componentCount == 1)
    {
      codestream.set_planar(true);
    }
    else
    {
      if (frameInfo.isUsingColorTransform)
      {
        codestream.set_planar(false);
      }
      else
      {
        codestream.set_planar(true);
      }
    }
    codestream.create();

    // Extract the data line by line
    ojph::ui32 comp_num;
    for (int y = 0; y < sizeAtDecompositionLevel.height; y++)
    {
      size_t lineStart = y * sizeAtDecompositionLevel.width * frameInfo.componentCount * bytesPerPixel;
      if (frameInfo.componentCount == 1)
      {
        ojph::line_buf *line = codestream.pull(comp_num);
        if (frameInfo.bitsPerSample <= 8)
        {
          unsigned char *pOut = (unsigned char *)&(decoded_buffer)[lineStart];
          for (size_t x = 0; x < sizeAtDecompositionLevel.width; x++)
          {
            int val = line->i32[x];
            pOut[x] = std::max(0, std::min(val, UCHAR_MAX));
          }
        }
        else
        {
          if (frameInfo.isSigned)
          {
            short *pOut = (short *)&(decoded_buffer)[lineStart];
            for (size_t x = 0; x < sizeAtDecompositionLevel.width; x++)
            {
              int val = line->i32[x];
              pOut[x] = std::max(SHRT_MIN, std::min(val, SHRT_MAX));
            }
          }
          else
          {
            unsigned short *pOut = (unsigned short *)&(decoded_buffer)[lineStart];
            for (size_t x = 0; x < sizeAtDecompositionLevel.width; x++)
            {
              int val = line->i32[x];
              pOut[x] = std::max(0, std::min(val, USHRT_MAX));
            }
          }
        }
      }
      else
      {
        for (int c = 0; c < frameInfo.componentCount; c++)
        {
          ojph::line_buf *line = codestream.pull(comp_num);
          if (frameInfo.bitsPerSample <= 8)
          {
            uint8_t *pOut = &(decoded_buffer)[lineStart] + c;
            for (size_t x = 0; x < sizeAtDecompositionLevel.width; x++)
            {
              int val = line->i32[x];
              pOut[x * frameInfo.componentCount] = std::max(0, std::min(val, UCHAR_MAX));
            }
          }
          else
          {
            if (frameInfo.isSigned)
            {
              short *pOut = (short *)&(decoded_buffer)[lineStart] + c;
              for (size_t x = 0; x < sizeAtDecompositionLevel.width; x++)
              {
                int val = line->i32[x];
                pOut[x * frameInfo.componentCount] = std::max(SHRT_MIN, std::min(val, SHRT_MAX));
              }
            }
            else
            {
              unsigned short *pOut = (unsigned short *)&(decoded_buffer)[lineStart] + c;
              for (size_t x = 0; x < sizeAtDecompositionLevel.width; x++)
              {
                int val = line->i32[x];
                pOut[x * frameInfo.componentCount] = std::max(0, std::min(val, USHRT_MAX));
              }
            }
          }
        }
      }
    }

    raw_outinfo->raw_buffer = new unsigned char[destinationSize];
    raw_outinfo->size_outbuffer = destinationSize;

    memcpy(raw_outinfo->raw_buffer, decoded_buffer.data(), sizeof(unsigned char) * (int)raw_outinfo->size_outbuffer);

    codestream.close();
    decoded_buffer.clear();
}

/*Isize calculateSizeAtDecompositionLevel(int decompositionLevel, Frameinfo frameInfo)
{
    Isize result(frameInfo.width, frameInfo.height);
    while (decompositionLevel > 0)
    {
        result.width = ojph_div_ceil(result.width, 2);
        result.height = ojph_div_ceil(result.height, 2);
        decompositionLevel--;
    }
    return result;
}*/