using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;

namespace Tesseract.Tests.Leptonica
{
    [TestClass]
    public class BitmapHelperTests
    {
        [TestMethod]
        public void ConvertRgb555ToPixColor()
        {
            ushort originalVal = 0x39EC;
            var convertedValue = BitmapHelper.ConvertRgb555ToRGBA(originalVal);
            //Assert.AreEqual(0x737B63FF, convertedValue);
            Assert.IsTrue(0x737B63FF == convertedValue);
        }

        //[DataTestMethod]
        //[DataRow(0xB9EC, 0x737B63FF)]
        //[DataRow(0xC6C60000, 0x737B6300)]
        //public void ConvertArgb555ToPixColor(int expectedVal, int originalVal)
        //{
        //    var convertedValue = BitmapHelper.ConvertArgb1555ToRGBA((ushort)originalVal);
        //    Assert.AreEqual((uint)expectedVal, convertedValue);
        //}

        [TestMethod]
        public void ConvertRgb565ToPixColor()
        {
            ushort originalVal = 0x73CC;
            var convertedValue = BitmapHelper.ConvertRgb565ToRGBA(originalVal);
            //Assert.AreEqual(0x737963FF, convertedValue);
            Assert.IsTrue(0x737963FF == convertedValue);
        }
    }
}