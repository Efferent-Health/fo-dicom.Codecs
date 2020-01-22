using System;
using System.Runtime.InteropServices;
using System.ComponentModel.Composition;

using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;
using Dicom.IO;
using Dicom.IO.Buffer;

namespace Dicom.Imaging.NativeCodec
{
    [Flags]
    public enum CharlsInterleaveModeType
    {
        None = 0,
        Line = 1,
        Sample = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JpegLSPresetCodingParameters
    {
        public int MaximumSampleValue;
        public int Threshold1;
        public int Threshold2;
        public int Threshold3;
        public int ResetValue;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct JfifParameters
    {
        public int version;
        public int units;
        public int Xdensity;
        public int Ydensity;
        public int Xthumbnail;
        public int Ythumbnail;
        public void* thumbnail;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct JlsParameters
    {
        public int width;
        public int height;
        public int bitsPerSample;
        public int stride;
        public int components;
        public int allowedLossyError;
        public CharlsInterleaveModeType interleaveMode;
        public CharlsColorTransformationType colorTransformation;
        public JpegLSPresetCodingParameters custom;
        public JfifParameters jfif;
        public char outputBgr;
    }

    [Flags]
    public enum CharlsApiResultType
    {
        OK = 0,                              // The operation completed without errors.
        InvalidJlsParameters = 1,            // One of the JLS parameters is invalid.
        ParameterValueNotSupported = 2,      // The parameter value not supported.
        UncompressedBufferTooSmall = 3,      // The uncompressed buffer is too small to hold all the output.
        CompressedBufferTooSmall = 4,        // The compressed buffer too small, more input data was expected.
        InvalidCompressedData = 5,           // This error is returned when the encoded bit stream contains a general structural problem.
        TooMuchCompressedData = 6,           // Too much compressed data.The decoding proccess is ready but the input buffer still contains encoded data.
        ImageTypeNotSupported = 7,           // This error is returned when the bit stream is encoded with an option that is not supported by this implementation.
        UnsupportedBitDepthForTransform = 8, // The bit depth for transformation is not supported.
        UnsupportedColorTransform = 9,       // The color transformation is not supported.
        UnsupportedEncoding = 10,            // This error is returned when an encoded frame is found that is not encoded with the JPEG-LS algorithm.
        UnknownJpegMarker = 11,              // This error is returned when an unknown JPEG marker code is detected in the encoded bit stream.
        MissingJpegMarkerStart = 12,         // This error is returned when the algorithm expect a 0xFF code (indicates start of a JPEG marker) but none was found.
        UnspecifiedFailure = 13,             // This error is returned when the implementation detected a failure, but no specific error is available.
        UnexpectedFailure = 14               // This error is returned when the implementation encountered a failure it didn't expect. No guarantees can be given for the state after this error.
    }

    [Flags]
    public enum CharlsColorTransformationType
    {
        None = 0,
        HP1 = 1,
        HP2 = 2,
        HP3 = 3
    }

    public enum DicomJpegLsInterleaveMode
    {
        None = 0,

        Line = 1,

        Sample = 2
    }

    public enum DicomJpegLsColorTransform
    {
        None = 0,

        HP1 = 1,

        HP2 = 2,

        HP3 = 3
    }

    //JpegLS Codec Development

    public abstract class DicomJpegLsCodec : IDicomCodec
    {
        public string Name
        {
            get
            {
                return TransferSyntax.UID.Name;
            }
        }

        public abstract DicomTransferSyntax TransferSyntax { get; }

        public DicomCodecParams GetDefaultParameters()
        {
            return new DicomJpegLsParams();
        }

        // Import Functions


        public abstract void Encode(
            DicomPixelData oldPixelData,
            DicomPixelData newPixelData,
            DicomCodecParams parameters);

        public abstract void Decode(
            DicomPixelData oldPixelData,
            DicomPixelData newPixelData,
            DicomCodecParams parameters);
    }

    public abstract class DicomJpegLsNativeCodec : DicomJpegLsCodec
    {
        //For Encode JPEGLS Windows
        [DllImport("Dicom.Codec.win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSEncode")]

        public static extern unsafe CharlsApiResultType JpegLSEncode_Windows64(void* destination, uint destinationLength, uint* bytesWritten, void* source, uint sourceLength, ref JlsParameters obj, char[] errorMessage);

        //For Encode JPEGLS Windows
        [DllImport("Dicom.Codec.win64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSDecode")]

        public static extern unsafe CharlsApiResultType JpegLSDecode_Windows64(void* destination, int destinationLength, void* source, uint sourceLength, ref JlsParameters obj, char[] errorMessage);

        //For Encode JPEGLS Linux
        [DllImport("Dicom.Codec.linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSEncode")]

        public static extern unsafe CharlsApiResultType JpegLSEncode_Linux64(void* destination, uint destinationLength, uint* bytesWritten, void* source, uint sourceLength, ref JlsParameters obj, char[] errorMessage);

        //For Encode JPEGLS Linux
        [DllImport("Dicom.Codec.linux64.so", CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSDecode")]

        public static extern unsafe CharlsApiResultType JpegLSDecode_Linux64(void* destination, int destinationLength, void* source, uint sourceLength, ref JlsParameters obj, char[] errorMessage);

        //For Encode JPEGLS MacOS
        [DllImport("Dicom.Codec.macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSEncode")]

        public static extern unsafe CharlsApiResultType JpegLSEncode_MacOS(void* destination, uint destinationLength, uint* bytesWritten, void* source, uint sourceLength, ref JlsParameters obj, char[] errorMessage);

        //For Encode JPEGLS Linux
        [DllImport("Dicom.Codec.macOS.dylib", CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSDecode")]

        public static extern unsafe CharlsApiResultType JpegLSDecode_MacOS(void* destination, int destinationLength, void* source, uint sourceLength, ref JlsParameters obj, char[] errorMessage);
        public override unsafe void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new InvalidOperationException("Unsupported OS Platform");
            }

            if ((oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrPartial422) ||
            (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrPartial420))
            {
                throw new DicomCodecException("Photometric Interpretation '{0}' not supported by JPEG-LS encoder", oldPixelData.PhotometricInterpretation);
            }

            DicomJpegLsParams jparams = (DicomJpegLsParams)parameters;
            if (jparams == null)
            {
                jparams = (DicomJpegLsParams)GetDefaultParameters();
            }

            //IMPORT JLSPARAMETERS (DLLIMPORT)

            JlsParameters jls = new JlsParameters
            {  
                width = oldPixelData.Width,
                height = oldPixelData.Height,
                bitsPerSample = oldPixelData.BitsStored,
                stride = oldPixelData.BytesAllocated * oldPixelData.Width * oldPixelData.SamplesPerPixel,
                components = oldPixelData.SamplesPerPixel,
                interleaveMode = oldPixelData.SamplesPerPixel == 1
                    ? CharlsInterleaveModeType.None
                    : oldPixelData.PlanarConfiguration == PlanarConfiguration.Interleaved
                        ? CharlsInterleaveModeType.Sample
                        : CharlsInterleaveModeType.Line,
                colorTransformation = CharlsColorTransformationType.None
            };

            if (TransferSyntax == DicomTransferSyntax.JPEGLSNearLossless)
            {
					jls.allowedLossyError = jparams.AllowedError;
            }

            for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
            {
                IByteBuffer frameData = oldPixelData.GetFrame(frame);

                //Converting photmetricinterpretation YbrFull or YbrFull422 to RGB
                if(oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                {
                    frameData = PixelDataConverter.YbrFullToRgb(frameData);
                    oldPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
                }
                else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                {
                    frameData = PixelDataConverter.YbrFull422ToRgb(frameData, oldPixelData.Width);
                    oldPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
                }

                PinnedByteArray frameArray = new PinnedByteArray(frameData.Data);

                byte[] jpegData = new byte[frameData.Size];

                PinnedByteArray jpegArray = new PinnedByteArray(jpegData);
                uint jpegDataSize = 0;

                char[] errorMessage = new char[256];

                // IMPORT JpegLsEncode
                unsafe {  
                    if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    { 
                        CharlsApiResultType err = JpegLSEncode_Linux64((void*)jpegArray.Pointer, checked((uint)jpegArray.Count), &jpegDataSize, (void*)frameArray.Pointer, checked((uint)frameArray.Count),ref jls , errorMessage);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {   
                        CharlsApiResultType err = JpegLSEncode_Windows64((void*)jpegArray.Pointer, checked((uint)jpegArray.Count), &jpegDataSize, (void*)frameArray.Pointer, checked((uint)frameArray.Count),ref jls , errorMessage);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        CharlsApiResultType err = JpegLSEncode_MacOS((void*)jpegArray.Pointer, checked((uint)jpegArray.Count), &jpegDataSize, (void*)frameArray.Pointer, checked((uint)frameArray.Count),ref jls , errorMessage);
                    }

                    Array.Resize(ref jpegData,(int)jpegDataSize);

                    IByteBuffer buffer;
                    if (jpegDataSize >= (1 * 1024 * 1024) || oldPixelData.NumberOfFrames > 1)
                        buffer = new TempFileBuffer(jpegData);
                    else
                        buffer = new MemoryByteBuffer(jpegData);
                    buffer = EvenLengthBuffer.Create(buffer);
                    newPixelData.AddFrame(buffer);
                }
            }
        }

        public override void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new InvalidOperationException("Unsupported OS Platform");
            }

            for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
            {
                IByteBuffer jpegData = oldPixelData.GetFrame(frame);

                //Converting photmetricinterpretation YbrFull or YbrFull422 to RGB
                if(oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                {
                    jpegData = PixelDataConverter.YbrFullToRgb(jpegData);
                    oldPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
                }
                else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                {
                    jpegData = PixelDataConverter.YbrFull422ToRgb(jpegData, oldPixelData.Width);
                    oldPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
                }

                PinnedByteArray jpegArray = new PinnedByteArray(jpegData.Data);

                byte[] frameData = new byte[newPixelData.UncompressedFrameSize];

                PinnedByteArray frameArray = new PinnedByteArray(frameData);

                JlsParameters jls = new JlsParameters();

                char[] errorMessage = new char[256];

                // IMPORT JpegLsDecode 
                unsafe
                {   
                    if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) 
                    {
                        CharlsApiResultType err = JpegLSDecode_Linux64((void*)frameArray.Pointer, frameData.Length, (void*)jpegArray.Pointer, Convert.ToUInt32(jpegData.Size), ref jls, errorMessage);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                         CharlsApiResultType err = JpegLSDecode_Windows64((void*)frameArray.Pointer, frameData.Length, (void*)jpegArray.Pointer, Convert.ToUInt32(jpegData.Size), ref jls, errorMessage);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                         CharlsApiResultType err = JpegLSDecode_MacOS((void*)frameArray.Pointer, frameData.Length, (void*)jpegArray.Pointer, Convert.ToUInt32(jpegData.Size), ref jls, errorMessage);
                    }
                    
                    IByteBuffer buffer;
                    if (frameData.Length >= (1 * 1024 * 1024) || oldPixelData.NumberOfFrames > 1)
                        buffer = new TempFileBuffer(frameData);
                    else
                        buffer = new MemoryByteBuffer(frameData);
                    buffer = EvenLengthBuffer.Create(buffer);

                    newPixelData.AddFrame(buffer);
                }
            }
        }
    }

    [Export(typeof(IDicomCodec))]
    public class DicomJpegLsLosslessCodec : DicomJpegLsNativeCodec
    {

        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.JPEGLSLossless;
            }
        }
    }

    [Export(typeof(IDicomCodec))]
    public class DicomJpegLsNearLosslessCodec : DicomJpegLsNativeCodec
    {

        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.JPEGLSNearLossless;
            }
        }
    }
}