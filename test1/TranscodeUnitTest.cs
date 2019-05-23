using System;
using System.Collections.Generic;
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
        const BindingFlags binding = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        private static string[] filenames = 
        {
            "PM5644-960x540_JPEG-Baseline_YBR422.dcm",
            "PM5644-960x540_JPEG-Baseline_YBRFull.dcm",
            "PM5644-960x540_JPEG-Lossless_RGB.dcm",
            "PM5644-960x540_JPEG-LS_Lossless.dcm",
            "PM5644-960x540_JPEG-LS_NearLossless.dcm",
            "PM5644-960x540_JPEG2000-Lossless.dcm",
            "PM5644-960x540_JPEG2000-Lossy.dcm",
            "PM5644-960x540_JPEG2000-Lossy50.dcm",
            "PM5644-960x540_Palette_8.dcm",
            "PM5644-960x540_Palette_16.dcm",
            "PM5644-960x540_RGB.dcm",
            "PM5644-960x540_RLE-Lossless.dcm"
        };

        private static string[] transferSyntaxes =
        {
            "RLELossless",
            "JPEG2000Lossless",
            "JPEG2000Lossy",
            "JPEGLSLossless",
            "JPEGLSNearLossless",
            "JPEGProcess1"
        };

        [TestInitialize]
        public void Initialization()
        {
            TranscoderManager.SetImplementation(new Efferent.Native.Codec.NativeTranscoderManager());

            Directory.CreateDirectory("out");
        }

        [DataTestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void PerformTranscode(string filename, string transferSyntax)
        {
            var output = $"out/{transferSyntax}_{filename}";
            var data = DicomFile.Open(filename);

            var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(transferSyntax, binding).GetValue(0);
            var image = new DicomFile(data.Dataset).Clone(ts);

            Assert.IsNotNull(image);

            image.Save(output);

            var data1 = DicomFile.Open(output);
            Assert.IsTrue(data1.Dataset.Contains(DicomTag.PixelData));
        }

        [DataTestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void InverseTranscode(string filename, string transferSyntax)
        {
            var input = $"out/{transferSyntax}_{filename}";
            var output = $"out/RAW_FROM_{transferSyntax}_{filename}";
            var data = DicomFile.Open(input);

            var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(transferSyntax, binding).GetValue(0);
            var image = new DicomFile(data.Dataset).Clone(DicomTransferSyntax.ExplicitVRLittleEndian);

            Assert.IsNotNull(image);

            image.Save(output);

            var data1 = DicomFile.Open(output);
            Assert.IsTrue(data1.Dataset.Contains(DicomTag.PixelData));
        }

        public static IEnumerable<object[]> Data
        {
            get 
            {
                foreach (var filename in filenames)
                {
                    foreach (var transferSyntax in transferSyntaxes)
                    {
                        yield return new object[] { filename, transferSyntax };
                    }
                }
            }
        }
    }
}
