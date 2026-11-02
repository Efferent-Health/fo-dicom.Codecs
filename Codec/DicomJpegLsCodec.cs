using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO;
using FellowOakDicom.IO.Buffer;

namespace FellowOakDicom.Imaging.NativeCodec
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
    public struct Charls_frame_info
    {
        /// <summary>
        /// Width of the image, range [1, 100000].
        /// </summary>
        public uint width;

        /// <summary>
        /// Height of the image, range [1, 100000].
        /// </summary>
        public uint height;

        /// <summary>
        /// Number of bits per sample, range [2, 16]
        /// </summary>
        public int bits_per_sample;

        /// <summary>
        /// Number of components contained in the frame, range [1, 255]
        /// </summary>
        public int component_count;
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
        public sbyte outputBgr;
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
        UnexpectedFailure = 14,
        Unknown = 15// This error is returned when the implementation encountered a failure it didn't expect. No guarantees can be given for the state after this error.
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

        public enum DicomJpegLsInterleaveMode
        {
            None = 0,
            Line = 1,
            Sample = 2
        };

        public enum DicomJpegLsColorTransform
        {
            None = 0,
            HP1 = 1,
            HP2 = 2,
            HP3 = 3
        };

        public class DicomJpegLsParams : DicomCodecParams
        {
            private int _allowedError;
            private DicomJpegLsInterleaveMode _ilMode;
            private DicomJpegLsColorTransform _colorTransform;

            public DicomJpegLsParams()
            {
                _allowedError = 3;
                _ilMode = DicomJpegLsInterleaveMode.Line;
                _colorTransform = DicomJpegLsColorTransform.HP1;
            }

            public int AllowedError
            {
                get
                {
                    return _allowedError;
                }
                set
                {
                    _allowedError = value;
                }
            }

            public DicomJpegLsInterleaveMode InterleaveMode
            {
                get
                {
                    return _ilMode;
                }
                set
                {
                    _ilMode = value;
                }
            }

            public DicomJpegLsColorTransform ColorTransform
            {
                get
                {
                    return _colorTransform;
                }
                set
                {
                    _colorTransform = value;
                }
            }
        }

        public DicomCodecParams GetDefaultParameters()
        {
            return new DicomJpegLsParams();
        }

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
        public class EncodeSafeContextHandle : SafeHandle
        {
            public EncodeSafeContextHandle() : base(IntPtr.Zero, true) { }
            public override bool IsInvalid => handle == IntPtr.Zero;
            protected override bool ReleaseHandle()
            {
                Charls_jpegls_encoder_destroy(handle);
                return true;
            }
        }

        public class DecodeSafeContextHandle : SafeHandle
        {
            public DecodeSafeContextHandle() : base(IntPtr.Zero, true) {}
            public override bool IsInvalid => handle == IntPtr.Zero;
            protected override bool ReleaseHandle()
            {
                Charls_jpegls_decoder_destroy(handle);
                return true;
            }
        }

        //Encode JPEGLS for linux or macOS

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_encoder_create")]
        public static extern EncodeSafeContextHandle Charls_jpegls_encoder_create();

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_encoder_set_frame_info")]
        public static extern int Charls_jpegls_encoder_set_frame_info(EncodeSafeContextHandle encoder, ref Charls_frame_info frameInfo);
        
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_encoder_encode_from_buffer")]
        public static extern unsafe int Charls_jpegls_encoder_encode_from_buffer(EncodeSafeContextHandle encoder, void * sourceBuffer, ulong source_size_bytes, uint stride);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_encoder_set_near_lossless")]
        public static extern int Charls_jpegls_encoder_set_near_lossless(EncodeSafeContextHandle encoder, int near_lossless);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_encoder_get_estimated_destination_size")]
        public static unsafe extern int Charls_jpegls_encoder_get_estimated_destination_size(EncodeSafeContextHandle encoder, ulong* size_in_bytes);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_encoder_set_destination_buffer")]
        public static unsafe extern int Charls_jpegls_encoder_set_destination_buffer(EncodeSafeContextHandle encoder, void* destination_buffer, ulong destination_size_bytes);
        
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_encoder_get_bytes_written")]
        public static unsafe extern int Charls_jpegls_encoder_get_bytes_written(EncodeSafeContextHandle encoder, ulong* bytes_written);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_encoder_set_interleave_mode")]
        public static extern int Charls_jpegls_encoder_set_interleave_mode(EncodeSafeContextHandle encoder, CharlsInterleaveModeType interleaveModeType);
        
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_encoder_destroy")]
        public static extern void Charls_jpegls_encoder_destroy(IntPtr encoder);

        //Decode JPEGLS for linux or macOS
        
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_decoder_create")]
        public static extern DecodeSafeContextHandle Charls_jpegls_decoder_create();
        
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_decoder_set_source_buffer")]
        public static unsafe extern int Charls_jpegls_decoder_set_source_buffer(DecodeSafeContextHandle decoder, void * sourceBuffer, ulong source_size_bytes);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_decoder_read_header")]
        public static extern int Charls_jpegls_decoder_read_header(DecodeSafeContextHandle decoder);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_decoder_get_destination_size")]
        public static unsafe extern int Charls_jpegls_decoder_get_destination_size(DecodeSafeContextHandle decoder, uint stride, ulong* destination_size_bytes);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_decoder_decode_to_buffer")]
        public static unsafe extern int Charls_jpegls_decoder_decode_to_buffer(DecodeSafeContextHandle decoder, void * destination_buffer, ulong destination_size_bytes, uint stride);
        
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Charls_jpegls_decoder_destroy")]
        public static extern void Charls_jpegls_decoder_destroy(IntPtr decoder);

        // Native methods deprecated
        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSEncode")]
        public static extern unsafe CharlsApiResultType JpegLSEncode_win(void* destination, ulong destinationLength, ulong* bytesWritten, void* source, ulong sourceLength, ref JlsParameters obj, char[] errorMessage);

        //Decode JPEGLS for winx64
        [DllImport("Dicom.Native.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSDecode")]
        public static extern unsafe CharlsApiResultType JpegLSDecode_win(void* destination, ulong destinationLength, void* source, ulong sourceLength, ref JlsParameters obj, char[] errorMessage);

        //For Encode JPEGLS
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSEncode")]
        public static extern unsafe CharlsApiResultType JpegLSEncode(void* destination, ulong destinationLength, ulong* bytesWritten, void* source, ulong sourceLength, ref JlsParameters obj, char[] errorMessage);

        //For Decode JPEGLS
        [DllImport("Dicom.Native", CallingConvention = CallingConvention.Cdecl, EntryPoint = "JpegLSDecode")]
        public static extern unsafe CharlsApiResultType JpegLSDecode(void* destination, ulong destinationLength, void* source, ulong sourceLength, ref JlsParameters obj, char[] errorMessage);

        public override void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            // IMPORT JpegLsEncode
            unsafe
            {
                if (Platform.Current == Platform.Type.unsupported)
                {
                    throw new InvalidOperationException("Unsupported OS Platform");
                }

                if ((oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrPartial422) ||
                (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrPartial420))
                {
                    throw new DicomCodecException($"Photometric Interpretation {oldPixelData.PhotometricInterpretation} not supported by JPEG-LS encoder");
                }

                DicomJpegLsParams jparams = (DicomJpegLsParams)parameters;

                if (jparams == null)
                {
                    jparams = (DicomJpegLsParams)GetDefaultParameters();
                }

                var frameInfo = new Charls_frame_info
                {
                    width = oldPixelData.Width,
                    height = oldPixelData.Height,
                    component_count = oldPixelData.SamplesPerPixel,
                    bits_per_sample = oldPixelData.BitsStored
                };

                for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
                {
                    IByteBuffer frameData = oldPixelData.GetFrame(frame);

                    //Converting photometric interpretation YbrFull or YbrFull422 to RGB
                    if (oldPixelData.PlanarConfiguration == PlanarConfiguration.Planar && oldPixelData.SamplesPerPixel > 1)
                    {
                        if (oldPixelData.SamplesPerPixel != 3 || oldPixelData.BitsStored > 8)
                            throw new DicomCodecException("Planar reconfiguration only implemented for SamplesPerPixel=3 && BitsStored <= 8");

                        frameData = PixelDataConverter.PlanarToInterleaved24(new MemoryByteBuffer(frameData.Data));
                    }
                    else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                    {
                        frameData = PixelDataConverter.YbrFullToRgb(frameData);
                    }
                    else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                    {
                        frameData = PixelDataConverter.YbrFull422ToRgb(frameData, oldPixelData.Width);
                    }

                    PinnedByteArray frameArray = new PinnedByteArray(frameData.Data);

                    var stride = oldPixelData.Width * oldPixelData.SamplesPerPixel * (oldPixelData.BitsStored <= 8 ? 1 : oldPixelData.BytesAllocated);
                    CharlsInterleaveModeType interleaveMode = oldPixelData.SamplesPerPixel == 1
                        ? CharlsInterleaveModeType.None
                        : oldPixelData.PlanarConfiguration == PlanarConfiguration.Interleaved
                            ? CharlsInterleaveModeType.Sample
                            : CharlsInterleaveModeType.Line;
                        
                    //var colorTransformation = CharlsColorTransformationType.None;

                    try
                    {
                        using (EncodeSafeContextHandle encoder = Charls_jpegls_encoder_create())
                        {      
                            if (encoder.IsInvalid) throw new DicomCodecException("Failed to create JPEG-LS encoder.");
                            Charls_jpegls_encoder_set_frame_info(encoder, ref frameInfo);

                            ulong size_estimated = 0;
                            int result;

                            Charls_jpegls_encoder_get_estimated_destination_size(encoder, &size_estimated);
                            Charls_jpegls_encoder_set_interleave_mode(encoder, interleaveMode);

                            if (TransferSyntax == DicomTransferSyntax.JPEGLSNearLossless)
                            {
                                result = Charls_jpegls_encoder_set_near_lossless(encoder, jparams.AllowedError);
                                if (result != 0) throw new DicomCodecException($"Failed to set near-lossless mode: {result}");
                            }

                            var jpeglsArray = new PinnedByteArray((int)size_estimated);

                            fixed (byte * jpeglsDataPointer = jpeglsArray.Data)
                            {
                                result = Charls_jpegls_encoder_set_destination_buffer(encoder, jpeglsDataPointer, size_estimated);
                                if (result != 0)
                                    throw new DicomCodecException($"set_destination_buffer failed: {result}");

                                result = Charls_jpegls_encoder_encode_from_buffer(encoder, (void*)frameArray.Pointer, (ulong)frameArray.Data.Length, (uint)stride);
                                if (result != 0)
                                    throw new DicomCodecException($"Charls_jpegls_encoder_encode_from_buffer failed: {result}");
                                
                                ulong jpeglsDataSize = 0;
                                result = Charls_jpegls_encoder_get_bytes_written(encoder, &jpeglsDataSize);

                                var encoded = new byte[jpeglsDataSize];
                                Buffer.BlockCopy(jpeglsArray.Data, 0, encoded, 0, (int)jpeglsDataSize);

                                IByteBuffer buffer;

                                if (jpeglsDataSize >= NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                                {
                                    buffer = new TempFileBuffer(encoded);
                                    buffer = EvenLengthBuffer.Create(buffer);
                                }
                                else
                                {
                                    buffer = new MemoryByteBuffer(encoded);
                                }

                                if (oldPixelData.NumberOfFrames == 1)
                                    buffer = EvenLengthBuffer.Create(buffer);

                                newPixelData.AddFrame(buffer);
                            }
                        }
                    }
                    catch (DicomCodecException e)
                    {
                        throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                    }
                    catch (Exception e)
                    {
                        throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                    }
                    finally
                    {
                        frameArray?.Dispose();
                    }
                }
            }
        }

        public override void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            if (Platform.Current == Platform.Type.unsupported)
            {
                throw new InvalidOperationException("Unsupported OS Platform");
            }

            var uncompressedSize = newPixelData.Height * newPixelData.Width * newPixelData.SamplesPerPixel * newPixelData.BytesAllocated;

            for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
            {
                IByteBuffer jpegData = oldPixelData.GetFrame(frame);

                try
                {
                    //Converting photmetricinterpretation YbrFull or YbrFull422 to RGB
                    if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                    {
                        jpegData = PixelDataConverter.YbrFullToRgb(jpegData);
                    }
                    else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                    {
                        jpegData = PixelDataConverter.YbrFull422ToRgb(jpegData, oldPixelData.Width);
                    }
                }
                catch (Exception ex)
                {
                    throw new DicomCodecException($"Cannot convert JPEG-LS buffer data from PhotometricInterpretation = {oldPixelData.PhotometricInterpretation.ToString()} to RGB => {ex.Message} => {ex.StackTrace}");
                }

                PinnedByteArray jpeglsArray = new PinnedByteArray(jpegData.Data);

                var frameSize = uncompressedSize > newPixelData.UncompressedFrameSize ? uncompressedSize : newPixelData.UncompressedFrameSize;

                try
                {
                    unsafe
                    {   

                        using (var decoder = Charls_jpegls_decoder_create())
                        {
                            if (decoder.IsInvalid) throw new DicomCodecException("Failed to create JPEG-LS encoder.");

                            int result;

                            result = Charls_jpegls_decoder_set_source_buffer(decoder, (void *)jpeglsArray.Pointer, (ulong)jpeglsArray.Data.Length);
                            
                            if (result != 0)
                                throw new DicomCodecException($"Charls_jpegls_decoder_set_source_buffer failed with error code: {result}");

                            result = Charls_jpegls_decoder_read_header(decoder);

                            if (result != 0)
                                throw new DicomCodecException($"Charls_jpegls_decoder_read_header failed with error code: {result}");

                            ulong destination_size_bytes = (ulong)frameSize;
                            result = Charls_jpegls_decoder_get_destination_size(decoder, 0, &destination_size_bytes);
                            if (result != 0)
                                throw new DicomCodecException($"Charls_jpegls_decoder_get_destination_size failed with error code: {result}");

                            var decodeBuffer = new byte[(int)destination_size_bytes];

                            fixed (byte * decodeBufferPtr = decodeBuffer)
                            {
                                result = Charls_jpegls_decoder_decode_to_buffer(decoder, decodeBufferPtr, destination_size_bytes, 0);
                                if (result != 0)
                                    throw new DicomCodecException($"Charls_jpegls_decoder_decode_to_buffer failed with error code: {result}");
                                
                                IByteBuffer buffer;
                                if (destination_size_bytes >= NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                                    buffer = new TempFileBuffer(decodeBuffer);
                                else
                                    buffer = new MemoryByteBuffer(decodeBuffer);

                                if (oldPixelData.NumberOfFrames == 1)
                                    buffer = EvenLengthBuffer.Create(buffer);

                                newPixelData.AddFrame(buffer);
                            }
                        }
                    }
                }
                catch (DicomCodecException e)
                {
                    throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                }
                catch (Exception e)
                {
                    throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                }
                finally
                {
                    jpeglsArray?.Dispose();
                }
            }
        }
    }

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
