using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FellowOakDicom;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.NativeCodec;

namespace FellowOakDicom.Imaging.NativeCodec.Test
{
    [TestClass]
    public class TranscodeUnitTest
    {
        const BindingFlags binding = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        [TestInitialize]
        public void Initialization()
        {
            //Setting fo-dicom.Codecs implementation into fo-dicom 5.0.0 
            new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<NativeTranscoderManager>())
                .SkipValidation()
                .Build();

            Directory.CreateDirectory("out");
        }

        [DataTestMethod]
        [DataRow("RLELossless")]
        [DataRow("JPEG2000Lossless")]
        [DataRow("JPEG2000Lossy")]
        [DataRow("JPEGProcess14")]
        [DataRow("JPEGProcess14SV1")]
        [DataRow("JPEGLSLossless")]
        [DataRow("JPEGLSNearLossless")]
        [DataRow("JPEGProcess1")]
        [DataRow("JPEGProcess2_4")]
        public void PerformTranscode8bits(string name)
        {   
            var output = $"out/{name}8bits.dcm";
            var data = DicomFile.Open("test8bits.dcm");

            var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(name, binding).GetValue(0);
            var image = new DicomFile(data.Dataset).Clone(ts);

            Assert.IsNotNull(image);

            image.Save(output);

            var data1 = DicomFile.Open(output);
            Assert.IsTrue(data1.Dataset.Contains(DicomTag.PixelData));
        }

        [DataTestMethod]
        [DataRow("RLELossless")]
        [DataRow("JPEG2000Lossless")]
        [DataRow("JPEG2000Lossy")]
        [DataRow("JPEGProcess14")]
        [DataRow("JPEGProcess14SV1")]
        [DataRow("JPEGLSLossless")]
        [DataRow("JPEGLSNearLossless")]
        [DataRow("JPEGProcess1")]
        [DataRow("JPEGProcess2_4")]
        public void InverseTranscode8bits(string name)
        {
            var input = $"out/{name}8bits.dcm";
            var output = $"out/from_{name}8bits.dcm";
            var data = DicomFile.Open(input);

            var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(name, binding).GetValue(0);
            var image = new DicomFile(data.Dataset).Clone(DicomTransferSyntax.ExplicitVRLittleEndian);

            Assert.IsNotNull(image);

            image.Save(output);

            var data1 = DicomFile.Open(output);
            Assert.IsTrue(data1.Dataset.Contains(DicomTag.PixelData));
        }

        [DataTestMethod]
        [DataRow("RLELossless")]
        [DataRow("JPEG2000Lossless")]
        [DataRow("JPEG2000Lossy")]
        [DataRow("JPEGProcess14")]
        [DataRow("JPEGProcess14SV1")]
        [DataRow("JPEGLSLossless")]
        [DataRow("JPEGLSNearLossless")]
        // [DataRow("JPEGProcess1")]    // Not passing as the input image is 16 bits
        // [DataRow("JPEGProcess2_4")]  // Not passing as the input image is 16 bits
        public void PerformTranscode16bits(string name)
        {
            var output = $"out/{name}16bits.dcm";
            var data = DicomFile.Open("test16bits.dcm");

            var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(name, binding).GetValue(0);
            var image = new DicomFile(data.Dataset).Clone(ts);

            Assert.IsNotNull(image);

            image.Save(output);

            var data1 = DicomFile.Open(output);
            Assert.IsTrue(data1.Dataset.Contains(DicomTag.PixelData));
        }

        [DataTestMethod]
        [DataRow("RLELossless")]
        [DataRow("JPEG2000Lossless")]
        [DataRow("JPEG2000Lossy")]
        [DataRow("JPEGProcess14")]
        [DataRow("JPEGProcess14SV1")]
        [DataRow("JPEGLSLossless")]
        [DataRow("JPEGLSNearLossless")]
        // [DataRow("JPEGProcess1")]    // Not passing as the input image is 16 bits
        // [DataRow("JPEGProcess2_4")]  // Not passing as the input image is 16 bits
        public void InverseTranscode16bits(string name)
        {
            var input = $"out/{name}16bits.dcm";
            var output = $"out/from_{name}16bits.dcm";
            var data = DicomFile.Open(input);

            var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(name, binding).GetValue(0);
            var image = new DicomFile(data.Dataset).Clone(DicomTransferSyntax.ExplicitVRLittleEndian);

            Assert.IsNotNull(image);

            image.Save(output);

            var data1 = DicomFile.Open(output);
            Assert.IsTrue(data1.Dataset.Contains(DicomTag.PixelData));
        }
    }
}
