using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO;
using FellowOakDicom.IO.Buffer;

using FellowOakDicom.Imaging.NativeCodec.Jpeg;

namespace FellowOakDicom.Imaging.NativeCodec
{
    // libjpeg colorspace identifiers. Kept as a managed enum so the codec can
    // map a DICOM PhotometricInterpretation to the value passed across the
    // native ABI (which exchanges the colorspace as a plain int).
    public enum J_COLOR_SPACE
    {
        JCS_UNKNOWN,            /* error/unspecified */
        JCS_GRAYSCALE,          /* monochrome */
        JCS_RGB,                /* red/green/blue */
        JCS_YCbCr,              /* Y/Cb/Cr (also known as YUV) */
        JCS_CMYK,               /* C/M/Y/K */
        JCS_YCCK
    }

    public enum DicomJpegSampleFactor
    {
        SF444,
        SF422,
        Unknown
    }

    public class DicomJpegParams : DicomCodecParams
    {
        public int Quality { get; set; }
        public int SmoothingFactor { get; set; }
        public bool ConvertColorSpaceToRGB { get; set; }
        public DicomJpegSampleFactor SampleFactor { get; set; }
        public int Predictor { get; set; }
        public int PointTransform { get; set; }

        public DicomJpegParams()
        {
            Quality = 90;
            SmoothingFactor = 0;
            ConvertColorSpaceToRGB = false;
            SampleFactor = DicomJpegSampleFactor.SF444;
            Predictor = 1;
            PointTransform = 0;
        }
    }

    public abstract class DicomJpegCodec : IDicomCodec
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
            return new DicomJpegParams();
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

    namespace Jpeg
    {
        public enum JpegMode : int
        {
            Baseline,
            Sequential,
            SpectralSelection,
            Progressive,
            Lossless
        };

        public abstract class JpegNativeCodec
        {
            public abstract void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams jpegParams, int frame);
            public abstract void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams jpegParams, int frame);

            internal abstract int ScanHeaderForPrecision(DicomPixelData pixelData, bool isjPEG, int frame = 0);
            internal JpegMode Mode;
            internal int Predictor;
            internal int PointTransform;
            internal int Bits;
        };

        // High-level JPEG codec over libjpeg-turbo. All of the per-field codec
        // orchestration lives in the native glue (Dicom.Imaging.Codec.Jpeg.cpp);
        // this managed layer only marshals buffers and parameters across a small,
        // stable ABI:
        //   DicomJpegEncode / DicomJpegDecode / DicomJpegReadPrecision / DicomJpegFreeBuffer
        public class JpegCodec : JpegNativeCodec
        {
            // ---- Native ABI (Windows: Dicom.Native.dll) ----
            [DllImport("Dicom.Native.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DicomJpegEncode")]
            private static extern unsafe int DicomJpegEncode_win(
                byte* pixelData, uint width, uint height, int inputComponents, int inColorSpace,
                int mode, int dataPrecision, int quality, int smoothingFactor, int predictor,
                int pointTransform, int sampleFactor, uint rowStride,
                out IntPtr outBuffer, out uint outSize, out int outJpegColorSpace,
                byte[] errorMessage, uint errorMessageSize);

            [DllImport("Dicom.Native.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DicomJpegDecode")]
            private static extern unsafe int DicomJpegDecode_win(
                byte* jpegData, uint jpegSize, int convertColorSpaceToRGB, int isSigned,
                out IntPtr outPixels, out uint outSize, out uint outWidth, out uint outHeight,
                out int outComponents, out int outPrecision, out int outColorSpace, out uint outRowSize,
                byte[] errorMessage, uint errorMessageSize);

            [DllImport("Dicom.Native.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DicomJpegReadPrecision")]
            private static extern unsafe int DicomJpegReadPrecision_win(
                byte* jpegData, uint jpegSize, out int outPrecision, byte[] errorMessage, uint errorMessageSize);

            [DllImport("Dicom.Native.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DicomJpegFreeBuffer")]
            private static extern unsafe void DicomJpegFreeBuffer_win(IntPtr buffer);

            // ---- Native ABI (Unix: Dicom.Native[.so/.dylib]) ----
            [DllImport("Dicom.Native", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DicomJpegEncode")]
            private static extern unsafe int DicomJpegEncode(
                byte* pixelData, uint width, uint height, int inputComponents, int inColorSpace,
                int mode, int dataPrecision, int quality, int smoothingFactor, int predictor,
                int pointTransform, int sampleFactor, uint rowStride,
                out IntPtr outBuffer, out uint outSize, out int outJpegColorSpace,
                byte[] errorMessage, uint errorMessageSize);

            [DllImport("Dicom.Native", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DicomJpegDecode")]
            private static extern unsafe int DicomJpegDecode(
                byte* jpegData, uint jpegSize, int convertColorSpaceToRGB, int isSigned,
                out IntPtr outPixels, out uint outSize, out uint outWidth, out uint outHeight,
                out int outComponents, out int outPrecision, out int outColorSpace, out uint outRowSize,
                byte[] errorMessage, uint errorMessageSize);

            [DllImport("Dicom.Native", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DicomJpegReadPrecision")]
            private static extern unsafe int DicomJpegReadPrecision(
                byte* jpegData, uint jpegSize, out int outPrecision, byte[] errorMessage, uint errorMessageSize);

            [DllImport("Dicom.Native", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DicomJpegFreeBuffer")]
            private static extern unsafe void DicomJpegFreeBuffer(IntPtr buffer);

            public JpegCodec(JpegMode mode, int predictor, int point_transform, int bits)
            {
                Mode = mode;
                Predictor = predictor;
                PointTransform = point_transform;
                Bits = bits;
            }

            private static bool IsWindows =>
                Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64);

            public static J_COLOR_SPACE getJpegColorSpace(PhotometricInterpretation photometricInterpretation)
            {
                if (photometricInterpretation == PhotometricInterpretation.Rgb)
                    return J_COLOR_SPACE.JCS_RGB;
                else if (photometricInterpretation == PhotometricInterpretation.Monochrome1 || photometricInterpretation == PhotometricInterpretation.Monochrome2)
                    return J_COLOR_SPACE.JCS_GRAYSCALE;
                else if (photometricInterpretation == PhotometricInterpretation.PaletteColor)
                    return J_COLOR_SPACE.JCS_UNKNOWN;
                else if (photometricInterpretation == PhotometricInterpretation.YbrFull || photometricInterpretation == PhotometricInterpretation.YbrFull422 || photometricInterpretation == PhotometricInterpretation.YbrPartial422)
                    return J_COLOR_SPACE.JCS_YCbCr;
                else
                    return J_COLOR_SPACE.JCS_UNKNOWN;
            }

            private static string ErrorText(byte[] buffer)
            {
                int len = Array.IndexOf(buffer, (byte)0);
                if (len < 0) len = buffer.Length;
                return System.Text.Encoding.ASCII.GetString(buffer, 0, len);
            }

            public override unsafe void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams jpegParams, int frame)
            {
                if (Platform.Current == Platform.Type.unsupported)
                {
                    throw new InvalidOperationException("Unsupported OS Platform");
                }

                if ((oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrIct) || (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrRct))
                    throw new DicomCodecException($"Photometric Interpretation {oldPixelData.PhotometricInterpretation} not supported by JPEG encoder!");

                PinnedByteArray frameArray;

                if (oldPixelData.BitsAllocated == 16 && oldPixelData.BitsStored <= 8)
                {
                    IByteBuffer frameBuffer = oldPixelData.GetFrame(frame);
                    frameArray = new PinnedByteArray(ByteConverter.UnpackLow16(frameBuffer).Data);
                }
                else
                {
                    frameArray = new PinnedByteArray(oldPixelData.GetFrame(frame).Data);

                    if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                    {
                        frameArray = new PinnedByteArray(PixelDataConverter.YbrFull422ToRgb(new MemoryByteBuffer(frameArray.Data), oldPixelData.Width).Data);
                    }
                }

                try
                {
                    if (oldPixelData.PlanarConfiguration == PlanarConfiguration.Planar && oldPixelData.SamplesPerPixel > 1)
                    {
                        if (oldPixelData.SamplesPerPixel != 3 || oldPixelData.BitsStored > 8)
                            throw new DicomCodecException("Planar reconfiguration only implemented for SamplesPerPixel=3 && BitsStored <= 8");

                        newPixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;
                        frameArray = new PinnedByteArray(PixelDataConverter.PlanarToInterleaved24(new MemoryByteBuffer(frameArray.Data)).Data);
                    }

                    int inColorSpace = (int)getJpegColorSpace(oldPixelData.PhotometricInterpretation);
                    int rowStride = oldPixelData.Width * oldPixelData.SamplesPerPixel * (oldPixelData.BitsStored <= 8 ? 1 : oldPixelData.BytesAllocated);

                    byte[] errorMessage = new byte[256];
                    IntPtr outBuffer;
                    uint outSize;
                    int outJpegColorSpace;
                    int rc;

                    byte* framePtr = (byte*)(void*)frameArray.Pointer;

                    if (IsWindows)
                        rc = DicomJpegEncode_win(framePtr, oldPixelData.Width, oldPixelData.Height, oldPixelData.SamplesPerPixel, inColorSpace,
                            (int)Mode, Bits, jpegParams.Quality, jpegParams.SmoothingFactor, Predictor, PointTransform, (int)jpegParams.SampleFactor, (uint)rowStride,
                            out outBuffer, out outSize, out outJpegColorSpace, errorMessage, (uint)errorMessage.Length);
                    else
                        rc = DicomJpegEncode(framePtr, oldPixelData.Width, oldPixelData.Height, oldPixelData.SamplesPerPixel, inColorSpace,
                            (int)Mode, Bits, jpegParams.Quality, jpegParams.SmoothingFactor, Predictor, PointTransform, (int)jpegParams.SampleFactor, (uint)rowStride,
                            out outBuffer, out outSize, out outJpegColorSpace, errorMessage, (uint)errorMessage.Length);

                    if (rc != 0)
                        throw new DicomCodecException("Unable to JPEG encode pixel data: " + ErrorText(errorMessage));

                    try
                    {
                        byte[] encoded = new byte[outSize];
                        Marshal.Copy(outBuffer, encoded, 0, (int)outSize);

                        if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.Rgb
                            && outJpegColorSpace == (int)J_COLOR_SPACE.JCS_YCbCr)
                        {
                            newPixelData.PhotometricInterpretation = PhotometricInterpretation.YbrFull422;
                        }

                        if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                        {
                            newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
                        }

                        IByteBuffer buffer;

                        if (encoded.Length >= (int)NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
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
                    finally
                    {
                        if (IsWindows)
                            DicomJpegFreeBuffer_win(outBuffer);
                        else
                            DicomJpegFreeBuffer(outBuffer);
                    }
                }
                catch (DicomCodecException e)
                {
                    throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                }
                finally
                {
                    if (frameArray != null)
                    {
                        frameArray.Dispose();
                        frameArray = null;
                    }
                }
            }

            public override unsafe void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams jpegParams, int frame)
            {
                if (Platform.Current == Platform.Type.unsupported)
                {
                    throw new InvalidOperationException("Unsupported OS Platform");
                }

                PinnedByteArray jpegArray = new PinnedByteArray(this.TrytoFixPixelData(oldPixelData.GetFrame(frame).Data));
                IntPtr outPixels = IntPtr.Zero;
                bool havePixels = false;

                try
                {
                    if (oldPixelData.PhotometricInterpretation != PhotometricInterpretation.Rgb)
                    {
                        jpegParams.ConvertColorSpaceToRGB = true;
                    }

                    newPixelData.PhotometricInterpretation = oldPixelData.PhotometricInterpretation;

                    int isSigned = oldPixelData.PixelRepresentation == PixelRepresentation.Signed ? 1 : 0;

                    byte[] errorMessage = new byte[256];
                    uint outSize, outWidth, outHeight, outRowSize;
                    int outComponents, outPrecision, outColorSpace;
                    int rc;

                    byte* jpegPtr = (byte*)(void*)jpegArray.Pointer;

                    if (IsWindows)
                        rc = DicomJpegDecode_win(jpegPtr, (uint)jpegArray.ByteSize, jpegParams.ConvertColorSpaceToRGB ? 1 : 0, isSigned,
                            out outPixels, out outSize, out outWidth, out outHeight, out outComponents, out outPrecision, out outColorSpace, out outRowSize,
                            errorMessage, (uint)errorMessage.Length);
                    else
                        rc = DicomJpegDecode(jpegPtr, (uint)jpegArray.ByteSize, jpegParams.ConvertColorSpaceToRGB ? 1 : 0, isSigned,
                            out outPixels, out outSize, out outWidth, out outHeight, out outComponents, out outPrecision, out outColorSpace, out outRowSize,
                            errorMessage, (uint)errorMessage.Length);

                    if (rc != 0)
                        throw new DicomCodecException("Unable to JPEG decode pixel data: " + ErrorText(errorMessage));

                    havePixels = true;

                    // If the native side converted YCbCr/RGB to RGB, reflect that in
                    // the output photometric interpretation and planar configuration.
                    if (outColorSpace == (int)J_COLOR_SPACE.JCS_RGB)
                    {
                        newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
                        newPixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;
                    }

                    int frameSize = (int)outSize;
                    int bufferSize = frameSize;
                    if ((bufferSize % 2) != 0 && oldPixelData.NumberOfFrames == 1)
                        bufferSize++;

                    byte[] decoded = new byte[bufferSize];
                    Marshal.Copy(outPixels, decoded, 0, frameSize);

                    IByteBuffer buffer;

                    if (bufferSize >= (int)NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                        buffer = new TempFileBuffer(decoded);
                    else
                        buffer = new MemoryByteBuffer(decoded);

                    if (newPixelData.PlanarConfiguration == PlanarConfiguration.Planar && newPixelData.SamplesPerPixel > 1)
                    {
                        if (oldPixelData.SamplesPerPixel != 3 || oldPixelData.BitsStored > 8)
                            throw new DicomCodecException("Planar reconfiguration only implemented for SamplesPerPixel = 3 && BitsStored <= 8");

                        buffer = PixelDataConverter.InterleavedToPlanar24(buffer);
                    }

                    newPixelData.AddFrame(buffer);
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
                    if (havePixels && outPixels != IntPtr.Zero)
                    {
                        if (IsWindows)
                            DicomJpegFreeBuffer_win(outPixels);
                        else
                            DicomJpegFreeBuffer(outPixels);
                    }
                    if (jpegArray != null)
                    {
                        jpegArray.Dispose();
                        jpegArray = null;
                    }
                }
            }

            internal override unsafe int ScanHeaderForPrecision(DicomPixelData pixelData, bool isjPEG, int frame = 0)
            {
                PinnedByteArray jpegArray = new PinnedByteArray(pixelData.GetFrame(frame).Data);

                var jpegFile = new byte[] { 255, 216, 255 };

                if (!jpegFile.SequenceEqual(jpegArray.Data.Take(jpegFile.Length)))
                {
                    jpegArray.Dispose();
                    throw new DicomCodecException("Not a JPEG file.");
                }

                try
                {
                    byte[] errorMessage = new byte[256];
                    int precision;
                    int rc;

                    byte* jpegPtr = (byte*)(void*)jpegArray.Pointer;

                    if (IsWindows)
                        rc = DicomJpegReadPrecision_win(jpegPtr, (uint)jpegArray.ByteSize, out precision, errorMessage, (uint)errorMessage.Length);
                    else
                        rc = DicomJpegReadPrecision(jpegPtr, (uint)jpegArray.ByteSize, out precision, errorMessage, (uint)errorMessage.Length);

                    if (rc != 0)
                        throw new DicomCodecException("Unable to read JPEG header: " + ErrorText(errorMessage));

                    return precision;
                }
                finally
                {
                    jpegArray.Dispose();
                }
            }

            private byte[] TrytoFixPixelData(byte[] buffer)
            {
                if (!buffer[buffer.Length - 1].Equals(217) && !buffer[buffer.Length - 2].Equals(255))
                {
                    var newbf = new byte[buffer.Length + 2];
                    buffer.CopyTo(newbf, 0);
                    newbf[buffer.Length] = 255;
                    newbf[buffer.Length + 1] = 217;

                    return newbf;
                }
                else
                {
                    return buffer;
                }
            }
        }
    }

    public abstract class DicomJpegNativeCodec : DicomJpegCodec
    {
        public override void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            if (oldPixelData.NumberOfFrames == 0)
                return;

            // IJG eats the extra padding bits. Is there a better way to test for this?
            if (oldPixelData.BitsAllocated == 16 && oldPixelData.BitsStored <= 8)
            {
                // check for embedded overlays?
                newPixelData.Dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)8);
            }

            if (parameters == null || parameters.GetType() != typeof(DicomJpegParams))
                parameters = GetDefaultParameters();

            DicomJpegParams jparams = (DicomJpegParams)parameters;
            JpegNativeCodec codec = GetCodec(oldPixelData.BitsStored, jparams);

            for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
            {
                codec.Encode(oldPixelData, newPixelData, jparams, frame);
            }
        }

        public override void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            try
            {
                if (oldPixelData.NumberOfFrames.Equals(0))
                {
                    return;
                }

                // IJG eats the extra padding bits. Is there a better way to test for this?
                if (newPixelData.BitsAllocated == 16 && newPixelData.BitsStored <= 8)
                {
                    // check for embedded overlays here or below?
                    newPixelData.Dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)8);
                }

                if (parameters == null || parameters.GetType() != typeof(DicomJpegParams))
                    parameters = GetDefaultParameters();

                DicomJpegParams jparams = (DicomJpegParams)parameters;
                int precision = 0;
                bool isjPEG = false;

                try
                {
                    try
                    {
                        precision = ScanJpegForBitDepth(oldPixelData); // scan header for only first frame
                    }
                    catch
                    {
                        // if the internal scanner chokes on an image, try again using the native header reader
                        JpegCodec c = new JpegCodec(JpegMode.Baseline, 0, 0, 8);

                        try
                        {
                            precision = c.ScanHeaderForPrecision(oldPixelData, isjPEG, 0); // scan header for only first frame
                        }
                        catch (DicomCodecException e)
                        {
                            throw new DicomCodecException(e.Message);
                        }
                    }
                }
                catch (DicomCodecException e)
                {
                    // the old scanner choked on several valid images...
                    // assume the correct encoder was used and let the native codec handle the rest
                    if (isjPEG)
                        precision = oldPixelData.BitsStored;
                    else
                        throw new DicomCodecException(e.Message);
                }

                if (newPixelData.BitsStored <= 8 && precision > 8)
                    newPixelData.Dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)16);

                var fragments = oldPixelData.Dataset.GetDicomItem<DicomFragmentSequence>(DicomTag.PixelData).Fragments;

                for (int frame = 0; frame < fragments.Count; frame++)
                {
                    if (!(fragments[frame] is EmptyBuffer))
                    {
                        try
                        {
                            var prec = ScanJpegForBitDepth(oldPixelData, frame);
                            JpegNativeCodec codec = GetCodec(prec, jparams);
                            codec.Decode(oldPixelData, newPixelData, jparams, frame);
                        }
                        catch (DicomCodecException e)
                        {
                            throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                        }
                    }
                }
            }
            catch (DicomCodecException e)
            {
                throw new DicomCodecException(e.Message + " => " + e.StackTrace);
            }
        }

        private static int ScanJpegForBitDepth(DicomPixelData pixelData, int frame = 0)
        {
            DicomItem item = pixelData.Dataset.GetDicomItem<DicomItem>(DicomTag.PixelData);
            IByteBuffer buffer = item is DicomFragmentSequence fragmentSequence ? fragmentSequence.Fragments[frame] : (item as DicomElement).Buffer;
            var ms = new MemoryStream(buffer.Data);
            BinaryReader br = EndianBinaryReader.Create(ms, Endian.Big, false);

            long length = ms.Length;
            while (ms.Position < length)
            {
                ushort marker = br.ReadUInt16();
                switch (marker)
                {
                    case 0xffc0: // SOF_0: JPEG baseline
                    case 0xffc1: // SOF_1: JPEG extended sequential DCT
                    case 0xffc2: // SOF_2: JPEG progressive DCT
                    case 0xffc3: // SOF_3: JPEG lossless sequential
                    case 0xffc5: // SOF_5: differential (hierarchical) extended sequential, Huffman
                    case 0xffc6: // SOF_6: differential (hierarchical) progressive, Huffman
                    case 0xffc7: // SOF_7: differential (hierarchical) lossless, Huffman
                        ms.Seek(2, SeekOrigin.Current);
                        return (int)br.ReadByte();
                    case 0xffc8: // Reserved for JPEG extentions
                        ms.Seek(br.ReadUInt16() - 2, SeekOrigin.Current);
                        break;
                    case 0xffc9: // SOF_9: extended sequential, arithmetic
                    case 0xffca: // SOF_10: progressive, arithmetic
                    case 0xffcb: // SOF_11: lossless, arithmetic
                    case 0xffcd: // SOF_13: differential (hierarchical) extended sequential, arithmetic
                    case 0xffce: // SOF_14: differential (hierarchical) progressive, arithmetic
                    case 0xffcf: // SOF_15: differential (hierarchical) lossless, arithmetic
                        ms.Seek(2, SeekOrigin.Current);
                        return (int)br.ReadByte();
                    case 0xffc4: // DHT
                    case 0xffcc: // DAC
                        ms.Seek(br.ReadUInt16() - 2, SeekOrigin.Current);
                        break;
                    case 0xffd0: // RST m
                    case 0xffd1:
                    case 0xffd2:
                    case 0xffd3:
                    case 0xffd4:
                    case 0xffd5:
                    case 0xffd6:
                    case 0xffd7:
                    case 0xffd8: // SOI
                    case 0xffd9: // EOI
                        break;
                    case 0xffda: // SOS
                    case 0xffdb: // DQT
                    case 0xffdc: // DNL
                    case 0xffdd: // DRI
                    case 0xffde: // DHP
                    case 0xffdf: // EXP
                    case 0xffe0: // APPn
                    case 0xffe1:
                    case 0xffe2:
                    case 0xffe3:
                    case 0xffe4:
                    case 0xffe5:
                    case 0xffe6:
                    case 0xffe7:
                    case 0xffe8:
                    case 0xffe9:
                    case 0xffea:
                    case 0xffeb:
                    case 0xffec:
                    case 0xffed:
                    case 0xffee:
                    case 0xffef:
                    case 0xfff0: // JPGn
                    case 0xfff1:
                    case 0xfff2:
                    case 0xfff3:
                    case 0xfff4:
                    case 0xfff5:
                    case 0xfff6:
                    case 0xfff7:
                    case 0xfff8:
                    case 0xfff9:
                    case 0xfffa:
                    case 0xfffb:
                    case 0xfffc:
                    case 0xfffd:
                    case 0xfffe: // COM
                        ms.Seek(br.ReadUInt16() - 2, SeekOrigin.Current);
                        break;
                    case 0xff01: // TEM
                        break;
                    default:
                        int b1 = br.ReadByte();
                        int b2 = br.ReadByte();
                        if (b1 == 0xff && b2 > 2 && b2 <= 0xbf) // RES reserved markers
                        {
                            break;
                        }
                        else
                        {
                            throw new DicomCodecException("Unable to determine bit depth: JPEG syntax error at frame number => " + frame);
                        }
                }
            }
            throw new DicomCodecException("Unable to determine bit depth: no JPEG SOF marker found!");
        }

        protected virtual JpegNativeCodec GetCodec(int bits, DicomJpegParams jparams)
        {
            if (bits == 8)
                return new JpegCodec(JpegMode.Baseline, 0, 0, bits);
            else
                throw new DicomCodecException(string.Format("Unable to create JPEG Process 1 codec for bits stored == {0}", bits));
        }
    }

    public class DicomJpegProcess1Codec : DicomJpegNativeCodec
    {
        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.JPEGProcess1;
            }
        }

        protected override JpegNativeCodec GetCodec(int bits, DicomJpegParams jparams)
        {
            if (bits == 8)
                return new JpegCodec(JpegMode.Baseline, 0, 0, bits);

            else
                throw new DicomCodecException(string.Format("Unable to create JPEG Process 1 codec for bits stored == {0}", bits));
        }

    };

    public class DicomJpegProcess4Codec : DicomJpegNativeCodec
    {
        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.JPEGProcess2_4;
            }
        }

        protected override JpegNativeCodec GetCodec(int bits, DicomJpegParams jparams)
        {
            if (bits == 8)
                return new JpegCodec(JpegMode.Sequential, 0, 0, bits);
            else if (bits > 8 && bits <= 12)
                return new JpegCodec(JpegMode.Sequential, 0, 0, bits);
            else
                throw new DicomCodecException(string.Format("Unable to create JPEG Process 4 codec for bits stored == {0}", bits));
        }
    }

    public class DicomJpegLossless14Codec : DicomJpegNativeCodec
    {
        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.JPEGProcess14;
            }
        }

        protected override JpegNativeCodec GetCodec(int bits, DicomJpegParams jparams)
        {
            if (bits == 8)
                return new JpegCodec(JpegMode.Lossless, jparams.Predictor, jparams.PointTransform, bits);
            else if (bits > 8 && bits <= 12)
                return new JpegCodec(JpegMode.Lossless, jparams.Predictor, jparams.PointTransform, bits);
            else if (bits > 12 && bits <= 16)
                return new JpegCodec(JpegMode.Lossless, jparams.Predictor, jparams.PointTransform, bits);
            else
                throw new DicomCodecException(String.Format("Unable to create JPEG Process 14 codec for bits stored == {0}", bits));
        }
    }

    public class DicomJpegLossless14SV1Codec : DicomJpegNativeCodec
    {
        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.JPEGProcess14SV1;
            }
        }

        protected override JpegNativeCodec GetCodec(int bits, DicomJpegParams jparams)
        {
            if (bits == 8)
                return new JpegCodec(JpegMode.Lossless, 1, jparams.PointTransform, bits);

            else if (bits > 8 && bits <= 12)
                return new JpegCodec(JpegMode.Lossless, 1, jparams.PointTransform, bits);

            else if (bits > 12 && bits <= 16)
                return new JpegCodec(JpegMode.Lossless, 1, jparams.PointTransform, bits);

            else
                throw new DicomCodecException(String.Format("Unable to create JPEG Process 14 [SV1] codec for bits stored == {0}", bits));
        }
    }
}
