using System;
using System.IO;
using TesseractOCR;
using TesseractOCR.Enums;

namespace Tesseract.Tests
{
    public abstract class TesseractTestBase
    {
        protected static TesseractEngine CreateEngine(string lang = "eng", EngineMode mode = EngineMode.Default)
        {
            var datapath = DataPath;
            return new TesseractEngine(datapath, lang, mode);
        }

        protected static string DataPath => AbsolutePath("tessdata");

        protected static string AbsolutePath(string relativePath)
        {
            //return Path.Combine(TestContext.CurrentContext.WorkDirectory, relativePath);
            return Path.Combine(Environment.CurrentDirectory, relativePath);
        }

        #region File Helpers
        protected static string TestFilePath(string path)
        {
            var basePath = AbsolutePath("Data");

            return Path.Combine(basePath, path);
        }

        protected static string TestResultPath(string path)
        {
            var basePath = AbsolutePath("Results");

            return Path.Combine(basePath, path);
        }

        protected static string TestResultRunDirectory(string path)
        {
            var runPath = AbsolutePath(
                string.Format("Runs/{0:yyyyMMddTHHmmss}", TestRun.Current.StartedAt)
            );
            var testResultRunDirectory = Path.Combine(runPath, path);
            Directory.CreateDirectory(testResultRunDirectory);

            return testResultRunDirectory;
        }

        protected static string TestResultRunFile(string path)
        {
            var testRunDirectory = TestResultRunDirectory(Path.GetDirectoryName(path));
            var testFileName = Path.GetFileName(path);

            return Path.Combine(testRunDirectory, testFileName);
        }

        protected static Pix LoadTestPix(string filename)
        {
            var testFilename = TestFilePath(filename);
            return Pix.LoadFromFile(testFilename);
        }

        /// <summary>
        ///     Normalise new line characters to unix (\n) so they are all the same.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected static string NormaliseNewLine(string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Trim();
        }
        #endregion File Helpers
    }
}