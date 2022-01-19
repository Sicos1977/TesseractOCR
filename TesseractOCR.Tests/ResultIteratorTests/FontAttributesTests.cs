using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tesseract.Tests.ResultIteratorTests
{
	[TestClass]
	public class FontAttributesTests: TesseractTestBase
	{
		private TesseractEngine Engine { get; set; }
		private Pix TestImage { get; set; }

		[TestInitialize]
		public void Init()
		{
			Engine = CreateEngine();
			TestImage = LoadTestPix("Ocr\\Fonts.tif");
		}

		[TestCleanup]
		public void Dispose()
		{
			if (TestImage != null)
			{
				TestImage.Dispose();
				TestImage = null;
			}

			if (Engine != null)
			{
				Engine.Dispose();
				Engine = null;
			}
		}

		#region Tests
		[TestMethod]
		public void GetWordFontAttributesWorks()
		{
			using (var page = Engine.Process(TestImage))
			using (var iter = page.GetIterator())
			{
				// font attributes come in this order in the test image:
				// bold, italic, monospace, serif, smallcaps
				//
				// there is no test for underline because in 3.04 IsUnderlined is
				// hard-coded to "false".  See: https://github.com/tesseract-ocr/tesseract/blob/3.04/ccmain/ltrresultiterator.cpp#182

				var fontAttrs = iter.GetWordFontAttributes();
				Assert.IsTrue(fontAttrs.FontInfo.IsBold);
				Assert.AreEqual(iter.GetWordRecognitionLanguage(),"eng");
				//Assert.That(iter.GetWordIsFromDictionary());
				iter.Next(PageIteratorLevel.TextLine,PageIteratorLevel.Word);

				fontAttrs = iter.GetWordFontAttributes();
				Assert.IsTrue(fontAttrs.FontInfo.IsItalic);
				iter.Next(PageIteratorLevel.TextLine,PageIteratorLevel.Word);

				fontAttrs = iter.GetWordFontAttributes();
				Assert.IsTrue(fontAttrs.FontInfo.IsFixedPitch);
				iter.Next(PageIteratorLevel.TextLine,PageIteratorLevel.Word);

				fontAttrs = iter.GetWordFontAttributes();
				Assert.IsTrue(fontAttrs.FontInfo.IsSerif);
				iter.Next(PageIteratorLevel.TextLine,PageIteratorLevel.Word);

				fontAttrs = iter.GetWordFontAttributes();
				Assert.IsTrue(fontAttrs.IsSmallCaps);
				iter.Next(PageIteratorLevel.TextLine,PageIteratorLevel.Word);

				Assert.IsTrue(iter.GetWordIsNumeric());

				iter.Next(PageIteratorLevel.TextLine,PageIteratorLevel.Word);
				iter.Next(PageIteratorLevel.Word,PageIteratorLevel.Symbol);

				Assert.IsTrue(iter.GetSymbolIsSuperscript());

				iter.Next(PageIteratorLevel.TextLine,PageIteratorLevel.Word);
				iter.Next(PageIteratorLevel.Word,PageIteratorLevel.Symbol);

				Assert.IsTrue(iter.GetSymbolIsSubscript());
			}
		}
		#endregion Tests
	}
}
