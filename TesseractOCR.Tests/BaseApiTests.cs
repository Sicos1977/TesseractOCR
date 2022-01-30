using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tesseract.Tests
{
    [TestClass]
    public class BaseApiTests : TesseractTestBase
    {
        [TestMethod]
        public void GetVersion_Is500()
        {
            using var engine = CreateEngine();
            var version = engine.Version;
            Assert.IsTrue(version.StartsWith("5.0.0"));
        }
    }
}