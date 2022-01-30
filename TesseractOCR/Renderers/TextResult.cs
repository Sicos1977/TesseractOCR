using TesseractOCR.Helpers;
using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result as Text
    /// </summary>
    public sealed class TextResult : Result
    {
        public TextResult(string outputFilename)
        {
            Logger.LogInformation("Create text renderer");

            var rendererHandle = TessApi.Native.TextRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}