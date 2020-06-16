using System.ComponentModel.Composition;

using FellowOakDicom.Imaging.Codec;

namespace FellowOakDicom.Imaging.NativeCodec
{    
    [Export(typeof(IDicomCodec))]
    public class DicomRleNativeCodec : DicomRleCodecImpl
    {
    }
}