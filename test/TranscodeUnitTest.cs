using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

        [DataTestMethod]
        [DataRow("RLELossless")]
        [DataRow("JPEG2000Lossless")]
        [DataRow("JPEG2000Lossy")]
        [DataRow("JPEGProcess1")]
        [DataRow("JPEGProcess2_4")]
        [DataRow("JPEGProcess14")]
        [DataRow("JPEGProcess14SV1")]
        [DataRow("JPEGLSLossless")]
        [DataRow("JPEGLSNearLossless")]
        public void PerformTranscode(string name)
        {
            var output = $"out/{name}.dcm";
            var data = DicomFile.Open("test.dcm");
            var binding = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

            var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(name, binding).GetValue(0);
            var image = new DicomFile(data.Dataset).Clone(ts);

            Assert.IsNotNull(image);

            image.Save(output);

            var data1 = DicomFile.Open(output);
            Assert.IsTrue(data1.Dataset.Contains(DicomTag.PixelData));
        }
    }
}
