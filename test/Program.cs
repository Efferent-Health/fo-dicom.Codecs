using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;

namespace Efferent.Native.Test
{
    class Program
    {
        static void Main() 
        {
            // For debugging purposes only
            // var tcm = new Efferent.Native.Codec.NativeTranscoderManager();
            TranscoderManager.SetImplementation(new Efferent.Native.Codec.NativeTranscoderManager());
            var output = "JPEG2000.dcm";
            var data = DicomFile.Open("testDicom.dcm");
            var image = new DicomFile(data.Dataset).Clone(DicomTransferSyntax.JPEG2000Lossy);
            image.Save(output);
        }
    }
}