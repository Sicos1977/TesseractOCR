using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Renderers;

namespace Tesseract.Tests
{
    [TestClass]
    public class ResultTests : TesseractTestBase
    {
        #region Test setup and teardown
        private Engine _engine;

        [TestInitialize]
        public void Initialize()
        {
            _engine = CreateEngine();
        }

        [TestCleanup]
        public void Dispose()
        {
            if (_engine == null) return;
            _engine.Dispose();
            _engine = null;
        }
        #endregion Test setup and teardown

        [TestMethod]
        public void CanRenderResultsIntoTextFile()
        {
            var resultPath = TestResultRunFile(@"Results\Text\phototest");
            using (var renderer = Result.CreateTextRenderer(resultPath))
            {
                var examplePixPath = TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }

            var expectedOutputFilename = Path.ChangeExtension(resultPath, "txt");
            Assert.IsTrue(File.Exists(expectedOutputFilename),
                $"Expected a Text file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [TestMethod]
        public void CanRenderResultsIntoPdfFile()
        {
            var resultPath = TestResultRunFile(@"Results\PDF\phototest");
            using (var renderer = Result.CreatePdfRenderer(resultPath, DataPath, false))
            {
                var examplePixPath = TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }

            var expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            using (var renderer = Result.CreatePdfRenderer(resultPath, DataPath, false))
            {
                var examplePixPath = TestFilePath("Ocr/phototest.tif");
                ProcessImageFile(renderer, examplePixPath);
            }

            Assert.IsTrue(File.Exists(expectedOutputFilename),
                $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [TestMethod]
        public void CanRenderResultsIntoPdfFile1()
        {
            var resultPath = TestResultRunFile(@"Results\PDF\phototest");
            using (var renderer = Result.CreatePdfRenderer(resultPath, DataPath, false))
            {
                var examplePixPath = TestFilePath("Ocr/phototest.tif");
                ProcessImageFile(renderer, examplePixPath);
            }

            var expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            Assert.IsTrue(File.Exists(expectedOutputFilename),
                $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [TestMethod]
        public void CanRenderMultiplePageDocumentToPdfFile()
        {
            var resultPath = TestResultRunFile(@"Results\PDF\multi-page");
            using (var renderer = Result.CreatePdfRenderer(resultPath, DataPath, false))
            {
                var examplePixPath = TestFilePath("processing/multi-page.tif");
                ProcessMultiPageTiff(renderer, examplePixPath);
            }

            var expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            using (var renderer = Result.CreatePdfRenderer(resultPath, DataPath, false))
            {
                var examplePixPath = TestFilePath("processing/multi-page.tif");
                ProcessImageFile(renderer, examplePixPath);
            }

            Assert.IsTrue(File.Exists(expectedOutputFilename),
                $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [TestMethod]
        public void CanRenderMultiplePageDocumentToPdfFile1()
        {
            var resultPath = TestResultRunFile(@"Results\PDF\multi-page");
            using (var renderer = Result.CreatePdfRenderer(resultPath, DataPath, false))
            {
                var examplePixPath = TestFilePath("processing/multi-page.tif");
                ProcessImageFile(renderer, examplePixPath);
            }

            var expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            Assert.IsTrue(File.Exists(expectedOutputFilename),
                $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [TestMethod]
        public void CanRenderResultsIntoHOcrFile()
        {
            var resultPath = TestResultRunFile(@"Results\HOCR\phototest");
            using (var renderer = Result.CreateHOcrRenderer(resultPath))
            {
                var examplePixPath = TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }

            var expectedOutputFilename = Path.ChangeExtension(resultPath, "hocr");
            Assert.IsTrue(File.Exists(expectedOutputFilename),
                $"Expected a HOCR file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [TestMethod]
        public void CanRenderResultsIntoUnlvFile()
        {
            var resultPath = TestResultRunFile(@"Results\UNLV\phototest");
            using (var renderer = Result.CreateUnlvRenderer(resultPath))
            {
                var examplePixPath = TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }

            var expectedOutputFilename = Path.ChangeExtension(resultPath, "unlv");
            Assert.IsTrue(File.Exists(expectedOutputFilename),
                $"Expected a Unlv file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [TestMethod]
        public void CanRenderResultsIntoBoxFile()
        {
            var resultPath = TestResultRunFile(@"Results\Box\phototest");
            using (var renderer = Result.CreateBoxRenderer(resultPath))
            {
                var examplePixPath = TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }

            var expectedOutputFilename = Path.ChangeExtension(resultPath, "box");
            Assert.IsTrue(File.Exists(expectedOutputFilename),
                $"Expected a Box file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [TestMethod]
        public void CanRenderResultsIntoMultipleOutputFormats()
        {
            var resultPath = TestResultRunFile(@"Results\PDF\phototest");
            var formats = new List<RenderFormat>
            {
                RenderFormat.Text,
                RenderFormat.Hocr,
                RenderFormat.Pdf,
                RenderFormat.PdfTextonly,
                RenderFormat.Box,
                RenderFormat.Unlv,
                // RenderFormat.Alto,
                RenderFormat.Tsv,
                RenderFormat.LstmBox,
                RenderFormat.WordStrBox
            };

            // TODO: Find out why Alto rendering fails

            var renderers = Result.CreateRenderers(resultPath, DataPath, formats);

            foreach (var renderer in renderers)
            {
                var examplePixPath = TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
                renderer.Dispose();
            }

            var expectedOutputFilename = Path.ChangeExtension(resultPath, "txt");
            Assert.IsTrue(File.Exists(expectedOutputFilename), $"Expected a TEXT file \"{expectedOutputFilename}\" to have been created; but none was found.");

            expectedOutputFilename = Path.ChangeExtension(resultPath, "hocr");
            Assert.IsTrue(File.Exists(expectedOutputFilename), $"Expected a HOCR file \"{expectedOutputFilename}\" to have been created; but none was found.");

            expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            Assert.IsTrue(File.Exists(expectedOutputFilename), $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");

            expectedOutputFilename = Path.ChangeExtension(resultPath, "box");
            Assert.IsTrue(File.Exists(expectedOutputFilename), $"Expected a BOX file \"{expectedOutputFilename}\" to have been created; but none was found.");

            expectedOutputFilename = Path.ChangeExtension(resultPath, "unlv");
            Assert.IsTrue(File.Exists(expectedOutputFilename), $"Expected a UNLV file \"{expectedOutputFilename}\" to have been created; but none was found.");

            expectedOutputFilename = Path.ChangeExtension(resultPath, "tsv");
            Assert.IsTrue(File.Exists(expectedOutputFilename), $"Expected a TSV file \"{expectedOutputFilename}\" to have been created; but none was found.");

            expectedOutputFilename = Path.ChangeExtension(resultPath, "box");
            Assert.IsTrue(File.Exists(expectedOutputFilename), $"Expected a LSTMBOX file \"{expectedOutputFilename}\" to have been created; but none was found.");

            expectedOutputFilename = Path.ChangeExtension(resultPath, "box");
            Assert.IsTrue(File.Exists(expectedOutputFilename), $"Expected a WORDSTRBOX file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [TestMethod]
        public void CanRenderMultiplePageDocumentIntoMultipleResults()
        {
            var resultPath = TestResultRunFile(@"Results\Aggregate\multi-page");
            using (var renderer = new AggregateResult(
                       Result.CreatePdfRenderer(resultPath, DataPath, false),
                       Result.CreateTextRenderer(resultPath)))
            {
                var examplePixPath = TestFilePath("processing/multi-page.tif");
                ProcessMultiPageTiff(renderer, examplePixPath);
            }

            var expectedPdfOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            Assert.IsTrue(File.Exists(expectedPdfOutputFilename),
                $"Expected a PDF file \"{expectedPdfOutputFilename}\" to have been created; but none was found.");

            var expectedTxtOutputFilename = Path.ChangeExtension(resultPath, "txt");
            Assert.IsTrue(File.Exists(expectedTxtOutputFilename),
                $"Expected a Text file \"{expectedTxtOutputFilename}\" to have been created; but none was found.");
        }

        private void ProcessMultiPageTiff(IResult renderer, string filename)
        {
            var imageName = Path.GetFileNameWithoutExtension(filename);
            using var pixA = TesseractOCR.Pix.Array.LoadMultiPageTiffFromFile(filename);
            var expectedPageNumber = -1;
            using (renderer.BeginDocument(imageName))
            {
                Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
                foreach (var pix in pixA)
                {
                    using var page = _engine.Process(pix);
                    var addedPage = renderer.AddPage(page);
                    expectedPageNumber++;

                    Assert.IsTrue(addedPage);
                    Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
                }
            }

            Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
        }

        private void ProcessFile(IResult renderer, string filename)
        {
            var imageName = Path.GetFileNameWithoutExtension(filename);
            using var pix = TesseractOCR.Pix.Image.LoadFromFile(filename);
            using (renderer.BeginDocument(imageName))
            {
                Assert.AreEqual(renderer.PageNumber, -1);
                using (var page = _engine.Process(pix))
                {
                    var addedPage = renderer.AddPage(page);

                    Assert.IsTrue(addedPage);
                    Assert.AreEqual(renderer.PageNumber, 0);
                }
            }

            Assert.AreEqual(renderer.PageNumber, 0);
        }

        private void ProcessImageFile(IResult renderer, string filename)
        {
            var imageName = Path.GetFileNameWithoutExtension(filename);
            using var pixA = ReadImageFileIntoPixArray(filename);
            var expectedPageNumber = -1;
            using (renderer.BeginDocument(imageName))
            {
                Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
                foreach (var pix in pixA)
                {
                    using var page = _engine.Process(pix);
                    var addedPage = renderer.AddPage(page);
                    expectedPageNumber++;

                    Assert.IsTrue(addedPage);
                    Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
                }
            }

            Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
        }

        private static TesseractOCR.Pix.Array ReadImageFileIntoPixArray(string filename)
        {
            if (filename.ToLower().EndsWith(".tif") || filename.ToLower().EndsWith(".tiff"))
                return TesseractOCR.Pix.Array.LoadMultiPageTiffFromFile(filename);

            var pa = TesseractOCR.Pix.Array.Create(0);
            pa.Add(TesseractOCR.Pix.Image.LoadFromFile(filename));
            return pa;
        }
    }
}