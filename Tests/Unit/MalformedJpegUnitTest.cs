using System;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FellowOakDicom.Imaging.NativeCodec.Test
{
    /// <summary>
    /// Regression coverage for a malformed JPEG codestream reaching the native
    /// decoder. A JPEG whose scan references a Huffman table that was never
    /// defined makes libjpeg raise JERR_NO_HUFF_TABLE, which unwinds through the
    /// decoder's setjmp handler. That error path must report the failure to the
    /// caller, not tear the process down.
    /// </summary>
    [TestClass]
    public class MalformedJpegUnitTest
    {
        // Minimal lossless (Process 14, SV1) codestream, 64x64, 8-bit, 1 component.
        // The two payloads below differ ONLY by the DHT segment.
        private const string Soi = "FFD8";
        private const string Sof3 = "FFC3000B080040004001011100";
        private const string Dht = "FFC4001F0000010501010101010100000000000000000102030405060708090A10";
        private const string Sos = "FFDA0008010100010000";
        private const string Eoi = "FFD9";

        [TestInitialize]
        public void Initialization()
        {
            new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<NativeTranscoderManager>())
                .SkipValidation()
                .Build();
        }

        private static byte[] BuildJpeg(bool includeHuffmanTable)
        {
            var hex = Soi + Sof3 + (includeHuffmanTable ? Dht : string.Empty) + Sos;
            var header = Convert.FromHexString(hex);
            var footer = Convert.FromHexString(Eoi);

            var jpeg = new byte[header.Length + 128 + footer.Length];
            Buffer.BlockCopy(header, 0, jpeg, 0, header.Length);
            for (var i = 0; i < 128; i++)
            {
                jpeg[header.Length + i] = 0x55; // entropy-coded bytes
            }
            Buffer.BlockCopy(footer, 0, jpeg, header.Length + 128, footer.Length);
            return jpeg;
        }

        private static DicomDataset BuildDataset(bool includeHuffmanTable)
        {
            var dataset = new DicomDataset(DicomTransferSyntax.JPEGProcess14SV1);
            dataset.AddOrUpdate(DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage);
            dataset.AddOrUpdate(DicomTag.SOPInstanceUID, DicomUID.Generate());
            dataset.AddOrUpdate(DicomTag.Rows, (ushort)64);
            dataset.AddOrUpdate(DicomTag.Columns, (ushort)64);
            dataset.AddOrUpdate(DicomTag.SamplesPerPixel, (ushort)1);
            dataset.AddOrUpdate(DicomTag.PhotometricInterpretation, "MONOCHROME2");
            dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)8);
            dataset.AddOrUpdate(DicomTag.BitsStored, (ushort)8);
            dataset.AddOrUpdate(DicomTag.HighBit, (ushort)7);
            dataset.AddOrUpdate(DicomTag.PixelRepresentation, (ushort)0);
            dataset.AddOrUpdate(DicomTag.NumberOfFrames, 1);

            var fragments = new DicomOtherByteFragment(DicomTag.PixelData);
            fragments.Fragments.Add(new MemoryByteBuffer(BuildJpeg(includeHuffmanTable)));
            dataset.AddOrUpdate(fragments);

            return dataset;
        }

        private static DicomDataset Decode(DicomDataset dataset)
        {
            var transcoder = new DicomTranscoder(dataset.InternalTransferSyntax,
                                                 DicomTransferSyntax.ExplicitVRLittleEndian);
            return transcoder.Transcode(dataset);
        }

        [TestMethod]
        public void DecodeJpegMissingHuffmanTableReportsErrorInsteadOfCrashing()
        {
            try
            {
                Decode(BuildDataset(includeHuffmanTable: false));
                Assert.Fail("Expected a DicomCodecException for a scan referencing an undefined Huffman table.");
            }
            catch (DicomCodecException)
            {
                // Expected: the libjpeg diagnostic is surfaced and the process survives.
            }
        }

        [TestMethod]
        public void DecodeJpegWithHuffmanTableSucceeds()
        {
            // Positive control: identical codestream plus the DHT segment. Guards
            // against "fixing" the crash by disabling the decode path outright.
            var decoded = Decode(BuildDataset(includeHuffmanTable: true));

            Assert.AreEqual(DicomTransferSyntax.ExplicitVRLittleEndian, decoded.InternalTransferSyntax);
            Assert.IsTrue(decoded.Contains(DicomTag.PixelData));
        }
    }
}
