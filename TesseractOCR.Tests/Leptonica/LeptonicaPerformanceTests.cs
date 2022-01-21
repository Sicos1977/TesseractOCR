using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tesseract.Tests
{
    [TestClass]
    [Ignore("Performance tests are disabled by default, there's probably a better way of doing this but for now it's ok")]
    public class LeptonicaPerformanceTests
    {
        [TestMethod]
        public void ConvertToBitmap()
        {
            const double baseRunTime = 793.382;
            const int runs = 1000;

            var sourceFilePath = Path.Combine("./Data/Conversion", "photo_palette_8bpp.tif");
            using (var bmp = new Bitmap(sourceFilePath))
            {
                // Don't include the first conversion since it will also handle loading the library etc (upfront costs).
                using (TesseractOCR.Pix.Converter.ToPix(bmp)) { }

                // copy 100 times take the average
                var watch = new Stopwatch();
                watch.Start();
                for (var i = 0; i < runs; i++)
                    using (TesseractOCR.Pix.Converter.ToPix(bmp)) { }

                watch.Stop();

                var delta = watch.ElapsedTicks / (baseRunTime * runs);
                Console.WriteLine("Delta: {0}", delta);
                Console.WriteLine("Elapsed Ticks: {0}", watch.ElapsedTicks);
                Console.WriteLine("Elapsed Time: {0}ms", watch.ElapsedMilliseconds);
                Console.WriteLine("Average Time: {0}ms", (double)watch.ElapsedMilliseconds / runs);

                Assert.AreEqual(delta, 1.0, 0.25);
            }
        }
    }
}