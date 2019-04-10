using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dicom;
using Dicom.Imaging.Codec;
using Dicom.Imaging;

namespace Efferent.Native.Test
{
    [TestClass]
    public class TranscodeUnitTest
    {
        [TestInitialize]
        public void Initialization()
        {
            TranscoderManager.SetImplementation(new Efferent.Native.Codec.NativeTranscoderManager());

            Directory.CreateDirectory("out");
        }

        [TestMethod]
        public void CanTranscodeJPEGLSLossless()
        {
            var output = "out/JPEG2000.dcm";
            var data = DicomFile.Open("test.dcm");
            var image = new DicomFile(data.Dataset).Clone(DicomTransferSyntax.JPEGLSLossless);

            Assert.IsNotNull(image);

            image.Save(output);

            var data1 = DicomFile.Open(output);
            Assert.IsTrue(data1.Dataset.Contains(DicomTag.PixelData));
        }
    }
}
