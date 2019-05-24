using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dicom;
using Dicom.Imaging.Codec;
using Dicom.Imaging;

namespace Efferent.Native.Test
{
    [TestClass]
    public class AcceptanceTests
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

        private static string[][] resultsPerform;
        private static string[][] resultsInverse;

        [ClassInitialize]
        public static void Initialization(TestContext context)
        {
            TranscoderManager.SetImplementation(new Efferent.Native.Codec.NativeTranscoderManager());
            resultsPerform = new string[filenames.Length][];
            resultsInverse = new string[filenames.Length][];

            for (int i = 0; i < filenames.Length; i++)
            {
                resultsPerform[i] = new string[transferSyntaxes.Length];
                resultsInverse[i] = new string[transferSyntaxes.Length];
            }

            Directory.CreateDirectory("out");
        }

        [DataTestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void PerformTranscode(int index0, int index1)
        { 
            var output = $"out/{transferSyntaxes[index1]}_{filenames[index0]}";

            try
            {
                var data = DicomFile.Open(filenames[index0]);

                var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(transferSyntaxes[index1], binding).GetValue(0);
                var image = new DicomFile(data.Dataset).Clone(ts);

                image.Save(output);

                resultsPerform[index0][index1] = "OK";
            }
            catch (Exception e)
            {
                resultsPerform[index0][index1] = "FAIL";

                File.WriteAllText(output, e.Message);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void InverseTranscode(int index0, int index1)
        {
            var input = $"out/{transferSyntaxes[index1]}_{filenames[index0]}";
            var output = $"out/RAW_FROM_{transferSyntaxes[index1]}_{filenames[index0]}";

            try
            {
                var data = DicomFile.Open(input);

                var ts = (DicomTransferSyntax)typeof(DicomTransferSyntax).GetField(transferSyntaxes[index1], binding).GetValue(0);
                var image = new DicomFile(data.Dataset).Clone(DicomTransferSyntax.ExplicitVRLittleEndian);

                image.Save(output);

                resultsInverse[index0][index1] = "OK";
            }
            catch (Exception e)
            {
                resultsInverse[index0][index1] = "FAIL";

                File.WriteAllText(output, e.Message);
            }
        }

        [ClassCleanup]
        public static void BuildReport()
        {
            string md = "# Acceptance tests\n";
            md += "The following results indicate the conversions that didn't break, but not the correctness of them. Inspect the Dicom files visually for correctness.\n";
            md += "## PerformTranscode\n\n";
            md += "Filename|" + string.Join(" | ", transferSyntaxes.Select(ts => splitName(ts))) + "\n";
            md += "-- | " + string.Join(" | ", transferSyntaxes.Select(ts=>":--:")) + "\n";

            for (int i=0; i < filenames.Length; i++)
            {
                md += filenames[i] + " | " + string.Join(" | ", resultsPerform[i]) + "\n";
            }

            md += "\n## InverseTranscode\n\n";
            md += "Filename|" + string.Join(" | ", transferSyntaxes.Select(ts => splitName(ts))) + "\n";
            md += "-- | " + string.Join(" | ", transferSyntaxes.Select(ts=>":--:")) + "\n";

            for (int i=0; i < filenames.Length; i++)
            {
                md += filenames[i] + " | " + string.Join(" | ", resultsInverse[i]) + "\n";
            }

            File.WriteAllText("out/Results.md", md);
        }

        public static IEnumerable<object[]> Data
        {
            get 
            {
                for (int i = 0; i < filenames.Length; i++)
                {
                    for (int j = 0; j < transferSyntaxes.Length; j++)
                    {
                        yield return new object[] { i, j };
                    }
                }
            }
        }
	
        private static string splitName(string name)
        {		
            return Regex.Replace(name, "([A-Z][a-z])", " $1");
        }	
    }
}
