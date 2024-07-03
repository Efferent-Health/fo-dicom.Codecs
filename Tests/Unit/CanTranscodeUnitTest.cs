using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FellowOakDicom;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Imaging.NativeCodec;

namespace FellowOakDicom.Imaging.NativeCodec.Test
{
    [TestClass]
    public class CanTranscodeUnitTest
    {
        [TestInitialize]
        public void Initialization()
        {
            //Setting fo-dicom.Codecs implementation into fo-dicom 5.0.0 
            new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<NativeTranscoderManager>())
                .SkipValidation()
                .Build();
        }

        [DataTestMethod]
        [DataRow("RLELossless")]
        [DataRow("JPEG2000Lossless")]
        [DataRow("JPEG2000Lossy")]
        [DataRow("HTJ2K")]
        [DataRow("HTJ2KLossless")]
        [DataRow("HTJ2KLosslessRPCL")]
        [DataRow("JPEGProcess1")]
        [DataRow("JPEGProcess2_4")]
        [DataRow("JPEGProcess14")]
        [DataRow("JPEGProcess14SV1")]
        [DataRow("JPEGLSLossless")]
        [DataRow("JPEGLSNearLossless")]
        public void PerformCanTranscode(string name)
        {
            const BindingFlags binding = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(name, binding).GetValue(0);
            
            var nativetranscoder = new NativeTranscoderManager();
            Assert.IsTrue(nativetranscoder.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, ts));
            Assert.IsTrue(nativetranscoder.CanTranscode(ts, DicomTransferSyntax.ExplicitVRLittleEndian));
        }
    }
}