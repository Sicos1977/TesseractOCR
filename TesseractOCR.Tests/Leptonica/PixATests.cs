using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tesseract.Tests.Leptonica.PixTests
{
    [TestClass]
    public class PixATests : TesseractTestBase
    {
        [TestMethod]
        public void CanCreatePixArray()
        {
            using var pixA = TesseractOCR.Pix.Array.Create(0);
            Assert.AreEqual(pixA.Count, 0);
        }

        [TestMethod]
        public void CanAddPixToPixArray()
        {
            var sourcePixPath = TestFilePath(@"Ocr\phototest.tif");
            using var pixA = TesseractOCR.Pix.Array.Create(0);
            using var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixPath);
            pixA.Add(sourcePix);
            Assert.AreEqual(pixA.Count, 1);
            using var targetPix = pixA.GetPix(0);
            Assert.AreEqual(targetPix, sourcePix);
        }

        [TestMethod]
        public void CanRemovePixFromArray()
        {
            var sourcePixPath = TestFilePath(@"Ocr\phototest.tif");
            using var pixA = TesseractOCR.Pix.Array.Create(0);
            using (var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixPath))
            {
                pixA.Add(sourcePix);
            }

            pixA.Remove(0);
            Assert.AreEqual(pixA.Count, 0);
        }

        [TestMethod]
        public void CanClearPixArray()
        {
            var sourcePixPath = TestFilePath(@"Ocr\phototest.tif");
            using var pixA = TesseractOCR.Pix.Array.Create(0);
            using (var sourcePix = TesseractOCR.Pix.Image.LoadFromFile(sourcePixPath))
            {
                pixA.Add(sourcePix);
            }

            pixA.Clear();

            Assert.AreEqual(pixA.Count, 0);
        }
    }
}