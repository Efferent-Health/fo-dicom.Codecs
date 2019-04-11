using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dicom;
using Dicom.Imaging.Codec;

namespace Efferent.Native.Test
{
    [TestClass]
    public class CanTranscodeUnitTest
    {
        [TestInitialize]
        public void Initialization()
        {
            TranscoderManager.SetImplementation(new Efferent.Native.Codec.NativeTranscoderManager());
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
        public void PerformCanTranscode(string name)
        {
            const BindingFlags binding = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(name, binding).GetValue(0);

            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, ts));
            Assert.IsTrue(TranscoderManager.CanTranscode(ts, DicomTransferSyntax.ExplicitVRLittleEndian));
        }
    }
}