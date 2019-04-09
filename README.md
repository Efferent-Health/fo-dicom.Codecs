# Dicom-Native

This is an alternative `TranscoderManager` class to be used along with the well-known [fo-dicom](https://github.com/fo-dicom/fo-dicom) project. It is associated to C++ libraries compiled natively for several platforms. So far:
- Windows 64 (similar to Native64.dll)
- Linux 64 (tested with Ubuntu 16/18 Desktop and Server)
- macOS (work in progress)

This transcoder differs from the existing fo-dicom implementations in the following:
- Unified C++ source code for all platforms
- Doesn't implement a Managed C++ middleware. Direct C# to C++ integration.
- Avoids pure C# implementation for performance reasons (with exception of RLE)

The following CODECS are implemented:
- JPEG
- JPEG 2000
- JPEG-LS
- RLE (C#)

## Usage

- Add the nuget package to your .Net Standard project (to be provided soon)
- Add the standard fo-dicom nuget packages to your project
- At the beginning of your application, replace the transcoder manager, as:
````C#
Dicom.Codec.TranscoderManager.SetImplementation(new Efferent.Native.NativeTranscoderManager());
````
