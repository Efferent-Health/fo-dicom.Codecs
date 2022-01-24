project (decoder)
cmake_minimum_required(VERSION 3.5.1)

# Increase this value if you get out of memory errors when decoding
# large images. Caution must be exercised though, as too large can cause
# the code to fail to load on mobile browsers.
set(TOTAL_WASM_MEMORY "128MB" CACHE STRING "")

add_library(openjpeg
    "../../Native/Common/OpenJPEG/bio.c"
    "../../Native/Common/OpenJPEG/cidx_manager.c"
    "../../Native/Common/OpenJPEG/cio.c"
    "../../Native/Common/OpenJPEG/dwt.c"
    "../../Native/Common/OpenJPEG/event.c"
    "../../Native/Common/OpenJPEG/image.c"
    "../../Native/Common/OpenJPEG/j2k_lib.c"
    "../../Native/Common/OpenJPEG/j2k.c"
    "../../Native/Common/OpenJPEG/jp2.c"
    "../../Native/Common/OpenJPEG/jpt.c"
    "../../Native/Common/OpenJPEG/mct.c"
    "../../Native/Common/OpenJPEG/mqc.c"
    "../../Native/Common/OpenJPEG/openjpeg.c"
    "../../Native/Common/OpenJPEG/phix_manager.c"
    "../../Native/Common/OpenJPEG/pi.c"
    "../../Native/Common/OpenJPEG/ppix_manager.c"
    "../../Native/Common/OpenJPEG/raw.c"
    "../../Native/Common/OpenJPEG/t1_generate_luts.c"
    "../../Native/Common/OpenJPEG/t1.c"
    "../../Native/Common/OpenJPEG/t2.c"
    "../../Native/Common/OpenJPEG/tcd.c"
    "../../Native/Common/OpenJPEG/tgt.c"
    "../../Native/Common/OpenJPEG/thix_manager.c"
    "../../Native/Common/OpenJPEG/tpix_manager.c"
    )

target_compile_options(openjpeg PRIVATE
    "-std=c11"
    "-O3"
    "-msimd128")

target_compile_definitions(openjpeg PRIVATE 
    __SSE__
    __SSE2__)

add_executable(DicomJpeg2000Decoder
    "../../Native/Dicom.Imaging.Codec.Jpeg2000.cpp"
    "src/decoder.cpp")

target_include_directories(DicomJpeg2000Decoder PRIVATE
    "../../Native")

target_compile_definitions(DicomJpeg2000Decoder PRIVATE 
    __linux__)

message(STATUS "Setting compilation target to WASM")
string(CONCAT WASM_LINK_FLAGS 
    "--bind -s WASM=1 -s ENVIRONMENT='web' -s SINGLE_FILE=1 "
    "-s INITIAL_MEMORY=${TOTAL_WASM_MEMORY}")
set_target_properties(DicomJpeg2000Decoder PROPERTIES LINK_FLAGS ${WASM_LINK_FLAGS})

target_link_libraries(DicomJpeg2000Decoder
    openjpeg
)

# Uncomment and edit these lines to install in a target location. You will also
# need to add a call to make install in ./build.sh
#install(TARGETS DicomJpeg2000Decoder
#    DESTINATION "/src/js/wasm/example/build/wasm")
