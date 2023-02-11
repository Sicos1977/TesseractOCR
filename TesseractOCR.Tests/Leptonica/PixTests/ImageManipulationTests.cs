using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;
using TesseractOCR.Enums;

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
            using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixPath);
            using var descewedImage = sourcePix.Deskew(new ScewSweep(range: 45), TesseractOCR.Pix.Image.DefaultBinarySearchReduction, TesseractOCR.Pix.Image.DefaultBinaryThreshold, out var scew);
            Assert.AreEqual(scew.Angle, -9.953125F, 0.00001);
            Assert.AreEqual(scew.Confidence, 3.782913F, 0.00001);


            SaveResult(descewedImage, "descewedImage.png");
        }

        [TestMethod]
        public void OtsuBinarizationTest()
        {
            var sourcePixFilename = TestFilePath(@"Binarization\neo-8bit.png");
            using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixFilename);
            using var binarizedImage = sourcePix.BinarizeOtsuAdaptiveThreshold(200, 200, 10, 10, 0.1F);
            Assert.IsNotNull(binarizedImage);
            SaveResult(binarizedImage, "binarizedOtsuImage.png");
        }

        [TestMethod]
        public void SauvolaBinarizationTest()
        {
            var sourcePixFilename = TestFilePath(@"Binarization\neo-8bit-grayscale.png");
            using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixFilename);
            using var grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1);
            using var binarizedImage = grayscalePix.BinarizeSauvola(10, 0.35f, false);
            Assert.IsNotNull(binarizedImage);
            SaveResult(binarizedImage, "binarizedSauvolaImage.png");
        }

        [TestMethod]
        public void SauvolaTiledBinarizationTest()
        {
            var sourcePixFilename = TestFilePath(@"Binarization\neo-8bit-grayscale.png");
            using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixFilename);
            using var grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1);
            using var binarizedImage = grayscalePix.BinarizeSauvolaTiled(10, 0.35f, 2, 2);
            Assert.IsNotNull(binarizedImage);
            SaveResult(binarizedImage, "binarizedSauvolaTiledImage.png");
        }

        //[TestMethod]
        //public void ConvertRgbToGrayTest()
        //{
        //    var sourcePixFilename = TestFilePath(@"Conversion\photo_rgb_32bpp.tif");
        //    using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixFilename);
        //    using var grayscaleImage = sourcePix.ConvertRGBToGray();
        //    Assert.AreEqual(grayscaleImage.Depth, 8);
        //    SaveResult(grayscaleImage, "grayscaleImage.jpg");
        //}

        //[DataTestMethod]
        //[DataRow(45)]
        //[DataRow(80)]
        //[DataRow(90)]
        //[DataRow(180)]
        //[DataRow(270)]
        //public void Rotate_ShouldBeAbleToRotateImageByXDegrees(float angle)
        //{
        //    const string fileNameFormat = "rotation_{0}degrees.jpg";
        //    var angleAsRadians = TesseractOCR.Helpers.Math.ToRadians(angle);

        //    var sourcePixFilename = TestFilePath(@"Conversion\photo_rgb_32bpp.tif");
        //    using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixFilename);
        //    using var result = sourcePix.Rotate(angleAsRadians);
        //    var filename = string.Format(fileNameFormat, angle);
        //    SaveResult(result, filename);
        //}

        [TestMethod]
        public void RemoveLinesTest()
        {
            var sourcePixFilename = TestFilePath(@"processing\table.png");
            using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixFilename);
            using var result = sourcePix.RemoveLines();
            using var result1 = result.Rotate90(RotationDirection.Clockwise);
            using var result2 = result1.RemoveLines();
            using var result3 = result2.Rotate90(RotationDirection.CounterClockwise);
            SaveResult(result3, "tableBordersRemoved.png");
        }

        [TestMethod]
        public void DespeckleTest()
        {
            var sourcePixFilename = TestFilePath(@"processing\w91frag.jpg");
            using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixFilename);
            using var result = sourcePix.Despeckle(TesseractOCR.Pix.Image.SEL_STR2, 2);
            SaveResult(result, "w91frag-despeckled.png");
        }

        //[TestMethod]
        //public void Scale_RGB_ShouldBeScaledBySpecifiedFactor(
        //)
        //{
        //    const string fileNameFormat = "scale_{0}.jpg";

        //    var sourcePixFilename = TestFilePath(@"Conversion\photo_rgb_32bpp.tif");

        //    using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixFilename);
        //    foreach (var scale in new[] { 0.25f, 0.5f, 0.75f, 1, 1.25f, 1.5f, 1.75f, 2, 4, 8 })
        //    {
        //        using var result = sourcePix.Scale(scale, scale);
        //        Assert.AreEqual(result.Width, (int)System.Math.Round(sourcePix.Width * scale));
        //        Assert.AreEqual(result.Height, (int)System.Math.Round(sourcePix.Height * scale));
        //        var filename = string.Format(fileNameFormat, scale);
        //        SaveResult(result, filename);
        //    }
        //}

        private static void SaveResult(TesseractOCR.Pix.Image result, string filename)
        {
            var runFilename = TestResultRunFile(Path.Combine(ResultsDirectory, filename));
            result.Save(runFilename);
        }
    }
}