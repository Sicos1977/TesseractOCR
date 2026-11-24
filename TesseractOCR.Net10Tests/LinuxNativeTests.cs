using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

namespace Tesseract.Tests;

[TestClass]
[OSCondition(OperatingSystems.Linux)]
public class LinuxNativeTests
{
    [TestMethod]
    [DataRow("PSM_SingleLine.png", PageSegMode.SingleLine)]
    [DataRow("phototest.tif", PageSegMode.Auto)]
    public void BundledLibrariesRecognizeImages(string fileName, PageSegMode mode)
    {
        var dataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
        var imagePath = Path.Combine(AppContext.BaseDirectory, "Data", "Ocr", fileName);
        using var engine = new Engine(dataPath, Language.English);
        Assert.AreEqual("5.5.4", engine.Version);
        using var image = Image.LoadFromFile(imagePath);
        using var page = engine.Process(image, mode);
        StringAssert.Contains(page.Text, "This is a lot of 12 point text to test the");
    }
}
