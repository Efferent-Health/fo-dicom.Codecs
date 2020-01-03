using System;
using System.IO;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;
using Dicom.Imaging.Codec.Jpeg;
using Dicom.IO;
using Dicom.IO.Buffer;
using Dicom.Log;

using Efferent.Native.Codec.Jpeg;

namespace Efferent.Native.Codec
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct SourceManagerStruct
    {
        public jpeg_source_mgr pub;
        public int skip_bytes;
        public byte* next_buffer;
        public uint next_buffer_size;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct j_common_ptr
    {
        public jpeg_error_mgr* err;
        public jpeg_progress_mgr* progress;
        public jpeg_memory_mgr* mem;
        public void* client_data;
        public int is_decompressor;
        public int global_state;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct j_compress_ptr
    {
        public jpeg_error_mgr* err;
        public jpeg_progress_mgr* progress;
        public jpeg_memory_mgr* mem;
        public void* client_data;
        public int is_decompressor;
        public int global_state;

        public jpeg_destination_mgr* dest;

        public uint image_width;
        public uint image_height;
        public int input_components;
        public J_COLOR_SPACE in_color_space;

        public double input_gamma;

        public int lossless;

        public int data_precision;
        public int num_components;
        public J_COLOR_SPACE jpeg_color_space;

        public jpeg_component_info* comp_info;

        public JQUANT_TBL* quant_tbl_ptrs_1;
        public JQUANT_TBL* quant_tbl_ptrs_2;
        public JQUANT_TBL* quant_tbl_ptrs_3;
        public JQUANT_TBL* quant_tbl_ptrs_4;


        public JHUFF_TBL* dc_huff_tbl_ptrs_1;
        public JHUFF_TBL* dc_huff_tbl_ptrs_2;
        public JHUFF_TBL* dc_huff_tbl_ptrs_3;
        public JHUFF_TBL* dc_huff_tbl_ptrs_4;


        public JHUFF_TBL* ac_huff_tbl_ptrs_1;
        public JHUFF_TBL* ac_huff_tbl_ptrs_2;
        public JHUFF_TBL* ac_huff_tbl_ptrs_3;
        public JHUFF_TBL* ac_huff_tbl_ptrs_4;

        public fixed byte arith_dc_L[16];
        public fixed byte arith_dc_U[16];
        public fixed byte arith_ac_K[16];

        public int num_scans;
        public jpeg_scan_info* scan_info;

        public int raw_data_in;          /* TRUE=caller supplies downsampled data */
        public int arith_code;           /* TRUE=arithmetic coding, FALSE=Huffman */
        public int optimize_coding;      /* TRUE=optimize entropy encoding parms */
        public int CCIR601_sampling;     /* TRUE=first samples are cosited */
        public int smoothing_factor;
        public J_DCT_METHOD dct_method;

        public uint restart_interval;
        public int restart_in_rows;

        public int write_JFIF_header;    /* should a JFIF marker be written? */
        public byte JFIF_major_version;     /* What to write for the JFIF version number */
        public byte JFIF_minor_version;

        public byte density_unit;           /* JFIF code for pixel size units */
        public ushort X_density;             /* Horizontal pixel density */
        public ushort Y_density;             /* Vertical pixel density */
        public int write_Adobe_marker;

        public uint next_scanline;
        public int data_unit;                /* size of data unit in samples */
        public J_CODEC_PROCESS process;      /* encoding process of JPEG image */

        public int max_h_samp_factor;        /* largest h_samp_factor */
        public int max_v_samp_factor;

        public uint total_iMCU_rows;

        public int comps_in_scan;
        public jpeg_component_info* cur_comp_info_1;
        public jpeg_component_info* cur_comp_info_2;
        public jpeg_component_info* cur_comp_info_3;
        public jpeg_component_info* cur_comp_info_4;

        public uint MCUs_per_row;      /* # of MCUs across the image */
        public uint MCU_rows_in_scan;

        public int data_units_in_MCU;
        public fixed int MCU_membership[10];

        public int Ss;
        public int Se;
        public int Ah;
        public int Al;

        public jpeg_comp_master* master;
        public jpeg_c_main_controller* main;
        public jpeg_c_prep_controller* prep;
        public jpeg_c_codec* codec;
        public jpeg_marker_writer* marker;
        public jpeg_color_converter* cconvert;
        public jpeg_downsampler* downsample;

        public jpeg_scan_info* script_space;
        public int script_space_size;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct j_decompress_ptr
    {
        public jpeg_error_mgr* err;
        public jpeg_progress_mgr* progress;
        public jpeg_memory_mgr* mem;
        public void* client_data;
        public int is_decompressor;
        public int global_state;

        public jpeg_source_mgr* src;

        public uint image_width;
        public uint image_height;
        public int num_components;
        public J_COLOR_SPACE jpeg_color_space;
        public J_COLOR_SPACE out_color_space;

        public uint scale_num;
        public uint scale_denom;
        public double output_gamma;

        public int buffered_image;
        public int raw_data_out;

        public J_DCT_METHOD dct_method;
        public int do_fancy_upsampling;
        public int do_block_smoothing;

        public int quantize_colors;
        public J_DITHER_MODE dither_mode;
        public int two_pass_quantize;

        public int desired_number_of_colors;

        public int enable_1pass_quant;
        public int enable_external_quant;
        public int enable_2pass_quant;

        public uint output_width;
        public uint output_height;
        public int out_color_components;
        public int output_components;

        public int rec_outbuf_height;

        public int actual_number_of_colors;

        public short** colormap;
        public uint output_scanline;

        public int input_scan_number;
        public uint input_iMCU_row;

        public int output_scan_number;
        public uint output_iMCU_row;

        //int (* coef_bits)[64];
        public IntPtr coef_bits;

        public JQUANT_TBL* quant_tbl_ptrs_1;
        public JQUANT_TBL* quant_tbl_ptrs_2;
        public JQUANT_TBL* quant_tbl_ptrs_3;
        public JQUANT_TBL* quant_tbl_ptrs_4;

        public JHUFF_TBL* dc_huff_tbl_ptrs_1;
        public JHUFF_TBL* dc_huff_tbl_ptrs_2;
        public JHUFF_TBL* dc_huff_tbl_ptrs_3;
        public JHUFF_TBL* dc_huff_tbl_ptrs_4;

        public JHUFF_TBL* ac_huff_tbl_ptrs_1;
        public JHUFF_TBL* ac_huff_tbl_ptrs_2;
        public JHUFF_TBL* ac_huff_tbl_ptrs_3;
        public JHUFF_TBL* ac_huff_tbl_ptrs_4;

        public int data_precision;
        public jpeg_component_info* comp_info;

        public int arith_code;

        public fixed byte arith_dc_L[16];
        public fixed byte arith_dc_U[16];
        public fixed byte arith_dc_K[16];

        public uint restart_interval;
        public int saw_JFIF_marker;
        public byte JFIF_major_version;
        public byte JFIF_minor_version;
        public byte density_unit;

        public ushort X_density;
        public ushort Y_density;
        public int saw_Adobe_marker;
        public byte Adobe_transform;

        public int CCIR601_sampling;
        public jpeg_marker_struct* marker_list;

        public int data_unit;
        public J_CODEC_PROCESS process;

        public int max_h_samp_factor;
        public int max_v_samp_factor;
        public int min_codec_data_unit;

        public uint total_iMCU_rows;

        public byte* sample_range_limit;
        public int comps_in_scan;

        public jpeg_component_info* cur_comp_info_1;
        public jpeg_component_info* cur_comp_info_2;
        public jpeg_component_info* cur_comp_info_3;
        public jpeg_component_info* cur_comp_info_4;

        public uint MCUs_per_row;
        public uint MCU_rows_in_scan;

        public int data_units_in_MCU;
        public fixed int MCU_membership[10];

        public int Ss;
        public int Se;
        public int Ah;
        public int Al;

        public int unread_marker;

        public IntPtr master;
        public IntPtr main;
        public IntPtr codec;
        public IntPtr post;
        public IntPtr inputctl;
        public IntPtr marker;
        public IntPtr upsample;
        public IntPtr cconvert;
        public IntPtr cquantize;

        public uint workaround_options;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_memory_mgr
    {
        public IntPtr alloc_small;
        public IntPtr alloc_large;
        public IntPtr alloc_sarray;
        public IntPtr alloc_barray;

        public IntPtr alloc_darray;

        public IntPtr request_virt_sarray;
        public IntPtr request_virt_barray;
        public IntPtr realize_virt_arrays;
        public IntPtr access_virt_sarray;
        public IntPtr access_virt_barray;
        public IntPtr free_pool;
        public IntPtr self_destruct;

        public int max_memory_to_use;
        public int max_alloc_chunk;

    }

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate void ouput_Message(ref j_common_ptr cinfo);

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate void errorexit(ref j_common_ptr cinfo);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_error_mgr
    {
        public IntPtr error_exit;
        public IntPtr emit_message;
        public IntPtr output_message;
        public IntPtr format_message;
        public IntPtr reset_error_mgr;

        public int msg_code;

        public msg_parm msg_Parm;

        public int trace_level;
        public int num_warnings;
        public char* jpeg_message_table;
        public int last_jpeg_message;
        public char* addon_message_table;
        public int first_addon_message;
        public int last_addon_message;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public unsafe struct msg_parm
    {
        [FieldOffset(0)]
        public fixed int i[8];
        [FieldOffset(0)]
        public fixed sbyte s[80];
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct JBLOCKARRAY
    {
        public fixed short JBLOCK[64];
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_progress_mgr
    {
        public IntPtr progress_monitor;

        public int pass_counter;            /* work units completed in this pass */
        public int pass_limit;              /* total number of work units in this pass */
        public int completed_passes;         /* passes completed so far */
        public int total_passes;             /* total number of passes expected */
    }

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate void init_destination(ref j_compress_ptr cinfo);
    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate int empty_output_buffer(ref j_compress_ptr cinfo);
    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate void term_destination(ref j_compress_ptr cinfo);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_destination_mgr
    {
        public IntPtr next_output_byte;
        public uint free_in_buffer;

        public IntPtr init_Destination;
        public IntPtr empty_Output_Buffer;
        public IntPtr term_Destination;
    }

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate void Init_source(ref j_decompress_ptr dinfo);
    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate int Fill_input_buffer(ref j_decompress_ptr dinfo);
    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate void Skip_input_data(ref j_decompress_ptr dinfo, int num_bytes);
    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public unsafe delegate int Resync_to_restart(ref j_decompress_ptr dinfo, int desired);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_source_mgr
    {
        public byte* next_input_byte;
        public uint bytes_in_buffer;

        public IntPtr init_source;
        public IntPtr fill_input_buffer;
        public IntPtr skip_input_data;
        public IntPtr resync_to_restart;
        public IntPtr term_source;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_component_info
    {
        public int component_id;             /* identifier for this component (0..255) */
        public int component_index;          /* its index in SOF or cinfo->comp_info[] */
        public int h_samp_factor;            /* horizontal sampling factor (1..4) */
        public int v_samp_factor;            /* vertical sampling factor (1..4) */
        public int quant_tbl_no;

        public int dc_tbl_no;                /* DC entropy table selector (0..3) */
        public int ac_tbl_no;
        public uint width_in_data_units;
        public uint height_in_data_units;

        public int codec_data_unit;
        public uint downsampled_width;
        public uint downsampled_height;

        public int component_needed;

        public int MCU_width;                /* number of data units per MCU, horizontally */
        public int MCU_height;               /* number of data units per MCU, vertically */
        public int MCU_data_units;           /* MCU_width * MCU_height */
        public int MCU_sample_width;         /* MCU width in samples, MCU_width*codec_data_unit */
        public int last_col_width;           /* # of non-dummy data_units across in last MCU */
        public int last_row_height;

        public void* dct_table;
        public JQUANT_TBL* quant_table;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_downsampler
    {
        public IntPtr start_pass;
        public IntPtr downsample;

        public int need_context_rows;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_color_converter
    {
        public IntPtr start_pass;
        public IntPtr color_convert;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_marker_struct
    {
        public jpeg_marker_struct* next;
        public byte marker;
        public uint original_length;
        public int data_length;
        public byte* data;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_marker_writer
    {
        public IntPtr write_file_header;
        public IntPtr write_frame_header;
        public IntPtr write_scan_header;
        public IntPtr write_file_trailer;
        public IntPtr write_tables_only;

        public IntPtr write_marker_header;
        public IntPtr write_marker_byte;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_c_codec
    {
        public IntPtr entropy_start_pass;
        public IntPtr entropy_finish_pass;
        public IntPtr need_optimization_pass;
        public IntPtr start_pass;
        public IntPtr compress_data;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_c_prep_controller
    {
        public IntPtr start_pass;
        public IntPtr pre_process_data;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_c_main_controller
    {
        public IntPtr start_pass;
        public IntPtr process_data;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_comp_master
    {
        public IntPtr prepare_for_pass;
        public IntPtr pass_startup;
        public IntPtr finish_pass;

        public int call_pass_startup;  /* True if pass_startup must be called */
        public int is_last_pass;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jpeg_scan_info
    {
        public int comps_in_scan;
        public fixed int component_index[4];
        public int Ss;
        public int Se;
        public int Ah;
        public int Al;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct JHUFF_TBL
    {
        public fixed byte bits[17];
        public fixed byte huffval[256];
        public int sent_table;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct JQUANT_TBL
    {
        public fixed ushort quantval[64];
        public int sent_table;

    }

    [Flags]
    public enum J_BUF_MODE
    {
        JBUF_PASS_THRU,     /* Plain stripwise operation */
        JBUF_SAVE_SOURCE,   /* Run source subobject only, save output */
        JBUF_CRANK_DEST,    /* Run dest subobject only, using saved data */
        JBUF_SAVE_AND_PASS
    }

    [Flags]
    public enum J_CODEC_PROCESS
    {
        JPROC_SEQUENTIAL,       /* baseline/extended sequential DCT */
        JPROC_PROGRESSIVE,      /* progressive DCT */
        JPROC_LOSSLESS
    }

    [Flags]
    public enum J_DCT_METHOD
    {
        JDCT_ISLOW,             /* slow but accurate integer algorithm */
        JDCT_IFAST,             /* faster, less accurate integer method */
        JDCT_FLOAT
    }

    [Flags]
    public enum J_COLOR_SPACE
    {
        JCS_UNKNOWN,            /* error/unspecified */
        JCS_GRAYSCALE,          /* monochrome */
        JCS_RGB,                /* red/green/blue */
        JCS_YCbCr,              /* Y/Cb/Cr (also known as YUV) */
        JCS_CMYK,               /* C/M/Y/K */
        JCS_YCCK
    }

    [Flags]
    public enum J_DITHER_MODE
    {
        JDITHER_NONE,           /* no dithering */
        JDITHER_ORDERED,        /* simple ordered dither */
        JDITHER_FS
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jvirt_sarray_control
    {
        public byte** mem_buffer;
        public uint rows_in_array;
        public uint samplesperrow;
        public uint maxaccess;
        public uint rows_in_mem;
        public uint rowsperchunk;
        public uint cur_start_row;
        public uint first_undef_row;

        public int pre_zero;
        public int dirty;
        public int b_s_open;

        public jvirt_sarray_control* next;
        public backing_store_struct b_s_info;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct backing_store_struct
    {
        public IntPtr read_backing_store;
        public IntPtr write_backing_store;
        public IntPtr close_backing_store;

        public fixed sbyte temp_name[64];
        public FILE* temp_file;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct FILE
    {
        public IntPtr _placeholder;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct jvirt_barray_control
    {
        public byte*** mem_buffer;
        public uint rows_in_array;
        public uint samplesperrow;
        public uint maxaccess;
        public uint rows_in_mem;
        public uint rowsperchunk;
        public uint cur_start_row;
        public uint first_undef_row;

        public int pre_zero;
        public int dirty;
        public int b_s_open;

        public jvirt_sarray_control* next;
        public backing_store_struct b_s_info;
    }

    public enum DicomJpegSampleFactor
    {
        SF444,

        SF422,

        Unknown
    }

    public class DicomJpegParams : DicomCodecParams
    {
        private int _quality;

        private int _smoothing;

        private bool _convertColorspace;

        private DicomJpegSampleFactor _sample;

        private int _predictor;

        private int _pointTransform;

        public DicomJpegParams()
        {
            _quality = 90;
            _smoothing = 0;
            _convertColorspace = false;
            _sample = DicomJpegSampleFactor.SF444;
            _predictor = 1;
            _pointTransform = 0;
        }

        public int Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                _quality = value;
            }
        }

        public int SmoothingFactor
        {
            get
            {
                return _smoothing;
            }
            set
            {
                _smoothing = value;
            }
        }

        public bool ConvertColorspaceToRGB
        {
            get
            {
                return _convertColorspace;
            }
            set
            {
                _convertColorspace = value;
            }
        }

        public DicomJpegSampleFactor SampleFactor
        {
            get
            {
                return _sample;
            }
            set
            {
                _sample = value;
            }
        }

        public int Predictor
        {
            get
            {
                return _predictor;
            }
            set
            {
                _predictor = value;
            }
        }

        public int PointTransform
        {
            get
            {
                return _pointTransform;
            }
            set
            {
                _pointTransform = value;
            }
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

            internal abstract int ScanHeaderForPrecision(DicomPixelData pixelData);
            internal MemoryStream MemoryBuffer;
            internal PinnedByteArray DataArray;
            internal JpegMode Mode;
            internal int Predictor;
            internal int PointTransform;
            internal int Bits;
        };

        public class JpegCodec : JpegNativeCodec
        {
            //DLLIMPORT libijg8 library for Windows

            //Encode Native functions

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_std_error_8")]

            public static extern unsafe jpeg_error_mgr* jpeg_std_error_8_Windows64(ref jpeg_error_mgr err);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_compress_8")]

            public static extern unsafe void jpeg_create_compress_8_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_defaults_8")]

            public static extern unsafe void jpeg_set_defaults_8_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_quality_8")]

            public static extern unsafe void jpeg_set_quality_8_Windows64(ref j_compress_ptr cinfo, int quality, int force_baseline);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_progression_8")]

            public static extern unsafe void jpeg_simple_progression_8_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_lossless_8")]

            public static extern unsafe void jpeg_simple_lossless_8_Windows64(ref j_compress_ptr cinfo, int predictor, int point_transform);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_colorspace_8")]

            public static extern unsafe void jpeg_set_colorspace_8_Windows64(ref j_compress_ptr cinfo, J_COLOR_SPACE in_color_space);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_compress_8")]

            public static extern unsafe void jpeg_start_compress_8_Windows64(ref j_compress_ptr cinfo, int write_all_tables);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_write_scanlines_8")]

            public static extern unsafe void jpeg_write_scanlines_8_Windows64(ref j_compress_ptr cinfo, byte** scanlines, uint num_lines);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_finish_compress_8")]

            public static extern unsafe void jpeg_finish_compress_8_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_compress_8")]

            public static extern unsafe void jpeg_destroy_compress_8_Windows64(ref j_compress_ptr cinfo);

            //Decode Native functions

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_decompress_8")]

            public static extern unsafe void jpeg_create_decompress_8_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_header_8")]

            public static extern unsafe int jpeg_read_header_8_Windows64(ref j_decompress_ptr dinfo, int require_image);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_calc_output_dimensions_8")]

            public static extern unsafe void jpeg_calc_output_dimensions_8_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_decompress_8")]

            public static extern unsafe int jpeg_start_decompress_8_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_scanlines_8")]

            public static extern unsafe uint jpeg_read_scanlines_8_Windows64(ref j_decompress_ptr dinfo, byte** scanlines,
                    uint max_lines);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_decompress_8")]

            public static extern unsafe void jpeg_destroy_decompress_8_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_resync_to_restart_8")]

            public static extern unsafe int jpeg_resync_to_restart_8_Windows64(ref j_decompress_ptr dinfo, int desired);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "format_message_8")]

            public static extern unsafe void format_message_8_Windows64(ref j_common_ptr cinfo, char[] buffer);


            //DLLIMPORT libijg8 library for Linux

            //Encode Native functions

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_std_error_8")]

            public static extern unsafe jpeg_error_mgr* jpeg_std_error_8_Linux64(ref jpeg_error_mgr err);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_compress_8")]

            public static extern unsafe void jpeg_create_compress_8_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_defaults_8")]

            public static extern unsafe void jpeg_set_defaults_8_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_quality_8")]

            public static extern unsafe void jpeg_set_quality_8_Linux64(ref j_compress_ptr cinfo, int quality, int force_baseline);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_progression_8")]

            public static extern unsafe void jpeg_simple_progression_8_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_lossless_8")]

            public static extern unsafe void jpeg_simple_lossless_8_Linux64(ref j_compress_ptr cinfo, int predictor, int point_transform);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_colorspace_8")]

            public static extern unsafe void jpeg_set_colorspace_8_Linux64(ref j_compress_ptr cinfo, J_COLOR_SPACE in_color_space);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_compress_8")]

            public static extern unsafe void jpeg_start_compress_8_Linux64(ref j_compress_ptr cinfo, int write_all_tables);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_write_scanlines_8")]

            public static extern unsafe void jpeg_write_scanlines_8_Linux64(ref j_compress_ptr cinfo, byte** scanlines, uint num_lines);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_finish_compress_8")]

            public static extern unsafe void jpeg_finish_compress_8_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_compress_8")]

            public static extern unsafe void jpeg_destroy_compress_8_Linux64(ref j_compress_ptr cinfo);


            //Decode Native functions

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_decompress_8")]

            public static extern unsafe void jpeg_create_decompress_8_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_header_8")]

            public static extern unsafe int jpeg_read_header_8_Linux64(ref j_decompress_ptr dinfo, int require_image);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_calc_output_dimensions_8")]

            public static extern unsafe void jpeg_calc_output_dimensions_8_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_decompress_8")]

            public static extern unsafe int jpeg_start_decompress_8_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_scanlines_8")]

            public static extern unsafe uint jpeg_read_scanlines_8_Linux64(ref j_decompress_ptr dinfo, byte** scanlines,
                    uint max_lines);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_decompress_8")]

            public static extern unsafe void jpeg_destroy_decompress_8_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_resync_to_restart_8")]

            public static extern unsafe int jpeg_resync_to_restart_8_Linux64(ref j_decompress_ptr dinfo, int desired);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "format_message_8")]

            public static extern unsafe void format_message_8_Linux64(ref j_common_ptr cinfo, char[] buffer);


            //DLLIMPORT libijg8 library for MacOS

            //Encode Native functions

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_std_error_8")]

            public static extern unsafe jpeg_error_mgr* jpeg_std_error_8_MacOS(ref jpeg_error_mgr err);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_compress_8")]

            public static extern unsafe void jpeg_create_compress_8_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_defaults_8")]

            public static extern unsafe void jpeg_set_defaults_8_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_quality_8")]

            public static extern unsafe void jpeg_set_quality_8_MacOS(ref j_compress_ptr cinfo, int quality, int force_baseline);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_progression_8")]

            public static extern unsafe void jpeg_simple_progression_8_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_lossless_8")]

            public static extern unsafe void jpeg_simple_lossless_8_MacOS(ref j_compress_ptr cinfo, int predictor, int point_transform);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_colorspace_8")]

            public static extern unsafe void jpeg_set_colorspace_8_MacOS(ref j_compress_ptr cinfo, J_COLOR_SPACE in_color_space);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_compress_8")]

            public static extern unsafe void jpeg_start_compress_8_MacOS(ref j_compress_ptr cinfo, int write_all_tables);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_write_scanlines_8")]

            public static extern unsafe void jpeg_write_scanlines_8_MacOS(ref j_compress_ptr cinfo, byte** scanlines, uint num_lines);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_finish_compress_8")]

            public static extern unsafe void jpeg_finish_compress_8_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_compress_8")]

            public static extern unsafe void jpeg_destroy_compress_8_MacOS(ref j_compress_ptr cinfo);

            //Decode Native functions

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_decompress_8")]

            public static extern unsafe void jpeg_create_decompress_8_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_header_8")]

            public static extern unsafe int jpeg_read_header_8_MacOS(ref j_decompress_ptr dinfo, int require_image);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_calc_output_dimensions_8")]

            public static extern unsafe void jpeg_calc_output_dimensions_8_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_decompress_8")]

            public static extern unsafe int jpeg_start_decompress_8_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_scanlines_8")]

            public static extern unsafe uint jpeg_read_scanlines_8_MacOS(ref j_decompress_ptr dinfo, byte** scanlines,
                    uint max_lines);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_decompress_8")]

            public static extern unsafe void jpeg_destroy_decompress_8_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_resync_to_restart_8")]

            public static extern unsafe int jpeg_resync_to_restart_8_MacOS(ref j_decompress_ptr dinfo, int desired);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "format_message_8")]

            public static extern unsafe void format_message_8_MacOS(ref j_common_ptr cinfo, char[] buffer);

            
            //DLLIMPORT libijg12 library for Windows

            //Encode Native functions

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_std_error_12")]

            public static extern unsafe jpeg_error_mgr* jpeg_std_error_12_Windows64(ref jpeg_error_mgr err);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_create_compress_12")]

            public static extern unsafe void jpeg_create_compress_12_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_defaults_12")]

            public static extern unsafe void jpeg_set_defaults_12_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_quality_12")]

            public static extern unsafe void jpeg_set_quality_12_Windows64(ref j_compress_ptr cinfo, int quality, int force_baseline);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_simple_progression_12")]

            public static extern unsafe void jpeg_simple_progression_12_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_simple_lossless_12")]

            public static extern unsafe void jpeg_simple_lossless_12_Windows64(ref j_compress_ptr cinfo, int predictor, int point_transform);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_colorspace_12")]

            public static extern unsafe void jpeg_set_colorspace_12_Windows64(ref j_compress_ptr cinfo, J_COLOR_SPACE in_color_space);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_start_compress_12")]

            public static extern unsafe void jpeg_start_compress_12_Windows64(ref j_compress_ptr cinfo, int write_all_tables);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_write_scanlines_12")]

            public static extern unsafe uint jpeg_write_scanlines_12_Windows64(ref j_compress_ptr cinfo, byte** scanlines, uint num_lines);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_finish_compress_12")]

            public static extern unsafe void jpeg_finish_compress_12_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, EntryPoint = "jpeg_destroy_compress_12")]

            public static extern unsafe void jpeg_destroy_compress_12_Windows64(ref j_compress_ptr cinfo);

            //Decode Native functions

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_decompress_12")]

            public static extern unsafe void jpeg_create_decompress_12_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_header_12")]

            public static extern unsafe int jpeg_read_header_12_Windows64(ref j_decompress_ptr dinfo, int require_image);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_calc_output_dimensions_12")]

            public static extern unsafe void jpeg_calc_output_dimensions_12_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_decompress_12")]

            public static extern unsafe int jpeg_start_decompress_12_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_scanlines_12")]

            public static extern unsafe uint jpeg_read_scanlines_12_Windows64(ref j_decompress_ptr dinfo, byte** scanlines, uint max_lines);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_decompress_12")]

            public static extern unsafe void jpeg_destroy_decompress_12_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_resync_to_restart_12")]

            public static extern unsafe int jpeg_resync_to_restart_12_Windows64(ref j_decompress_ptr dinfo, int desired);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "format_message_12")]

            public static extern unsafe void format_message_12_Windows64(ref j_common_ptr cinfo, char[] buffer);


            //DLLIMPORT libijg12 library for Linux

            //Encode Native functions

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_std_error_12")]

            public static extern unsafe jpeg_error_mgr* jpeg_std_error_12_Linux64(ref jpeg_error_mgr err);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_create_compress_12")]

            public static extern unsafe void jpeg_create_compress_12_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_defaults_12")]

            public static extern unsafe void jpeg_set_defaults_12_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_quality_12")]

            public static extern unsafe void jpeg_set_quality_12_Linux64(ref j_compress_ptr cinfo, int quality, int force_baseline);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_simple_progression_12")]

            public static extern unsafe void jpeg_simple_progression_12_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_simple_lossless_12")]

            public static extern unsafe void jpeg_simple_lossless_12_Linux64(ref j_compress_ptr cinfo, int predictor, int point_transform);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_colorspace_12")]

            public static extern unsafe void jpeg_set_colorspace_12_Linux64(ref j_compress_ptr cinfo, J_COLOR_SPACE in_color_space);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_start_compress_12")]

            public static extern unsafe void jpeg_start_compress_12_Linux64(ref j_compress_ptr cinfo, int write_all_tables);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_write_scanlines_12")]

            public static extern unsafe uint jpeg_write_scanlines_12_Linux64(ref j_compress_ptr cinfo, byte** scanlines, uint num_lines);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_finish_compress_12")]

            public static extern unsafe void jpeg_finish_compress_12_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, EntryPoint = "jpeg_destroy_compress_12")]

            public static extern unsafe void jpeg_destroy_compress_12_Linux64(ref j_compress_ptr cinfo);

            
            //Decode Native functions

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_decompress_12")]

            public static extern unsafe void jpeg_create_decompress_12_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_header_12")]

            public static extern unsafe int jpeg_read_header_12_Linux64(ref j_decompress_ptr dinfo, int require_image);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_calc_output_dimensions_12")]

            public static extern unsafe void jpeg_calc_output_dimensions_12_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_decompress_12")]

            public static extern unsafe int jpeg_start_decompress_12_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_scanlines_12")]

            public static extern unsafe uint jpeg_read_scanlines_12_Linux64(ref j_decompress_ptr dinfo, byte** scanlines, uint max_lines);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_decompress_12")]

            public static extern unsafe void jpeg_destroy_decompress_12_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_resync_to_restart_12")]

            public static extern unsafe int jpeg_resync_to_restart_12_Linux64(ref j_decompress_ptr dinfo, int desired);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "format_message_12")]

            public static extern unsafe void format_message_12_Linux64(ref j_common_ptr cinfo, char[] buffer);

            
            //DLLIMPORT libijg12 library for MacOS

            //Encode Native functions

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_std_error_12")]

            public static extern unsafe jpeg_error_mgr* jpeg_std_error_12_MacOS(ref jpeg_error_mgr err);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_create_compress_12")]

            public static extern unsafe void jpeg_create_compress_12_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_defaults_12")]

            public static extern unsafe void jpeg_set_defaults_12_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_quality_12")]

            public static extern unsafe void jpeg_set_quality_12_MacOS(ref j_compress_ptr cinfo, int quality, int force_baseline);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_simple_progression_12")]

            public static extern unsafe void jpeg_simple_progression_12_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_simple_lossless_12")]

            public static extern unsafe void jpeg_simple_lossless_12_MacOS(ref j_compress_ptr cinfo, int predictor, int point_transform);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_colorspace_12")]

            public static extern unsafe void jpeg_set_colorspace_12_MacOS(ref j_compress_ptr cinfo, J_COLOR_SPACE in_color_space);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_start_compress_12")]

            public static extern unsafe void jpeg_start_compress_12_MacOS(ref j_compress_ptr cinfo, int write_all_tables);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_write_scanlines_12")]

            public static extern unsafe uint jpeg_write_scanlines_12_MacOS(ref j_compress_ptr cinfo, byte** scanlines, uint num_lines);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_finish_compress_12")]

            public static extern unsafe void jpeg_finish_compress_12_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_destroy_compress_12")]

            public static extern unsafe void jpeg_destroy_compress_12_MacOS(ref j_compress_ptr cinfo);

            
            //Decode Native functions

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_decompress_12")]

            public static extern unsafe void jpeg_create_decompress_12_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_header_12")]

            public static extern unsafe int jpeg_read_header_12_MacOS(ref j_decompress_ptr dinfo, int require_image);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_calc_output_dimensions_12")]

            public static extern unsafe void jpeg_calc_output_dimensions_12_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_decompress_12")]

            public static extern unsafe int jpeg_start_decompress_12_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_scanlines_12")]

            public static extern unsafe uint jpeg_read_scanlines_12_MacOS(ref j_decompress_ptr dinfo, byte** scanlines, uint max_lines);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_decompress_12")]

            public static extern unsafe void jpeg_destroy_decompress_12_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_resync_to_restart_12")]

            public static extern unsafe int jpeg_resync_to_restart_12_MacOS(ref j_decompress_ptr dinfo, int desired);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "format_message_12")]

            public static extern unsafe void format_message_12_MacOS(ref j_common_ptr cinfo, char[] buffer);

            //DLLIMPORT libijg16 library for Windows

            //Encode Native functions

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_std_error_16")]

            public static extern unsafe jpeg_error_mgr* jpeg_std_error_16_Windows64(ref jpeg_error_mgr err);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_compress_16")]

            public static extern unsafe void jpeg_create_compress_16_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_defaults_16")]

            public static extern unsafe void jpeg_set_defaults_16_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_quality_16")]

            public static extern unsafe void jpeg_set_quality_16_Windows64(ref j_compress_ptr cinfo, int quality, int force_baseline);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_progression_16")]

            public static extern unsafe void jpeg_simple_progression_16_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_lossless_16")]

            public static extern unsafe void jpeg_simple_lossless_16_Windows64(ref j_compress_ptr cinfo, int predictor, int point_transform);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_colorspace_16")]

            public static extern unsafe void jpeg_set_colorspace_16_Windows64(ref j_compress_ptr cinfo, J_COLOR_SPACE in_color_space);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_compress_16")]

            public static extern unsafe void jpeg_start_compress_16_Windows64(ref j_compress_ptr cinfo, int write_all_tables);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_write_scanlines_16")]

            public static extern unsafe void jpeg_write_scanlines_16_Windows64(ref j_compress_ptr cinfo, byte** scanlines, uint num_lines);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_finish_compress_16")]

            public static extern unsafe void jpeg_finish_compress_16_Windows64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_compress_16")]

            public static extern unsafe void jpeg_destroy_compress_16_Windows64(ref j_compress_ptr cinfo);

            //Decode native functions

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_decompress_16")]

            public static extern unsafe void jpeg_create_decompress_16_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_header_16")]

            public static extern unsafe int jpeg_read_header_16_Windows64(ref j_decompress_ptr dinfo, int require_image);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_calc_output_dimensions_16")]

            public static extern unsafe void jpeg_calc_output_dimensions_16_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_decompress_16")]

            public static extern unsafe int jpeg_start_decompress_16_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_scanlines_16")]

            public static extern unsafe uint jpeg_read_scanlines_16_Windows64(ref j_decompress_ptr dinfo, byte** scanlines,
                    uint max_lines);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_decompress_16")]

            public static extern unsafe void jpeg_destroy_decompress_16_Windows64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_resync_to_restart_16")]

            public static extern unsafe int jpeg_resync_to_restart_16_Windows64(ref j_decompress_ptr dinfo, int desired);

            [DllImport("Efferent.Native-win64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "format_message_16")]

            public static extern unsafe void format_message_16_Windows64(ref j_common_ptr cinfo, char[] buffer);

            //DLLIMPORT libijg16 library for Linux

            //Encode Native functions

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_std_error_16")]

            public static extern unsafe jpeg_error_mgr* jpeg_std_error_16_Linux64(ref jpeg_error_mgr err);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_compress_16")]

            public static extern unsafe void jpeg_create_compress_16_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_defaults_16")]

            public static extern unsafe void jpeg_set_defaults_16_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_quality_16")]

            public static extern unsafe void jpeg_set_quality_16_Linux64(ref j_compress_ptr cinfo, int quality, int force_baseline);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_progression_16")]

            public static extern unsafe void jpeg_simple_progression_16_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_simple_lossless_16")]

            public static extern unsafe void jpeg_simple_lossless_16_Linux64(ref j_compress_ptr cinfo, int predictor, int point_transform);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_set_colorspace_16")]

            public static extern unsafe void jpeg_set_colorspace_16_Linux64(ref j_compress_ptr cinfo, J_COLOR_SPACE in_color_space);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_compress_16")]

            public static extern unsafe void jpeg_start_compress_16_Linux64(ref j_compress_ptr cinfo, int write_all_tables);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_write_scanlines_16")]

            public static extern unsafe void jpeg_write_scanlines_16_Linux64(ref j_compress_ptr cinfo, byte** scanlines, uint num_lines);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_finish_compress_16")]

            public static extern unsafe void jpeg_finish_compress_16_Linux64(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_compress_16")]

            public static extern unsafe void jpeg_destroy_compress_16_Linux64(ref j_compress_ptr cinfo);

            //Decode native functions

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_decompress_16")]

            public static extern unsafe void jpeg_create_decompress_16_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_header_16")]

            public static extern unsafe int jpeg_read_header_16_Linux64(ref j_decompress_ptr dinfo, int require_image);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_calc_output_dimensions_16")]

            public static extern unsafe void jpeg_calc_output_dimensions_16_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_decompress_16")]

            public static extern unsafe int jpeg_start_decompress_16_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_scanlines_16")]

            public static extern unsafe uint jpeg_read_scanlines_16_Linux64(ref j_decompress_ptr dinfo, byte** scanlines,
                    uint max_lines);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_decompress_16")]

            public static extern unsafe void jpeg_destroy_decompress_16_Linux64(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_resync_to_restart_16")]

            public static extern unsafe int jpeg_resync_to_restart_16_Linux64(ref j_decompress_ptr dinfo, int desired);


            [DllImport("Efferent.Native-linux64.so", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "format_message_16")]

            public static extern unsafe void format_message_16_Linux64(ref j_common_ptr cinfo, char[] buffer);

            
            //DLLIMPORT libijg16 library for MacOS

            //Encode Native functions

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_std_error_16")]

            public static extern unsafe jpeg_error_mgr* jpeg_std_error_16_MacOS(ref jpeg_error_mgr err);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_create_compress_16")]

            public static extern unsafe void jpeg_create_compress_16_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_defaults_16")]

            public static extern unsafe void jpeg_set_defaults_16_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_quality_16")]

            public static extern unsafe void jpeg_set_quality_16_MacOS(ref j_compress_ptr cinfo, int quality, int force_baseline);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_simple_progression_16")]

            public static extern unsafe void jpeg_simple_progression_16_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_simple_lossless_16")]

            public static extern unsafe void jpeg_simple_lossless_16_MacOS(ref j_compress_ptr cinfo, int predictor, int point_transform);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_set_colorspace_16")]

            public static extern unsafe void jpeg_set_colorspace_16_MacOS(ref j_compress_ptr cinfo, J_COLOR_SPACE in_color_space);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_start_compress_16")]

            public static extern unsafe void jpeg_start_compress_16_MacOS(ref j_compress_ptr cinfo, int write_all_tables);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_write_scanlines_16")]

            public static extern unsafe uint jpeg_write_scanlines_16_MacOS(ref j_compress_ptr cinfo, byte** scanlines, uint num_lines);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_finish_compress_16")]

            public static extern unsafe void jpeg_finish_compress_16_MacOS(ref j_compress_ptr cinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, EntryPoint = "jpeg_destroy_compress_16")]

            public static extern unsafe void jpeg_destroy_compress_16_MacOS(ref j_compress_ptr cinfo);

            
            //Decode Native functions

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_create_decompress_16")]

            public static extern unsafe void jpeg_create_decompress_16_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_header_16")]

            public static extern unsafe int jpeg_read_header_16_MacOS(ref j_decompress_ptr dinfo, int require_image);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_calc_output_dimensions_16")]

            public static extern unsafe void jpeg_calc_output_dimensions_16_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_start_decompress_16")]

            public static extern unsafe int jpeg_start_decompress_16_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_read_scanlines_16")]

            public static extern unsafe uint jpeg_read_scanlines_16_MacOS(ref j_decompress_ptr dinfo, byte** scanlines, uint max_lines);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_destroy_decompress_16")]

            public static extern unsafe void jpeg_destroy_decompress_16_MacOS(ref j_decompress_ptr dinfo);

            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "jpeg_resync_to_restart_16")]

            public static extern unsafe int jpeg_resync_to_restart_16_MacOS(ref j_decompress_ptr dinfo, int desired);


            [DllImport("Efferent.Native-macOS.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "format_message_16")]

            public static extern unsafe void format_message_16_MacOS(ref j_common_ptr cinfo, char[] buffer);

            public JpegCodec(JpegMode mode, int predictor, int point_transform, int bits)
            {
                Mode = mode;
                Predictor = predictor;
                PointTransform = point_transform;
                Bits = bits;
            }

            public static unsafe void initDestination(ref j_compress_ptr cinfo)
            {
                JpegCodec thisPtr = This;
                thisPtr.MemoryBuffer = new MemoryStream();
                thisPtr.DataArray = new PinnedByteArray(16384);
                cinfo.dest->next_output_byte = thisPtr.DataArray.Pointer;
                cinfo.dest->free_in_buffer = 16384;
            }

            public static unsafe int emptyOutputBuffer(ref j_compress_ptr cinfo)
            {
                JpegCodec thisPtr = This;
                thisPtr.MemoryBuffer.Write(thisPtr.DataArray.Data, 0, 16384);
                cinfo.dest->next_output_byte = thisPtr.DataArray.Pointer;
                cinfo.dest->free_in_buffer = 16384;
                return 1;
            }

            public static unsafe void termDestination(ref j_compress_ptr cinfo)
            {
                JpegCodec thisPtr = This;
                int count = 16384 - (int)cinfo.dest->free_in_buffer;
                thisPtr.MemoryBuffer.Write(thisPtr.DataArray.Data, 0, count);
                thisPtr.DataArray = null;
            }

            public static unsafe void initSource(ref j_decompress_ptr dinfo)
            {//dinfo) {               
            }

            public static unsafe void ErrorExit(ref j_common_ptr cinfo)
            {
                jpeg_error_mgr * myerr = (jpeg_error_mgr*)cinfo.err;
                char[] buffer = new char[200];
                
                //format_message for Linux, Windows and MacOS for 64 bits
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    format_message_8_Windows64(ref cinfo, buffer);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    format_message_8_Linux64(ref cinfo, buffer);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    format_message_8_MacOS(ref cinfo, buffer);

                throw new DicomCodecException(new String(buffer));             
            }
            public static unsafe void OutputMessage(ref j_common_ptr cinfo)
            {
                jpeg_error_mgr* myerr = (jpeg_error_mgr*)cinfo.err;
                char[] buffer = new char[200];

                //format_message for Linux, Windows and MacOS for 64 bits
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    format_message_8_Windows64(ref cinfo, buffer);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    format_message_8_Linux64(ref cinfo, buffer);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    format_message_8_MacOS(ref cinfo, buffer);
                
                LogManager.GetLogger("Efferent.Imaging.Codec").Debug("IJG: {0}", new String(buffer));
            }

            public static unsafe int fillInputBuffer(ref j_decompress_ptr dinfo)
            {
                SourceManagerStruct* src = (SourceManagerStruct*)(dinfo.src);
                if (src->next_buffer != null)
                {
                    src->pub.next_input_byte = src->next_buffer;
                    src->pub.bytes_in_buffer = src->next_buffer_size;
                    src->next_buffer = null;
                    src->next_buffer_size = 0;

                    if (src->skip_bytes > 0)
                    {
                        if (src->pub.bytes_in_buffer < (uint)src->skip_bytes)
                        {
                            src->skip_bytes -= Convert.ToInt32(src->pub.bytes_in_buffer);
                            src->pub.next_input_byte += src->pub.bytes_in_buffer;
                            src->pub.bytes_in_buffer = 0;
                            // cause a suspension return
                            return 0;
                        }
                        else
                        {
                            src->pub.bytes_in_buffer -= (uint)src->skip_bytes;
                            src->pub.next_input_byte += src->skip_bytes;
                            src->skip_bytes = 0;
                        }
                    }
                    return 1;
                }
                return 0;
            }

            public static unsafe void skipInputData(ref j_decompress_ptr dinfo, int num_bytes)
            {
                SourceManagerStruct* src = (SourceManagerStruct*)(dinfo.src);
                if (src->pub.bytes_in_buffer < (uint)num_bytes)
                {
                    src->skip_bytes = num_bytes - Convert.ToInt32(src->pub.bytes_in_buffer);
                    src->pub.next_input_byte += src->pub.bytes_in_buffer;
                    src->pub.bytes_in_buffer = 0; // causes a suspension return
                }
                else
                {
                    src->pub.bytes_in_buffer -= (uint)num_bytes;
                    src->pub.next_input_byte += num_bytes;
                    src->skip_bytes = 0;
                }
            }

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

            public unsafe static void jpeg_simple_spectral_selection(ref j_compress_ptr cinfo)
            {
                int ncomps = cinfo.num_components;
                jpeg_scan_info* scanptr = null;
                int nscans = 0;

                if (ncomps == 3 && cinfo.jpeg_color_space == J_COLOR_SPACE.JCS_YCbCr) nscans = 7;
                else nscans = 1 + 2 * ncomps;   /* 1 DC scan; 2 AC scans per component */

                if (cinfo.script_space == null || cinfo.script_space_size < nscans)
                {
                    cinfo.script_space_size = nscans > 7 ? nscans : 7;
                }

                scanptr = cinfo.script_space;
                cinfo.scan_info = scanptr;
                cinfo.num_scans = nscans;

                if (ncomps == 3 && cinfo.jpeg_color_space == J_COLOR_SPACE.JCS_YCbCr)
                {
                    /* Custom script for YCbCr color images. */
                    // Interleaved DC scan for Y,Cb,Cr:
                    scanptr[0].component_index[0] = 0;
                    scanptr[0].component_index[1] = 1;
                    scanptr[0].component_index[2] = 2;
                    scanptr[0].comps_in_scan = 3;
                    scanptr[0].Ss = 0;
                    scanptr[0].Se = 0;
                    scanptr[0].Ah = 0;
                    scanptr[0].Al = 0;

                    // AC scans
                    // First two Y AC coefficients
                    scanptr[1].component_index[0] = 0;
                    scanptr[1].comps_in_scan = 1;
                    scanptr[1].Ss = 1;
                    scanptr[1].Se = 2;
                    scanptr[1].Ah = 0;
                    scanptr[1].Al = 0;

                    // Three more
                    scanptr[2].component_index[0] = 0;
                    scanptr[2].comps_in_scan = 1;
                    scanptr[2].Ss = 3;
                    scanptr[2].Se = 5;
                    scanptr[2].Ah = 0;
                    scanptr[2].Al = 0;

                    // All AC coefficients for Cb
                    scanptr[3].component_index[0] = 1;
                    scanptr[3].comps_in_scan = 1;
                    scanptr[3].Ss = 1;
                    scanptr[3].Se = 63;
                    scanptr[3].Ah = 0;
                    scanptr[3].Al = 0;

                    // All AC coefficients for Cr
                    scanptr[4].component_index[0] = 2;
                    scanptr[4].comps_in_scan = 1;
                    scanptr[4].Ss = 1;
                    scanptr[4].Se = 63;
                    scanptr[4].Ah = 0;
                    scanptr[4].Al = 0;

                    // More Y coefficients
                    scanptr[5].component_index[0] = 0;
                    scanptr[5].comps_in_scan = 1;
                    scanptr[5].Ss = 6;
                    scanptr[5].Se = 9;
                    scanptr[5].Ah = 0;
                    scanptr[5].Al = 0;

                    // Remaining Y coefficients
                    scanptr[6].component_index[0] = 0;
                    scanptr[6].comps_in_scan = 1;
                    scanptr[6].Ss = 10;
                    scanptr[6].Se = 63;
                    scanptr[6].Ah = 0;
                    scanptr[6].Al = 0;
                }
                else
                {
                    /* All-purpose script for other color spaces. */
                    int j = 0;

                    // Interleaved DC scan for all components
                    for (j = 0; j < ncomps; j++) scanptr[0].component_index[j] = j;
                    scanptr[0].comps_in_scan = ncomps;
                    scanptr[0].Ss = 0;
                    scanptr[0].Se = 0;
                    scanptr[0].Ah = 0;
                    scanptr[0].Al = 0;

                    // first AC scan for each component
                    for (j = 0; j < ncomps; j++)
                    {
                        scanptr[j + 1].component_index[0] = j;
                        scanptr[j + 1].comps_in_scan = 1;
                        scanptr[j + 1].Ss = 1;
                        scanptr[j + 1].Se = 5;
                        scanptr[j + 1].Ah = 0;
                        scanptr[j + 1].Al = 0;
                    }

                    // second AC scan for each component
                    for (j = 0; j < ncomps; j++)
                    {
                        scanptr[j + ncomps + 1].component_index[0] = j;
                        scanptr[j + ncomps + 1].comps_in_scan = 1;
                        scanptr[j + ncomps + 1].Ss = 6;
                        scanptr[j + ncomps + 1].Se = 63;
                        scanptr[j + ncomps + 1].Ah = 0;
                        scanptr[j + ncomps + 1].Al = 0;
                    }
                }
            }

            public override void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams jpegParams, int frame)
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    throw new InvalidOperationException("Unsupported OS Platform");
                }

                if ((oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrIct) || (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrRct))
                    throw new DicomCodecException("Photometric Interpretation '{0}' not supported by JPEG encoder!", oldPixelData.PhotometricInterpretation);

                PinnedByteArray frameArray = null;

                if (oldPixelData.BitsAllocated == 16 && oldPixelData.BitsStored <= 8)
                {
                    IByteBuffer frameBuffer = oldPixelData.GetFrame(frame);
                    frameArray = new PinnedByteArray(ByteConverter.UnpackLow16(frameBuffer).Data);
                }
                else
                    frameArray = new PinnedByteArray(oldPixelData.GetFrame(frame).Data);


                try
                {
                    if (oldPixelData.PlanarConfiguration == PlanarConfiguration.Planar && oldPixelData.SamplesPerPixel > 1)
                    {
                        if (oldPixelData.SamplesPerPixel != 3 || oldPixelData.BitsStored > 8)
                            throw new DicomCodecException("Planar reconfiguration only implemented for SamplesPerPixel=3 && BitsStores <= 8");

                        newPixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;
                        frameArray = new PinnedByteArray(PixelDataConverter.PlanarToInterleaved24(new MemoryByteBuffer(frameArray.Data)).Data);
                    }

                    unsafe
                    {
                        j_compress_ptr cinfo = new j_compress_ptr();
                        jpeg_error_mgr jerr = new jpeg_error_mgr();

                        //jpeg_std_error_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            cinfo.err = jpeg_std_error_8_Windows64(ref jerr);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            cinfo.err = jpeg_std_error_8_Linux64(ref jerr);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            cinfo.err = jpeg_std_error_8_MacOS(ref jerr);

                        //jpeg_std_error_12 for Linux, Windows and MacOS for 64 bits
                        else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            cinfo.err = jpeg_std_error_12_Windows64(ref jerr);
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            cinfo.err = jpeg_std_error_12_Linux64(ref jerr);
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            cinfo.err = jpeg_std_error_12_MacOS(ref jerr);

                        //jpeg_std_error_16 for Linux, Windows and MacOS for 64 bits
                        else if(Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            cinfo.err = jpeg_std_error_16_Windows64(ref jerr);
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            cinfo.err = jpeg_std_error_16_Linux64(ref jerr);
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            cinfo.err = jpeg_std_error_16_MacOS(ref jerr);

                        errorexit errorexit_ = ErrorExit;
                        jerr.error_exit = Marshal.GetFunctionPointerForDelegate(errorexit_);

                        ouput_Message ouput_Message_ = OutputMessage;
                        jerr.output_message = Marshal.GetFunctionPointerForDelegate(ouput_Message_);

                        //jpeg_create_compress_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_create_compress_8_Windows64(ref cinfo);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_create_compress_8_Linux64(ref cinfo);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_create_compress_8_MacOS(ref cinfo);

                        //jpeg_create_compress_12 for Linux, Windows and MacOS for 64 bits
                        else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_create_compress_12_Windows64(ref cinfo);
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_create_compress_12_Linux64(ref cinfo);
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_create_compress_12_MacOS(ref cinfo);

                        //jpeg_create_compress_16 for Linux, Windows and MacOS for 64 bits
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_create_compress_16_Windows64(ref cinfo);
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_create_compress_16_Linux64(ref cinfo);
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_create_compress_16_MacOS(ref cinfo);
                        
                        cinfo.client_data = null;

                        This = this;
                        // Specify destination manager
                        jpeg_destination_mgr dest;

                        init_destination init_Destination_ = initDestination;
                        dest.init_Destination = Marshal.GetFunctionPointerForDelegate((init_Destination_));

                        empty_output_buffer empty_Output_Buffer_ = emptyOutputBuffer;
                        dest.empty_Output_Buffer = Marshal.GetFunctionPointerForDelegate(empty_Output_Buffer_);

                        term_destination term_Destination_ = termDestination;
                        dest.term_Destination = Marshal.GetFunctionPointerForDelegate(term_Destination_);

                        cinfo.dest = &dest;

                        cinfo.image_width = oldPixelData.Width;
                        cinfo.image_height = oldPixelData.Height;
                        cinfo.input_components = oldPixelData.SamplesPerPixel;
                        cinfo.in_color_space = getJpegColorSpace(oldPixelData.PhotometricInterpretation);

                        //jpeg_set_defaults_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_set_defaults_8_Windows64(ref cinfo);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_set_defaults_8_Linux64(ref cinfo);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_set_defaults_8_MacOS(ref cinfo);

                        //jpeg_set_defaults_12 for Linux, Windows and MacOS for 64 bits
                        else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_set_defaults_12_Windows64(ref cinfo);
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_set_defaults_12_Linux64(ref cinfo);
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_set_defaults_12_MacOS(ref cinfo);

                        //jpeg_set_defaults_16 for Linux, Windows and MacOS for 64 bits
                        else if(Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_set_defaults_16_Windows64(ref cinfo);
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_set_defaults_16_Linux64(ref cinfo);
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_set_defaults_16_MacOS(ref cinfo);

                        cinfo.optimize_coding = 1;

                        if (Mode == JpegMode.Baseline || Mode == JpegMode.Sequential)
                        {
                            //jpeg_set_quality_8 for Linux, Windows and MacOS for 64 bits
                            if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_set_quality_8_Windows64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_set_quality_8_Linux64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_set_quality_8_MacOS(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));

                            //jpeg_set_quality_12 for Linux, Windows and MacOS for 64 bits
                            else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_set_quality_12_Windows64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_set_quality_12_Linux64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_set_quality_12_MacOS(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));

                            //jpeg_set_quality_16 for Linux, Windows and MacOS for 64 bits
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_set_quality_16_Windows64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_set_quality_16_Linux64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_set_quality_16_MacOS(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));

                        }
                        else if (Mode == JpegMode.SpectralSelection)
                        {
                            //jpeg_set_quality_8 for Linux, Windows and MacOS for 64 bits
                            if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_set_quality_8_Windows64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_set_quality_8_Linux64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_set_quality_8_MacOS(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));

                            //jpeg_set_quality_12 for Linux, Windows and MacOS for 64 bits
                            else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_set_quality_12_Windows64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_set_quality_12_Linux64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_set_quality_12_MacOS(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));

                            //jpeg_set_quality_16 for Linux, Windows and MacOS for 64 bits
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_set_quality_16_Windows64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_set_quality_16_Linux64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_set_quality_16_MacOS(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));

                            jpeg_simple_spectral_selection(ref cinfo);

                        }
                        else if (Mode == JpegMode.Progressive)
                        {
                            //jpeg_set_quality_8 for Linux, Windows and MacOS for 64 bits
                            if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                jpeg_set_quality_8_Windows64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                                jpeg_simple_progression_8_Windows64(ref cinfo);
                            }
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                jpeg_set_quality_8_Linux64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                                jpeg_simple_progression_8_Linux64(ref cinfo);
                            }
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            {
                                jpeg_set_quality_8_MacOS(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                                jpeg_simple_progression_8_MacOS(ref cinfo);
                            }

                            //jpeg_set_quality_12 for Linux, Windows and MacOS for 64 bits
                            else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                jpeg_set_quality_12_Windows64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                                jpeg_simple_progression_12_Windows64(ref cinfo);
                            }
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                jpeg_set_quality_12_Linux64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                                jpeg_simple_progression_12_Linux64(ref cinfo);
                            }
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            {
                                jpeg_set_quality_12_MacOS(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                                jpeg_simple_progression_12_MacOS(ref cinfo);
                            }

                            //jpeg_set_quality_16 for Linux, Windows and MacOS for 64 bits
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                jpeg_set_quality_16_Windows64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                                jpeg_simple_progression_16_Windows64(ref cinfo);
                            }
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                jpeg_set_quality_16_Linux64(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                                jpeg_simple_progression_16_Linux64(ref cinfo);
                            }
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            {
                                jpeg_set_quality_16_MacOS(ref cinfo, jpegParams.Quality, Convert.ToInt32(false));
                                jpeg_simple_progression_16_MacOS(ref cinfo);
                            }

                        }
                        else
                        {
                            //jpeg_simple_lossless_8 for Linux, Windows and MacOS for 64 bits
                            if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_simple_lossless_8_Windows64(ref cinfo, Predictor, PointTransform);
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_simple_lossless_8_Linux64(ref cinfo, Predictor, PointTransform);
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_simple_lossless_8_MacOS(ref cinfo, Predictor, PointTransform);

                            //jpeg_simple_lossless_12 for Linux, Windows and MacOS for 64 bits
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_simple_lossless_12_Windows64(ref cinfo, Predictor, PointTransform);
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_simple_lossless_12_Linux64(ref cinfo, Predictor, PointTransform);
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_simple_lossless_12_MacOS(ref cinfo, Predictor, PointTransform);

                            //jpeg_simple_lossless_16 for Linux, Windows and MacOS for 64 bits
                            else if(Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_simple_lossless_16_Windows64(ref cinfo, Predictor, PointTransform);
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_simple_lossless_16_Linux64(ref cinfo, Predictor, PointTransform);
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_simple_lossless_16_MacOS(ref cinfo, Predictor, PointTransform);

                        }

                        cinfo.smoothing_factor = jpegParams.SmoothingFactor;

                        if (Mode == JpegMode.Lossless)
                        {
                            //jpeg_set_colorspace_8 for Linux, Windows and MacOS for 64 bits
                            if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_set_colorspace_8_Windows64(ref cinfo, cinfo.in_color_space);
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_set_colorspace_8_Linux64(ref cinfo, cinfo.in_color_space);
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_set_colorspace_8_MacOS(ref cinfo, cinfo.in_color_space);

                            //jpeg_set_colorspace_12 for Linux, Windows and MacOS for 64 bits
                            else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_set_colorspace_12_Windows64(ref cinfo, cinfo.in_color_space);
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_set_colorspace_12_Linux64(ref cinfo, cinfo.in_color_space);
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_set_colorspace_12_MacOS(ref cinfo, cinfo.in_color_space);

                            //jpeg_set_colorspace_16 for Linux, Windows and MacOS for 64 bits
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_set_colorspace_16_Windows64(ref cinfo, cinfo.in_color_space);
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_set_colorspace_16_Linux64(ref cinfo, cinfo.in_color_space);
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_set_colorspace_16_MacOS(ref cinfo, cinfo.in_color_space);

                            cinfo.comp_info->h_samp_factor = 1;
                            cinfo.comp_info->v_samp_factor = 1;
                        }
                        else
                        {
                            if (cinfo.jpeg_color_space == J_COLOR_SPACE.JCS_YCbCr && jpegParams.SampleFactor != DicomJpegSampleFactor.Unknown)
                            {
                                switch (jpegParams.SampleFactor)
                                {
                                    case DicomJpegSampleFactor.SF444:
                                        cinfo.comp_info->h_samp_factor = 1;
                                        cinfo.comp_info->v_samp_factor = 1;
                                        break;
                                    case DicomJpegSampleFactor.SF422:
                                        cinfo.comp_info->h_samp_factor = 2;
                                        cinfo.comp_info->v_samp_factor = 1;
                                        break;
                                }
                            }
                            else
                            {
                                if (jpegParams.SampleFactor == DicomJpegSampleFactor.Unknown)
                                {
                                    //jpeg_set_colorspace_8 for Linux, Windows and MacOS for 64 bits
                                    if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                        jpeg_set_colorspace_8_Windows64(ref cinfo, cinfo.in_color_space);
                                    else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                        jpeg_set_colorspace_8_Linux64(ref cinfo, cinfo.in_color_space);
                                    else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                        jpeg_set_colorspace_8_MacOS(ref cinfo, cinfo.in_color_space);

                                    //jpeg_set_colorspace_12 for Linux, Windows and MacOS for 64 bits
                                    else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                        jpeg_set_colorspace_12_Windows64(ref cinfo, cinfo.in_color_space);
                                    else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                        jpeg_set_colorspace_12_Linux64(ref cinfo, cinfo.in_color_space);
                                    else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                        jpeg_set_colorspace_12_MacOS(ref cinfo, cinfo.in_color_space);

                                    //jpeg_set_colorspace_16 for Linux, Windows and MacOS for 64 bits
                                    else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                        jpeg_set_colorspace_16_Windows64(ref cinfo, cinfo.in_color_space);
                                    else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                        jpeg_set_colorspace_16_Linux64(ref cinfo, cinfo.in_color_space);
                                    else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                        jpeg_set_colorspace_16_MacOS(ref cinfo, cinfo.in_color_space);

                                }

                                cinfo.comp_info[0].h_samp_factor = 1;
                                cinfo.comp_info[0].v_samp_factor = 1;

                            }
                        }

                        for (int sfi = 1; sfi < 10; sfi++)
                        {
                            cinfo.comp_info[sfi].h_samp_factor = 1;
                            cinfo.comp_info[sfi].v_samp_factor = 1;
                        }

                        //jpeg_start_compress_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_start_compress_8_Windows64(ref cinfo, Convert.ToInt32(true));
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_start_compress_8_Linux64(ref cinfo, Convert.ToInt32(true));
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_start_compress_8_MacOS(ref cinfo, Convert.ToInt32(true));

                        //jpeg_start_compress_12 for Linux, Windows and MacOS for 64 bits
                        else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_start_compress_12_Windows64(ref cinfo, Convert.ToInt32(true));
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_start_compress_12_Linux64(ref cinfo, Convert.ToInt32(true));
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_start_compress_12_MacOS(ref cinfo, Convert.ToInt32(true));

                        //jpeg_start_compress_16 for Linux, Windows and MacOS for 64 bits
                        else if(Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_start_compress_16_Windows64(ref cinfo, Convert.ToInt32(true));
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_start_compress_16_Linux64(ref cinfo, Convert.ToInt32(true));
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_start_compress_16_MacOS(ref cinfo, Convert.ToInt32(true));

                        byte* row_pointer;
                        int row_stride = oldPixelData.Width * oldPixelData.SamplesPerPixel * (oldPixelData.BitsStored <= 8 ? 1 : oldPixelData.BytesAllocated);

                        byte* framePtr = (byte*)(void*)frameArray.Pointer;

                        while (cinfo.next_scanline < cinfo.image_height)
                        {
                            row_pointer = &framePtr[cinfo.next_scanline * row_stride];

                            //jpeg_write_scanlines_8 for Linux, Windows and MacOS for 64 bits
                            if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_write_scanlines_8_Windows64(ref cinfo, &row_pointer, 1);
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_write_scanlines_8_Linux64(ref cinfo, &row_pointer, 1);
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_write_scanlines_8_MacOS(ref cinfo, &row_pointer, 1);

                            //jpeg_write_scanlines_12 for Linux, Windows and MacOS for 64 bits   
                            else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_write_scanlines_12_Windows64(ref cinfo, &row_pointer, 1);
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_write_scanlines_12_Linux64(ref cinfo, &row_pointer, 1);
                            else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_write_scanlines_12_MacOS(ref cinfo, &row_pointer, 1);

                            //jpeg_write_scanlines_16 for Linux, Windows and MacOS for 64 bits
                            else if(Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                jpeg_write_scanlines_16_Windows64(ref cinfo, &row_pointer, 1);
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                jpeg_write_scanlines_16_Linux64(ref cinfo, &row_pointer, 1);
                            else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                jpeg_write_scanlines_16_MacOS(ref cinfo, &row_pointer, 1);
                        
                        }

                        //jpeg_finish_compress_8 and jpeg_destroy_compress_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            jpeg_finish_compress_8_Windows64(ref cinfo);

                            jpeg_destroy_compress_8_Windows64(ref cinfo);
                        }
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            jpeg_finish_compress_8_Linux64(ref cinfo);

                            jpeg_destroy_compress_8_Linux64(ref cinfo);
                        }
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            jpeg_finish_compress_8_MacOS(ref cinfo);

                            jpeg_destroy_compress_8_MacOS(ref cinfo);
                        }

                        //jpeg_finish_compress_12 and jpeg_destroy_compress_12 for Linux, Windows and MacOS for 64 bits
                        else if(Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            jpeg_finish_compress_12_Windows64(ref cinfo);

                            jpeg_destroy_compress_12_Windows64(ref cinfo);
                        }
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            jpeg_finish_compress_12_Linux64(ref cinfo);

                            jpeg_destroy_compress_12_Linux64(ref cinfo);
                        }
                        else if (Bits <= 12 && Bits > 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            jpeg_finish_compress_12_MacOS(ref cinfo);

                            jpeg_destroy_compress_12_MacOS(ref cinfo);
                        }

                        //jpeg_finish_compress_16 and jpeg_destroy_compress_16 for Linux, Windows and MacOS for 64 bits
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            jpeg_finish_compress_16_Windows64(ref cinfo);

                            jpeg_destroy_compress_16_Windows64(ref cinfo);
                        }
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            jpeg_finish_compress_16_Linux64(ref cinfo);

                            jpeg_destroy_compress_16_Linux64(ref cinfo);
                        }
                        else if (Bits <= 16 && Bits > 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            jpeg_finish_compress_16_MacOS(ref cinfo);

                            jpeg_destroy_compress_16_MacOS(ref cinfo);
                        }

                        if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.Rgb && cinfo.jpeg_color_space == J_COLOR_SPACE.JCS_YCbCr)
                        {
                            if (jpegParams.SampleFactor == DicomJpegSampleFactor.SF422)
                            {
                                newPixelData.PhotometricInterpretation = PhotometricInterpretation.YbrFull422;
                            }
                            else
                            {
                                newPixelData.PhotometricInterpretation = PhotometricInterpretation.YbrFull;
                            }
                        }

                        IByteBuffer buffer;
                        if (MemoryBuffer.Length >= (1 * 1024 * 1024) || oldPixelData.NumberOfFrames > 1)
                        {
                            buffer = new TempFileBuffer(MemoryBuffer.ToArray());
                        }

                        else
                        {
                            buffer = new MemoryByteBuffer(MemoryBuffer.ToArray());
                        }

                        buffer = EvenLengthBuffer.Create(buffer);
                        newPixelData.AddFrame(buffer);
                    }
                }

                finally
                {
                    MemoryBuffer = null;
                    if (frameArray != null)
                    {
                        frameArray = null;
                    }
                }
            }

            public override void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams jpegParams, int frame)
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    throw new InvalidOperationException("Unsupported OS Platform");
                }

                unsafe
                {
                    PinnedByteArray jpegArray = new PinnedByteArray(oldPixelData.GetFrame(frame).Data);
                    PinnedByteArray frameArray = null;
                    try
                    {
                        j_decompress_ptr dinfo = new j_decompress_ptr();

                        SourceManagerStruct src = new SourceManagerStruct();

                        Init_source init_Source_ = initSource;
                        src.pub.init_source = Marshal.GetFunctionPointerForDelegate(init_Source_);

                        Fill_input_buffer fill_input_buffer_ = fillInputBuffer;
                        src.pub.fill_input_buffer = Marshal.GetFunctionPointerForDelegate(fill_input_buffer_);

                        Skip_input_data skip_input_data_ = skipInputData;
                        src.pub.skip_input_data = Marshal.GetFunctionPointerForDelegate(skip_input_data_);

                        //jpeg_resync_to_restart_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_8_Windows64;
                            src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                        }
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_8_Linux64;
                            src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                        }
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_8_MacOS;
                            src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                        }

                        //jpeg_resync_to_restart_12 for Linux, Windows and MacOS for 64 bits
                        else if(Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_12_Windows64;
                            src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                        }
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_12_Linux64;
                            src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                        }
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_12_MacOS;
                            src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                        }

                        //jpeg_resync_to_restart_16 for Linux, Windows and MacOS for 64 bits
                        else if(Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_16_Windows64;
                            src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                        }
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_16_Linux64;
                            src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                        }
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_16_MacOS;
                            src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                        }

                        src.pub.term_source = IntPtr.Zero;
                        src.pub.bytes_in_buffer = 0;
                        src.pub.next_input_byte = null;
                        src.skip_bytes = 0;
                        src.next_buffer = (byte*)(void*)jpegArray.Pointer;
                        src.next_buffer_size = (uint)jpegArray.ByteSize;

                        jpeg_error_mgr jerr = new jpeg_error_mgr();

                        //jpeg_std_error_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            dinfo.err = jpeg_std_error_8_Windows64(ref jerr);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            dinfo.err = jpeg_std_error_8_Linux64(ref jerr);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            dinfo.err = jpeg_std_error_8_MacOS(ref jerr);

                        //jpeg_std_error_12 for Linux, Windows and MacOS for 64 bits
                        else if(Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            dinfo.err = jpeg_std_error_12_Windows64(ref jerr);
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            dinfo.err = jpeg_std_error_12_Linux64(ref jerr);
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            dinfo.err = jpeg_std_error_12_MacOS(ref jerr);

                        //jpeg_std_error_16 for Linux, Windows and MacOS for 64 bits
                        else if(Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            dinfo.err = jpeg_std_error_16_Windows64(ref jerr);
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            dinfo.err = jpeg_std_error_16_Linux64(ref jerr);
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            dinfo.err = jpeg_std_error_16_MacOS(ref jerr);

                        errorexit errorexit_ = ErrorExit;
                        jerr.error_exit = Marshal.GetFunctionPointerForDelegate(errorexit_);

                        ouput_Message ouput_Message_ = OutputMessage;
                        jerr.output_message = Marshal.GetFunctionPointerForDelegate(ouput_Message_);

                        //jpeg_create_decompress_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_create_decompress_8_Windows64(ref dinfo);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_create_decompress_8_Linux64(ref dinfo);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_create_decompress_8_MacOS(ref dinfo);

                        //jpeg_create_decompress_12 for Linux, Windows and MacOS for 64 bits
                        else if(Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_create_decompress_12_Windows64(ref dinfo);
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_create_decompress_12_Linux64(ref dinfo);
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_create_decompress_12_MacOS(ref dinfo);

                        //jpeg_create_decompress_16 for Linux, Windows and MacOS for 64 bits
                        else if(Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_create_decompress_16_Windows64(ref dinfo);
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_create_decompress_16_Linux64(ref dinfo);
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_create_decompress_16_MacOS(ref dinfo);

                        dinfo.src = (jpeg_source_mgr*)&src.pub;

                        //jpeg_read_header_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            int jpeg_read_header_value = 0;
                            try
                            {
                                jpeg_read_header_value = jpeg_read_header_8_Windows64(ref dinfo, 1);
                            }
                            catch
                            {
                                throw new DicomCodecException("Unable to read header value : Suspended");
                            }

                            if (jpeg_read_header_value == 0)
                            {
                                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                            }
                        }
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            int jpeg_read_header_value = 0;
                            try
                            {
                                jpeg_read_header_value = jpeg_read_header_8_Linux64(ref dinfo, 1);
                            }
                            catch
                            {
                                throw new DicomCodecException("Unable to read header value : Suspended");
                            }

                            if (jpeg_read_header_value == 0)
                            {
                                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                            }
                        }
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            int jpeg_read_header_value = 0;
                            try
                            {
                                jpeg_read_header_value = jpeg_read_header_8_MacOS(ref dinfo, 1);
                            }
                            catch
                            {
                                throw new DicomCodecException("Unable to read header value : Suspended");
                            }

                            if (jpeg_read_header_value == 0)
                            {
                                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                            }
                        }

                        //jpeg_read_header_12 for Linux, Windows and MacOS for 64 bits
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            int jpeg_read_header_value = 0;
                            try
                            {
                                jpeg_read_header_value = jpeg_read_header_12_Windows64(ref dinfo, 1);
                            }
                            catch
                            {
                                throw new DicomCodecException("Unable to read header value : Suspended");
                            }

                            if (jpeg_read_header_value == 0)
                            {
                                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                            }
                        }
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            int jpeg_read_header_value = 0;
                            try
                            {
                                jpeg_read_header_value = jpeg_read_header_12_Linux64(ref dinfo, 1);
                            }
                            catch
                            {
                                throw new DicomCodecException("Unable to read header value : Suspended");
                            }

                            if (jpeg_read_header_value == 0)
                            {
                                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                            }
                        }
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            int jpeg_read_header_value = 0;
                            try
                            {
                                jpeg_read_header_value = jpeg_read_header_12_MacOS(ref dinfo, 1);
                            }
                            catch
                            {
                                throw new DicomCodecException("Unable to read header value : Suspended");
                            }

                            if (jpeg_read_header_value == 0)
                            {
                                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                            }
                        }

                        //jpeg_read_header_12 for Linux, Windows and MacOS for 64 bits
                        else if(Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            int jpeg_read_header_value = 0;
                            try
                            {
                                jpeg_read_header_value = jpeg_read_header_16_Windows64(ref dinfo, 1);
                            }
                            catch
                            {
                                throw new DicomCodecException("Unable to read header value : Suspended");
                            }

                            if (jpeg_read_header_value == 0)
                            {
                                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                            }
                        }
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            int jpeg_read_header_value = 0;
                            try
                            {
                                jpeg_read_header_value = jpeg_read_header_16_Linux64(ref dinfo, 1);
                            }
                            catch
                            {
                                throw new DicomCodecException("Unable to read header value : Suspended");
                            }

                            if (jpeg_read_header_value == 0)
                            {
                                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                            }
                        }
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            int jpeg_read_header_value = 0;
                            try
                            {
                                jpeg_read_header_value = jpeg_read_header_16_MacOS(ref dinfo, 1);
                            }
                            catch
                            {
                                throw new DicomCodecException("Unable to read header value : Suspended");
                            }

                            if (jpeg_read_header_value == 0)
                            {
                                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                            }
                        }

                        if(oldPixelData.PhotometricInterpretation != PhotometricInterpretation.Rgb)
                        {
                            jpegParams.ConvertColorspaceToRGB = true;
                        }
                        
                        newPixelData.PhotometricInterpretation = oldPixelData.PhotometricInterpretation;
                        if (jpegParams.ConvertColorspaceToRGB && (dinfo.out_color_space == J_COLOR_SPACE.JCS_YCbCr || dinfo.out_color_space == J_COLOR_SPACE.JCS_RGB))
                        {
                            if (oldPixelData.PixelRepresentation == PixelRepresentation.Signed)
                                throw new DicomCodecException("JPEG codec unable to perform colorspace conversion on signed pixel data");
                            dinfo.out_color_space = J_COLOR_SPACE.JCS_RGB;
                            newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
                            newPixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;
                        }

                        else
                        {
                            dinfo.jpeg_color_space = J_COLOR_SPACE.JCS_UNKNOWN;
                            dinfo.out_color_space = J_COLOR_SPACE.JCS_UNKNOWN;
                        }

                        //jpeg_calc_output_dimensions_8 and jpeg_start_decompress_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            jpeg_calc_output_dimensions_8_Windows64(ref dinfo);
                            jpeg_start_decompress_8_Windows64(ref dinfo);
                        }
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            jpeg_calc_output_dimensions_8_Linux64(ref dinfo);
                            jpeg_start_decompress_8_Linux64(ref dinfo);
                        }
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            jpeg_calc_output_dimensions_8_MacOS(ref dinfo);
                            jpeg_start_decompress_8_MacOS(ref dinfo);
                        }

                        //jpeg_calc_output_dimensions_12 and jpeg_start_decompress_12 for Linux, Windows and MacOS for 64 bits
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            jpeg_calc_output_dimensions_12_Windows64(ref dinfo);
                            jpeg_start_decompress_12_Windows64(ref dinfo);
                        }
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            jpeg_calc_output_dimensions_12_Linux64(ref dinfo);
                            jpeg_start_decompress_12_Linux64(ref dinfo);
                        }
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            jpeg_calc_output_dimensions_12_MacOS(ref dinfo);
                            jpeg_start_decompress_12_MacOS(ref dinfo);
                        }

                        //jpeg_calc_output_dimensions_16 and jpeg_start_decompress_16 for Linux, Windows and MacOS for 64 bits
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            jpeg_calc_output_dimensions_16_Windows64(ref dinfo);
                            jpeg_start_decompress_16_Windows64(ref dinfo);
                        }
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            jpeg_calc_output_dimensions_16_Linux64(ref dinfo);
                            jpeg_start_decompress_16_Linux64(ref dinfo);
                        }
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            jpeg_calc_output_dimensions_16_MacOS(ref dinfo);
                            jpeg_start_decompress_16_MacOS(ref dinfo);
                        }

                        int rowSize;
                        if (Bits == 8) 
                            rowSize = Convert.ToInt32(dinfo.output_width * dinfo.output_components * sizeof(short) / 2);
                        else 
                            rowSize = Convert.ToInt32(dinfo.output_width * dinfo.output_components * sizeof(short));

                        int frameSize = Convert.ToInt32(rowSize * dinfo.output_height);
                        if ((frameSize % 2) != 0)
                            frameSize++;

                        frameArray = new PinnedByteArray(frameSize);
                        byte* framePtr = (byte*)(void*)frameArray.Pointer;

                        while (dinfo.output_scanline < dinfo.output_height)
                        {
                            //jpeg_read_scanlines_8 for Linux, Windows and MacOS for 64 bits
                            if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                int rows = Convert.ToInt32(jpeg_read_scanlines_8_Windows64(ref dinfo, (byte**)&framePtr, 1));
                                
                                if (rows == 0)
                                {
                                    throw new DicomCodecException("JPEG 8 bit codec unable to perform reading scanlines on pixel data");
                                }
                                framePtr += rows * rowSize;
                            }
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                int rows = Convert.ToInt32(jpeg_read_scanlines_8_Linux64(ref dinfo, (byte**)&framePtr, 1));
                                
                                if (rows == 0)
                                {
                                    throw new DicomCodecException("JPEG 8 bit codec unable to perform reading scanlines on pixel data");
                                }
                                framePtr += rows * rowSize;
                            }
                            else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            {
                                int rows = Convert.ToInt32(jpeg_read_scanlines_8_MacOS(ref dinfo, (byte**)&framePtr, 1));
                                
                                if (rows == 0)
                                {
                                    throw new DicomCodecException("JPEG 8 bit codec unable to perform reading scanlines on pixel data");
                                }
                                framePtr += rows * rowSize;
                            }

                            //jpeg_read_scanlines_12 for Linux, Windows and MacOS for 64 bits
                            else if(Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                int rows = Convert.ToInt32(jpeg_read_scanlines_12_Windows64(ref dinfo, (byte**)&framePtr, 1));
                                
                                if (rows == 0)
                                {
                                    throw new DicomCodecException("JPEG 12 bit codec unable to perform reading scanlines pixel data");
                                }
                                framePtr += rows * rowSize;
                            }
                            else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                int rows = Convert.ToInt32(jpeg_read_scanlines_12_Linux64(ref dinfo, (byte**)&framePtr, 1));
                                
                                if (rows == 0)
                                {
                                    throw new DicomCodecException("JPEG 12 bit codec unable to perform reading scanlines pixel data");
                                }
                                framePtr += rows * rowSize;
                            }
                            else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            {
                                int rows = Convert.ToInt32(jpeg_read_scanlines_12_MacOS(ref dinfo, (byte**)&framePtr, 1));
                                
                                if (rows == 0)
                                {
                                    throw new DicomCodecException("JPEG 12 bit codec unable to perform reading scanlines on pixel data");
                                }
                                framePtr += rows * rowSize;
                            }

                            //jpeg_read_scanlines_12 for Linux, Windows and MacOS for 64 bits
                            else if(Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                int rows = Convert.ToInt32(jpeg_read_scanlines_16_Windows64(ref dinfo, (byte**)&framePtr, 1));
                                
                                if (rows == 0)
                                {
                                    throw new DicomCodecException("JPEG 16 bit codec unable to perform reading scanlines on pixel data");
                                }
                                framePtr += rows * rowSize;
                            }
                            else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                int rows = Convert.ToInt32(jpeg_read_scanlines_16_Linux64(ref dinfo, (byte**)&framePtr, 1));
                                
                                if (rows == 0)
                                {
                                    throw new DicomCodecException("JPEG 16 bit codec unable to perform reading scanlines on pixel data");
                                }
                                framePtr += rows * rowSize;;
                            }
                            else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            {
                                int rows = Convert.ToInt32(jpeg_read_scanlines_16_MacOS(ref dinfo, (byte**)&framePtr, 1));

                                if (rows == 0)
                                {
                                    throw new DicomCodecException("JPEG 16 bit codec unable to perform reading scanlines on pixel data");
                                }
                                framePtr += rows * rowSize;
                            }

                        }

                        //jpeg_destroy_decompress_8 for Linux, Windows and MacOS for 64 bits
                        if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_destroy_decompress_8_Windows64(ref dinfo);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_destroy_decompress_8_Linux64(ref dinfo);
                        else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_destroy_decompress_8_MacOS(ref dinfo);

                        //jpeg_destroy_decompress_12 for Linux, Windows and MacOS for 64 bits
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_destroy_decompress_12_Windows64(ref dinfo);
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_destroy_decompress_12_Linux64(ref dinfo);
                        else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_destroy_decompress_12_MacOS(ref dinfo);

                        //jpeg_destroy_decompress_16 for Linux, Windows and MacOS for 64 bits
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            jpeg_destroy_decompress_16_Windows64(ref dinfo);
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            jpeg_destroy_decompress_16_Linux64(ref dinfo);
                        else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            jpeg_destroy_decompress_16_MacOS(ref dinfo);

                        IByteBuffer buffer;
                        if (frameArray.Count >= (1 * 1024 * 1024) || oldPixelData.NumberOfFrames > 1)
                            buffer = new TempFileBuffer(frameArray.Data);
                        else
                            buffer = new MemoryByteBuffer(frameArray.Data);
                        buffer = EvenLengthBuffer.Create(buffer);

                        if (newPixelData.PlanarConfiguration == PlanarConfiguration.Planar && newPixelData.SamplesPerPixel > 1)
                        {
                            if (oldPixelData.SamplesPerPixel != 3 || oldPixelData.BitsStored > 8)
                                throw new DicomCodecException("Planar reconfiguration only implemented for SamplesPerPixel=3 && BitsStores <= 8");

                            buffer = PixelDataConverter.InterleavedToPlanar24(buffer);
                        }

                        newPixelData.AddFrame(buffer);
                    }

                    finally
                    {
                        if (frameArray != null)
                        {
                            frameArray = null;
                        }
                        if (jpegArray != null)
                        {
                            jpegArray = null;
                        }
                    }
                }
            }

            internal override unsafe int ScanHeaderForPrecision(DicomPixelData pixelData)
            {
                PinnedByteArray jpegArray = new PinnedByteArray(pixelData.GetFrame(0).Data);

                j_decompress_ptr dinfo = new j_decompress_ptr();

                SourceManagerStruct src;

                src.pub.init_source = IntPtr.Zero;

                Fill_input_buffer fillInputBuffer_ = fillInputBuffer;
                src.pub.fill_input_buffer = Marshal.GetFunctionPointerForDelegate(fillInputBuffer_);

                Skip_input_data skip_input_data_ = skipInputData;
                src.pub.skip_input_data = Marshal.GetFunctionPointerForDelegate(skip_input_data_);

                //jpeg_resync_to_restart_8 for Linux, Windows and MacOS for 64 bits
                if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_8_Windows64;
                    src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                }
                else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_8_Linux64;
                    src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                }
                else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_8_MacOS;
                    src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                }

                //jpeg_resync_to_restart_12 for Linux, Windows and MacOS for 64 bits
                else if(Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_12_Windows64;
                    src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                }
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_12_Linux64;
                    src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                }
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_12_MacOS;
                    src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                }

                //jpeg_resync_to_restart_16 for Linux, Windows and MacOS for 64 bits
                else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_16_Windows64;
                    src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                }
                else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_16_Linux64;
                    src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                }
                else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Resync_to_restart resync_to_restart_ = jpeg_resync_to_restart_16_MacOS;
                    src.pub.resync_to_restart = Marshal.GetFunctionPointerForDelegate(resync_to_restart_);
                }

                src.pub.term_source = IntPtr.Zero;
                src.pub.bytes_in_buffer = 0;
                src.pub.next_input_byte = null;
                src.skip_bytes = 0;
                src.next_buffer = (byte*)(void*)jpegArray.Pointer;
                src.next_buffer_size = (uint)jpegArray.ByteSize;

                jpeg_error_mgr jerr = new jpeg_error_mgr();

                //jpeg_std_error_8 for Linux, Windows and MacOS for 64 bits
                if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    dinfo.err = jpeg_std_error_8_Windows64(ref jerr);
                else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    dinfo.err = jpeg_std_error_8_Linux64(ref jerr);
                else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    dinfo.err = jpeg_std_error_8_MacOS(ref jerr);

                //jpeg_std_error_12 for Linux, Windows and MacOS for 64 bits
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    dinfo.err = jpeg_std_error_12_Windows64(ref jerr);
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    dinfo.err = jpeg_std_error_12_Linux64(ref jerr);
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    dinfo.err = jpeg_std_error_12_MacOS(ref jerr);

                //jpeg_std_error_16 for Linux, Windows and MacOS for 64 bits
                else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    dinfo.err = jpeg_std_error_16_Windows64(ref jerr);
                else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    dinfo.err = jpeg_std_error_16_Linux64(ref jerr);
                else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    dinfo.err = jpeg_std_error_16_MacOS(ref jerr);
                
                errorexit errorexit_ = ErrorExit;
                jerr.error_exit = Marshal.GetFunctionPointerForDelegate(errorexit_);

                ouput_Message ouput_Message_ = OutputMessage;
                jerr.output_message = Marshal.GetFunctionPointerForDelegate(ouput_Message_);

                //jpeg_create_decompress_8 for Linux, Windows and MacOS for 64 bits
                if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    jpeg_create_decompress_8_Windows64(ref dinfo);
                else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    jpeg_create_decompress_8_Linux64(ref dinfo);
                else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    jpeg_create_decompress_8_MacOS(ref dinfo);

                //jpeg_create_decompress_12 for Linux, Windows and MacOS for 64 bits
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    jpeg_create_decompress_12_Windows64(ref dinfo);
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    jpeg_create_decompress_12_Linux64(ref dinfo);
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    jpeg_create_decompress_12_MacOS(ref dinfo);
                
                //jpeg_create_decompress_16 for Linux, Windows and MacOS for 64 bits
                else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    jpeg_create_decompress_16_Windows64(ref dinfo);
                else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    jpeg_create_decompress_16_Linux64(ref dinfo);
                else if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    jpeg_create_decompress_16_MacOS(ref dinfo);                

                dinfo.src = (jpeg_source_mgr*)&src.pub;

                //jpeg_read_header_8 for Linux, Windows and MacOS for 64 bits
                if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (jpeg_read_header_8_Windows64(ref dinfo, 1) == 0)
                    {
                        throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                    }
                }
                else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    if (jpeg_read_header_8_Linux64(ref dinfo, 1) == 0)
                    {
                        throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                    }
                }
                else if (Bits == 8 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    if (jpeg_read_header_8_MacOS(ref dinfo, 1) == 0)
                    {
                        throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                    }
                }

                //jpeg_read_header_12 for Linux, Windows and MacOS for 64 bits
                if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (jpeg_read_header_12_Windows64(ref dinfo, 1) == 0)
                    {
                        throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                    }
                }
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    if (jpeg_read_header_12_Linux64(ref dinfo, 1) == 0)
                    {
                        throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                    }
                }
                else if (Bits > 8 && Bits <= 12 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    if (jpeg_read_header_12_MacOS(ref dinfo, 1) == 0)
                    {
                        throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                    }
                }

                //jpeg_read_header_12 for Linux, Windows and MacOS for 64 bits
                if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (jpeg_read_header_16_Windows64(ref dinfo, 1) == 0)
                    {
                        throw new DicomCodecException("Unable to read JPEG header: Suspended");
                    }
                }
                if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    if (jpeg_read_header_16_Linux64(ref dinfo, 1) == 0)
                    {
                        throw new DicomCodecException("Unable to read JPEG header: Suspended");
                    }
                }
                if (Bits > 12 && Bits <= 16 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    if (jpeg_read_header_16_MacOS(ref dinfo, 1) == 0)
                    {
                        throw new DicomCodecException("Unable to read JPEG header: Suspended");
                    }
                }

                return dinfo.data_precision;
            }

            [ThreadStatic] 
            internal static JpegCodec This;
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
            if (oldPixelData.NumberOfFrames == 0)
                return;

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

            try
            {
                try
                {
                    precision = JpegHelper.ScanJpegForBitDepth(oldPixelData);
                }
                catch
                {
                    // if the internal scanner chokes on an image, try again using ijg
                    JpegCodec c = new JpegCodec(JpegMode.Baseline, 0, 0, 8);
                    precision = c.ScanHeaderForPrecision(oldPixelData);
                }
            }

            catch
            {
                // the old scanner choked on several valid images...
                // assume the correct encoder was used and let libijg handle the rest
                precision = oldPixelData.BitsStored;
            }

            if (newPixelData.BitsStored <= 8 && precision > 8)
                newPixelData.Dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)16); // embedded overlay?

            JpegNativeCodec codec = GetCodec(precision, jparams);

            for (int frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
            {
                codec.Decode(oldPixelData, newPixelData, jparams, frame);
            }
        }

        protected virtual JpegNativeCodec GetCodec(int bits, DicomJpegParams jparams)
        {
            if (bits == 8)
                return new JpegCodec(JpegMode.Baseline, 0, 0, bits);
            else
                throw new DicomCodecException(String.Format("Unable to create JPEG Process 1 codec for bits stored == {0}", bits));
        }
    }

    [Export(typeof(IDicomCodec))]
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
                throw new DicomCodecException(String.Format("Unable to create JPEG Process 1 codec for bits stored == {0}", bits));
        }

    };

    [Export(typeof(IDicomCodec))]
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

            else if (bits <= 12)
                return new JpegCodec(JpegMode.Sequential, 0, 0, bits);

            else
                throw new DicomCodecException(String.Format("Unable to create JPEG Process 4 codec for bits stored == {0}", bits));
        }
    }

    [Export(typeof(IDicomCodec))]
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

            else if (bits <= 12)
                return new JpegCodec(JpegMode.Lossless, jparams.Predictor, jparams.PointTransform, bits);

            else if (bits <= 16)
                return new JpegCodec(JpegMode.Lossless, jparams.Predictor, jparams.PointTransform, bits);

            else
                throw new DicomCodecException(String.Format("Unable to create JPEG Process 14 codec for bits stored == {0}", bits));
        }
    }

    [Export(typeof(IDicomCodec))]
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

            else if (bits <= 12)
                return new JpegCodec(JpegMode.Lossless, 1, jparams.PointTransform, bits);

            else if (bits <= 16)
                return new JpegCodec(JpegMode.Lossless, 1, jparams.PointTransform, bits);

            else
                throw new DicomCodecException(String.Format("Unable to create JPEG Process 14 [SV1] codec for bits stored == {0}", bits));
        }
    }
}
