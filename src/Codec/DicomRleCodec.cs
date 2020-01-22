using System.ComponentModel.Composition;

using Dicom.Imaging.Codec;

namespace Dicom.Imaging.NativeCodec
{    
    [Export(typeof(IDicomCodec))]
    public class DicomRleNativeCodec : DicomRleCodecImpl
    {
    }
}