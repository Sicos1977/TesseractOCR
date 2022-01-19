using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;
using TesseractOCR.Enums;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Tesseract.Tests
{
    [TestClass]
    public class AnalyseResultTests : TesseractTestBase
    {
        #region Fields
        private static string ResultsDirectory => TestResultPath(@"Analysis\");
        private const string ExampleImagePath = @"Ocr\phototest.tif";
        #endregion

        #region Setup\TearDown
        private TesseractEngine _engine;

        [TestCleanup]
        public void Dispose()
        {
            _engine?.Dispose();
            _engine = null;
        }

        [TestInitialize]
        public void Init()
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);
            _engine = CreateEngine("osd");
        }
        #endregion Setup\TearDown

        #region Tests
        [DataTestMethod]
        [DataRow(null)]
        [DataRow(90f)]
        [DataRow(180f)]
        public void AnalyseLayout_RotatedImage(float? angle)
        {
            using (var img = LoadTestImage(ExampleImagePath))
            {
                using (var rotatedImage = angle.HasValue ? img.Rotate(MathHelper.ToRadians(angle.Value)) : img.Clone())
                {
                    rotatedImage.Save(TestResultRunFile($@"AnalyseResult\AnalyseLayout_RotateImage_{angle}.png"));

                    _engine.DefaultPageSegMode = PageSegMode.AutoOsd;
                    using (var page = _engine.Process(rotatedImage))
                    {
                        using (var pageLayout = page.GetIterator())
                        {
                            pageLayout.Begin();
                            do
                            {
                                var result = pageLayout.GetProperties();

                                ExpectedOrientation(angle ?? 0, out var orient);
                                Assert.AreEqual(result.Orientation, orient);

                                if (angle.HasValue)
                                {
                                    switch (angle)
                                    {
                                        case 180f:
                                            // This isn't correct...
                                            Assert.AreEqual(result.WritingDirection, WritingDirection.LeftToRight);
                                            Assert.AreEqual(result.TextLineOrder, TextLineOrder.TopToBottom);
                                            break;

                                        case 90f:
                                            Assert.AreEqual(result.WritingDirection, WritingDirection.LeftToRight);
                                            Assert.AreEqual(result.TextLineOrder, TextLineOrder.TopToBottom);
                                            break;

                                        default:
                                            Assert.Fail("Angle not supported.");
                                            break;
                                    }
                                }
                                else
                                {
                                    Assert.AreEqual(result.WritingDirection, WritingDirection.LeftToRight);
                                    Assert.AreEqual(result.TextLineOrder, TextLineOrder.TopToBottom);
                                }
                            } while (pageLayout.Next(PageIteratorLevel.Block));
                        }
                    }
                }
            }
        }

        //[TestMethod]
        //public void CanDetectOrientationForMode(
        //	//[Values(PageSegMode.Auto,
        //	//    PageSegMode.AutoOnly,
        //	//    PageSegMode.AutoOsd,
        //	//    PageSegMode.CircleWord,
        //	//    PageSegMode.OsdOnly,
        //	//    PageSegMode.SingleBlock,
        //	//    PageSegMode.SingleBlockVertText,
        //	//    PageSegMode.SingleChar,
        //	//    PageSegMode.SingleColumn,
        //	//    PageSegMode.SingleLine,
        //	//    PageSegMode.SingleWord)]
        //	//PageSegMode pageSegMode
        //	)
        //{
        //	foreach (var pageSegMode in new[] {
        //	PageSegMode.Auto,
        //		PageSegMode.AutoOnly,
        //		PageSegMode.AutoOsd,
        //		PageSegMode.CircleWord,
        //		PageSegMode.OsdOnly,
        //		PageSegMode.SingleBlock,
        //		PageSegMode.SingleBlockVertText,
        //		PageSegMode.SingleChar,
        //		PageSegMode.SingleColumn,
        //		PageSegMode.SingleLine,
        //		PageSegMode.SingleWord
        //	})
        //		using (var img = LoadTestImage(ExampleImagePath))
        //		{
        //			using (var rotatedPix = img.Rotate((float)Math.PI))
        //			{
        //				using (var page = engine.Process(rotatedPix,pageSegMode))
        //				{
        //					int orientation;
        //					float confidence;
        //					string scriptName;
        //					float scriptConfidence;

        //					page.DetectBestOrientationAndScript(out orientation,out confidence,out scriptName,out scriptConfidence);

        //					Assert.AreEqual(orientation,180);
        //					Assert.AreEqual(scriptName,"Latin");
        //				}
        //			}
        //		}
        //}

        //[DataTestMethod]
        //[DataRow(0)]
        //[DataRow(90)]
        //[DataRow(180)]
        //[DataRow(270)]
        //public void DetectOrientation_Degrees_RotatedImage(int expectedOrientation)
        //{
        //	using (var img = LoadTestImage(ExampleImagePath))
        //	{
        //		using (var rotatedPix = img.Rotate((float)expectedOrientation / 360 * (float)Math.PI * 2))
        //		{
        //			using (var page = engine.Process(rotatedPix,PageSegMode.OsdOnly))
        //			{

        //				int orientation;
        //				float confidence;
        //				string scriptName;
        //				float scriptConfidence;

        //				page.DetectBestOrientationAndScript(out orientation,out confidence,out scriptName,out scriptConfidence);

        //				Assert.AreEqual(orientation,expectedOrientation);
        //				Assert.AreEqual(scriptName,"Latin");
        //			}
        //		}
        //	}
        //}

        //[DataTestMethod]
        //[DataRow(0)]
        //[DataRow(90)]
        //[DataRow(180)]
        //[DataRow(270)]
        //public void DetectOrientation_Legacy_RotatedImage(int expectedOrientationDegrees)
        //{
        //	using (var img = LoadTestImage(ExampleImagePath))
        //	{
        //		using (var rotatedPix = img.Rotate((float)expectedOrientationDegrees / 360 * (float)Math.PI * 2))
        //		{
        //			using (var page = engine.Process(rotatedPix,PageSegMode.OsdOnly))
        //			{
        //				Orientation orientation;
        //				float confidence;

        //				page.DetectBestOrientation(out orientation,out confidence);

        //				Orientation expectedOrientation;
        //				float expectedDeskew;
        //				ExpectedOrientation(expectedOrientationDegrees,out expectedOrientation,out expectedDeskew);

        //				Assert.AreEqual(orientation,expectedOrientation);
        //			}
        //		}
        //	}
        //}


        [TestMethod]
        public void GetImage(
            //[Values(PageIteratorLevel.Block, PageIteratorLevel.Para, PageIteratorLevel.TextLine, PageIteratorLevel.Word, PageIteratorLevel.Symbol)] PageIteratorLevel level,
            //[Values(0, 3)] int padding
        )
        {
            foreach (var level in new[]
                     {
                         PageIteratorLevel.Block, PageIteratorLevel.Para, PageIteratorLevel.TextLine,
                         PageIteratorLevel.Word,
                         PageIteratorLevel.Symbol
                     })
            foreach (var padding in new[] { 0, 3 })

                using (var img = LoadTestImage(ExampleImagePath))
                {
                    using (var page = _engine.Process(img))
                    {
                        using (var pageLayout = page.GetIterator())
                        {
                            pageLayout.Begin();
                            using (var elementImg = pageLayout.GetImage(level, padding, out var x, out var y))
                            {
                                var elementImgFilename =
                                    $@"AnalyseResult\GetImage\ResultIterator_Image_{level}_{padding}_at_({x},{y}).png";

                                // TODO: Ensure generated pix is equal to expected pix, only saving it if it's not.
                                var destFilename = TestResultRunFile(elementImgFilename);
                                elementImg.Save(destFilename, ImageFormat.Png);
                            }
                        }
                    }
                }
        }
        #endregion Tests

        #region Helpers
        private static void ExpectedOrientation(float rotation, out Orientation orientation)
        {
            rotation %= 360f;
            rotation = rotation < 0 ? rotation + 360 : rotation;

            if (rotation >= 315 || rotation < 45)
                orientation = Orientation.PageUp;
            else if (rotation >= 45 && rotation < 135)
                orientation = Orientation.PageRight;
            else if (rotation >= 135 && rotation < 225)
                orientation = Orientation.PageDown;
            else if (rotation >= 225 && rotation < 315)
                orientation = Orientation.PageLeft;
            else
                throw new ArgumentOutOfRangeException(nameof(rotation));
        }

        private static Pix LoadTestImage(string path)
        {
            var fullExampleImagePath = TestFilePath(path);
            return Pix.LoadFromFile(fullExampleImagePath);
        }
        #endregion
    }
}