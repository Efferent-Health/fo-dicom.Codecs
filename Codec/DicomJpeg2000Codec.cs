using System;
using System.Runtime.InteropServices;

using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO;
using FellowOakDicom.IO.Buffer;

namespace FellowOakDicom.Imaging.NativeCodec
{   
    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate void opj_msg_callback(char *msg, void *client_data);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct opj_event_mgr_t
    {
        /** Error message callback if available, NULL otherwise */
        public IntPtr error_handler;
        /** Warning message callback if available, NULL otherwise */
        public IntPtr warning_handler;
        /** Debug message callback if available, NULL otherwise */
        public IntPtr info_handler;
    }

    [Flags]
    public enum OPJ_CODEC_FORMAT
    {
        CODEC_UNKNOWN = -1, /**< place-holder */
        CODEC_J2K = 0,      /**< JPEG-2000 codestream : read/write */
        CODEC_JPT = 1,      /**< JPT-stream (JPEG 2000, JPIP) : read only */
        CODEC_JP2 = 2 		/**< JPEG-2000 file format : read/write */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_common_ptr
    {
        public opj_event_mgr_t* event_mgr;
        public void* client_data;
        public int is_decompressor;
        public OPJ_CODEC_FORMAT codec_format;
        public void* j2k_handle;
        public void* jp2_handle;
        public void* mj2_handle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_cinfo_t
    {
        public opj_event_mgr_t* event_mgr;
        public void* client_data;
        public int is_decompressor;
        public OPJ_CODEC_FORMAT codec_format;
        public void* j2k_handle;
        public void* jp2_handle;
        public void* mj2_handle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_cio_t
    {
        /** open mode (read/write) either OPJ_STREAM_READ or OPJ_STREAM_WRITE */
        public opj_common_ptr* cinfo;
        public int openmode;
        /** pointer to the start of the buffer */
        public byte* buffer;
        /** buffer size in bytes */
        public int length;
        /** pointer to the start of the stream */
        public byte* start;
        /** pointer to the end of the stream */
        public byte* end;
        /** pointer to the current position */
        public byte* bp;
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
        public int dx;
        /** YRsiz: vertical separation of a sample of ith component with respect to the reference grid */
        public int dy;
        /** data width */
        public int w;
        /** data height */
        public int h;
        /** x component offset compared to the whole image */
        public int x0;
        /** y component offset compared to the whole image */
        public int y0;
        /** precision */
        public int prec;
        /** image depth in bits */
        public int bpp;
        /** signed (1) / unsigned (0) */
        public int sgnd;
        /** number of decoded resolution */
        public int resno_decoded;
        /** number of division by 2 of the out image compared to the original size of image */
        public int factor;
        /** image component data */
        public int* data;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_image_t
    {
        /** XOsiz: horizontal offset from the origin of the reference grid to the left side of the image area */
        public int x0;
        /** YOsiz: vertical offset from the origin of the reference grid to the top side of the image area */
        public int y0;
        /** Xsiz: width of the reference grid */
        public int x1;
        /** Ysiz: height of the reference grid */
        public int y1;
        /** number of components in the image */
        public int numcomps;
        /** color space: sRGB, Greyscale or YUV */
        public OPJ_COLOR_SPACE color_space;
        /** image components */
        public opj_image_comp_t* comps;
        /** 'restricted' ICC profile */
        public byte* icc_profile_buf;
        /** size of ICC profile */
        public int icc_profile_len;
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
        public unsafe int* cp_comment;
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
        public int numpocs;
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
        public char tp_on;
        /** Flag for Tile part generation*/
        public char tp_flag;
        /** MCT (multiple component transform) */
        public char tcp_mct;
        /** Enable JPIP indexing*/
        public int jpip_on;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct opj_poc_t
    {
        public int resno0, compno0;
        /** Layer num end,Resolution num end, Component num end, given by POC */
        public int layno1, resno1, compno1;
        /** Layer num start,Precinct num start, Precinct num end */
        public int layno0, precno0, precno1;
        /** Progression order enum*/
        public OPJ_PROG_ORDER prg1, prg;
        /** Progression order string*/
        public unsafe fixed sbyte progorder[5];
        /** Tile number */
        public int tile;
        /** Start and end values for Tile width and height*/
        public int tx0, tx1, ty0, ty1;
        /** Start value, initialised in pi_initialise_encode*/
        public int layS, resS, compS, prcS;
        /** End value, initialised in pi_initialise_encode */
        public int layE, resE, compE, prcE;
        /** Start and end values of Tile width and height, initialised in pi_initialise_encode*/
        public int txS, txE, tyS, tyE, dx, dy;
        /** Temporary values for Tile parts, initialised in pi_create_encode */
        public int lay_t, res_t, comp_t, prc_t, tx0_t, ty0_t;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_image_cmptparm_t
    {
        public int dx;
        /** YRsiz: vertical separation of a sample of ith component with respect to the reference grid */
        public int dy;
        /** data width */
        public int w;
        /** data height */
        public int h;
        /** x component offset compared to the whole image */
        public int x0;
        /** y component offset compared to the whole image */
        public int y0;
        /** precision */
        public int prec;
        /** image depth in bits */
        public int bpp;
        /** signed (1) / unsigned (0) */
        public int sgnd;
    }

    [Flags]
    public enum OPJ_LIMIT_DECODING
    {
        NO_LIMITATION = 0,
        LIMIT_TO_MAIN_HEADER = 1,
        DECODE_ALL_BUT_PACKETS = 2
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_dparameters_t
    {
        public int cp_reduce;
        public int cp_layer;
        public unsafe fixed sbyte infile[4096];
        public unsafe fixed sbyte outfile[4096];
        public OPJ_CODEC_FORMAT decod_format;
        public int cod_format;
        public int jpwl_correct;
        public int jpwl_exp_comps;
        public int jpwl_max_tiles;
        public OPJ_LIMIT_DECODING cp_limit_decoding;
        public uint flags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct opj_dinfo_t
    {
        public opj_event_mgr_t* event_mgr;
        public void* client_data;
        public int is_decompressor;
        public OPJ_CODEC_FORMAT codec_format;
        public void* j2k_handle;
        public void* jp2_handle;
        public void* mj2_handle;
    }

    public class DicomJpeg2000Params : DicomCodecParams
    {
        private int[] _rates;
        public DicomJpeg2000Params()
        {
            Irreversible = true;
            Rate = 20;
            IsVerbose = false;
            AllowMCT = true;
            UpdatePhotometricInterpretation = true;
            EncodeSignedPixelValuesAsUnsigned = false;

            _rates = new int[9];
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
        public int Rate { get; set; }
        public OPJ_PROG_ORDER ProgressionOrder { get; set; } = OPJ_PROG_ORDER.LRCP;
        public int[] RateLevels { get; set; }
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
        //Encode OpenJPEG library

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_create_compress")]
        public static extern unsafe opj_cinfo_t* Opj_create_compress(OPJ_CODEC_FORMAT format);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_set_event_mgr")]
        public static extern unsafe opj_event_mgr_t* Opj_set_event_mgr(opj_common_ptr* cinfo, opj_event_mgr_t* e, void* context);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_image_create")]
        public static extern unsafe opj_image_t* Opj_image_create(int numcmpts, ref opj_image_cmptparm_t cmptparms, OPJ_COLOR_SPACE clrspc);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_setup_encoder")]
        public static extern unsafe void Opj_setup_encoder(opj_cinfo_t* cinfo, ref opj_cparameters_t parameters, opj_image_t* image);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_cio_open")]
        public static extern unsafe opj_cio_t* Opj_cio_open(opj_common_ptr* cinfo, byte* buffer, int length);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_encode")]
        public static extern unsafe int Opj_encode(opj_cinfo_t* cinfo, opj_cio_t* cio, opj_image_t* image, sbyte* index);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_cio_close")]
        public static extern unsafe void Opj_cio_close(opj_cio_t* cio);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_image_destroy")]
        public static extern unsafe void Opj_image_destroy(opj_image_t* image);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_destroy_compress")]
        public static extern unsafe void Opj_destroy_compress(opj_cinfo_t* cinfo);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Cio_tell")]
        public static extern unsafe int Cio_tell(opj_cio_t* cio);

        //Decode OpenJPEG library

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_create_decompress")]
        public static extern unsafe opj_dinfo_t* Opj_create_decompress(OPJ_CODEC_FORMAT format);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_setup_decoder")]
        public static extern unsafe void Opj_setup_decoder(opj_dinfo_t* dinfo, opj_dparameters_t* parameters);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_set_default_decode_parameters")]
        public static extern unsafe void Opj_set_default_decoder(opj_dparameters_t* parameters);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_decode")]
        public static extern unsafe opj_image_t* Opj_decode(opj_dinfo_t* dinfo, opj_cio_t* cio);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Opj_destroy_decompress")]
        public static extern unsafe void Opj_destroy_decompress(opj_dinfo_t* dinfo);

        [DllImport("Dicom.Native", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Memset")]
        public static extern unsafe void Memset(void * ptr, int value, uint num);

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

        public static unsafe void opj_error_callback(char * msg, void * usr)
        {    
        }
        public static unsafe void opj_warning_callback(char * msg, void * usr)
        {
        } 
        public static unsafe void opj_info_callback(char * msg, void * usr)
        {
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

                for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
                {
                    IByteBuffer frameData = oldPixelData.GetFrame(frame);

                    //Converting photmetricinterpretation YbrFull or YbrFull422 to RGB
                    if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                    {
                        frameData = PixelDataConverter.YbrFullToRgb(frameData);
                    }
                    else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                    {
                        frameData = PixelDataConverter.YbrFull422ToRgb(frameData, oldPixelData.Width);
                    }    

                    PinnedByteArray frameArray = new PinnedByteArray(frameData.Data);

                    opj_image_cmptparm_t[] cmptparm = new opj_image_cmptparm_t[3];

                    opj_cparameters_t eparams= new opj_cparameters_t();
                    opj_event_mgr_t event_mgr = new opj_event_mgr_t();
                    opj_cinfo_t* cinfo= null;  /* handle to a compressor */
                    opj_image_t* image = null;
                    opj_cio_t* cio = null;
                    
                    event_mgr.error_handler = IntPtr.Zero;
                    if (jparams.IsVerbose)
                    {
                        event_mgr.warning_handler = IntPtr.Zero;
                        event_mgr.info_handler = IntPtr.Zero;
                    }

                    cinfo = Opj_create_compress(OPJ_CODEC_FORMAT.CODEC_J2K);
                    Opj_set_event_mgr((opj_common_ptr*)cinfo, &event_mgr, null);
                    
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
                    eparams.tp_on = (char)0;
                    eparams.decod_format = -1;
                    eparams.cod_format = -1; 
                    eparams.tcp_rates[0]= 0;
                    eparams.tcp_numlayers = 0;
                    eparams.cp_disto_alloc = 0;
                    eparams.cp_fixed_alloc = 0;
                    eparams.cp_fixed_quality = 0;
                    eparams.jpip_on = 0; 
                    eparams.cp_disto_alloc = 1;

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
                    eparams.tcp_rates[r] = (float)jparams.Rate;

                    if (newPixelData.Syntax == DicomTransferSyntax.JPEG2000Lossless && jparams.Rate > 0)
                        eparams.tcp_rates[eparams.tcp_numlayers++] = 0;

                    if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.Rgb && jparams.AllowMCT)
                        eparams.tcp_mct = (char)1;

                    for (int i = 0; i < oldPixelData.SamplesPerPixel; i++)
                    {
                        cmptparm[i].bpp = oldPixelData.BitsAllocated;
                        cmptparm[i].prec = oldPixelData.BitsStored;
                        if (!jparams.EncodeSignedPixelValuesAsUnsigned)
                            cmptparm[i].sgnd = Convert.ToInt32(oldPixelData.PixelRepresentation == PixelRepresentation.Signed);

                        cmptparm[i].dx = eparams.subsampling_dx;
                        cmptparm[i].dy = eparams.subsampling_dy;
                        cmptparm[i].h = oldPixelData.Height;
                        cmptparm[i].w = oldPixelData.Width;
                    }

                    try
                    {
                        OPJ_COLOR_SPACE color_space = getOpenJpegColorSpace(oldPixelData.PhotometricInterpretation);

                        image = Opj_image_create(oldPixelData.SamplesPerPixel, ref cmptparm[0], color_space);

                        image->x0 = eparams.image_offset_x0;
                        image->y0 = eparams.image_offset_y0;
                        image->x1 = image->x0 + ((oldPixelData.Width - 1) * eparams.subsampling_dx) + 1;
                        image->y1 = image->y0 + ((oldPixelData.Height - 1) * eparams.subsampling_dy) + 1;

                        for (int c = 0; c < image->numcomps; c++)
                        {
                            opj_image_comp_t* comp = &image->comps[c];

                            int pos = oldPixelData.PlanarConfiguration == PlanarConfiguration.Planar ? (c * pixelCount) : c;
                            int offset = oldPixelData.PlanarConfiguration == PlanarConfiguration.Planar ? 1 : image->numcomps;
                            
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
                                        short* frameData16 = (short*)(void*)frameArray.Pointer;
                                        for (int p = 0; p < pixelCount; p++)
                                        {
                                            comp->data[p] = frameData16[pos];
                                            pos += offset;
                                        }
                                    }
                                }
                                else
                                {
                                    ushort* frameData16 = (ushort*)(void*)frameArray.Pointer;
                                    for (int p = 0; p < pixelCount; p++)
                                    {
                                        comp->data[p] = frameData16[pos];
                                        pos += offset;
                                    }
                                }
                            }
                            else
                                throw new DicomCodecException("JPEG 2000 codec only supports Bits Allocated == 8 or 16");
                        }

                        Opj_setup_encoder(cinfo, ref eparams, image);
                        cio = Opj_cio_open((opj_common_ptr*)cinfo, null, 0);

                        if (Convert.ToBoolean(Opj_encode(cinfo, cio, image, eparams.index)))
                        {
                                int clen = Cio_tell(cio);
                                byte[] cbuf = new byte[clen];

                                Marshal.Copy((IntPtr)cio->buffer, cbuf, 0, clen);

                                IByteBuffer buffer;
                                if (clen >= NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                                {
                                    buffer = new TempFileBuffer(cbuf);
                                    buffer = EvenLengthBuffer.Create(buffer);
                                }
                                else
                                    buffer = new MemoryByteBuffer(cbuf);

                                if (oldPixelData.NumberOfFrames == 1)
                                    buffer = EvenLengthBuffer.Create(buffer);

                                newPixelData.AddFrame(buffer);
                        }
                        else
                            throw new DicomCodecException("Unable to JPEG 2000 encode image");                     
                    }
                    finally
                    {
                        if (cio != null)
                        {
                            Opj_cio_close(cio);                          
                        }

                        if (image != null)
                        {
                            Opj_image_destroy(image);                           
                        }                       

                        if (cinfo != null)
                        {
                            Opj_destroy_compress(cinfo);                      
                        }
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

            int pixelCount = oldPixelData.Height * oldPixelData.Width;
            
            if (newPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrIct || newPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrRct)
                newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;

            if (newPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422 || newPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrPartial422)
                newPixelData.PhotometricInterpretation = PhotometricInterpretation.YbrFull;

            if (newPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                newPixelData.PlanarConfiguration = PlanarConfiguration.Planar;

            for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
            {
                IByteBuffer jpegData = oldPixelData.GetFrame(frame);

                //Converting photometricinterpretation YbrFull or YbrFull422 to RGB
                if(oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull)
                {
                    jpegData = PixelDataConverter.YbrFullToRgb(jpegData);
                }
                else if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422)
                {
                    jpegData = PixelDataConverter.YbrFull422ToRgb(jpegData, oldPixelData.Width);
                }

                PinnedByteArray jpegArray = new PinnedByteArray(jpegData.Data);
                PinnedByteArray destArray = new PinnedByteArray(newPixelData.UncompressedFrameSize);

                unsafe
                {
                    opj_dparameters_t dparams = new opj_dparameters_t();
                    opj_event_mgr_t event_mgr = new opj_event_mgr_t();
                    opj_image_t* image = null;
                    opj_dinfo_t* dinfo = null;
                    opj_cio_t* cio = null;
                    
                    Memset(&event_mgr,0, (uint)sizeof(opj_event_mgr_t));

                    opj_msg_callback error_handler = null;
                    opj_msg_callback warning_handler = null;
                    opj_msg_callback info_handler = null;

                    error_handler = opj_error_callback;
                    event_mgr.error_handler = Marshal.GetFunctionPointerForDelegate((error_handler));

                    if (jparams.IsVerbose)
                    {   
                        warning_handler = opj_warning_callback;
                        event_mgr.warning_handler = Marshal.GetFunctionPointerForDelegate((warning_handler));

                        info_handler = opj_info_callback;
                        event_mgr.info_handler = Marshal.GetFunctionPointerForDelegate((info_handler));
                    }

                    Opj_set_default_decoder(&dparams);
                    
                    dparams.cp_layer = 0;
                    dparams.cp_reduce = 0;

                    byte* buf = (byte*)(void*)jpegArray.Pointer;

                    OPJ_CODEC_FORMAT format;

                    try
                    {
                        format = GetCodecFormat(buf);

                        dinfo = Opj_create_decompress(format);
                        dparams.decod_format = format;

                        Opj_set_event_mgr((opj_common_ptr*)dinfo, &event_mgr, null);
                        Opj_setup_decoder(dinfo, &dparams);

                        bool opj_err = false;
                        dinfo->client_data = (void*)&opj_err;

                        cio = Opj_cio_open((opj_common_ptr*)dinfo, buf, (int)jpegArray.ByteSize);
                        image = Opj_decode(dinfo, cio);

                        if (image == null)
                            throw new DicomCodecException("Error in JPEG 2000 code stream!");

                        for (int c = 0; c < image->numcomps; c++)
                        {
                            opj_image_comp_t* comp = &image->comps[c];

                            int pos = newPixelData.PlanarConfiguration == PlanarConfiguration.Planar ? (c * pixelCount) : c;
                            int offset = newPixelData.PlanarConfiguration == PlanarConfiguration.Planar ? 1 : image->numcomps;

                            if (newPixelData.BytesAllocated == 1)
                            {
                                if (Convert.ToBoolean(comp->sgnd))
                                {
                                    byte sign = (byte)(1 << newPixelData.HighBit);
                                    byte mask = (byte)(0xFF ^ sign);
                                    for (int p = 0; p < pixelCount; p++)
                                    {
                                        int i = comp->data[p];
                                        if (i < 0)
                                            //destArray->Data[pos] = (unsigned char)(-i | sign);
                                            destArray.Data[pos] = (byte)((i & mask) | sign);
                                        else
                                            //destArray->Data[pos] = (unsigned char)(i);
                                            destArray.Data[pos] = (byte)(i & mask);
                                        pos += offset;
                                    }
                                }

                                else
                                {
                                    for (int p = 0; p < pixelCount; p++)
                                    {
                                        destArray.Data[pos] = (byte)comp->data[p];
                                        pos += offset;
                                    }
                                }
                            }

                            else if (newPixelData.BytesAllocated == 2)
                            {
                                ushort sign = (ushort)(1 << newPixelData.HighBit);
                                ushort mask = (ushort)(0xFFFF ^ sign);
                                ushort* destData16 = (ushort*)(void*)destArray.Pointer;

                                if (Convert.ToBoolean(comp->sgnd))
                                {
                                    for (int p = 0; p < pixelCount; p++)
                                    {
                                        int i = comp->data[p];

                                        if (i < 0)
                                            destData16[pos] = (ushort)((i & mask) | sign);
                                        else
                                            destData16[pos] = (ushort)(i & mask);
                                        pos += offset;
                                    }
                                }

                                else
                                {
                                    for (int p = 0; p < pixelCount; p++)
                                    {
                                        destData16[pos] = (ushort)comp->data[p];
                                        pos += offset;
                                        //Console.WriteLine("{0}",comp->data[p]);
                                    }
                                }
                            }

                            else
                                throw new DicomCodecException("JPEG 2000 module only supports Bytes Allocated == 8 or 16!");
                        }

                        IByteBuffer buffer;
                        if (destArray.Count >= NativeTranscoderManager.MemoryBufferThreshold || oldPixelData.NumberOfFrames > 1)
                            buffer = new TempFileBuffer(destArray.Data);
                        else
                            buffer = new MemoryByteBuffer(destArray.Data);

                        if (oldPixelData.NumberOfFrames == 1)
                            buffer = EvenLengthBuffer.Create(buffer);

                        newPixelData.AddFrame(buffer);

                        GC.KeepAlive(error_handler);
                        GC.KeepAlive(warning_handler);
                        GC.KeepAlive(info_handler);
                    }
                    finally
                    {
                        if (cio != null) 
                        {
                            Opj_cio_close(cio);
                        }

                        if (dinfo != null)
                        {
                            Opj_destroy_decompress(dinfo);
                        }

                        if (image != null)
                        {
                            Opj_image_destroy(image);
                        }
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