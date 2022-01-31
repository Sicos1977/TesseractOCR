using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable UnusedMember.Global

namespace Tesseract.Tests
{
    /// <summary>
    ///     Determines what action is taken when the test result doesn't match the expected (reference) result.
    /// </summary>
    public interface ITestDifferenceHandler
    {
        void Execute(string actualResultFilename, string expectedResultFilename);
    }

    /// <summary>
    ///     Fails the test if the actual result file doesn't match the expected result (ignoring line ending type(s)).
    /// </summary>
    public class FailTestDifferenceHandler : ITestDifferenceHandler
    {
        public void Execute(string actualResultFilename, string expectedResultFilename)
        {
            if (File.Exists(expectedResultFilename))
            {
                var actualResult = TestUtils.NormalizeNewLine(File.ReadAllText(actualResultFilename));
                var expectedResult = TestUtils.NormalizeNewLine(File.ReadAllText(expectedResultFilename));
                if (expectedResult != actualResult)
                    Assert.Fail("Expected results to be \"{0}\" but was \"{1}\".", expectedResultFilename,
                        actualResultFilename);
            }
            else
            {
                File.Copy(actualResultFilename, expectedResultFilename);
                Console.WriteLine(
                    $"Expected result did not exist, the file \"{actualResultFilename}\" was used as a reference. Please check the file");
            }
        }
    }
}