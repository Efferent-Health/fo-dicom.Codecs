# fo-dicom.Codecs

[![NuGet](https://img.shields.io/nuget/v/fo-dicom.Codecs.svg)](https://www.nuget.org/packages/fo-dicom.Codecs/)
[![github](https://img.shields.io/github/stars/Efferent-Health/fo-dicom.Codecs.svg)]()
[![Build Status](https://dev.azure.com/efferent/open-source/_apis/build/status/Efferent-Health.fo-dicom.Codecs?branchName=master)](https://dev.azure.com/efferent/open-source/_build/latest?definitionId=9&branchName=master)
[![Join the chat at https://gitter.im/fo-dicom/fo-dicom](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/fo-dicom/fo-dicom?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

> **ANNOUNCEMENT: Please check our incoming Milestone:** 
> https://github.com/Efferent-Health/fo-dicom.Codecs/milestone/1

<img src="https://lh3.googleusercontent.com/-Fq3nigRUo7U/VfaIPuJMjfI/AAAAAAAAALo/7oaLrrTBhnw/s1600/Fellow%2BOak%2BSquare%2BTransp.png" alt="fo-dicom logo" height="80" /><img src="efferent_logo.png" alt="Efferent logo" height="80" />

This is collaborative project mantained by [Fellow Oak Dicom](https://github.com/fo-dicom/fo-dicom) and [Efferent Health, LLC](https://efferenthealth.com).
The codecs in this repository are written in pure C/C++ code and wrapped with C# and netstandard2.0. The supported platforms so far are:
- Windows 64-bit (tested with Windows 10)
- Linux 64-bit (tested with Ubuntu 16/18/20 Desktop and Server)
- MacOS 64-bit Intel and Apple architectures (tested with Ventura)

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
