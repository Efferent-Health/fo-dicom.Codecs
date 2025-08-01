using System;
using System.Buffers;
using System.Runtime.InteropServices;

using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO;
using FellowOakDicom.IO.Buffer;

namespace FellowOakDicom.Imaging.NativeCodec
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Raw_outdata
    {
        public unsafe byte* buffer;
        public unsafe uint size_outbuffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Htj2k_outdata
    {
        public unsafe byte* buffer;
        public unsafe uint size_outbuffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Frameinfo
    {
        /// <summary>
        /// Width of the image, range [1, 65535].
        /// </summary>
        public ushort width;

        /// <summary>
        /// Height of the image, range [1, 65535].
        /// </summary>
        public ushort height;

        /// <summary>
        /// Number of bits per sample, range [2, 16]
        /// </summary>
        public byte bitsPerSample;

        /// <summary>
        /// Number of components contained in the frame, range [1, 255]
        /// </summary>
        public byte componentCount;

        /// <summary>
        /// true if signed, false if unsigned
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool isSigned;

        /// <summary>
        /// true if color transform is used, false if not
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool isUsingColorTransform;

        /// <summary>
        /// true if lossless, false is lossy
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool isReversible;
    }

    public class DicomHtJpeg2000Params : DicomCodecParams
    {
        public DicomHtJpeg2000Params()
        {
        }

        public OPJ_PROG_ORDER ProgressionOrder { get; set; } = OPJ_PROG_ORDER.RPCL;
    }

    public abstract class DicomHtJpeg2000Codec : IDicomCodec
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
            return new DicomHtJpeg2000Params();
        }

        public abstract void Encode(
            DicomPixelData oldPixelData,
            DicomPixelData newPixelData,
            DicomCodecParams parameters);

        public abstract void Decode(
            DicomPixelData oldPixelData,
            DicomPixelData newPixelData,
            DicomCodecParams parameters);
    };

    public abstract class DicomHtJpeg2000NativeCodec : DicomHtJpeg2000Codec
    {
        // Encode HTJ2K for win_x64
        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "InvokeHTJ2KEncode")]
        public static extern unsafe void InvokeHTJ2KEncode_win(ref Htj2k_outdata j2c_outinfo, byte* source, uint sourceLength, ref Frameinfo frameinfo, OPJ_PROG_ORDER progressionOrder = OPJ_PROG_ORDER.PROG_UNKNOWN);

        // Decode HTJ2K for win_x64
        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "InvokeHTJ2KDecode")]
        public static extern unsafe void InvokeHTJ2KDecode_win(ref Raw_outdata raw_outinfo, byte* source, uint sourceLength);

        // Encode HTJ2k
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "InvokeHTJ2KEncode")]
        public static extern unsafe void InvokeHTJ2KEncode(ref Htj2k_outdata j2c_outinfo, byte* source, uint sourceLength, ref Frameinfo frameinfo, OPJ_PROG_ORDER progressionOrder = OPJ_PROG_ORDER.PROG_UNKNOWN);

        // Decode HTJ2k
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "InvokeHTJ2KDecode")]
        public static extern unsafe void InvokeHTJ2KDecode(ref Raw_outdata raw_outinfo, byte* source, uint sourceLength);

        public override unsafe void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            unsafe
            {
                if (Platform.Current == Platform.Type.unsupported)
                {
                    throw new InvalidOperationException("Unsupported OS Platform");
                }

                DicomHtJpeg2000Params jparams = (DicomHtJpeg2000Params)parameters;

                if (jparams == null)
                    jparams = (DicomHtJpeg2000Params)GetDefaultParameters();

                for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
                {
                    IByteBuffer frameData = oldPixelData.GetFrame(frame);

                    var pool = ArrayPool<byte>.Shared;
                    byte[] jpegHT2KData = null;

                    try
                    {
                        //Converting photmetricinterpretation YbrFull or YbrFull422 to RGB
                        if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                        {
                            frameData = PixelDataConverter.YbrFullToRgb(frameData);
                        }
                        else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                        {
                            frameData = PixelDataConverter.YbrFull422ToRgb(frameData, oldPixelData.Width);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Cannot convert HTJ2K buffer data from PhotometricInterpretation = {0} to RGB => {1} => {2}", oldPixelData
                        .PhotometricInterpretation.ToString(), ex.Message, ex.StackTrace);
                    }

                    PinnedByteArray frameArray = new PinnedByteArray(frameData.Data);

                    uint jpegHT2KDataSize = 0;
                    Frameinfo frameinfo = new Frameinfo
                    {
                        width = oldPixelData.Width,
                        height = oldPixelData.Height,
                        bitsPerSample = (byte)oldPixelData.BitsAllocated,
                        componentCount = (byte)oldPixelData.SamplesPerPixel,
                        isSigned = oldPixelData.PixelRepresentation == PixelRepresentation.Signed ? true : false,
                        isUsingColorTransform = oldPixelData.SamplesPerPixel > 1 ? true : false
                    };

                    if (newPixelData.Syntax.Equals(DicomTransferSyntax.HTJ2KLossless) || newPixelData.Syntax.Equals(DicomTransferSyntax.HTJ2KLosslessRPCL))
                        frameinfo.isReversible = true;
                    else
                        frameinfo.isReversible = false;

                    var progressionOrder = OPJ_PROG_ORDER.PROG_UNKNOWN;
                    if (newPixelData.Syntax.Equals(DicomTransferSyntax.HTJ2KLosslessRPCL))
                        progressionOrder = jparams.ProgressionOrder;

                    Htj2k_outdata j2c_outinfo = new Htj2k_outdata();

                    try
                    {
                        if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                            InvokeHTJ2KEncode_win(ref j2c_outinfo, (byte*)frameArray.Pointer, (uint)frameArray.Count, ref frameinfo, progressionOrder);
                        else
                            InvokeHTJ2KEncode(ref j2c_outinfo, (byte*)frameArray.Pointer, (uint)frameArray.Count, ref frameinfo, progressionOrder);

                        jpegHT2KDataSize = j2c_outinfo.size_outbuffer;

                        jpegHT2KData = pool.Rent((int)jpegHT2KDataSize);
                        Marshal.Copy((IntPtr)j2c_outinfo.buffer, jpegHT2KData, 0, (int)jpegHT2KDataSize);

                        IByteBuffer buffer;

                        if (jpegHT2KDataSize >= NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                        {
                            buffer = new TempFileBuffer(jpegHT2KData);
                            buffer = EvenLengthBuffer.Create(buffer);
                        }
                        else
                            buffer = new MemoryByteBuffer(jpegHT2KData);

                        if (oldPixelData.NumberOfFrames == 1)
                            buffer = EvenLengthBuffer.Create(buffer);

                        newPixelData.AddFrame(buffer);
                    }
                    catch (DicomCodecException d)
                    {
                        throw new DicomCodecException(d.Message + " => " + d.StackTrace);
                    }
                    catch (Exception e)
                    {
                        throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                    }
                    finally
                    {
                        if (jpegHT2KData != null)
                        {
                            pool.Return(jpegHT2KData);
                            jpegHT2KData = null;
                        }

                        if (frameArray != null)
                        {
                            frameArray.Dispose();
                            frameArray = null;
                        }
                    }
                }
            }
        }

        public override unsafe void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            try
            {   
                if (Platform.Current == Platform.Type.unsupported)
                {
                    throw new InvalidOperationException("Unsupported OS Platform");
                }

                for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
                {
                    IByteBuffer htjpeg2kData = oldPixelData.GetFrame(frame);

                    var pool = ArrayPool<byte>.Shared;
                    byte[] frameData = null;

                    //Converting photmetricinterpretation YbrFull or YbrFull422 to RGB
                    if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                    {
                        htjpeg2kData = PixelDataConverter.YbrFullToRgb(htjpeg2kData);
                    }
                    else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                    {
                        htjpeg2kData = PixelDataConverter.YbrFull422ToRgb(htjpeg2kData, oldPixelData.Width);
                    }

                    PinnedByteArray htjpeg2kArray = new PinnedByteArray(htjpeg2kData.Data);

                    frameData = pool.Rent(newPixelData.UncompressedFrameSize);
                    PinnedByteArray frameArray = new PinnedByteArray(frameData);

                    try
                    {
                        Raw_outdata raw_Outdata = new Raw_outdata();

                        unsafe
                        {
                            if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                InvokeHTJ2KDecode_win(ref raw_Outdata, (byte*)htjpeg2kArray.Pointer, (uint)htjpeg2kArray.Count);
                            else
                                InvokeHTJ2KDecode(ref raw_Outdata, (byte*)htjpeg2kArray.Pointer, (uint)htjpeg2kArray.Count); ;

                            Marshal.Copy((IntPtr)raw_Outdata.buffer, frameData, 0, (int)raw_Outdata.size_outbuffer);

                            IByteBuffer buffer;
                            if (frameData.Length >= NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                                buffer = new TempFileBuffer(frameData);
                            else
                                buffer = new MemoryByteBuffer(frameData);

                            if (oldPixelData.NumberOfFrames == 1)
                                buffer = EvenLengthBuffer.Create(buffer);

                            newPixelData.AddFrame(buffer);
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
                        if (frameData != null)
                        {
                            pool.Return(frameData);
                            frameData = null;
                        }
                        if (htjpeg2kArray != null)
                        {
                            htjpeg2kArray.Dispose();
                            htjpeg2kArray = null;
                        }

                        if (frameArray != null)
                        {
                            frameArray.Dispose();
                            frameArray = null;
                        }
                    }
                }
            }
            catch (DicomCodecException e)
            { 
                throw new DicomCodecException(e.Message + " => " + e.StackTrace);
            }
        }
    }

    public class DicomHtJpeg2000LosslessRPCLCodec : DicomHtJpeg2000NativeCodec
    {
        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.HTJ2KLosslessRPCL;
            }
        }
    }

    public class DicomHtJpeg2000LosslessCodec : DicomHtJpeg2000NativeCodec
    {
        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.HTJ2KLossless;
            }
        }
    }

    public class DicomHtJpeg2000LossyCodec : DicomHtJpeg2000NativeCodec
    {
        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.HTJ2K;
            }
        }
    }
}