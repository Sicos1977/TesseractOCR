using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Exceptions;

// ReSharper disable UnusedMember.Global

namespace Tesseract.Tests
{
    [TestClass]
    public class EngineTests : TesseractTestBase
    {
        private const string TestImagePath = "Ocr/phototest.tif";
        private const string TestImageFileColumn = "Ocr/ocr-five-column.jpg";

        [TestMethod]
        public void CanParseMultiPageTif()
        {
            using var engine = CreateEngine();
            using var pixA = TesseractOCR.Pix.Array.LoadMultiPageTiffFromFile(TestFilePath("./processing/multi-page.tif"));
            var i = 1;
            foreach (var pix in pixA)
            {
                using (var page = engine.Process(pix))
                {
                    var text = page.Text.Trim();

                    var expectedText = $"Page {i}";
                    Assert.AreEqual(text, expectedText);
                }

                i++;
            }
        }

        [DataTestMethod]
        [DataRow(PageSegMode.SingleBlock, "This is a lot of 12 point text to test the\n" +
                                          "ocr code and see if it works on all types\n" +
                                          "of file format.")]
        [DataRow(PageSegMode.SingleColumn, "This is a lot of 12 point text to test the")]
        [DataRow(PageSegMode.SingleLine, "This is a lot of 12 point text to test the")]
        [DataRow(PageSegMode.SingleWord, "This")]
        [DataRow(PageSegMode.SingleChar, "T")]
        //[DataRow(PageSegMode.SingleBlockVertText, "A line of text", Ignore = "Vertical data missing")]
        public void CanParseText_UsingMode(PageSegMode mode, string expectedText)
        {
            using var engine = CreateEngine(mode: EngineMode.TesseractAndLstm);
            var demoFilename = $"./Ocr/PSM_{mode}.png";
            using var pix = LoadTestPix(demoFilename);
            using var page = engine.Process(pix, mode);
            var text = page.Text.Trim();
            Assert.AreEqual(text, expectedText);
        }

        [TestMethod]
        public void CanParseText()
        {
            using var engine = CreateEngine();
            using var img = LoadTestPix(TestImagePath);
            using var page = engine.Process(img);
            var text = page.Text;

            const string expectedText =
                "This is a lot of 12 point text to test the\n" +
                "ocr code and see if it works on all types\n" +
                "of file format.\n\n" +
                "The quick brown dog jumped over the\n" +
                "lazy fox. The quick brown dog jumped\n" +
                "over the lazy fox. The quick brown dog\n" +
                "jumped over the lazy fox. The quick\n" +
                "brown dog jumped over the lazy fox.\n";

            Assert.AreEqual(text, expectedText);
        }

        [TestMethod]
        public void CanParseUznFile()
        {
            using var engine = CreateEngine();
            var inputFilename = TestFilePath(@"Ocr\uzn-test.png");
            using var img = TesseractOCR.Pix.Image.LoadFromFile(inputFilename);
            using var page = engine.Process(img, PageSegMode.AutoOnly);
            var text = page.Text;

            const string expectedText =
                "This is another test\n";

            Assert.IsTrue(text.Contains(expectedText));
        }


        [TestMethod]
        public void CanProcessSpecifiedRegionInImage()
        {
            using var engine = CreateEngine(mode: EngineMode.LstmOnly);
            using var img = LoadTestPix(TestImagePath);
            // See other tests about this bug on coords 0,0
            using var page = engine.Process(img, Rect.FromCoords(1, 1, img.Width, 188));
            var region1Text = page.Text;

            const string expectedTextRegion1 =
                "This is a lot of 12 point text to test the\n" +
                "ocr code and see if it works on all types\n" +
                "of file format.\n";

            Assert.AreEqual(region1Text, expectedTextRegion1);
        }

        /// <summary>
        ///     Tesseract seems to have a bug processing a region from 0,0, but if you set it to 1,1 things work again. Not sure
        ///     why this is.
        /// </summary>
        [TestMethod]
        public void CanProcessDifferentRegionsInSameImage()
        {
            using var engine = CreateEngine();
            using var img = LoadTestPix(TestImagePath);
            using var page = engine.Process(img, Rect.FromCoords(1, 1, img.Width, 188));
            var region1Text = page.Text;

            const string expectedTextRegion1 = "This is a lot of 12 point text to test the\n" +
                                               "ocr code and see if it works on all types\n" +
                                               "of file format.\n";

            Assert.AreEqual(region1Text, expectedTextRegion1);

            page.RegionOfInterest = Rect.FromCoords(0, 188, img.Width, img.Height);

            var region2Text = page.Text;
            const string expectedTextRegion2 = "The quick brown dog jumped over the\n" +
                                               "lazy fox. The quick brown dog jumped\n" +
                                               "over the lazy fox. The quick brown dog\n" +
                                               "jumped over the lazy fox. The quick\n" +
                                               "brown dog jumped over the lazy fox.\n";

            Assert.AreEqual(region2Text, expectedTextRegion2);
        }

        [TestMethod]
        public void CanGetSegmentedRegions()
        {
            const int expectedCount = 8; // number of text lines in test image

            using var engine = CreateEngine();
            var imgPath = TestFilePath(TestImagePath);
            using var img = TesseractOCR.Pix.Image.LoadFromFile(imgPath);
            using var page = engine.Process(img);
            var boxes = page.GetSegmentedRegions(PageIteratorLevel.TextLine);

            for (var i = 0; i < boxes.Count; i++)
            {
                var box = boxes[i];
                Console.WriteLine("Box[{0}]: x={1}, y={2}, w={3}, h={4}", i, box.X, box.Y, box.Width,
                    box.Height);
            }

            Assert.AreEqual(boxes.Count, expectedCount);
        }

        [TestMethod]
        public void CanProcessEmptyPixUsingResultIterator()
        {
            var result = new StringBuilder();
            using var engine = CreateEngine();
            using var img = LoadTestPix("ocr/empty.png");
            using var page = engine.Process(img);

            foreach (var block in page.Layout)
            {
                result.AppendLine($"Block text: {block.Text}");
                result.AppendLine($"Block confidence: {block.Confidence}");

                foreach (var paragraph in block.Paragraphs)
                {
                    result.AppendLine($"Paragraph text: {paragraph.Text}");
                    result.AppendLine($"Paragraph confidence: {paragraph.Confidence}");

                    foreach (var textLine in paragraph.TextLines)
                    {
                        result.AppendLine($"Text line text: {textLine.Text}");
                        result.AppendLine($"Text line confidence: {textLine.Confidence}");

                        foreach (var word in textLine.Words)
                        {
                            result.AppendLine($"Word text: {word.Text}");
                            result.AppendLine($"Word confidence: {word.Confidence}");
                            result.AppendLine($"Word is from dictionary: {word.IsFromDictionary}");
                            result.AppendLine($"Word is numeric: {word.IsNumeric}");
                            result.AppendLine($"Word language: {word.Language}");

                            //foreach (var symbol in word.Symbols)
                            //{
                            //    Debug.Print($"Symbol text: {symbol.Text}");
                            //    Debug.Print($"Symbol confidence: {symbol.Confidence}");
                            //    Debug.Print($"Symbol is superscript: {symbol.IsSuperscript}");
                            //    Debug.Print($"Symbol is dropcap: {symbol.IsDropcap}");
                            //}
                        }
                    }
                }

            }

            Assert.AreEqual(result.ToString(), "");
        }

        [TestMethod]
        public void CanProcessMultiplePixs()
        {
            using var engine = CreateEngine();
            for (var i = 0; i < 3; i++)
            {
                using var img = LoadTestPix(TestImagePath);
                using var page = engine.Process(img);
                var text = page.Text;

                const string expectedText =
                    "This is a lot of 12 point text to test the\n" +
                    "ocr code and see if it works on all types\n" +
                    "of file format.\n\n" +
                    "The quick brown dog jumped over the" +
                    "\nlazy fox. The quick brown dog jumped\n" +
                    "over the lazy fox. The quick brown dog\n" +
                    "jumped over the lazy fox. The quick\n" +
                    "brown dog jumped over the lazy fox.\n";

                Assert.AreEqual(text, expectedText);
            }
        }

        [TestMethod]
        public void CanProcessPixUsingResultIterator()
        {
            const string resultPath = @"EngineTests\CanProcessPixUsingResultIterator.txt";

            string actualResult;
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImageFileColumn))
                {
                    using (var page = engine.Process(img))
                    {
                        foreach (var block in page.Layout)
                        {
                            Debug.Print($"Block text: {block.Text}");
                            Debug.Print($"Block confidence: {block.Confidence}");
                            var test0 = block.BlockType;

                            foreach (var paragraph in block.Paragraphs)
                            {
                                Debug.Print($"Paragraph text: {paragraph.Text}");
                                Debug.Print($"Paragraph confidence: {paragraph.Confidence}");
                                var test1 = block.BlockType;

                                foreach (var textLine in paragraph.TextLines)
                                {
                                    Debug.Print($"Text line text: {textLine.Text}");
                                    Debug.Print($"Text line confidence: {textLine.Confidence}");
                                    var tes2 = block.BlockType;

                                    foreach (var word in textLine.Words)
                                    {
                                        Debug.Print($"Word text: {word.Text}");
                                        Debug.Print($"Word confidence: {word.Confidence}");
                                        Debug.Print($"Word is from dictionary: {word.IsFromDictionary}");
                                        Debug.Print($"Word is numeric: {word.IsNumeric}");
                                        Debug.Print($"Word language: {word.Language}");
                                        var test3 = block.BlockType;

                                        //foreach (var symbol in word.Symbols)
                                        //{
                                        //    Debug.Print($"Symbol text: {symbol.Text}");
                                        //    Debug.Print($"Symbol confidence: {symbol.Confidence}");
                                        //    Debug.Print($"Symbol is superscript: {symbol.IsSuperscript}");
                                        //    Debug.Print($"Symbol is dropcap: {symbol.IsDropcap}");
                                        //}
                                    }
                                }
                            }
                        }

                        // TODO : Do some checking here

                        actualResult = "";
                    }
                }
            }

            var expectedResultPath = TestResultPath(resultPath);
            var expectedResult = NormalizeNewLine(File.ReadAllText(expectedResultPath));
            
            if (expectedResult == actualResult) return;

            var actualResultPath = TestResultRunFile(resultPath);
            
            Assert.Fail("Expected results to be \"{0}\" but was \"{1}\".", expectedResultPath, actualResultPath);
        }
        
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CanGenerateHOcrOutput(bool useXHtml)
        {
            string actualResult;
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        actualResult = NormalizeNewLine(page.HOcrText(useXHtml));
                    }
                }
            }

            var resultFilename = $"EngineTests/CanGenerateHOCROutput_{useXHtml}.txt";
            var expectedFilename = TestResultPath(resultFilename);

            if (File.Exists(expectedFilename))
            {
                var expectedResult = NormalizeNewLine(File.ReadAllText(expectedFilename));
                if (expectedResult != actualResult)
                {
                    var actualFilename = TestResultRunFile(resultFilename);
                    Assert.Fail("Expected results to be {0} but was {1}", expectedFilename, actualFilename);
                }
            }
            else
            {
                var actualFilename = TestResultRunFile(resultFilename);
                Assert.Fail("Expected result did not exist, actual results saved to {0}", actualFilename);
            }
        }

        [TestMethod]
        public void CanGenerateAltoOutput()
        {
            string actualResult;
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        actualResult = NormalizeNewLine(page.AltoText);
                    }
                }
            }

            var resultFilename = "EngineTests/CanGenerateAltoOutput.txt";
            var expectedFilename = TestResultPath(resultFilename);
            if (File.Exists(expectedFilename))
            {
                var expectedResult = NormalizeNewLine(File.ReadAllText(expectedFilename));
                if (expectedResult != actualResult)
                {
                    var actualFilename = TestResultRunFile(resultFilename);
                    //File.WriteAllText(actualFilename,actualResult);
                    Assert.Fail("Expected results to be {0} but was {1}", expectedFilename, actualFilename);
                }
            }
            else
            {
                var actualFilename = TestResultRunFile(resultFilename);
                //File.WriteAllText(actualFilename,actualResult);
                Assert.Fail("Expected result did not exist, actual results saved to {0}", actualFilename);
            }
        }

        [TestMethod]
        public void CanGenerateTsvOutput()
        {
            string actualResult;
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        actualResult = NormalizeNewLine(page.TsvText);
                    }
                }
            }

            var resultFilename = "EngineTests/CanGenerateTsvOutput.txt";
            var expectedFilename = TestResultPath(resultFilename);
            if (File.Exists(expectedFilename))
            {
                var expectedResult = NormalizeNewLine(File.ReadAllText(expectedFilename));
                if (expectedResult != actualResult)
                {
                    var actualFilename = TestResultRunFile(resultFilename);
                    //File.WriteAllText(actualFilename,actualResult);
                    Assert.Fail("Expected results to be {0} but was {1}", expectedFilename, actualFilename);
                }
            }
            else
            {
                var actualFilename = TestResultRunFile(resultFilename);
                //File.WriteAllText(actualFilename,actualResult);
                Assert.Fail("Expected result did not exist, actual results saved to {0}", actualFilename);
            }
        }

        [TestMethod]
        public void CanGenerateBoxOutput()
        {
            string actualResult;
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        actualResult = NormalizeNewLine(page.BoxText);
                    }
                }
            }

            var resultFilename = "EngineTests/CanGenerateBoxOutput.txt";
            var expectedFilename = TestResultPath(resultFilename);

            if (File.Exists(expectedFilename))
            {
                var expectedResult = NormalizeNewLine(File.ReadAllText(expectedFilename));
                if (expectedResult != actualResult)
                {
                    var actualFilename = TestResultRunFile(resultFilename);
                    Assert.Fail("Expected results to be {0} but was {1}", expectedFilename, actualFilename);
                }
            }
            else
            {
                var actualFilename = TestResultRunFile(resultFilename);
                Assert.Fail("Expected result did not exist, actual results saved to {0}", actualFilename);
            }
        }

        [TestMethod]
        public void CanGenerateLSTMBoxOutput()
        {
            string actualResult;
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        actualResult = NormalizeNewLine(page.LstmBoxText);
                    }
                }
            }

            var resultFilename = "EngineTests/CanGenerateLSTMBoxOutput.txt";
            var expectedFilename = TestResultPath(resultFilename);
            if (File.Exists(expectedFilename))
            {
                var expectedResult = NormalizeNewLine(File.ReadAllText(expectedFilename));
                if (expectedResult != actualResult)
                {
                    var actualFilename = TestResultRunFile(resultFilename);
                    //File.WriteAllText(actualFilename,actualResult);
                    Assert.Fail("Expected results to be {0} but was {1}", expectedFilename, actualFilename);
                }
            }
            else
            {
                var actualFilename = TestResultRunFile(resultFilename);
                //File.WriteAllText(actualFilename,actualResult);
                Assert.Fail("Expected result did not exist, actual results saved to {0}", actualFilename);
            }
        }

        [TestMethod]
        public void CanGenerateWordStrBoxOutput()
        {
            string actualResult;
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        actualResult = NormalizeNewLine(page.WordStrBoxText);
                    }
                }
            }

            var resultFilename = "EngineTests/CanGenerateWordStrBoxOutput.txt";
            var expectedFilename = TestResultPath(resultFilename);
            if (File.Exists(expectedFilename))
            {
                var expectedResult = NormalizeNewLine(File.ReadAllText(expectedFilename));
                if (expectedResult != actualResult)
                {
                    var actualFilename = TestResultRunFile(resultFilename);
                    //File.WriteAllText(actualFilename,actualResult);
                    Assert.Fail("Expected results to be {0} but was {1}", expectedFilename, actualFilename);
                }
            }
            else
            {
                var actualFilename = TestResultRunFile(resultFilename);
                //File.WriteAllText(actualFilename,actualResult);
                Assert.Fail("Expected result did not exist, actual results saved to {0}", actualFilename);
            }
        }

        [TestMethod]
        public void CanGenerateUNLVOutput()
        {
            string actualResult;
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        actualResult = NormalizeNewLine(page.UnlvText);
                    }
                }
            }

            const string resultFilename = "EngineTests/CanGenerateUNLVOutput.txt";
            var expectedFilename = TestResultPath(resultFilename);
            if (File.Exists(expectedFilename))
            {
                var expectedResult = NormalizeNewLine(File.ReadAllText(expectedFilename));
                if (expectedResult == actualResult) return;
                var actualFilename = TestResultRunFile(resultFilename);
                //File.WriteAllText(actualFilename,actualResult);
                Assert.Fail("Expected results to be {0} but was {1}", expectedFilename, actualFilename);
            }
            else
            {
                var actualFilename = TestResultRunFile(resultFilename);
                //File.WriteAllText(actualFilename,actualResult);
                Assert.Fail("Expected result did not exist, actual results saved to {0}", actualFilename);
            }
        }

        [TestMethod]
        public void CanProcessPixUsingResultIteratorAndChoiceIterator()
        {
            string actualResult;
            
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        // TODO : Fix this
                        actualResult = "";
                    }
                }
            }

            const string resultFilename = @"EngineTests\CanProcessPixUsingResultIteratorAndChoiceIterator.txt";
            
            var expectedResultFilename = TestResultPath(resultFilename);
            var expectedResult = NormalizeNewLine(File.ReadAllText(expectedResultFilename));

            if (expectedResult != actualResult)
            {
                var actualResultPath = TestResultRunFile(resultFilename);
                Assert.Fail("Expected results to be {0} but was {1}", expectedResultFilename, actualResultPath);
            }
        }

        [TestMethod]
        public void Initialise_CanLoadConfigFile()
        {
            using var engine = new Engine(DataPath, Language.English, EngineMode.Default, "bazzar");
            // verify that the config file was loaded
            if (engine.TryGetStringVariable("user_words_suffix", out var userPatternsSuffix))
                Assert.AreEqual(userPatternsSuffix, "user-words");
            else
                Assert.Fail("Failed to retrieve value for 'user_words_suffix'.");

            using var img = LoadTestPix(TestImagePath);
            using var page = engine.Process(img);
            var text = page.Text;

            const string expectedText =
                "This is a lot of 12 point text to test the\n" +
                "ocr code and see if it works on all types\n" +
                "of file format.\n\n" +
                "The quick brown dog jumped over the\n" +
                "lazy fox. The quick brown dog jumped\n" +
                "over the lazy fox. The quick brown dog\n" +
                "jumped over the lazy fox. The quick\n" +
                "brown dog jumped over the lazy fox.\n";
            Assert.AreEqual(text, expectedText);
        }

        [TestMethod]
        public void Initialise_CanPassInitVariables()
        {
            var initVars = new Dictionary<string, object>
            {
                { "load_system_dawg", false }
            };
            using var engine = new Engine(DataPath, Language.English, EngineMode.Default, Enumerable.Empty<string>(),
                initVars, false);
            if (!engine.TryGetBoolVariable("load_system_dawg", out var loadSystemDawg))
                Assert.Fail("Failed to get 'load_system_dawg'");
            Assert.IsFalse(loadSystemDawg);
        }

        [Ignore("Missing russian language data")]
        public static void Initialise_Rus_ShouldStartEngine()
        {
            using (new Engine(DataPath, Language.Russian, EngineMode.Default))
            {
            }
        }

        [TestMethod]
        public void Initialize_ShouldStartEngine()
        {
            const string dataPath = "tessdata";

            using (new Engine(dataPath, Language.English, EngineMode.Default))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TesseractException))]
        public void Initialize_ShouldThrowErrorIfDatapathNotCorrect()
        {
            using (new Engine(AbsolutePath(@"./IDontExist"), Language.English, EngineMode.Default))
            {
            }
        }
        
        #region Variable set\get
        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CanSetBooleanVariable(bool variableValue)
        {
            const string variableName = "classify_enable_learning";
            using var engine = CreateEngine();
            var variableWasSet = engine.SetVariable(variableName, variableValue);
            Assert.IsTrue(variableWasSet, "Failed to set variable '{0}'.", variableName);
                
            if (engine.TryGetBoolVariable(variableName, out var result))
                Assert.AreEqual(result, variableValue);
            else
                Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
        }

        /// <summary>
        ///     As per Bug #52 setting 'classify_bln_numeric_mode' variable to '1' causes the engine to fail on processing.
        /// </summary>
        [TestMethod]
        public void CanSetClassifyBlnNumericModeVariable()
        {
            using var engine = CreateEngine();
            engine.SetVariable("classify_bln_numeric_mode", 1);

            using var img = TesseractOCR.Pix.Image.LoadFromFile(TestFilePath("./processing/numbers.png"));
            using var page = engine.Process(img);
            var text = page.Text;

            const string expectedText = "1234567890\n";

            Assert.AreEqual(text, expectedText);
        }

        [DataTestMethod]
        [DataRow("edges_boxarea", 0.875)]
        [DataRow("edges_boxarea", 0.9)]
        [DataRow("edges_boxarea", -0.9)]
        public void CanSetDoubleVariable(string variableName, double variableValue)
        {
            using var engine = CreateEngine();
            var variableWasSet = engine.SetVariable(variableName, variableValue);
            Assert.IsTrue(variableWasSet, "Failed to set variable '{0}'.", variableName);
                
            if (engine.TryGetDoubleVariable(variableName, out var result))
                Assert.AreEqual(result, variableValue);
            else
                Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
        }

        [DataTestMethod]
        [DataRow("edges_children_count_limit", 45)]
        [DataRow("edges_children_count_limit", 20)]
        [DataRow("textord_testregion_left", 20)]
        [DataRow("textord_testregion_left", -20)]
        public void CanSetIntegerVariable(string variableName, int variableValue)
        {
            using var engine = CreateEngine();
            var variableWasSet = engine.SetVariable(variableName, variableValue);
            Assert.IsTrue(variableWasSet, "Failed to set variable '{0}'.", variableName);
                
            if (engine.TryGetIntVariable(variableName, out var result))
                Assert.AreEqual(result, variableValue);
            else
                Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
        }

        [DataTestMethod]
        [DataRow("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [DataRow("tessedit_char_whitelist", "")]
        [DataRow("tessedit_char_whitelist", "Test")]
        [DataRow("tessedit_char_whitelist", "chinese 漢字")] // Issue 68
        public void CanSetStringVariable(string variableName, string variableValue)
        {
            using var engine = CreateEngine();
            var variableWasSet = engine.SetVariable(variableName, variableValue);
            Assert.IsTrue(variableWasSet, "Failed to set variable '{0}'.", variableName);
                
            if (engine.TryGetStringVariable(variableName, out var result))
                Assert.AreEqual(result, variableValue);
            else
                Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
        }

        [TestMethod]
        public void CanGetStringVariableThatDoesNotExist()
        {
            using var engine = CreateEngine();
            var success = engine.TryGetStringVariable("illegal-variable", out var result);
            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        #endregion Variable set\get

        #region Variable print
        [TestMethod]
        public void CanPrintVariables()
        {
            const string resultFilename = @"EngineTests\CanPrintVariables.txt";

            using var engine = CreateEngine();
            var actualResultsFilename = TestResultRunFile(resultFilename);
            Assert.IsTrue(engine.TryPrintVariablesToFile(actualResultsFilename));
            var actualResult = NormalizeNewLine(File.ReadAllText(actualResultsFilename));

            // Load the expected results and verify that they match
            var expectedResultFilename = TestResultPath(resultFilename);
            var expectedResult = NormalizeNewLine(File.ReadAllText(expectedResultFilename));

            if (expectedResult != actualResult)
                Assert.Fail("Expected results to be \"{0}\" but was \"{1}\".", expectedResultFilename,
                    actualResultsFilename);
        }
        #endregion
    }
}