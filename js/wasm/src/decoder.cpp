
#include <cstdint>
#include <iostream>
#include <string>
#include <vector>

#include <emscripten.h>
#include <emscripten/bind.h>
#include <emscripten/val.h>

using namespace emscripten;

extern "C"
{
#include "Common/OpenJPEG/openjpeg.h"
#include "Common/OpenJPEG/j2k.h"

OPJ_CODEC_FORMAT GetCodecFormat(unsigned char* buffer);
}

class InputData
{
public:
    InputData(size_t size)
        : data_(size)
    {
    }

    val dataJS() const { return val(typed_memory_view(data_.size(), &data_[0])); }

    size_t size() const { return data_.size(); }
    uint8_t* data() { return data_.data(); }

private:
    std::vector<uint8_t> data_;
};

class OutputData
{
public:
    OutputData(opj_image_t* pImage)     // OutputData takes ownership of pImage and will destroy it in its destructor
        : pImage_(pImage)
    {
    }

    OutputData(OutputData&& rhs)        // Move constructor for return by value
    {
        pImage_ = rhs.pImage_;
        rhs.pImage_ = nullptr;
    } 

    virtual ~OutputData()
    {
        if (pImage_)
        {
            opj_image_destroy(pImage_);
            pImage_ = nullptr;
        }
    }

    bool hasFailed() const { return pImage_ == nullptr; }
    int width() const { return pImage_ ? pImage_->x1 : 0; }
    int height() const { return pImage_ ? pImage_->y1 : 0; }
    int numComponents() const { return pImage_ ? pImage_->numcomps : 0; }
    opj_image_comp_t getComponent(int compIdx) const
    {
        return pImage_->comps[compIdx];
    }

private:
    opj_image_t* pImage_;
};

// OpenJPEG Event handlers
void onOpenJpegInfo(const char *msg, void *client_data)
{
    std::cout << "Info OpenJPEG: " << msg << std::endl;
}

void onOpenJpegWarning(const char *msg, void *client_data)
{
    std::cout << "Warning OpenJPEG: " << msg << std::endl;
}

void onOpenJpegError(const char *msg, void *client_data)
{
    std::cout << "Error OpenJPEG: " << msg << std::endl;
}

OutputData decodeJpeg2000(InputData& inputData) 
{
    OPJ_CODEC_FORMAT codecFormat = GetCodecFormat(inputData.data());
    
    opj_dparameters_t dparams = {};
    opj_set_default_decoder_parameters(&dparams);
    dparams.cp_layer = 0;
    dparams.cp_reduce = 0;

    opj_event_mgr_t event_mgr = {};
    event_mgr.info_handler = onOpenJpegInfo;
    event_mgr.warning_handler = onOpenJpegWarning;
    event_mgr.error_handler = onOpenJpegError;

    opj_dinfo_t* dinfo = opj_create_decompress(codecFormat);

    dparams.decod_format = codecFormat;

    opj_set_event_mgr((opj_common_ptr)dinfo, &event_mgr, nullptr);

    opj_setup_decoder(dinfo, &dparams);

    opj_cio_t* cio = nullptr;
    opj_image_t* image = nullptr;

    bool opj_err = false;
    dinfo->client_data = (void*)&opj_err;

    cio = opj_cio_open((opj_common_ptr)dinfo, inputData.data(), inputData.size());
    OutputData result(opj_decode(dinfo, cio));  // If image decode fails then returned image pointer
                                                // will be null which can be seen with OutputData::hasFailed

    // Clean up
    if (cio != nullptr) 
        opj_cio_close(cio);
    
    if (dinfo != nullptr)
        opj_destroy_decompress(dinfo);

    return result;
}

// Getter and setter to extract component data
val getImageCompData(const opj_image_comp_t& comp) 
{ 
    return val(typed_memory_view(comp.w*comp.h, comp.data)); 
}

void setImageCompData(const opj_image_comp_t& comp, const int& data) 
{ 
    // No-op. We cannot set 
}

EMSCRIPTEN_BINDINGS(decoder) {
    value_object<opj_image_comp_t>("opj_image_comp_t")
        .field("dx", &opj_image_comp_t::dx)
        .field("dy", &opj_image_comp_t::dy)
        .field("w", &opj_image_comp_t::w)
        .field("h", &opj_image_comp_t::h)
        .field("x0", &opj_image_comp_t::x0)
        .field("y0", &opj_image_comp_t::y0)
        .field("prec", &opj_image_comp_t::prec)
        .field("bpp", &opj_image_comp_t::bpp)
        .field("sgnd", &opj_image_comp_t::sgnd)
        .field("resno_decoded", &opj_image_comp_t::resno_decoded)
        .field("factor", &opj_image_comp_t::factor)
        .field("data", &getImageCompData, &setImageCompData);

    class_<InputData>("InputData")
        .constructor<size_t>()
        .property("dataJS", &InputData::dataJS);

    class_<OutputData>("OutputData")
        .property("hasFailed", &OutputData::hasFailed)
        .property("width", &OutputData::width)
        .property("height", &OutputData::height)
        .property("numComponents", &OutputData::numComponents)
        .function("getComponent", &OutputData::getComponent);    

    function("decodeJpeg2000", &decodeJpeg2000);

    register_vector<uint8_t>("vector<uint8_t>");
}