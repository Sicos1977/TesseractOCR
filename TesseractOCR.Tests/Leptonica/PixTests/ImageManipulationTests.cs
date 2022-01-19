using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;

namespace Tesseract.Tests.Leptonica.PixTests
{
    [TestClass]
    public class ImageManipulationTests : TesseractTestBase
    {
        private const string ResultsDirectory = @"Results\ImageManipulation\";

        [TestMethod]
        public void DescewTest()
        {
            var sourcePixPath = TestFilePath(@"Scew\scewed-phototest.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixPath))
            {
                Scew scew;
                using (var descewedImage = sourcePix.Deskew(new ScewSweep(range: 45), Pix.DefaultBinarySearchReduction,
                           Pix.DefaultBinaryThreshold, out scew))
                {
                    Assert.AreEqual(scew.Angle, -9.953125F, 0.00001);
                    Assert.AreEqual(scew.Confidence, 3.782913F, 0.00001);


                    SaveResult(descewedImage, "descewedImage.png");
                }
            }
        }

        [TestMethod]
        public void OtsuBinarizationTest()
        {
            var sourcePixFilename = TestFilePath(@"Binarization\neo-8bit.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
            {
                using (var binarizedImage = sourcePix.BinarizeOtsuAdaptiveThreshold(200, 200, 10, 10, 0.1F))
                {
                    Assert.IsNotNull(binarizedImage);
                    Assert.AreNotEqual(binarizedImage.Handle, IntPtr.Zero);
                    SaveResult(binarizedImage, "binarizedOtsuImage.png");
                }
            }
        }

        [TestMethod]
        public void SauvolaBinarizationTest()
        {
            var sourcePixFilename = TestFilePath(@"Binarization\neo-8bit-grayscale.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
            {
                using (var grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1))
                {
                    using (var binarizedImage = grayscalePix.BinarizeSauvola(10, 0.35f, false))
                    {
                        Assert.IsNotNull(binarizedImage);
                        Assert.AreNotEqual(binarizedImage.Handle, IntPtr.Zero);
                        SaveResult(binarizedImage, "binarizedSauvolaImage.png");
                    }
                }
            }
        }

        [TestMethod]
        public void SauvolaTiledBinarizationTest()
        {
            var sourcePixFilename = TestFilePath(@"Binarization\neo-8bit-grayscale.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
            {
                using (var grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1))
                {
                    using (var binarizedImage = grayscalePix.BinarizeSauvolaTiled(10, 0.35f, 2, 2))
                    {
                        Assert.IsNotNull(binarizedImage);
                        Assert.AreNotEqual(binarizedImage.Handle, IntPtr.Zero);
                        SaveResult(binarizedImage, "binarizedSauvolaTiledImage.png");
                    }
                }
            }
        }

        [TestMethod]
        public void ConvertRGBToGrayTest()
        {
            var sourcePixFilename = TestFilePath(@"Conversion\photo_rgb_32bpp.tif");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
            using (var grayscaleImage = sourcePix.ConvertRGBToGray())
            {
                Assert.AreEqual(grayscaleImage.Depth, 8);
                SaveResult(grayscaleImage, "grayscaleImage.jpg");
            }
        }

        [DataTestMethod]
        [DataRow(45)]
        [DataRow(80)]
        [DataRow(90)]
        [DataRow(180)]
        [DataRow(270)]
        public void Rotate_ShouldBeAbleToRotateImageByXDegrees(float angle)
        {
            const string FileNameFormat = "rotation_{0}degrees.jpg";
            var angleAsRadians = MathHelper.ToRadians(angle);

            var sourcePixFilename = TestFilePath(@"Conversion\photo_rgb_32bpp.tif");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
            {
                using (var result = sourcePix.Rotate(angleAsRadians))
                {
                    // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
                    var filename = string.Format(FileNameFormat, angle);
                    SaveResult(result, filename);
                }
            }
        }

        [TestMethod]
        public void RemoveLinesTest()
        {
            var sourcePixFilename = TestFilePath(@"processing\table.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
            {
                // remove horizontal lines
                using (var result = sourcePix.RemoveLines())
                {
                    // rotate 90 degrees cw
                    using (var result1 = result.Rotate90(1))
                    {
                        // effectively remove vertical lines
                        using (var result2 = result1.RemoveLines())
                        {
                            // rotate 90 degrees ccw
                            using (var result3 = result2.Rotate90(-1))
                            {
                                // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
                                SaveResult(result3, "tableBordersRemoved.png");
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void DespeckleTest()
        {
            var sourcePixFilename = TestFilePath(@"processing\w91frag.jpg");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
            {
                // remove speckles
                using (var result = sourcePix.Despeckle(Pix.SEL_STR2, 2))
                {
                    // TODO: Visualy confirm successful despeckle and then setup an assertion to compare that result is the same.
                    SaveResult(result, "w91frag-despeckled.png");
                }
            }
        }

        [TestMethod]
        public void Scale_RGB_ShouldBeScaledBySpecifiedFactor(
        )
        {
            const string FileNameFormat = "scale_{0}.jpg";

            var sourcePixFilename = TestFilePath(@"Conversion\photo_rgb_32bpp.tif");

            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
            {
                foreach (var scale in new[] { 0.25f, 0.5f, 0.75f, 1, 1.25f, 1.5f, 1.75f, 2, 4, 8 })
                    using (var result = sourcePix.Scale(scale, scale))
                    {
                        Assert.AreEqual(result.Width, (int)Math.Round(sourcePix.Width * scale));
                        Assert.AreEqual(result.Height, (int)Math.Round(sourcePix.Height * scale));

                        // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
                        var filename = string.Format(FileNameFormat, scale);
                        SaveResult(result, filename);
                    }
            }
        }

        private static void SaveResult(Pix result, string filename)
        {
            var runFilename = TestResultRunFile(Path.Combine(ResultsDirectory, filename));
            result.Save(runFilename);
        }
    }
}