using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;
using ImageFormat = TesseractOCR.Enums.ImageFormat;

namespace Tesseract.Tests.Leptonica
{
    public class ConvertBitmapToPixTests : TesseractTestBase
    {
        // Test for [Issue #166](https://github.com/charlesw/tesseract/issues/166)
        [TestMethod]
        public void Convert_ScaledBitmapToPix()
        {
            var sourceFilePath = TestFilePath("Conversion/photo_rgb_32bpp.tif");
            var bitmapConverter = new BitmapToPixConverter();
            using (var source = new Bitmap(sourceFilePath))
            {
                using (var scaledSource = new Bitmap(source, new Size(source.Width * 2, source.Height * 2)))
                {
                    Assert.AreEqual(scaledSource.GetBPP(), 32);
                    using (var dest = bitmapConverter.Convert(scaledSource))
                    {
                        dest.Save(TestResultRunFile("Conversion/ScaledBitmapToPix_rgb_32bpp.tif"), ImageFormat.Tiff);

                        AssertAreEquivalent(scaledSource, dest, true);
                    }
                }
            }
        }

        [DataTestMethod]
        [DataRow(PixelFormat
            .Format1bppIndexed)] // Note: 1bpp will not save pixmap when writing out the result, this is a limitation of leptonica (see pixWriteToTiffStream)
        //[DataRow(PixelFormat.Format4bppIndexed, Ignore = "4bpp images not supported.")]
        [DataRow(PixelFormat.Format8bppIndexed)]
        [DataRow(PixelFormat.Format32bppRgb)]
        [DataRow(PixelFormat.Format32bppArgb)]
        public void Convert_BitmapToPix(PixelFormat pixelFormat)
        {
            var depth = Image.GetPixelFormatSize(pixelFormat);
            string pixType;
            if (depth < 16) pixType = "palette";
            else if (depth == 16) pixType = "grayscale";
            else pixType = Image.IsAlphaPixelFormat(pixelFormat) ? "argb" : "rgb";

            var sourceFile = $"Conversion/photo_{pixType}_{depth}bpp.tif";
            var sourceFilePath = TestFilePath(sourceFile);
            var bitmapConverter = new BitmapToPixConverter();
            using (var source = new Bitmap(sourceFilePath))
            {
                Assert.AreEqual(source.PixelFormat, pixelFormat);
                Assert.AreEqual(source.GetBPP(), depth);
                using (var dest = bitmapConverter.Convert(source))
                {
                    var destFilename = $"Conversion/BitmapToPix_{pixType}_{depth}bpp.tif";
                    dest.Save(TestResultRunFile(destFilename), ImageFormat.Tiff);

                    AssertAreEquivalent(source, dest, true);
                }
            }
        }

        /// <summary>
        ///     Test case for https://github.com/charlesw/tesseract/issues/180
        /// </summary>
        [TestMethod]
        public void Convert_BitmapToPix_Format8bppIndexed()
        {
            var sourceFile = TestFilePath("Conversion/photo_palette_8bpp.png");
            var bitmapConverter = new BitmapToPixConverter();
            using (var source = new Bitmap(sourceFile))
            {
                Assert.AreEqual(source.GetBPP(), 8);
                Assert.AreEqual(source.PixelFormat, PixelFormat.Format8bppIndexed);
                using (var dest = bitmapConverter.Convert(source))
                {
                    var destFilename = TestResultRunFile("Conversion/BitmapToPix_palette_8bpp.png");
                    dest.Save(destFilename, ImageFormat.Png);

                    AssertAreEquivalent(source, dest, true);
                }
            }
        }

        [DataTestMethod]
        [DataRow(1, true, false)]
        //[DataRow(1, false, false, Ignore = "Images that contain a palette get converted to 32bit")]
        //[DataRow(4, false, false, Ignore = "4bpp images not supported.")]
        //[DataRow(4, true, false, Ignore = "4bpp images not supported.")]
        [DataRow(8, false, false)]
        //[DataRow(8, true, false, Ignore = "Haven't yet created a 8bpp grayscale test image.")]
        [DataRow(32, false, true)]
        [DataRow(32, false, false)]
        public void Convert_PixToBitmap(int depth, bool isGrayscale, bool includeAlpha)
        {
            var hasPalette = depth < 16 && !isGrayscale;
            string pixType;
            if (isGrayscale) pixType = "grayscale";
            else if (hasPalette) pixType = "palette";
            else pixType = "rgb";

            var sourceFile = TestFilePath($"Conversion/photo_{pixType}_{depth}bpp.tif");
            var converter = new PixToBitmapConverter();
            using (var source = Pix.LoadFromFile(sourceFile))
            {
                Assert.AreEqual(source.Depth, depth);
                if (hasPalette)
                    Assert.IsNotNull(source.Colormap, "Expected source image to have color map\\palette.");
                else
                    Assert.IsNull(source.Colormap, "Expected source image to be grayscale.");
                using (var dest = converter.Convert(source, includeAlpha))
                {
                    var destFilename =
                        TestResultRunFile($"Conversion/PixToBitmap_{pixType}_{depth}bpp.tif");
                    dest.Save(destFilename, System.Drawing.Imaging.ImageFormat.Tiff);

                    AssertAreEquivalent(dest, source, includeAlpha);
                }
            }
        }

        private void AssertAreEquivalent(Bitmap bmp, Pix pix, bool checkAlpha)
        {
            // verify img metadata
            Assert.AreEqual(pix.Width, bmp.Width);
            Assert.AreEqual(pix.Height, bmp.Height);
            //Assert.That(pix.Resolution.X, bmp.HorizontalResolution));
            //Assert.That(pix.Resolution.Y, bmp.VerticalResolution));

            // do some random sampling over image
            var height = pix.Height;
            var width = pix.Width;
            for (var y = 0; y < height; y += height)
            for (var x = 0; x < width; x += width)
            {
                var sourcePixel = bmp.GetPixel(x, y).ToPixColor();
                var destPixel = GetPixel(pix, x, y);
                if (checkAlpha)
                    Assert.AreEqual(destPixel, sourcePixel,
                        "Expected pixel at <{0},{1}> to be same in both source and dest.", x, y);
            }
        }

        private static unsafe PixColor GetPixel(Pix pix, int x, int y)
        {
            var pixDepth = pix.Depth;
            var pixData = pix.GetData();
            var pixLine = (uint*)pixData.Data + pixData.WordsPerLine * y;
            uint pixValue;
            if (pixDepth == 1)
                pixValue = PixData.GetDataBit(pixLine, x);
            else if (pixDepth == 4)
                pixValue = PixData.GetDataQBit(pixLine, x);
            else if (pixDepth == 8)
                pixValue = PixData.GetDataByte(pixLine, x);
            else if (pixDepth == 32)
                pixValue = PixData.GetDataFourByte(pixLine, x);
            else
                throw new ArgumentException($"Bit depth of {pix.Depth} is not supported.",
                    nameof(pix));

            if (pix.Colormap != null) return pix.Colormap[(int)pixValue];

            if (pixDepth == 32)
            {
                return PixColor.FromRgba(pixValue);
            }

            var grayscale = (byte)(pixValue * 255 / ((1 << 16) - 1));
            return new PixColor(grayscale, grayscale, grayscale);
        }
    }
}