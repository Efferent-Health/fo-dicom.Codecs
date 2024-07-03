using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FellowOakDicom;
using FellowOakDicom.Imaging.Codec;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace FellowOakDicom.Imaging.NativeCodec.Test
{
    [TestClass]
    public class AcceptanceTests
    {
        const BindingFlags binding = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        private static string[] filenames;

        private static DicomTransferSyntax[] transferSyntaxes =
        {
            DicomTransferSyntax.RLELossless,
            DicomTransferSyntax.JPEG2000Lossless,
            DicomTransferSyntax.JPEG2000Lossy,
            DicomTransferSyntax.HTJ2K,
            DicomTransferSyntax.HTJ2KLossless,
            DicomTransferSyntax.HTJ2KLosslessRPCL,
            DicomTransferSyntax.JPEGLSLossless,
            DicomTransferSyntax.JPEGLSNearLossless,
            DicomTransferSyntax.JPEGProcess1
        };

        private static string[][] resultsPerform;
        private static string[][] resultsInverse;
        private static string[] resultsRender;

        [ClassInitialize]
        public static void Initialization(TestContext context)
        {
            //Setting fo-dicom.Codecs implementation into fo-dicom 5.0.0 
            new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<NativeTranscoderManager>())
                .SkipValidation()
                .Build();

            filenames = Directory.GetFiles(".","*.dcm").Select(fn => Path.GetFileName(fn)).ToArray();

            resultsPerform = new string[filenames.Length][];
            resultsInverse = new string[filenames.Length][];
            resultsRender = new string[filenames.Length];

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

                var ts = transferSyntaxes[index1];

                new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<NativeTranscoderManager>())
                .Build();
                var image = new DicomFile(data.Dataset).Clone(ts);

                image.Save(output);

                resultsPerform[index0][index1] = "OK";
            }
            catch
            {
                resultsPerform[index0][index1] = "FAIL";

                Assert.Fail("Couldn't change Transfer syntax " + transferSyntaxes[index1] + " from: " + filenames[index0] + " file.");
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

                var ts = transferSyntaxes[index1];

                new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<NativeTranscoderManager>())
                .Build();
                
                var image = new DicomFile(data.Dataset).Clone(DicomTransferSyntax.ExplicitVRLittleEndian);

                image.Save(output);

                resultsInverse[index0][index1] = "OK";
            }
            catch
            {
                resultsInverse[index0][index1] = "FAIL";
                Assert.Fail("Couldn't Decode from: " + filenames[index0] + " file.");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(ImageData), DynamicDataSourceType.Property)]
        public void RenderImages(int index0)
        {
            var outputFile = Path.ChangeExtension(Path.GetFileNameWithoutExtension(filenames[index0]), ".png");

            try
            {
                var img = new DicomImage(filenames[index0]);

                new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddImageManager<ImageSharpImageManager>().AddTranscoderManager<NativeTranscoderManager>())
                .Build();

                img.RenderImage().AsSharpImage().SaveAsPng(Path.Combine($"out", outputFile));
                
                resultsRender[index0] = "OK";
            }
            catch
            {
                resultsRender[index0] = "FAIL";
            
                Assert.Fail("Couldn't extract image from: " + filenames[index0] + " file.");
            }
        }

        [ClassCleanup]
        public static void BuildReport()
        {
            string md = "# Acceptance tests\n";
            md += "The following results indicate the conversions that didn't break, but not the correctness of them. Inspect the Dicom files visually for correctness.\n";
            md += "## PerformTranscode\n\n";
            md += "Filename|" + string.Join(" | ", transferSyntaxes.Select(ts => splitName(ts.ToString()))) + "\n";
            md += "-- | " + string.Join(" | ", transferSyntaxes.Select(ts=>":--:")) + "\n";

            for (int i=0; i < filenames.Length; i++)
            {
                md += filenames[i] + " | " + string.Join(" | ", resultsPerform[i]) + "\n";
            }

            md += "\n## InverseTranscode\n\n";
            md += "Filename|" + string.Join(" | ", transferSyntaxes.Select(ts => splitName(ts.ToString()))) + "\n";
            md += "-- | " + string.Join(" | ", transferSyntaxes.Select(ts=>":--:")) + "\n";

            for (int i=0; i < filenames.Length; i++)
            {
                md += filenames[i] + " | " + string.Join(" | ", resultsInverse[i]) + "\n";
            }

            md += "\n## RenderImages\n\n";
            md += "Filename|Rendering\n";
            md += "-- | --\n";

            for (int i = 0; i < filenames.Length; i++)
            {
                md += filenames[i] + " | " + string.Join(" | ", resultsRender[i]) + "\n";
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

        public static IEnumerable<object[]> ImageData
        {
            get
            {
                for (int i = 0; i < filenames.Length; i++)
                {
                        yield return new object[] { i };
                }
            }
        }

        private static string splitName(string name)
        {		
            return Regex.Replace(name, "([A-Z][a-z])", " $1");
        }	
    }
}
