using System.ComponentModel.Composition;

using Dicom.Imaging.Codec;

namespace Efferent.Native.Codec
{    
    [Export(typeof(IDicomCodec))]
    public class DicomRleNativeCodec : DicomRleCodecImpl
    {
    }
}