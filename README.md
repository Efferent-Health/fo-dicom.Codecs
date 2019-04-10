# Dicom-Native

This is an alternative `TranscoderManager` class to be used along with the well-known [fo-dicom](https://github.com/fo-dicom/fo-dicom) project. It is associated to C++ libraries compiled natively for several platforms. So far:
- Windows 64 (similar to Native64.dll)
- Linux 64 (tested with Ubuntu 16/18 Desktop and Server)
- macOS (work in progress)

This transcoder differs from the existing fo-dicom implementations in the following:
- Unified C++ source code for all platforms
- Doesn't implement a Managed C++ middleware. Direct C# to C++ integration.
- Avoids pure C# implementation for performance reasons (with exception of RLE)

## Supported CODECs

The following CODECS are implemented:
- JPEG 2000 Image Compression (Lossless Only)
- JPEG 2000 Image Compression
- JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression
- JPEG Extended (Process 2 & 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression - (Process 4 only)
- JPEG Lossless, Non-Hierarchical (Process 14)
- JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression
- JPEG-LS Lossless Image Compression
- JPEG-LS Lossy (Near-Lossless) Image Compression
- RLE Lossless

## Usage

- Add the nuget package to your .Net Standard project (to be provided soon)
- Add the standard fo-dicom nuget packages to your project
- At the beginning of your application, replace the transcoder manager, as:
````C#
Dicom.Imaging.Codec.TranscoderManager.SetImplementation(new Efferent.Native.Codec.NativeTranscoderManager());
````
