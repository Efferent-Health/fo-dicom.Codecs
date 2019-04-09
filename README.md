# Efferent-Native

This is an alternative `TranscoderManager` class to be used along with the well-known [fo-dicom](https://github.com/fo-dicom/fo-dicom) project. It is associated to C++ libraries compiled natively for several platforms. So far:
- Windows (similar to Native64.DLL)
- Linux (tested with Ubuntu 16/18)
- macOS (work in progress)

This transcoder differs from the existing fo-dicom implementations in the following:
- Doesn't implement a Managed C++ middleware. Pure C# to C++ integration.
- Avoids pure C# implementation for performance reasons (with exception of RLE)

The following CODECS are implemented:
- JPEG
- JPEG 2000
- JPEG-LS
- RLE (C#)

## Usage

- Add the nuget package to your .Net Standard project (to be provided soon)
- Add the standard fo-dicom nuget packages to your project
- At the beginning of your application, install the transcoder manager, as:
````C#
Dicom.Codec.TranscoderManager.SetImplementation(new Efferent.Native.NativeTranscoderManager());
````

