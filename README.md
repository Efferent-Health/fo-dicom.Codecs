<img src="fo-dicom_logo.png" alt="fo-dicom logo" height="80" /> <img src="efferent_logo.png" alt="Efferent logo" height="80" />

# fo-dicom.Codecs

[![NuGet](https://img.shields.io/nuget/v/fo-dicom.Codecs.svg)](https://www.nuget.org/packages/fo-dicom.Codecs/)
![downloads](https://img.shields.io/nuget/dt/fo-dicom.Codecs)
![github](https://img.shields.io/github/stars/Efferent-Health/fo-dicom.Codecs?style=flat&color=yellow)
![build](https://github.com/Efferent-Health/fo-dicom.Codecs/actions/workflows/main.yml/badge.svg?branch=master)

This is collaborative project mantained by [Fellow Oak Dicom](https://github.com/fo-dicom/fo-dicom) and [Efferent, Inc.](https://efferenthealth.com).
The codecs in this repository are written in pure C/C++ code and wrapped with C# and netstandard2.0. The supported platforms so far are:

|OS|&numsp;&numsp;x64&numsp;&numsp;|Arm64|Tested with|
|:--|:--:|:--:|:--|
|Windows|:white_check_mark:|:white_check_mark:|Windows 10/11|
|Linux|:white_check_mark:|:white_check_mark:|Ubuntu 18/20/22/24|
|MacOS|:white_check_mark:|:white_check_mark:|Ventura/Sonoma/Sequoia|

## Supported CODECs

The following CODECS are implemented:

|Transfer Syntax UID|Description|
|:--|--|
|1.2.840.10008.1.2.5|RLE Lossless|
|1.2.840.10008.1.2.4.50|JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression|
|1.2.840.10008.1.2.4.51|JPEG Extended (Process 2 & 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression (Process 4 only)|
|1.2.840.10008.1.2.4.57|JPEG Lossless, Non-Hierarchical (Process 14)|
|1.2.840.10008.1.2.4.70|JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression|
|1.2.840.10008.1.2.4.80|JPEG-LS Lossless Image Compression|
|1.2.840.10008.1.2.4.81|JPEG-LS Lossy (Near-Lossless) Image Compression|
|1.2.840.10008.1.2.4.90|JPEG 2000 Image Compression (Lossless Only)|
|1.2.840.10008.1.2.4.91|JPEG 2000 Image Compression|
|1.2.840.10008.1.2.4.201|High-Throughput JPEG 2000 Image Compression (Lossless Only)|
|1.2.840.10008.1.2.4.202|High-Throughput JPEG 2000 with RPCL Options Image Compression (Lossless Only)|
|1.2.840.10008.1.2.4.203|High-Throughput JPEG 2000 Image Compression|

The HT-JPEG2000 family of codecs is only supported since fo-dicom version 5.1.3

## Usage
  
### With fo-dicom 4.x
- Add the [nuget package](https://www.nuget.org/packages/Efferent.Native/) to your .NET Standard or .NET Core project (minimum version is 2.0) 
- Add the standard fo-dicom nuget packages to your project (version 4.x)
- At the beginning of your application, replace the transcoder manager, as:
  ````C#
  Dicom.Imaging.Codec.TranscoderManager.SetImplementation(new Dicom.Imaging.NativeCodec.NativeTranscoderManager());
  ````
### With fo-dicom 5.x
- Add the [nuget package](https://www.nuget.org/packages/fo-dicom.Codecs) to your .NET Standard or .NET project (minimum version is .NET 5.0) 
- Add the standard fo-dicom nuget packages to your project (version 5.x)
- At the beginning of your application, replace the transcoder manager, as:
  ````C#
  new DicomSetupBuilder()
    .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<FellowOakDicom.Imaging.NativeCodec.NativeTranscoderManager>())
    .SkipValidation()
    .Build();
  ````

### Temporary file management

The transcoders use temporary files during the conversion of the pixel data, and the required space surpasses a threshold of 1 MB by default.
This treshold can be adjusted by setting a static field, as follows:

````C#
NativeTranscoderManager.MemoryBufferThreshold = // new value in bytes
````

As with any application, the temporary files won't be deleted in case of a crash or Operating System shutdown.

## Dependencies

### Windows
It is required to have Visual C++ Redistributable v14 installed in the target Windows machine. Otherwise, it will throw a runtime error:

````
Unable to load DLL 'Dicom.Native': The specified module could not be found. (Exception from HRESULT: 0x8007007E)
````

The installer can be downloaded directly from https://aka.ms/vs/17/release/vc_redist.x64.exe

### Linux
The native library has been built on an Ubuntu 20.04 environment using GNU C Compiler version 9.4.x.

Therefore, it requires a GLIBC library runtime version 2.27 and GLIBCXX version 3.4. Otherwise, it can throw a runtime error like:

````
Unhandled Exception: System.DllNotFoundException: Unable to load shared library 'Dicom.Native.so' or one of its dependencies.
````

