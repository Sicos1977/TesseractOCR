using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tesseract.Tests;

[TestClass]
public class BaseApiTests : TesseractTestBase
{
    [TestMethod]
    public void GetVersion_Is554()
    {
        using var engine = CreateEngine();
        var version = engine.Version;
        Assert.IsTrue(version.StartsWith("5.5.4"));
    }

    [TestMethod]
    public void LoadedLanguages()
    {
        using var engine = CreateEngine();
        var dp = engine.DataPath;
        engine.ClearAdaptiveClassifier();
        engine.ClearPersistentCache();
        var languages = engine.AvailableLanguages;
        Assert.IsTrue(languages.Contains(TesseractOCR.Enums.Language.English));
        Assert.IsTrue(languages.Contains(TesseractOCR.Enums.Language.Osd));
    }

}