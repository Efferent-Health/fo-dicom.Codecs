using System;
using System.Buffers;
using System.Runtime.InteropServices;

using System.Linq;

using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO;
using FellowOakDicom.IO.Buffer;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace FellowOakDicom.Imaging.NativeCodec
{
    [Flags]
    public enum OPJ_CODEC_FORMAT
    {
        CODEC_UNKNOWN = -1, /**< place-holder */
        CODEC_J2K = 0,      /**< JPEG-2000 codestream : read/write */
        CODEC_JPT = 1,      /**< JPT-stream (JPEG 2000, JPIP) : read only */
        CODEC_JP2 = 2 		/**< JPEG-2000 file format : read/write */
    }

    [Flags]
    public enum OPJ_COLOR_SPACE
    {
        CLRSPC_UNKNOWN = -1,    /**< not supported by the library */
        CLRSPC_UNSPECIFIED = 0, /**< not specified in the codestream */
        CLRSPC_SRGB = 1,        /**< sRGB */
        CLRSPC_GRAY = 2,        /**< grayscale */
        CLRSPC_SYCC = 3
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_image_comp_t
    {
        /** XRsiz: horizontal separation of a sample of ith component with respect to the reference grid */
        public uint dx;
        /** YRsiz: vertical separation of a sample of ith component with respect to the reference grid */
        public uint dy;
        /** data width */
        public uint w;
        /** data height */
        public uint h;
        /** x component offset compared to the whole image */
        public uint x0;
        /** y component offset compared to the whole image */
        public uint y0;
        /** precision */
        public uint prec;
        /** image depth in bits */
        public uint bpp;
        /** signed (1) / unsigned (0) */
        public uint sgnd;
        /** number of decoded resolution */
        public uint resno_decoded;
        /** number of division by 2 of the out image compared to the original size of image */
        public uint factor;
        /** image component data */
        public int* data;
        public ushort alpha;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_image_t
    {
        /** XOsiz: horizontal offset from the origin of the reference grid to the left side of the image area */
        public uint x0;
        /** YOsiz: vertical offset from the origin of the reference grid to the top side of the image area */
        public uint y0;
        /** Xsiz: width of the reference grid */
        public uint x1;
        /** Ysiz: height of the reference grid */
        public uint y1;
        /** number of components in the image */
        public uint numcomps;
        /** color space: sRGB, Greyscale or YUV */
        public OPJ_COLOR_SPACE color_space;
        /** image components */
        public opj_image_comp_t* comps;
        /** 'restricted' ICC profile */
        public byte* icc_profile_buf;
        /** size of ICC profile */
        public uint icc_profile_len;
    }

    [Flags]
    public enum OPJ_RSIZ_CAPABILITIES
    {
        STD_RSIZ = 0,       /** Standard JPEG2000 profile*/
        CINEMA2K = 3,       /** Profile name for a 2K image*/
        CINEMA4K = 4		/** Profile name for a 4K image*/
    }

    [Flags]
    public enum OPJ_CINEMA_MODE
    {
        OFF = 0,                    /** Not Digital Cinema*/
        CINEMA2K_24 = 1,    /** 2K Digital Cinema at 24 fps*/
        CINEMA2K_48 = 2,    /** 2K Digital Cinema at 48 fps*/
        CINEMA4K_24 = 3		/** 4K Digital Cinema at 24 fps*/
    }

    [Flags]
    public enum OPJ_PROG_ORDER
    {
        PROG_UNKNOWN = -1,  /**< place-holder */
        LRCP = 0,       /**< layer-resolution-component-precinct order */
        RLCP = 1,       /**< resolution-layer-component-precinct order */
        RPCL = 2,       /**< resolution-precinct-component-layer order */
        PCRL = 3,       /**< precinct-component-resolution-layer order */
        CPRL = 4		/**< component-precinct-resolution-layer order */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_cparameters_t
    {
        public int tile_size_on;
        public int cp_tx0;
        /** YTOsiz */
        public int cp_ty0;
        /** XTsiz */
        public int cp_tdx;
        /** YTsiz */
        public int cp_tdy;
        /** allocation by rate/distortion */
        public int cp_disto_alloc;
        /** allocation by fixed layer */
        public int cp_fixed_alloc;
        /** add fixed_quality */
        public int cp_fixed_quality;
        /** fixed layer */
        public unsafe int* cp_matrice;
        /** comment for coding */
        public unsafe char* cp_comment;
        /** csty : coding style */
        public int csty;
        public OPJ_PROG_ORDER prog_order;
        public opj_poc_t POC1;
        public opj_poc_t POC2;
        public opj_poc_t POC3;
        public opj_poc_t POC4;
        public opj_poc_t POC5;
        public opj_poc_t POC6;
        public opj_poc_t POC7;
        public opj_poc_t POC8;
        public opj_poc_t POC9;
        public opj_poc_t POC10;
        public opj_poc_t POC11;
        public opj_poc_t POC12;
        public opj_poc_t POC13;
        public opj_poc_t POC14;
        public opj_poc_t POC15;
        public opj_poc_t POC16;
        public opj_poc_t POC17;
        public opj_poc_t POC18;
        public opj_poc_t POC19;
        public opj_poc_t POC20;
        public opj_poc_t POC21;
        public opj_poc_t POC22;
        public opj_poc_t POC23;
        public opj_poc_t POC24;
        public opj_poc_t POC25;
        public opj_poc_t POC26;
        public opj_poc_t POC27;
        public opj_poc_t POC28;
        public opj_poc_t POC29;
        public opj_poc_t POC30;
        public opj_poc_t POC31;
        public opj_poc_t POC32;
        public uint numpocs;
        /** number of layers */
        public int tcp_numlayers;
        /** rates of layers */
        public unsafe fixed float tcp_rates[100];
        /** different psnr for successive layers */
        public unsafe fixed float tcp_distoratio[100];
        /** number of resolutions */
        public int numresolution;
        /** initial code block width, default to 64 */
        public int cblockw_init;
        /** initial code block height, default to 64 */
        public int cblockh_init;
        /** mode switch (cblk_style) */
        public int mode;
        /** 1 : use the irreversible DWT 9-7, 0 : use lossless compression (default) */
        public int irreversible;
        /** region of interest: affected component in [0..3], -1 means no ROI */
        public int roi_compno;
        /** region of interest: upshift value */
        public int roi_shift;
        /* number of precinct size specifications */
        public int res_spec;
        public unsafe fixed int prcw_init[33];
        /** initial precinct height */
        public unsafe fixed int prch_init[33];
        /** input file name */
        public unsafe fixed sbyte infile[4096];
        /** output file name */
        public unsafe fixed sbyte outfile[4096];
        /** DEPRECATED. Index generation is now handeld with the opj_encode_with_info() function. Set to NULL */
        public int index_on;
        /** DEPRECATED. Index generation is now handeld with the opj_encode_with_info() function. Set to NULL */
        public unsafe fixed sbyte index[4096];
        /** subimage encoding: origin image offset in x direction */
        public int image_offset_x0;
        /** subimage encoding: origin image offset in y direction */
        public int image_offset_y0;
        /** subsampling value for dx */
        public int subsampling_dx;
        /** subsampling value for dy */
        public int subsampling_dy;
        /** input file format 0: PGX, 1: PxM, 2: BMP 3:TIF*/
        public int decod_format;
        /** output file format 0: J2K, 1: JP2, 2: JPT */
        public int cod_format;
        public int jpwl_epc_on;
        /** error protection method for MH (0,1,16,32,37-128) */
        public int jpwl_hprot_MH;
        /** tile number of header protection specification (>=0) */
        public unsafe fixed int jpwl_hprot_TPH_tileno[16];
        /** error protection methods for TPHs (0,1,16,32,37-128) */
        public unsafe fixed int jpwl_hprot_TPH[16];
        /** tile number of packet protection specification (>=0) */
        public unsafe fixed int jpwl_pprot_tileno[16];
        /** packet number of packet protection specification (>=0) */
        public unsafe fixed int jpwl_pprot_packno[16];
        /** error protection methods for packets (0,1,16,32,37-128) */
        public unsafe fixed int jpwl_pprot[16];
        /** enables writing of ESD, (0=no/1/2 bytes) */
        public int jpwl_sens_size;
        /** sensitivity addressing size (0=auto/2/4 bytes) */
        public int jpwl_sens_addr;
        /** sensitivity range (0-3) */
        public int jpwl_sens_range;
        /** sensitivity method for MH (-1=no,0-7) */
        public int jpwl_sens_MH;
        /** tile number of sensitivity specification (>=0) */
        public unsafe fixed int jpwl_sens_TPH_tileno[16];
        /** sensitivity methods for TPHs (-1=no,0-7) */
        public unsafe fixed int jpwl_sens_TPH[16];
        public OPJ_CINEMA_MODE cp_cinema;
        /** Maximum rate for each component. If == 0, component size limitation is not considered */
        public int max_comp_size;
        /** Profile name*/
        public OPJ_RSIZ_CAPABILITIES cp_rsiz;
        /** Tile part generation*/
        public sbyte tp_on;
        /** Flag for Tile part generation*/
        public sbyte tp_flag;
        /** MCT (multiple component transform) */
        public sbyte tcp_mct;
        /** Enable JPIP indexing*/
        public int jpip_on;
        public void* mct_data;
        /**
        * Maximum size (in bytes) for the whole codestream.
        * If == 0, codestream size limitation is not considered
        * If it does not comply with tcp_rates, max_cs_size prevails
        * and a warning is issued.
        * */
        public int max_cs_size;
        public ushort rsiz;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct opj_poc_t
    {
        public uint resno0, compno0;
        /** Layer num end,Resolution num end, Component num end, given by POC */
        public uint layno1, resno1, compno1;
        /** Layer num start,Precinct num start, Precinct num end */
        public uint layno0, precno0, precno1;
        /** Progression order enum*/
        public OPJ_PROG_ORDER prg1, prg;
        /** Progression order string*/
        public unsafe fixed sbyte progorder[5];
        /** Tile number */
        public uint tile;
        /** Start and end values for Tile width and height*/
        public uint tx0, tx1, ty0, ty1;
        /** Start value, initialised in pi_initialise_encode*/
        public uint layS, resS, compS, prcS;
        /** End value, initialised in pi_initialise_encode */
        public uint layE, resE, compE, prcE;
        /** Start and end values of Tile width and height, initialised in pi_initialise_encode*/
        public uint txS, txE, tyS, tyE, dx, dy;
        /** Temporary values for Tile parts, initialised in pi_create_encode */
        public uint lay_t, res_t, comp_t, prc_t, tx0_t, ty0_t;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_image_cmptparm_t
    {
        public uint dx;
        /** YRsiz: vertical separation of a sample of ith component with respect to the reference grid */
        public uint dy;
        /** data width */
        public uint w;
        /** data height */
        public uint h;
        /** x component offset compared to the whole image */
        public uint x0;
        /** y component offset compared to the whole image */
        public uint y0;
        /** precision */
        public uint prec;
        /** image depth in bits */
        public uint bpp;
        /** signed (1) / unsigned (0) */
        public uint sgnd;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_dparameters_t
    {
        public uint cp_reduce;
        public uint cp_layer;
        public unsafe fixed sbyte infile[4096];
        public unsafe fixed sbyte outfile[4096];
        public int decod_format;
        public int cod_format;
        public uint DA_x0;
        public uint DA_x1;
        public uint DA_y0;
        public uint DA_y1;
        public int m_verbose;
        public uint tile_index;
        public uint nb_tile_to_decode;
        public int jpwl_correct;
        public int jpwl_exp_comps;
        public int jpwl_max_tiles;
        public uint flags;
    }

    public class DicomJpeg2000Params : DicomCodecParams
    {
        private double[] _rates;
        public DicomJpeg2000Params()
        {
            Irreversible = true;
            Rate = 20;
            IsVerbose = false;
            AllowMCT = true;
            UpdatePhotometricInterpretation = true;
            EncodeSignedPixelValuesAsUnsigned = false;

            _rates = new double[9];
            _rates[0] = 1280;
            _rates[1] = 640;
            _rates[2] = 320;
            _rates[3] = 160;
            _rates[4] = 80;
            _rates[5] = 40;
            _rates[6] = 20;
            _rates[7] = 10;
            _rates[8] = 5;

            RateLevels = _rates;
        }

        public bool Irreversible { get; set; }
        public double Rate { get; set; }
        public OPJ_PROG_ORDER ProgressionOrder { get; set; } = OPJ_PROG_ORDER.LRCP;
        public double[] RateLevels { get; set; }
        public bool IsVerbose { get; set; }
        public bool AllowMCT { get; set; }
        public bool UpdatePhotometricInterpretation { get; set; }
        public bool EncodeSignedPixelValuesAsUnsigned { get; set; }
    }

    public abstract class DicomJpeg2000Codec : IDicomCodec
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
            return new DicomJpeg2000Params();
        }

        public unsafe PinnedByteArray ExtractDataLineByLinefor8bit(PinnedByteArray destData, int pixelCount, opj_image_comp_t* component, int offsetAcumulator, int offset)
        {
            if (Convert.ToBoolean(component->sgnd))
            {
                byte sign = (byte)(1 << (byte)(component->prec - 1));
                byte mask = (byte)(0xFF ^ sign);

                for (int p = 0; p < 2 * pixelCount; p++)
                {
                    try
                    {
                        int i = component->data[p];
                        if (i < 0)
                            //destArray->Data[pos] = (unsigned char)(-i | sign);
                            destData.Data[offsetAcumulator] = (byte)((i & mask) | sign);
                        else
                            //destArray->Data[pos] = (unsigned char)(i);
                            destData.Data[offsetAcumulator] = (byte)(i & mask);
                        offsetAcumulator += offset;
                    }
                    catch (DicomCodecException e)
                    {
                        throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                    }
                    catch (Exception e)
                    {
                        throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                    }
                }
            }
            else
            {
                for (int p = 0; p < pixelCount; p++)
                {
                    try
                    {
                        destData.Data[offsetAcumulator] = (byte)component->data[p];
                        offsetAcumulator += offset;
                    }
                    catch (DicomCodecException e)
                    {
                        throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                    }
                    catch (Exception e)
                    {
                        throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                    }
                }
            }

            return destData;
        }

        public unsafe PinnedByteArray ExtractDataLineByLinefor16bit(PinnedByteArray destData, int pixelCount, opj_image_comp_t* component, int offsetAcumulator, int offset)
        {
            ushort sign = (ushort)(1 << (ushort)(component->prec - 1));
            ushort mask = (ushort)(0xFFFF ^ sign);
            ushort* destData16 = (ushort*)(void*)destData.Pointer;

            if (Convert.ToBoolean(component->sgnd))
            {
                try
                {
                    for (int p = 0; p < pixelCount; p++)
                    {
                        int i = component->data[p];

                        if (i < 0)
                            destData16[offsetAcumulator] = (ushort)((i & mask) | sign);
                        else
                            destData16[offsetAcumulator] = (ushort)(i & mask);
                        offsetAcumulator += offset;
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
            }
            else
            {
                for (int p = 0; p < pixelCount; p++)
                {
                    try
                    {
                        var pixel = (ushort)component->data[p];
                        destData16[offsetAcumulator] = pixel;
                        offsetAcumulator += offset;
                    }
                    catch (DicomCodecException e)
                    {
                        throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                    }
                    catch (Exception e)
                    {
                        throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                    }
                }
            }

            return destData;
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

    public abstract class DicomJpeg2000NativeCodec : DicomJpeg2000Codec
    {
        //Encode OpenJPEG library for win_x64 or win_arm64

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_create_compress")]
        public static extern unsafe void* Opj_create_compress_win(OPJ_CODEC_FORMAT format);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_image_create")]
        public static extern unsafe opj_image_t* Opj_image_create_win(uint numcmpts, ref opj_image_cmptparm_t cmptparms, OPJ_COLOR_SPACE clrspc);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_setup_encoder")]
        public static extern unsafe void Opj_setup_encoder_win(void* codec, ref opj_cparameters_t parameters, opj_image_t* image);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_create_stream")]
        public static extern unsafe void* Opj_create_stream_win(byte* buffer, uint length, bool isDecompressor);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_encode")]
        public static extern unsafe int Opj_encode_win(void* codec, void* stream, opj_image_t* image);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_stream_close")]
        public static extern unsafe void Opj_stream_close_win(void* stream);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_image_destroy")]
        public static extern unsafe void Opj_image_destroy_win(opj_image_t* image);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_destroy_compress")]
        public static extern unsafe void Opj_destroy_compress_win(void* codec);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_stream_tell")]
        public static extern unsafe long Opj_stream_tell_win(void* stream);

        //Decode OpenJPEG library for win_x64 or win_arm64

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_create_decompress")]
        public static extern unsafe void* Opj_create_decompress_win(OPJ_CODEC_FORMAT format);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_setup_decoder")]
        public static extern unsafe void Opj_setup_decoder_win(void* codec, opj_dparameters_t* parameters);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_set_default_decode_parameters")]
        public static extern unsafe void Opj_set_default_decode_parameters_win(opj_dparameters_t* parameters);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_decode")]
        public static extern unsafe opj_image_t* Opj_decode_win(void* codec, void* stream);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_destroy_decompress")]
        public static extern unsafe void Opj_destroy_decompress_win(void* codec);

        [DllImport("Dicom.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetCodecFormat")]
        public static extern unsafe OPJ_CODEC_FORMAT GetCodecFormat_win(byte* buffer);

        //Encode OpenJPEG library

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_create_compress")]
        public static extern unsafe void* Opj_create_compress(OPJ_CODEC_FORMAT format);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_image_create")]
        public static extern unsafe opj_image_t* Opj_image_create(int numcmpts, ref opj_image_cmptparm_t cmptparms, OPJ_COLOR_SPACE clrspc);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_setup_encoder")]
        public static extern unsafe void Opj_setup_encoder(void* codec, ref opj_cparameters_t parameters, opj_image_t* image);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_create_stream")]
        public static extern unsafe void* Opj_create_stream(byte* buffer, uint length, bool isDecompressor);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_encode")]
        public static extern unsafe int Opj_encode(void* codec, void* stream, opj_image_t* image);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_stream_close")]
        public static extern unsafe void Opj_stream_close(void* stream);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_image_destroy")]
        public static extern unsafe void Opj_image_destroy(opj_image_t* image);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_destroy_compress")]
        public static extern unsafe void Opj_destroy_compress(void* codec);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_stream_tell")]
        public static extern unsafe long Opj_stream_tell(void* stream);

        //Decode OpenJPEG library

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_create_decompress")]
        public static extern unsafe void* Opj_create_decompress(OPJ_CODEC_FORMAT format);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_setup_decoder")]
        public static extern unsafe void Opj_setup_decoder(void* codec, opj_dparameters_t* parameters);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_set_default_decode_parameters")]
        public static extern unsafe void Opj_set_default_decode_parameters(opj_dparameters_t* parameters);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_decode")]
        public static extern unsafe opj_image_t* Opj_decode(void* codec, void* stream);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_destroy_decompress")]
        public static extern unsafe void Opj_destroy_decompress(void* codec);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetCodecFormat")]
        public static extern unsafe OPJ_CODEC_FORMAT GetCodecFormat(byte* buffer);

        public static OPJ_COLOR_SPACE getOpenJpegColorSpace(PhotometricInterpretation photometricInterpretation)
        {
            if (photometricInterpretation == PhotometricInterpretation.Rgb)
                return OPJ_COLOR_SPACE.CLRSPC_SRGB;
            else if (photometricInterpretation == PhotometricInterpretation.Monochrome1 || photometricInterpretation == PhotometricInterpretation.Monochrome2)
                return OPJ_COLOR_SPACE.CLRSPC_GRAY;
            else if (photometricInterpretation == PhotometricInterpretation.PaletteColor)
                return OPJ_COLOR_SPACE.CLRSPC_GRAY;
            else if (photometricInterpretation == PhotometricInterpretation.YbrFull || photometricInterpretation == PhotometricInterpretation.YbrFull422 || photometricInterpretation == PhotometricInterpretation.YbrPartial422)
                return OPJ_COLOR_SPACE.CLRSPC_SYCC;
            else
                return OPJ_COLOR_SPACE.CLRSPC_UNKNOWN;
        }

        public override void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            if (Platform.Current == Platform.Type.unsupported)
            {
                throw new InvalidOperationException("Unsupported OS Platform");
            }

            unsafe
            {
                if ((oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrPartial422) ||
                        (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrPartial420))
                    throw new DicomCodecException($"Photometric Interpretation {oldPixelData.PhotometricInterpretation} not supported by JPEG 2000 encoder");

                DicomJpeg2000Params jparams = (DicomJpeg2000Params)parameters;

                if (jparams == null)
                    jparams = (DicomJpeg2000Params)GetDefaultParameters();

                int pixelCount = oldPixelData.Height * oldPixelData.Width;

                var pool = ArrayPool<byte>.Shared;
                byte[] cbuf = null;

                try
                {
                    for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
                    {
                        IByteBuffer frameData = oldPixelData.GetFrame(frame);

                        //Converting photometric interpretation YbrFull or YbrFull422 to RGB
                        if (oldPixelData.PlanarConfiguration == PlanarConfiguration.Planar && oldPixelData.SamplesPerPixel > 1)
                        {
                            if (oldPixelData.SamplesPerPixel != 3 || oldPixelData.BitsStored > 8)
                                throw new DicomCodecException("Planar reconfiguration only implemented for SamplesPerPixel=3 && BitsStored <= 8");

                            frameData = PixelDataConverter.PlanarToInterleaved24(new MemoryByteBuffer(frameData.Data));
                            oldPixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;

                            if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                                frameData = PixelDataConverter.YbrFullToRgb(frameData);
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

                        try
                        {
                            opj_image_cmptparm_t[] cmptparm = new opj_image_cmptparm_t[3];

                            opj_cparameters_t eparams = new opj_cparameters_t();
                            void* codec = null;  /* handle to a compressor */
                            opj_image_t* image = null;
                            void* c_stream = null;

                            if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                            {
                                codec = Opj_create_compress_win(OPJ_CODEC_FORMAT.CODEC_J2K);
                            }
                            else
                            {
                                codec = Opj_create_compress(OPJ_CODEC_FORMAT.CODEC_J2K);
                            }

                            eparams.cp_cinema = OPJ_CINEMA_MODE.OFF;
                            eparams.max_comp_size = 0;
                            eparams.numresolution = 6;
                            eparams.cp_rsiz = OPJ_RSIZ_CAPABILITIES.STD_RSIZ;
                            eparams.cblockw_init = 64;
                            eparams.cblockh_init = 64;
                            eparams.prog_order = jparams.ProgressionOrder;
                            eparams.roi_compno = -1;
                            eparams.subsampling_dx = 1;
                            eparams.subsampling_dy = 1;
                            eparams.tp_on = 0;
                            eparams.decod_format = -1;
                            eparams.cod_format = -1;
                            eparams.tcp_rates[0] = 0;
                            eparams.tcp_numlayers = 0;
                            eparams.cp_disto_alloc = 0;
                            eparams.cp_fixed_alloc = 0;
                            eparams.cp_fixed_quality = 0;
                            eparams.jpip_on = 0;
                            //eparams.cp_disto_alloc = 1;

                            if (newPixelData.Syntax == DicomTransferSyntax.JPEG2000Lossy && jparams.Irreversible)
                                eparams.irreversible = 1;

                            int r = 0;
                            for (; r < jparams.RateLevels.Length; r++)
                            {
                                if (jparams.RateLevels[r] > jparams.Rate)
                                {
                                    eparams.tcp_numlayers++;
                                    eparams.tcp_rates[r] = (float)jparams.RateLevels[r];
                                }
                                else
                                    break;
                            }

                            eparams.tcp_numlayers++;
                            eparams.tcp_rates[r] = (float)(jparams.Rate * oldPixelData.BitsStored / oldPixelData.BitsAllocated);

                            if (newPixelData.Syntax == DicomTransferSyntax.JPEG2000Lossless && jparams.Rate > 0)
                                eparams.tcp_rates[eparams.tcp_numlayers++] = 0;

                            if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.Rgb && jparams.AllowMCT)
                                eparams.tcp_mct = 1;

                            for (int i = 0; i < oldPixelData.SamplesPerPixel; i++)
                            {
                                cmptparm[i].bpp = oldPixelData.BitsAllocated;
                                cmptparm[i].prec = oldPixelData.BitsStored;
                                if (!jparams.EncodeSignedPixelValuesAsUnsigned)
                                    cmptparm[i].sgnd = (uint)(oldPixelData.PixelRepresentation == PixelRepresentation.Signed ? 1 : 0);

                                cmptparm[i].dx = (uint)eparams.subsampling_dx;
                                cmptparm[i].dy = (uint)eparams.subsampling_dy;
                                cmptparm[i].h = oldPixelData.Height;
                                cmptparm[i].w = oldPixelData.Width;
                            }

                            try
                            {
                                OPJ_COLOR_SPACE color_space = getOpenJpegColorSpace(oldPixelData.PhotometricInterpretation);

                                if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                    image = Opj_image_create_win(oldPixelData.SamplesPerPixel, ref cmptparm[0], color_space);
                                else
                                    image = Opj_image_create(oldPixelData.SamplesPerPixel, ref cmptparm[0], color_space);

                                image->x0 = (uint)eparams.image_offset_x0;
                                image->y0 = (uint)eparams.image_offset_y0;
                                image->x1 = (uint)(image->x0 + ((oldPixelData.Width - 1) * eparams.subsampling_dx) + 1);
                                image->y1 = (uint)(image->y0 + ((oldPixelData.Height - 1) * eparams.subsampling_dy) + 1);

                                for (int c = 0; c < image->numcomps; c++)
                                {
                                    opj_image_comp_t* comp = &image->comps[c];

                                    int pos = oldPixelData.PlanarConfiguration == PlanarConfiguration.Planar ? (c * pixelCount) : c;
                                    int offset = oldPixelData.PlanarConfiguration == PlanarConfiguration.Planar ? 1 : (int)image->numcomps;

                                    if (oldPixelData.BytesAllocated == 1)
                                    {
                                        if (Convert.ToBoolean(comp->sgnd))
                                        {
                                            if (oldPixelData.BitsStored < 8)
                                            {
                                                byte sign = (byte)(1 << oldPixelData.HighBit);
                                                byte mask = (byte)(0xff >> (oldPixelData.BitsAllocated - oldPixelData.BitsStored));
                                                for (int p = 0; p < pixelCount; p++)
                                                {
                                                    byte pixel = frameArray.Data[pos];
                                                    if (Convert.ToBoolean(pixel & sign))
                                                        comp->data[p] = -(((-pixel) & mask) + 1);
                                                    else
                                                        comp->data[p] = pixel;
                                                    pos += offset;
                                                }
                                            }
                                            else
                                            {
                                                char* frameData8 = (char*)(void*)frameArray.Pointer;
                                                for (int p = 0; p < pixelCount; p++)
                                                {
                                                    comp->data[p] = frameData8[pos];
                                                    pos += offset;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int p = 0; p < pixelCount; p++)
                                            {
                                                comp->data[p] = frameArray.Data[pos];
                                                pos += offset;
                                            }
                                        }
                                    }
                                    else if (oldPixelData.BytesAllocated == 2)
                                    {
                                        if (Convert.ToBoolean(comp->sgnd))
                                        {
                                            if (oldPixelData.BitsStored < 16)
                                            {
                                                ushort* frameData16 = (ushort*)(void*)frameArray.Pointer;
                                                ushort sign = (ushort)(1 << oldPixelData.HighBit);
                                                ushort mask = (ushort)(0xffff >> (oldPixelData.BitsAllocated - oldPixelData.BitsStored));
                                                for (int p = 0; p < pixelCount; p++)
                                                {
                                                    ushort pixel = frameData16[pos];
                                                    if (Convert.ToBoolean(pixel & sign))
                                                        comp->data[p] = -(((-pixel) & mask) + 1);
                                                    else
                                                        comp->data[p] = pixel;
                                                    pos += offset;
                                                }
                                            }
                                            else
                                            {
                                                short* frameData16 = (short*)frameArray.Pointer.ToPointer();
                                                for (int p = 0; p < pixelCount; p++)
                                                {
                                                    comp->data[p] = frameData16[pos];
                                                    pos += offset;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (oldPixelData.BitsStored < 16)
                                            {
                                                ushort* frameData16 = (ushort*)frameArray.Pointer.ToPointer();
                                                ushort mask = (ushort)(0xffff >> (oldPixelData.BitsAllocated - oldPixelData.BitsStored));

                                                for (int p = 0; p < pixelCount; p++)
                                                {
                                                    ushort pixel = frameData16[pos];
                                                    comp->data[p] = pixel & mask;
                                                    pos += offset;
                                                }
                                            }
                                            else
                                            {
                                                ushort* frameData16 = (ushort*)frameArray.Pointer.ToPointer();
                                                for (int p = 0; p < pixelCount; p++)
                                                {
                                                    comp->data[p] = frameData16[pos];
                                                    pos += offset;
                                                }
                                            }
                                        }
                                    }
                                    else
                                        throw new DicomCodecException("JPEG 2000 codec only supports Bits Allocated == 8 or 16");
                                }

                                uint img_size = 0;
                                for (int i = 0; i < image->numcomps; i++)
                                {
                                    img_size += image->comps[i].w * image->comps[i].h * image->comps[i].prec;
                                }

                                var outlen = (uint)(0.1625 * img_size + 2000); /* 0.1625 = 1.3/8 and 2000 bytes as a minimum for headers */
                                var buf = new PinnedByteArray(new byte[outlen]);

                                if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                {
                                    Opj_setup_encoder_win(codec, ref eparams, image);
                                    c_stream = Opj_create_stream_win((byte*)buf.Pointer, (uint)buf.ByteSize, false);
                                }
                                else
                                {
                                    Opj_setup_encoder(codec, ref eparams, image);
                                    c_stream = Opj_create_stream((byte*)buf.Pointer, (uint)buf.ByteSize, false);
                                }

                                var isEncodeSuccess = false;
                                if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                    isEncodeSuccess = Convert.ToBoolean(Opj_encode_win(codec, c_stream, image));
                                else
                                    isEncodeSuccess = Convert.ToBoolean(Opj_encode(codec, c_stream, image));

                                if (isEncodeSuccess)
                                {
                                    int clen = 0;

                                    if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                        clen = (int)Opj_stream_tell_win(c_stream);
                                    else
                                        clen = (int)Opj_stream_tell(c_stream);

                                    //cbuf = pool.Rent(clen);
                                    //Marshal.Copy(buf.Pointer, cbuf, 0, clen);
                                    var cbuf1 = buf.Data.Take(clen).ToArray();

                                    IByteBuffer buffer;
                                    if (clen >= NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                                    {
                                        buffer = new TempFileBuffer(cbuf1);
                                        buffer = EvenLengthBuffer.Create(buffer);
                                    }
                                    else
                                        buffer = new MemoryByteBuffer(cbuf1);

                                    if (oldPixelData.NumberOfFrames == 1)
                                        buffer = EvenLengthBuffer.Create(buffer);

                                    newPixelData.AddFrame(buffer);
                                }
                                else
                                    throw new DicomCodecException("Unable to JPEG 2000 encode image");
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
                                if (c_stream != null)
                                {
                                    if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                        Opj_stream_close_win(c_stream);
                                    else
                                        Opj_stream_close(c_stream);
                                }

                                if (image != null)
                                {
                                    if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                        Opj_image_destroy_win(image);
                                    else
                                        Opj_image_destroy(image);
                                }

                                if (codec != null)
                                {
                                    if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                        Opj_destroy_compress_win(codec);
                                    else
                                        Opj_destroy_compress(codec);
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
                catch (Exception e)
                {
                    throw new DicomCodecException(e.Message + " => " + e.StackTrace);
                }
                finally
                {
                    if (cbuf != null)
                    {
                        pool.Return(cbuf);
                        cbuf = null;
                    }
                }

                if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.Rgb || oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull || oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                {
                    newPixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;

                    if (jparams.AllowMCT && jparams.UpdatePhotometricInterpretation)
                    {
                        if (newPixelData.Syntax == DicomTransferSyntax.JPEG2000Lossy && jparams.Irreversible)
                            newPixelData.PhotometricInterpretation = PhotometricInterpretation.YbrIct;
                        else
                            newPixelData.PhotometricInterpretation = PhotometricInterpretation.YbrRct;
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

            DicomJpeg2000Params jparams = (DicomJpeg2000Params)parameters;

            if (jparams == null)
                jparams = (DicomJpeg2000Params)GetDefaultParameters();

            if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrIct || oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrRct)
                newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;

            if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422 || oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrPartial422 || oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;

            //if (newPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
            //    newPixelData.PlanarConfiguration = PlanarConfiguration.Planar;

            for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
            {
                IByteBuffer j2kData = oldPixelData.GetFrame(frame);

                try
                {
                    //Converting photometricinterpretation YbrFull or YbrFull422 to RGB
                    if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                    {
                        j2kData = PixelDataConverter.YbrFullToRgb(j2kData);
                    }
                    else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                    {
                        j2kData = PixelDataConverter.YbrFull422ToRgb(j2kData, oldPixelData.Width);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot convert J2k buffer data from PhotometricInterpretation = {0} to RGB => {1} => {2}", oldPixelData
                    .PhotometricInterpretation.ToString(), ex.Message, ex.StackTrace);
                }

                PinnedByteArray j2kArray = new PinnedByteArray(j2kData.Data);
                PinnedByteArray destArray = new PinnedByteArray(newPixelData.UncompressedFrameSize);

                try
                {
                    unsafe
                    {
                        opj_dparameters_t dparams = new opj_dparameters_t();
                        opj_image_t* image = null;
                        void* codec = null;
                        void* d_stream = null;

                        if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                            Opj_set_default_decode_parameters_win(&dparams);
                        else
                            Opj_set_default_decode_parameters(&dparams);

                        dparams.cp_layer = 0;
                        dparams.cp_reduce = 0;

                        byte* buf = (byte*)(void*)j2kArray.Pointer;

                        OPJ_CODEC_FORMAT format;

                        try
                        {
                            format = OPJ_CODEC_FORMAT.CODEC_UNKNOWN;
                            if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                            {
                                format = GetCodecFormat_win(buf);
                                dparams.decod_format = (int)format;

                                codec = Opj_create_decompress_win(format);
                                Opj_setup_decoder_win(codec, &dparams);

                                d_stream = Opj_create_stream_win(buf, (uint)j2kArray.ByteSize, true);
                                image = Opj_decode_win(codec, d_stream);
                            }
                            else
                            {
                                format = GetCodecFormat(buf);
                                dparams.decod_format = (int)format;

                                codec = Opj_create_decompress(format);
                                Opj_setup_decoder(codec, &dparams);

                                d_stream = Opj_create_stream(buf, (uint)j2kArray.ByteSize, true);
                                image = Opj_decode(codec, d_stream);
                            }

                            int pixelCount = 0;

                            if (image == null)
                            {
                                throw new DicomCodecException("Error in JPEG 2000 decode stream => output image data is null");
                            }
                            else
                            {
                                pixelCount = (int)(image->x1 * image->y1);
                            }

                            for (int c = 0; c < image->numcomps; c++)
                            {
                                opj_image_comp_t* comp = &image->comps[c];

                                if (comp->data == null)
                                {
                                    throw new DicomCodecException("Error in JPEG 2000 decode stream => output image component data is null");
                                }
                                else
                                {
                                    if (comp->h != image->y1)
                                        throw new DicomCodecException("Error in JPEG 2000 decode stream");

                                    if (comp->w != image->x1)
                                        throw new DicomCodecException("Error in JPEG 2000 decode stream");
                                }

                                int pos = newPixelData.PlanarConfiguration == PlanarConfiguration.Planar ? (c * pixelCount) : c;
                                int offset = (int)(newPixelData.PlanarConfiguration == PlanarConfiguration.Planar ? 1 : image->numcomps);

                                var prec = comp->prec < oldPixelData.BitsStored ? oldPixelData.BitsStored : comp->prec;

                                if (oldPixelData.BytesAllocated == 1)
                                {
                                    if (prec <= 8)
                                    {
                                        destArray = ExtractDataLineByLinefor8bit(destArray, pixelCount, comp, pos, offset);
                                    }
                                }
                                else if (oldPixelData.BytesAllocated == 2)
                                {   
                                    if (prec <= 8)
                                    {
                                        destArray = ExtractDataLineByLinefor8bit(destArray, pixelCount, comp, pos, offset);
                                    }
                                    else if (prec <= 16)
                                    {
                                        destArray = ExtractDataLineByLinefor16bit(destArray, pixelCount, comp, pos, offset);
                                    }
                                }
                                else
                                    throw new DicomCodecException("JPEG 2000 module only supports bits Allocated == 8 or 16!");
                            }

                            IByteBuffer buffer;
                            if (destArray.Count >= NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                                buffer = new TempFileBuffer(destArray.Data);
                            else
                                buffer = new MemoryByteBuffer(destArray.Data);

                            if (oldPixelData.NumberOfFrames == 1)
                                buffer = EvenLengthBuffer.Create(buffer);

                            newPixelData.AddFrame(buffer);
                        }
                        catch (DicomCodecException ex)
                        {
                            throw new DicomCodecException(ex.Message + " => " + ex.StackTrace);
                        }
                        finally
                        {
                            if (codec != null)
                            {
                                if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                    Opj_destroy_decompress_win(codec);
                                else
                                    Opj_destroy_decompress(codec);
                            }

                            if (image != null)
                            {
                                if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                    Opj_image_destroy_win(image);
                                else
                                    Opj_image_destroy(image);
                            }

                            if (d_stream != null)
                            {
                                if (Platform.Current.Equals(Platform.Type.win_x64) || Platform.Current.Equals(Platform.Type.win_arm64))
                                {
                                    Opj_stream_close_win(d_stream);
                                }
                                else
                                    Opj_stream_close(d_stream);
                            }
                        }
                    }
                }
                catch (DicomCodecException ex)
                {
                    throw new DicomCodecException(ex.Message + " => " + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    throw new DicomCodecException(ex.Message + " => " + ex.StackTrace);
                }
                finally
                {
                    if (j2kArray != null)
                    {
                        j2kArray.Dispose();
                        j2kArray = null;
                    }

                    if (destArray != null)
                    {
                        destArray.Dispose();
                        destArray = null;
                    }
                }
            }
        }
    }

    public class DicomJpeg2000LosslessCodec : DicomJpeg2000NativeCodec
    {
        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.JPEG2000Lossless;
            }
        }
    }

    public class DicomJpeg2000LossyCodec : DicomJpeg2000NativeCodec
    {
        public override DicomTransferSyntax TransferSyntax
        {
            get
            {
                return DicomTransferSyntax.JPEG2000Lossy;
            }
        }
    }
}