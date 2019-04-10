using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dicom;
using Dicom.Imaging.Codec;

namespace Efferent.Native.Test
{
    [TestClass]
    public class CanTranscodeUnitTest
    {
        [TestInitialize]
        public void InitializationTest()
        {
            TranscoderManager.SetImplementation(new Efferent.Native.Codec.NativeTranscoderManager());
        }

        [TestMethod]
        public void CanTranscodeRLELossless()
        {
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.RLELossless));
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.RLELossless, DicomTransferSyntax.ExplicitVRLittleEndian));
        }

        [TestMethod]
        public void CanTranscodeJpeg2000Lossless()
        {
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.JPEG2000Lossless));
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.JPEG2000Lossless, DicomTransferSyntax.ExplicitVRLittleEndian));
        }

        [TestMethod]
        public void CanTranscodeJpeg2000Lossy()
        {
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.JPEG2000Lossy));
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.JPEG2000Lossy, DicomTransferSyntax.ExplicitVRLittleEndian));
        }

        [TestMethod]
        public void CanTranscodeJPEGProcess1()
        {
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.JPEGProcess1));
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.JPEGProcess1, DicomTransferSyntax.ExplicitVRLittleEndian));
        }

        [TestMethod]
        public void CanTranscodeJPEGProcess2_4()
        {
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.JPEGProcess2_4));
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.JPEGProcess2_4, DicomTransferSyntax.ExplicitVRLittleEndian));
        }

        [TestMethod]
        public void CanTranscodeJPEGProcess14()
        {
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.JPEGProcess14));
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.JPEGProcess14, DicomTransferSyntax.ExplicitVRLittleEndian));
        }

        [TestMethod]
        public void CanTranscodeJPEGProcess14SV1()
        {
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.JPEGProcess14SV1));
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.JPEGProcess14SV1, DicomTransferSyntax.ExplicitVRLittleEndian));
        }

        [TestMethod]
        public void CanTranscodeJPEGLSLossless()
        {
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.JPEGLSLossless));
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.JPEGLSLossless, DicomTransferSyntax.ExplicitVRLittleEndian));
        }

        [TestMethod]
        public void CanTranscodeJPEGLSNearLossless()
        {
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.JPEGLSNearLossless));
            Assert.IsTrue(TranscoderManager.CanTranscode(DicomTransferSyntax.JPEGLSNearLossless, DicomTransferSyntax.ExplicitVRLittleEndian));
        }
    }
}