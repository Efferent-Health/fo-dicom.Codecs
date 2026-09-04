using System;
using System.Buffers;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO;
using FellowOakDicom.IO.Buffer;

namespace FellowOakDicom.Imaging.NativeCodec
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Raw_outdata
    {
        public unsafe byte* buffer;
        public unsafe ulong size_outbuffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Htj2k_outdata
    {
        public unsafe byte* buffer;
        public unsafe ulong size_outbuffer;
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

    [Flags]
    public enum EncodeStatus
    {
        Success = 1,
        Failed = 0,
        Unknown = -1
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
        public static extern unsafe EncodeStatus InvokeHTJ2KEncode_win(ref Htj2k_outdata j2c_outinfo, byte* source, ulong sourceLength, ref Frameinfo frameinfo, OPJ_PROG_ORDER progressionOrder = OPJ_PROG_ORDER.PROG_UNKNOWN);

        // Decode HTJ2K for win_x64
        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "InvokeHTJ2KDecode")]
        public static extern unsafe void InvokeHTJ2KDecode_win(ref Raw_outdata raw_outinfo, byte* source, ulong sourceLength);

        // Encode HTJ2k
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "InvokeHTJ2KEncode")]
        public static extern unsafe EncodeStatus InvokeHTJ2KEncode(ref Htj2k_outdata j2c_outinfo, byte* source, ulong sourceLength, ref Frameinfo frameinfo, OPJ_PROG_ORDER progressionOrder = OPJ_PROG_ORDER.PROG_UNKNOWN);

        // Decode HTJ2k
        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "InvokeHTJ2KDecode")]
        public static extern unsafe void InvokeHTJ2KDecode(ref Raw_outdata raw_outinfo, byte* source, ulong sourceLength);

        /// <summary>
        /// Reads the decoded size the native decoder will produce, from the codestream's own
        /// SIZ marker. Mirrors HTJpeg2000DecodeStream term for term: width and height are the
        /// image extent minus the image offset, multiplied by the component count and by the
        /// bytes per sample taken from component zero's precision.
        /// </summary>
        /// <returns>
        /// false when the header cannot be read, in which case the caller must not reject:
        /// a guard that turned an unreadable header into a refusal would fail files the decoder
        /// handles today, which is a worse regression than the one it is closing.
        /// </returns>
        private static bool TryGetCodestreamLength(byte[] codestream, out ulong length)
        {
            length = 0;

            //SOC then SIZ, at fixed offsets. The native side hands these bytes straight to
            //ojph::codestream::read_headers, which expects a raw codestream laid out exactly
            //this way, so anything else is not something this decoder would have decoded.
            //Searching for the marker instead would risk matching 0xFF51 inside arbitrary
            //data and computing a size from it, which could refuse a perfectly good file.
            //43 bytes is the smallest prefix that still carries component zero's Ssiz, which
            //sits at 42; anything shorter cannot be read without going out of bounds here.
            if (codestream == null || codestream.Length < 43)
                return false;
            if (codestream[0] != 0xFF || codestream[1] != 0x4F)
                return false;
            if (codestream[2] != 0xFF || codestream[3] != 0x51)
                return false;

            //Big-endian, per the JPEG 2000 codestream syntax. Offsets are from the SIZ marker
            //at 2: Lsiz 4, Rsiz 6, Xsiz 8, Ysiz 12, XOsiz 16, YOsiz 20, then the four tile
            //fields, Csiz at 40 and component zero's Ssiz at 42.
            uint xsiz = ReadUInt32(codestream, 8);
            uint ysiz = ReadUInt32(codestream, 12);
            uint xosiz = ReadUInt32(codestream, 16);
            uint yosiz = ReadUInt32(codestream, 20);
            ushort csiz = (ushort)((codestream[40] << 8) | codestream[41]);

            if (xsiz <= xosiz || ysiz <= yosiz || csiz == 0)
                return false;

            //Ssiz carries the precision less one in its low seven bits; the high bit is the
            //sign flag and does not change the byte width.
            int bitsPerSample = (codestream[42] & 0x7F) + 1;
            ulong bytesPerSample = (ulong)((bitsPerSample + 8 - 1) / 8);

            length = (ulong)(xsiz - xosiz) * (ysiz - yosiz) * csiz * bytesPerSample;
            return true;
        }

        private static uint ReadUInt32(byte[] data, int offset)
        {
            return ((uint)data[offset] << 24) | ((uint)data[offset + 1] << 16) |
                   ((uint)data[offset + 2] << 8) | data[offset + 3];
        }

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

                    Frameinfo frameinfo = new Frameinfo
                    {
                        width = oldPixelData.Width,
                        height = oldPixelData.Height,
                        bitsPerSample = (byte)oldPixelData.BitsAllocated,
                        componentCount = (byte)oldPixelData.SamplesPerPixel,
                        isSigned = oldPixelData.PixelRepresentation == PixelRepresentation.Signed,
                        isUsingColorTransform = oldPixelData.SamplesPerPixel > 1
                    };

                    if (newPixelData.Syntax.Equals(DicomTransferSyntax.HTJ2KLossless) || newPixelData.Syntax.Equals(DicomTransferSyntax.HTJ2KLosslessRPCL))
                        frameinfo.isReversible = true;
                    else
                        frameinfo.isReversible = false;

                    var progressionOrder = OPJ_PROG_ORDER.PROG_UNKNOWN;
                    if (newPixelData.Syntax.Equals(DicomTransferSyntax.HTJ2KLosslessRPCL))
                        progressionOrder = jparams.ProgressionOrder;

                    var pool = ArrayPool<byte>.Shared;
                    byte[] jpegHT2KData = pool.Rent(frameData.Data.Length);
                    
                    try
                    {   
                        Htj2k_outdata j2c_outinfo;

                        fixed (byte * pjpegHT2KData = jpegHT2KData)
                        {
                            j2c_outinfo = new Htj2k_outdata
                            {
                                buffer = pjpegHT2KData
                            };

                            EncodeStatus status = EncodeStatus.Unknown;

                            if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                status = InvokeHTJ2KEncode_win(ref j2c_outinfo, (byte*)frameArray.Pointer, (ulong)frameData.Data.Length, ref frameinfo, progressionOrder);
                            else
                                status = InvokeHTJ2KEncode(ref j2c_outinfo, (byte*)frameArray.Pointer, (ulong)frameData.Data.Length, ref frameinfo, progressionOrder);

                            if (!status.Equals(EncodeStatus.Success))
                            {
                                throw new DicomCodecException("Error in HTJ2K encode stream => output buffer data has an incorrect size");
                            }

                            // The pooled array is returned to the pool in the finally block below, so it
                            // must not be handed over to the buffer: the array can be rented again and
                            // overwritten while this dataset still points at it. Copy it out at its exact
                            // size, the way DicomJpeg2000Codec already does.
                            var encoded = new byte[j2c_outinfo.size_outbuffer];
                            Buffer.BlockCopy(jpegHT2KData, 0, encoded, 0, (int)j2c_outinfo.size_outbuffer);

                            IByteBuffer buffer;

                            if (j2c_outinfo.size_outbuffer >= NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
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

                        frameArray?.Dispose();
                        frameArray = null;
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

                var uncompressedSize = newPixelData.Height * newPixelData.Width * newPixelData.SamplesPerPixel * newPixelData.BytesAllocated;

                for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
                {
                    IByteBuffer htjpeg2kData = oldPixelData.GetFrame(frame);

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

                    var pool = ArrayPool<byte>.Shared;
                    byte[] frameData = pool.Rent(uncompressedSize > newPixelData.UncompressedFrameSize ? uncompressedSize : newPixelData.UncompressedFrameSize);

                    try
                    {
                        //The native decoder sizes its own output from the codestream's SIZ marker and
                        //memcpy's that many bytes into frameData, which was rented from the geometry the
                        //dataset declares. Neither side compares the two, so a codestream covering a
                        //larger extent than the declared geometry writes past the end of the rented array.
                        //Reject before the call rather than after, since by then the write has happened.
                        //Compare against the rented length rather than recomputing the declared size, so
                        //the check holds even where the sizing arithmetic above overflows. Only the
                        //larger-than case is rejected, and only when the extent was positively read, so a
                        //smaller codestream and an unparsable header both decode as they always have.
                        if (TryGetCodestreamLength(htjpeg2kData.Data, out ulong required) &&
                            required > (ulong)frameData.Length)
                            throw new DicomCodecException("Error in HTJ2K decode stream => output image is larger than the pixel buffer the dataset declares");

                        Raw_outdata raw_Outdata;

                        unsafe
                        {   
                            fixed (byte * prawdata = frameData)
                            {
                                raw_Outdata = new Raw_outdata
                                {
                                    buffer = prawdata
                                };

                                if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                    InvokeHTJ2KDecode_win(ref raw_Outdata, (byte*)htjpeg2kArray.Pointer, (ulong)htjpeg2kArray.Count);
                                else
                                    InvokeHTJ2KDecode(ref raw_Outdata, (byte*)htjpeg2kArray.Pointer, (ulong)htjpeg2kArray.Count); ;

                                // Same reason as in Encode: copy out of the pooled array before it goes
                                // back to the pool, and at the exact decoded size instead of the pool
                                // bucket size.
                                var decoded = new byte[raw_Outdata.size_outbuffer];
                                Buffer.BlockCopy(frameData, 0, decoded, 0, (int)raw_Outdata.size_outbuffer);

                                IByteBuffer buffer;
                                if ((int)raw_Outdata.size_outbuffer >= (int)NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                                    buffer = new TempFileBuffer(decoded);
                                else
                                    buffer = new MemoryByteBuffer(decoded);

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