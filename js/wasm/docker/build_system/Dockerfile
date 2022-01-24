
FROM ubuntu:20.04
ARG EMSCRIPTEN_TAG=2.0.10
ARG USER_ID
ARG USER_NAME

RUN apt-get update && DEBIAN_FRONTEND=noninteractive apt-get install -y \
    git python3 cmake cmake-curses-gui

# Sync and install the Emscripten toolchain
WORKDIR /
RUN git clone --depth 1 --branch ${EMSCRIPTEN_TAG} https://github.com/emscripten-core/emsdk.git

WORKDIR /emsdk
RUN ./emsdk install ${EMSCRIPTEN_TAG}
RUN ./emsdk activate ${EMSCRIPTEN_TAG}
RUN chmod 777 upstream/emscripten && chmod -R 777 upstream/emscripten/cache

# Build a test program to confirm that Emscripten is setup correctly
WORKDIR /emsdk/build_test
ADD test_prog.cpp ./
RUN /bin/bash -c 'source /emsdk/emsdk_env.sh && emcc test_prog.cpp -s USE_BOOST_HEADERS=1'

# Setup username and id so that it matches host machine (otherwise files will be written as root)
RUN useradd -u ${USER_ID} ${USER_NAME}
ENV USER=${USER_NAME}
USER ${USER_NAME}


