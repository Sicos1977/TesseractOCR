using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;
using TesseractOCR.Pix;

namespace Tesseract.Tests.Leptonica;

[TestClass]
public unsafe class PreprocessingTests
{
    private static void SetGray(Image image, int x, int y, byte value)
    {
        var data = image.GetData();
        Data.SetDataByte((uint*)data.PixData + y * data.WordsPerLine, x, value);
    }

    private static uint Gray(Image image, int x, int y)
    {
        var data = image.GetData();
        return Data.GetDataByte((uint*)data.PixData + y * data.WordsPerLine, x);
    }

    private static uint Bit(Image image, int x, int y)
    {
        var data = image.GetData();
        return Data.GetDataBit((uint*)data.PixData + y * data.WordsPerLine, x);
    }

    [TestMethod]
    public void ThresholdAndInvertKeepSourceAndUseExpectedPixelValues()
    {
        using var source = Image.Create(3, 1, 8);
        SetGray(source, 0, 0, 0);
        SetGray(source, 1, 0, 128);
        SetGray(source, 2, 0, 255);
        using var binary = source.Binarize(128);
        using var inverted = source.Invert();
        Assert.AreEqual(1, binary.Depth);
        Assert.AreEqual(1u, Bit(binary, 0, 0));
        Assert.AreEqual(0u, Bit(binary, 1, 0));
        Assert.AreEqual(255u, Gray(inverted, 0, 0));
        Assert.AreEqual(127u, Gray(inverted, 1, 0));
        Assert.AreEqual(0u, Gray(inverted, 2, 0));
        Assert.AreEqual(128u, Gray(source, 1, 0));
    }

    [TestMethod]
    public void CropAndBordersPreservePixelsAndDimensions()
    {
        using var source = Image.Create(4, 4, 8);
        SetGray(source, 1, 1, 72);
        using var crop = source.Crop(new Rect(1, 1, 2, 2));
        using var bordered = crop.AddBorder(1);
        using var restored = bordered.RemoveBorder(1);
        Assert.AreEqual(2, crop.Width);
        Assert.AreEqual(72u, Gray(crop, 0, 0));
        Assert.AreEqual(4, bordered.Width);
        Assert.AreEqual(255u, Gray(bordered, 0, 0));
        Assert.AreEqual(72u, Gray(bordered, 1, 1));
        Assert.AreEqual(2, restored.Width);
        Assert.AreEqual(72u, Gray(restored, 0, 0));
        Assert.AreEqual(72u, Gray(source, 1, 1));
    }

    [TestMethod]
    public void BinaryMorphologyModifiesForegroundAsExpected()
    {
        using var source = Image.Create(9, 9, 1);
        var data = source.GetData();
        Data.SetDataBit((uint*)data.PixData + 4 * data.WordsPerLine, 4, 1);
        using var expanded = source.Dilate(3, 3);
        using var eroded = expanded.Erode(3, 3);
        using var opened = source.Open(3, 3);
        using var closed = source.Close(3, 3);
        Assert.AreEqual(1u, Bit(expanded, 3, 3));
        Assert.AreEqual(1u, Bit(eroded, 4, 4));
        Assert.AreEqual(0u, Bit(eroded, 3, 3));
        Assert.AreEqual(0u, Bit(opened, 4, 4));
        Assert.AreEqual(1u, Bit(closed, 4, 4));
        Assert.AreEqual(1u, Bit(source, 4, 4));
    }

    [TestMethod]
    public void FiltersAndConversionReturnUsableImages()
    {
        using var source = Image.Create(120, 120, 8);
        for (var y = 0; y < 120; y++)
        for (var x = 0; x < 120; x++) SetGray(source, x, y, (byte)(100 + x));
        SetGray(source, 60, 60, 0);
        using var filtered = source.MedianFilter(3, 3);
        using var normalized = source.NormalizeBackground();
        using var flexible = source.NormalizeBackgroundFlexible();
        using var contrasted = source.NormalizeContrast(30, 30, 20);
        using var gray = source.ConvertToGray();
        Assert.AreEqual(160u, Gray(filtered, 60, 60));
        Assert.AreEqual(8, normalized.Depth);
        Assert.AreEqual(8, flexible.Depth);
        Assert.AreEqual(120, contrasted.Width);
        Assert.AreEqual(0u, Gray(gray, 60, 60));
        Assert.AreEqual(0u, Gray(source, 60, 60));
    }

    [TestMethod]
    public void RemoveAlphaOnRgbImageReturnsImage()
    {
        using var source = Image.Create(2, 2, 32);
        using var output = source.RemoveAlpha();
        Assert.AreEqual(32, output.Depth);
        Assert.AreEqual(2, output.Width);
    }

    [TestMethod]
    public void InvalidArgumentsAndDisposedInputsAreRejected()
    {
        using var gray = Image.Create(10, 10, 8);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => gray.Binarize(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => gray.Binarize(257));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => gray.Crop(new Rect(9, 0, 2, 1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => gray.RemoveBorder(5));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => gray.AddBorder(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => gray.MedianFilter(0, 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => gray.NormalizeBackgroundFlexible(delta: -1));
        Assert.ThrowsExactly<InvalidOperationException>(() => gray.Dilate(3, 3));
        gray.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(() => gray.Invert());
    }
}
