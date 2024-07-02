
#pragma once

#include <exception>
#include <memory>

#include "../common/ojph_arch.h"
#include "../common/ojph_file.h"
#include "../common/ojph_mem.h"
#include "../common/ojph_params.h"
#include "../common/ojph_codestream.h"

#include <vector>

namespace
{
    class encoded_buffer : public ojph::outfile_base
    {
        public:
            /**  A constructor */
            OJPH_EXPORT encoded_buffer() {}
            /**  A destructor */
            OJPH_EXPORT ~encoded_buffer() {}

            OJPH_EXPORT
            void open(size_t initial_size = 65536)
            {   
                buffer_.resize(0);
                buffer_.reserve(initial_size);
            }

            OJPH_EXPORT
            virtual size_t write(const void *ptr, size_t size)
            {
                auto bytes = reinterpret_cast<uint8_t const*>(ptr);
                buffer_.insert(buffer_.end(), bytes, bytes + size);
                return size;
            }

            OJPH_EXPORT
            virtual ojph::si64 tell() { return buffer_.size(); }

            OJPH_EXPORT
            virtual void close() {}

            OJPH_EXPORT
            const ojph::ui8* get_data() { return buffer_.data(); }

            OJPH_EXPORT
            const ojph::ui8* get_data() const { return buffer_.data(); }
            
            OJPH_EXPORT
            const std::vector<uint8_t>& getBuffer() const {return buffer_;}

        private:
            std::vector<uint8_t> buffer_;
    };
}