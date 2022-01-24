This directory contains bindings to allow the JPEG 2000 codex to be compiled to
WebAssembly and used from Javascript.

## Building the JS library

The library is built using Emscripten. Emscripten can be tricky to setup at times
so a containerised build process is provided to give repeatable builds. On Linux run

```bash
./build.sh
```

to build the library. This will setup a Docker container that has Emscripten installed in
it and then use the Docker container to build the library using CMake. If you would rather
not use the Docker containerised build then you can use docker/build_system/Dockerfile as
a guide for setting up your environment, and then use the commands given in build.sh to
build the project using CMake. These are

```bash
mkdir build
cd build
emcmake cmake ../   
make
```

## Example Usage

Once the library is built, open example.html to see the library in action.

The library provides a straightforward interface for decoding pixel data that is encoded
in JPEG2000 format.

First include the DicomJpeg2000Decoder.js file, this provides a global object called Module that exposes input and output data structures along with a decode routine decodeJpeg2000.

Then, assuming you have a block of pixel data encoded in JPEG2000 format, copy it into an InputData object and call decodeJpeg2000.

```js
let inputData = new Module.InputData(pixelData.length)
inputData.dataJS.set(pixelData)
let jpegOutputData = Module.decodeJpeg2000(inputData)
```

The OutputData object has a hasFailed property that you can use to check for success. If hasFailed == false then you can use the OutputData object to get access to the decoded color components of the image stored in opj_image_comp_t structs. Conversion to an RGBA buffer
for display on a HTML Canvas is *not* done by the WASM module but should instead be done in
external JS code to reduce load on the WASM heap. The supplied example.html contains rough color space conversions for MONOCHROME1, MONOCHROME2 and RGB so that the library can be seen
in action.

Once decoding of the image has been completed and the decoded pixel data extracted from the OutputData object, both the InputData and OutputData objects should be deleted to free their
memory back to the WASM heap.

```js
inputData.delete()
jpegOutputData.delete()
```