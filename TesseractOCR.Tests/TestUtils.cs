namespace Tesseract.Tests
{
    public static class TestUtils
    {
        /// <summary>
        ///     Normalize new line characters to unix (\n) so they are all the same.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string NormalizeNewLine(string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
        }
    }
}