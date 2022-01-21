using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR.Pix;

namespace Tesseract.Tests.Leptonica
{
    [TestClass]
    public class ColorTests
    {
        [TestMethod]
        public void Color_CastColorToNetColor()
        {
            var color = new TesseractOCR.Pix.Color(100, 150, 200);
            var castColor = (System.Drawing.Color)color;
            Assert.AreEqual(castColor.R, color.Red);
            Assert.AreEqual(castColor.G, color.Green);
            Assert.AreEqual(castColor.B, color.Blue);
            Assert.AreEqual(castColor.A, color.Alpha);
        }

        [TestMethod]
        public void Color_ConvertColorToNetColor()
        {
            var color = new TesseractOCR.Pix.Color(100, 150, 200);
            var castColor = color.ToColor();
            Assert.AreEqual(castColor.R, color.Red);
            Assert.AreEqual(castColor.G, color.Green);
            Assert.AreEqual(castColor.B, color.Blue);
            Assert.AreEqual(castColor.A, color.Alpha);
        }
    }
}