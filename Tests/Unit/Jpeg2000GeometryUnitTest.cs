using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FellowOakDicom.Imaging.NativeCodec.Test
{
    /// <summary>
    /// Regression coverage for a JPEG 2000 codestream whose extent disagrees with
    /// the geometry the dataset declares. Decode sizes its destination buffer from
    /// Rows/Columns/BitsAllocated/SamplesPerPixel, but drives the extraction loop
    /// from the codestream's own x1/y1. The two were never compared, so a
    /// codestream covering a larger extent than the declared geometry wrote past
    /// the end of the buffer.
    /// </summary>
    [TestClass]
    public class Jpeg2000GeometryUnitTest
    {
        // Side of the image handed to the encoder, and therefore the extent the
        // resulting codestream reports. The declared geometry is rewritten
        // afterwards, so it can differ from this in either direction.
        private const ushort CodestreamSide = 64;

        [TestInitialize]
        public void Initialization()
        {
            new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<NativeTranscoderManager>())
                .SkipValidation()
                .Build();
        }

        /// <summary>
        /// Encodes a square image with the JPEG 2000 encoder, then rewrites Rows and
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

            var encoded = dataset.Clone(DicomTransferSyntax.JPEG2000Lossless);
            encoded.AddOrUpdate(DicomTag.Rows, declaredSide);
            encoded.AddOrUpdate(DicomTag.Columns, declaredSide);
            return encoded;
        }

        /// <summary>
        /// Builds a 3-component (RGB) codestream and then declares SamplesPerPixel=1
        /// on the encapsulated dataset. Rows and Columns are left agreeing with the
        /// codestream, so the per-pixel extent matches and only the samples-per-pixel
        /// factor is wrong.
        /// </summary>
        private static DicomDataset BuildEncodedWithComponentMismatch(ushort bitsAllocated)
        {
            var dataset = new DicomDataset(DicomTransferSyntax.ExplicitVRLittleEndian);
            dataset.AddOrUpdate(DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage);
            dataset.AddOrUpdate(DicomTag.SOPInstanceUID, DicomUID.Generate());
            dataset.AddOrUpdate(DicomTag.Rows, CodestreamSide);
            dataset.AddOrUpdate(DicomTag.Columns, CodestreamSide);
            dataset.AddOrUpdate(DicomTag.SamplesPerPixel, (ushort)3);
            dataset.AddOrUpdate(DicomTag.PhotometricInterpretation, "RGB");
            dataset.AddOrUpdate(DicomTag.PlanarConfiguration, (ushort)0);
            dataset.AddOrUpdate(DicomTag.BitsAllocated, bitsAllocated);
            dataset.AddOrUpdate(DicomTag.BitsStored, bitsAllocated);
            dataset.AddOrUpdate(DicomTag.HighBit, (ushort)(bitsAllocated - 1));
            dataset.AddOrUpdate(DicomTag.PixelRepresentation, (ushort)0);

            var pixels = new byte[CodestreamSide * CodestreamSide * 3 * (bitsAllocated / 8)];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = (byte)(i & 0xFF);
            }

            DicomPixelData.Create(dataset, true).AddFrame(new MemoryByteBuffer(pixels));

            var encoded = dataset.Clone(DicomTransferSyntax.JPEG2000Lossless);
            encoded.AddOrUpdate(DicomTag.SamplesPerPixel, (ushort)1);
            encoded.AddOrUpdate(DicomTag.PhotometricInterpretation, "MONOCHROME2");
            return encoded;
        }

        [TestMethod]
        [DataRow((ushort)8)]
        [DataRow((ushort)16)]
        public void DecodeJpeg2000LargerThanDeclaredGeometryReportsErrorInsteadOfOverrunning(ushort bitsAllocated)
        {
            // A 64x64 codestream against a dataset declaring 8x8: extraction would
            // write 4096 pixels into a buffer sized for 64 of them.
            var encoded = BuildEncoded(bitsAllocated, 8);

            try
            {
                encoded.Clone(DicomTransferSyntax.ExplicitVRLittleEndian);
                Assert.Fail("Expected a DicomCodecException for a codestream larger than the declared geometry.");
            }
            catch (DicomCodecException e)
            {
                // Assert on the message, not just the type. The 8-bit extractor writes
                // through a bounds-checked managed array and already surfaced a wrapped
                // IndexOutOfRangeException before this guard existed, so a bare
                // catch(DicomCodecException) would pass with or without the fix.
                StringAssert.Contains(e.Message, "larger than the pixel buffer");
            }
        }

        [TestMethod]
        [DataRow((ushort)8)]
        [DataRow((ushort)16)]
        public void DecodeJpeg2000MoreComponentsThanDeclaredReportsErrorInsteadOfOverrunning(ushort bitsAllocated)
        {
            // Same pixel extent, three samples per pixel in the codestream against a
            // dataset declaring one. Comparing pixel counts alone misses this: the
            // buffer holds Width*Height samples while extraction writes three times
            // that, so the geometry check has to carry the samples-per-pixel factor.
            var encoded = BuildEncodedWithComponentMismatch(bitsAllocated);

            try
            {
                encoded.Clone(DicomTransferSyntax.ExplicitVRLittleEndian);
                Assert.Fail("Expected a DicomCodecException for a codestream with more components than declared.");
            }
            catch (DicomCodecException e)
            {
                StringAssert.Contains(e.Message, "larger than the pixel buffer");
            }
        }

        [TestMethod]
        public void DecodeJpeg2000GeometryOverflowingTheSizeComputationReportsErrorInsteadOfOverrunning()
        {
            // Rows*Columns*SamplesPerPixel*BytesAllocated is computed in int, so this
            // geometry wraps to a small positive buffer (1,830,704 bytes) rather than
            // the ~4.3 GB it names. A check that recomputes the declared size instead
            // of measuring the buffer compares against the unwrapped value, passes,
            // and the extraction then overruns the real allocation.
            var dataset = new DicomDataset(DicomTransferSyntax.ExplicitVRLittleEndian);
            dataset.AddOrUpdate(DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage);
            dataset.AddOrUpdate(DicomTag.SOPInstanceUID, DicomUID.Generate());
            dataset.AddOrUpdate(DicomTag.Rows, (ushort)2048);
            dataset.AddOrUpdate(DicomTag.Columns, (ushort)2048);
            dataset.AddOrUpdate(DicomTag.SamplesPerPixel, (ushort)1);
            dataset.AddOrUpdate(DicomTag.PhotometricInterpretation, "MONOCHROME2");
            dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)16);
            dataset.AddOrUpdate(DicomTag.BitsStored, (ushort)16);
            dataset.AddOrUpdate(DicomTag.HighBit, (ushort)15);
            dataset.AddOrUpdate(DicomTag.PixelRepresentation, (ushort)0);

            var pixels = new byte[2048 * 2048 * 2];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = (byte)(i & 0xFF);
            }

            DicomPixelData.Create(dataset, true).AddFrame(new MemoryByteBuffer(pixels));

            var encoded = dataset.Clone(DicomTransferSyntax.JPEG2000Lossless);
            encoded.AddOrUpdate(DicomTag.Rows, (ushort)33000);
            encoded.AddOrUpdate(DicomTag.Columns, (ushort)65103);

            try
            {
                encoded.Clone(DicomTransferSyntax.ExplicitVRLittleEndian);
                Assert.Fail("Expected a DicomCodecException for geometry whose size computation overflows.");
            }
            catch (DicomCodecException e)
            {
                StringAssert.Contains(e.Message, "larger than the pixel buffer");
            }
        }

        [TestMethod]
        [DataRow((ushort)8)]
        [DataRow((ushort)16)]
        public void DecodeJpeg2000SmallerThanDeclaredGeometrySucceeds(ushort bitsAllocated)
        {
            // Positive control: over-declared geometry leaves spare room in the
            // destination buffer and has always decoded, so the check stays
            // one-sided instead of demanding an exact match.
            var encoded = BuildEncoded(bitsAllocated, 128);

            var decoded = encoded.Clone(DicomTransferSyntax.ExplicitVRLittleEndian);

            Assert.AreEqual(DicomTransferSyntax.ExplicitVRLittleEndian, decoded.InternalTransferSyntax);
            Assert.AreEqual(128 * 128 * (bitsAllocated / 8), DicomPixelData.Create(decoded).GetFrame(0).Data.Length);
        }
    }
}
