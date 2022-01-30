using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tesseract.Tests.Leptonica.PixTests
{
    [TestClass]
    public unsafe class DataAccessTests
    {
        private const int Width = 59, Height = 53;


        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(4)]
        [DataRow(8)]
        [DataRow(16)]
        [DataRow(32)]
        public void CanReadAndWriteData(int depth)
        {
            using var pix = TesseractOCR.Pix.Image.Create(Width, Height, depth);
            var data = pix.GetData();

            for (var y = 0; y < Height; y++)
            {
                var line = (uint*)data.PixData + y * data.WordsPerLine;
                for (var x = 0; x < Width; x++)
                {
                    var val = (uint)((y * Width + x) % (1 << depth));
                    uint readVal;
                    if (depth == 1)
                    {
                        TesseractOCR.Pix.Data.SetDataBit(line, x, val);
                        readVal = TesseractOCR.Pix.Data.GetDataBit(line, x);
                    }
                    else if (depth == 2)
                    {
                        TesseractOCR.Pix.Data.SetDataDIBit(line, x, val);
                        readVal = TesseractOCR.Pix.Data.GetDataDIBit(line, x);
                    }
                    else if (depth == 4)
                    {
                        TesseractOCR.Pix.Data.SetDataQBit(line, x, val);
                        readVal = TesseractOCR.Pix.Data.GetDataQBit(line, x);
                    }
                    else if (depth == 8)
                    {
                        TesseractOCR.Pix.Data.SetDataByte(line, x, val);
                        readVal = TesseractOCR.Pix.Data.GetDataByte(line, x);
                    }
                    else if (depth == 16)
                    {
                        TesseractOCR.Pix.Data.SetDataTwoByte(line, x, val);
                        readVal = TesseractOCR.Pix.Data.GetDataTwoByte(line, x);
                    }
                    else if (depth == 32)
                    {
                        TesseractOCR.Pix.Data.SetDataFourByte(line, x, val);
                        readVal = TesseractOCR.Pix.Data.GetDataFourByte(line, x);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    Assert.AreEqual(readVal, val);
                }
            }
        }
    }
}