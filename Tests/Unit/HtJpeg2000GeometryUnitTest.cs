using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FellowOakDicom.Imaging.NativeCodec.Test
{
    /// <summary>
    /// Regression coverage for an HTJ2K codestream whose extent disagrees with the
    /// geometry the dataset declares. Decode rents its destination buffer from
    /// Rows/Columns/SamplesPerPixel/BitsAllocated, while the native decoder sizes its
    /// own output from the codestream's SIZ marker and memcpy's that many bytes into
    /// it. Neither side compared the two, so a codestream covering a larger extent
    /// than the declared geometry wrote past the end of the rented array.
    ///
    /// This is the same defect Jpeg2000GeometryUnitTest covers for the classic
    /// decoder. The guard there does not apply here: it sits in DicomJpeg2000Codec,
    /// and the HTJ2K codecs are a separate IDicomCodec registered by reflection for
    /// transfer syntaxes 1.2.840.10008.1.2.4.201 / .202 / .203.
    /// </summary>
    [TestClass]
    public class HtJpeg2000GeometryUnitTest
    {
        // Side of the image handed to the encoder, and therefore the extent the
        // resulting codestream reports. The declared geometry is rewritten
        // afterwards, so it can differ from this in either direction.
        private const ushort CodestreamSide = 64;

        private static readonly DicomTransferSyntax HtJ2KLossless =
            DicomTransferSyntax.Parse("1.2.840.10008.1.2.4.201");

        [TestInitialize]
        public void Initialization()
        {
            new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<NativeTranscoderManager>())
                .SkipValidation()
                .Build();
        }

        /// <summary>
        /// Encodes a square image with the HTJ2K encoder, then rewrites Rows and
        /// Columns on the encapsulated dataset. The codestream is left untouched, so
        /// its extent no longer matches the geometry the dataset declares.
        /// </summary>
        private static DicomDataset BuildEncoded(ushort bitsAllocated, ushort declaredSide)
        {
            var dataset = new DicomDataset(DicomTransferSyntax.ExplicitVRLittleEndian);
            dataset.AddOrUpdate(DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage);
            dataset.AddOrUpdate(DicomTag.SOPInstanceUID, DicomUID.Generate());
            dataset.AddOrUpdate(DicomTag.Rows, CodestreamSide);
            dataset.AddOrUpdate(DicomTag.Columns, CodestreamSide);
            dataset.AddOrUpdate(DicomTag.SamplesPerPixel, (ushort)1);
            dataset.AddOrUpdate(DicomTag.PhotometricInterpretation, "MONOCHROME2");
            dataset.AddOrUpdate(DicomTag.BitsAllocated, bitsAllocated);
            dataset.AddOrUpdate(DicomTag.BitsStored, bitsAllocated);
            dataset.AddOrUpdate(DicomTag.HighBit, (ushort)(bitsAllocated - 1));
            dataset.AddOrUpdate(DicomTag.PixelRepresentation, (ushort)0);
            dataset.AddOrUpdate(DicomTag.NumberOfFrames, 1);

            var pixels = new byte[CodestreamSide * CodestreamSide * (bitsAllocated / 8)];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = (byte)(i & 0xFF);
            }

            DicomPixelData.Create(dataset, true).AddFrame(new MemoryByteBuffer(pixels));

            var encoded = dataset.Clone(HtJ2KLossless);
            encoded.AddOrUpdate(DicomTag.Rows, declaredSide);
            encoded.AddOrUpdate(DicomTag.Columns, declaredSide);
            return encoded;
        }

        [TestMethod]
        [DataRow((ushort)8)]
        [DataRow((ushort)16)]
        public void DecodeHtJpeg2000LargerThanDeclaredGeometryReportsErrorInsteadOfOverrunning(ushort bitsAllocated)
        {
            // A 64x64 codestream against a dataset declaring 8x8: the native decoder
            // produces 4096 pixels for a buffer rented to hold 64 of them.
            var encoded = BuildEncoded(bitsAllocated, 8);

            try
            {
                encoded.Clone(DicomTransferSyntax.ExplicitVRLittleEndian);
                Assert.Fail("Expected a DicomCodecException for a codestream larger than the declared geometry.");
            }
            catch (DicomCodecException e)
            {
                // Assert on the message, not just the type. Without the guard this
                // input still ends in a DicomCodecException: the native memcpy
                // overruns the rented array first, and the Buffer.BlockCopy that
                // follows then throws because the decoded size exceeds it. A bare
                // catch(DicomCodecException) therefore passes with or without the
                // fix, while the write it is meant to prevent has already happened.
                // At larger extents the same input instead kills the process outright
                // (Fatal error 0x80131506, the runtime failing inside its own
                // exception path because the heap is already damaged), which is why
                // this test uses the smaller one that reports rather than terminates.
                StringAssert.Contains(e.Message, "larger than the pixel buffer");
            }
        }

        /// <summary>
        /// The control that keeps the guard honest. Only the larger-than case may be
        /// rejected: a codestream smaller than the declared geometry fits the rented
        /// buffer and has always decoded, so rejecting it would be a regression
        /// dressed up as a fix.
        /// </summary>
        [TestMethod]
        [DataRow((ushort)8)]
        [DataRow((ushort)16)]
        public void DecodeHtJpeg2000SmallerThanDeclaredGeometryStillDecodes(ushort bitsAllocated)
        {
            var encoded = BuildEncoded(bitsAllocated, 128);

            var decoded = encoded.Clone(DicomTransferSyntax.ExplicitVRLittleEndian);

            Assert.IsNotNull(DicomPixelData.Create(decoded).GetFrame(0));
        }

        /// <summary>
        /// The other half of the control: a codestream that agrees with the declared
        /// geometry must be untouched by the guard.
        /// </summary>
        [TestMethod]
        [DataRow((ushort)8)]
        [DataRow((ushort)16)]
        public void DecodeHtJpeg2000MatchingGeometryStillDecodes(ushort bitsAllocated)
        {
            var encoded = BuildEncoded(bitsAllocated, CodestreamSide);

            var decoded = encoded.Clone(DicomTransferSyntax.ExplicitVRLittleEndian);

            var frame = DicomPixelData.Create(decoded).GetFrame(0);
            Assert.AreEqual(CodestreamSide * CodestreamSide * (bitsAllocated / 8), frame.Size);
        }
    }
}
