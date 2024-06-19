# fo-dicom.Codecs

[![NuGet](https://img.shields.io/nuget/v/fo-dicom.Codecs.svg)](https://www.nuget.org/packages/fo-dicom.Codecs/)
[![github](https://img.shields.io/github/stars/Efferent-Health/fo-dicom.Codecs.svg)]()
[![Build Status](https://dev.azure.com/efferent/open-source/_apis/build/status/Efferent-Health.fo-dicom.Codecs?branchName=master)](https://dev.azure.com/efferent/open-source/_build/latest?definitionId=9&branchName=master)

<img src="https://lh3.googleusercontent.com/-Fq3nigRUo7U/VfaIPuJMjfI/AAAAAAAAALo/7oaLrrTBhnw/s1600/Fellow%2BOak%2BSquare%2BTransp.png" alt="fo-dicom logo" height="80" /><img src="efferent_logo.png" alt="Efferent logo" height="80" />

This is collaborative project mantained by [Fellow Oak Dicom](https://github.com/fo-dicom/fo-dicom) and [Efferent Health, LLC](https://efferenthealth.com).
The codecs in this repository are written in pure C/C++ code and wrapped with C# and netstandard2.0. The supported platforms so far are:
- Windows 64-bit (tested with Windows 10/11)
- Linux 64-bit Intel and ARM architectures (tested with Ubuntu 16/18/20 Desktop and Server)
- MacOS 64-bit Intel and Apple architectures (tested with Ventura/Sonoma)

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
- **High-Throughput JPEG 2000 (HTJ2K) encoding is already in beta phase!!!** (read https://github.com/Efferent-Health/fo-dicom.Codecs/issues/54)

## Usage
 
- Add the standard fo-dicom nuget packages to your project (tested with 4.0.8 and 5.0.3)
 
### With fo-dicom 4.0
- Add the [nuget package](https://www.nuget.org/packages/Efferent.Native/) to your .net Standard or .net Core project (minimum version is 2.0) 
- Add the standard fo-dicom nuget packages to your project (version 4.0.x)
- At the beginning of your application, replace the transcoder manager, as:
  ````C#
  Dicom.Imaging.Codec.TranscoderManager.SetImplementation(new Dicom.Imaging.NativeCodec.NativeTranscoderManager());
  ````
### With fo-dicom 5.0
- Add the [nuget package](https://www.nuget.org/packages/fo-dicom.Codecs) to your .net Standard or .NET project (minimum version is .NET 5.0) 
- Add the standard fo-dicom nuget packages to your project (version 5.0.x)
- At the beginning of your application, replace the transcoder manager, as:
  ````C#
  new DicomSetupBuilder()
    .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<FellowOakDicom.Imaging.NativeCodec.NativeTranscoderManager>())
    .SkipValidation()
    .Build();
  ````

## Dependencies

### Windows
It is required to have Visual C++ Redistributable v14 installed in the target Windows machine. Otherwise, it will throw a runtime error:

````
Unable to load DLL 'Dicom.Native': The specified module could not be found. (Exception from HRESULT: 0x8007007E)
````

The installer can be downloaded directly from https://aka.ms/vs/17/release/vc_redist.x64.exe

### Linux
The native library has been built on an Ubuntu 18 environment using GNU C Compiler version 7.4.x.

Therefore, it requires a GLIBC library runtime version 2.27 and GLIBCXX version 3.4. Otherwise, it can throw a runtime error like:

````
Unhandled Exception: System.DllNotFoundException: Unable to load shared library 'Dicom.Native.so' or one of its dependencies.
````

