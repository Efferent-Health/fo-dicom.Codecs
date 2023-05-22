#include "openjpeg.h"
#include "cio.h"
#include "event.h"
/** Common fields between JPEG-2000 compression and decompression master structs. */

/*
==========================================================
   event manager typedef definitions
==========================================================
*/

/**
Callback function prototype for events
@param msg Event message
@param client_data
*/
typedef void (*opj_msg_callback) (const char* msg, void* client_data);

///**
//Message handler object
//used for
//<ul>
//<li>Error messages
//<li>Warning messages
//<li>Debugging messages
//</ul>
//*/
//typedef struct opj_event_mgr {
//	/** Error message callback if available, NULL otherwise */
//	opj_msg_callback error_handler;
//	/** Warning message callback if available, NULL otherwise */
//	opj_msg_callback warning_handler;
//	/** Debug message callback if available, NULL otherwise */
//	opj_msg_callback info_handler;
//} opj_event_mgr_t;


#define opj_common_fields \
	opj_event_mgr_t *event_mgr;	/**< pointer to the event manager */\
	void * client_data;			/**< Available for use by application */\
	OPJ_BOOL is_decompressor;	/**< So common code can tell which is which */\
	OPJ_CODEC_FORMAT codec_format;	/**< selected codec */\
	void *j2k_handle;			/**< pointer to the J2K codec */\
	void *jp2_handle;			/**< pointer to the JP2 codec */\
	void *mj2_handle			/**< pointer to the MJ2 codec */

/* Routines that are to be used by both halves of the library are declared
 * to receive a pointer to this structure.  There are no actual instances of
 * opj_common_struct_t, only of opj_cinfo_t and opj_dinfo_t.
 */
typedef struct opj_common_struct {
	opj_common_fields;		/* Fields common to both master struct types */
	/* Additional fields follow in an actual opj_cinfo_t or
	 * opj_dinfo_t.  All three structs must agree on these
	 * initial fields!  (This would be a lot cleaner in C++.)
	 */
} opj_common_struct_t;

typedef opj_common_struct_t* opj_common_ptr;

/**
Compression context info
*/
typedef struct opj_cinfo {
	/** Fields shared with opj_dinfo_t */
	opj_common_fields;
	/* other specific fields go here */
} opj_cinfo_t;

/**
Decompression context info
*/
typedef struct opj_dinfo {
	/** Fields shared with opj_cinfo_t */
	opj_common_fields;
	/* other specific fields go here */
} opj_dinfo_t;

opj_event_mgr_t* OPJ_CALLCONV opj_set_event_mgr(opj_common_ptr cinfo, opj_event_mgr_t* event_mgr, void* context) {
	if (cinfo) {
		opj_event_mgr_t* previous = cinfo->event_mgr;
		cinfo->event_mgr = event_mgr;
		cinfo->client_data = context;
		return previous;
	}

	return NULL;
}

///*
//==========================================================
//   I/O stream typedef definitions
//==========================================================
//*/
//
///*
// * Stream open flags.
// */
// /** The stream was opened for reading. */
//#define OPJ_STREAM_READ	0x0001
///** The stream was opened for writing. */
//#define OPJ_STREAM_WRITE 0x0002
//
///**
//Byte input-output stream (CIO)
//*/
//typedef struct opj_cio {
//	/** codec context */
//	opj_common_ptr cinfo;
//
//	/** open mode (read/write) either OPJ_STREAM_READ or OPJ_STREAM_WRITE */
//	int openmode;
//	/** pointer to the start of the buffer */
//	unsigned char* buffer;
//	/** buffer size in bytes */
//	int length;
//
//	/** pointer to the start of the stream */
//	unsigned char* start;
//	/** pointer to the end of the stream */
//	unsigned char* end;
//	/** pointer to the current position */
//	unsigned char* bp;
//} opj_cio_t;
//
//opj_cio_t* OPJ_CALLCONV opj_cio_open(opj_common_ptr cinfo, unsigned char* buffer, int length) {
//	opj_cp_t* cp = NULL;
//	opj_cio_t* cio = (opj_cio_t*)opj_malloc(sizeof(opj_cio_t));
//	if (!cio) return NULL;
//	cio->cinfo = cinfo;
//	if (buffer && length) {
//		/* wrap a user buffer containing the encoded image */
//		cio->openmode = OPJ_STREAM_READ;
//		cio->buffer = buffer;
//		cio->length = length;
//	}
//	else if (!buffer && !length && cinfo) {
//		/* allocate a buffer for the encoded image */
//		cio->openmode = OPJ_STREAM_WRITE;
//		switch (cinfo->codec_format) {
//		case OPJ_CODEC_J2K:
//			cp = &((opj_j2k_t*)cinfo->j2k_handle)->m_cp;
//			break;
//		case OPJ_CODEC_JP2:
//			cp = &((opj_jp2_t*)cinfo->jp2_handle)->j2k->m_cp;
//			break;
//		default:
//			opj_free(cio);
//			return NULL;
//		}
//		cio->length = (unsigned int)(0.1625 * cp->img_size + 2000); /* 0.1625 = 1.3/8 and 2000 bytes as a minimum for headers */
//		cio->buffer = (unsigned char*)opj_malloc(cio->length);
//		if (!cio->buffer) {
//			opj_event_msg(cio->cinfo, EVT_ERROR, "Error allocating memory for compressed bitstream\n");
//			opj_free(cio);
//			return NULL;
//		}
//	}
//	else {
//		opj_free(cio);
//		return NULL;
//	}
//
//	/* Initialize byte IO */
//	cio->start = cio->buffer;
//	cio->end = cio->buffer + cio->length;
//	cio->bp = cio->buffer;
//
//	return cio;
//}
//
//void OPJ_CALLCONV opj_cio_close(opj_cio_t* cio) {
//	if (cio) {
//		if (cio->openmode == OPJ_STREAM_WRITE) {
//			/* destroy the allocated buffer */
//			opj_free(cio->buffer);
//		}
//		/* destroy the cio */
//		opj_free(cio);
//	}
//}
