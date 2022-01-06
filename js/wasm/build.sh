#!/usr/bin/env bash

USER_ID=`id -u`

# Create a Docker image that contains the Emscripten toolchain for building 
# WebAssembly. The steps to setup a machine with Emscripten can be seen in
# docker/build_system/Dockerfile
docker build -t fo-dicom/build docker/build_system/. \
    --build-arg "USER_ID=${USER_ID}" --build-arg "USER_NAME=${USER}"

# Build the library using the build system container
docker run --rm -v $(pwd)/../../:/src fo-dicom/build /bin/bash -c \
    'source /emsdk/emsdk_env.sh \
    && cd /src/js/wasm \
    && mkdir -p build \
    && cd build \
    && emcmake cmake ../ \
    && make'