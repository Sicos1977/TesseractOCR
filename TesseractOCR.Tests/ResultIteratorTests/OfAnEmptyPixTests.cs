using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tesseract.Tests.ResultIteratorTests
{
	[TestClass]
	public class OfAnEmptyPixTests: TesseractTestBase
	{
		private TesseractEngine Engine { get; set; }
		private Pix EmptyPix { get; set; }

		[TestInitialize]
		public void Init()
		{
			Engine = CreateEngine();
			EmptyPix = LoadTestPix("Ocr\\blank.tif");
		}

		[TestCleanup]
		public void Dispose()
		{
			if (EmptyPix != null)
			{
				EmptyPix.Dispose();
				EmptyPix = null;
			}

			if (Engine != null)
			{
				Engine.Dispose();
				Engine = null;
			}
		}

		[DataTestMethod]
		[DataRow]
		public void GetTextReturnNullForEachLevel(PageIteratorLevel level)
		{
			using (var page = Engine.Process(EmptyPix))
			{
				using (var iter = page.GetIterator())
				{
					//Assert.That(iter.GetText(level), Is.Null);
					Assert.IsNull(iter.GetText(level));
				}
			}
		}

	}
}