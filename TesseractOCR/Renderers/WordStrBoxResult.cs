using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in WORD STRING BOX format
    /// </summary>

    public sealed class WordStrBoxResult : Result
    {
        public WordStrBoxResult(string outputFilename)
        {
            Logger.LogInformation("Create word string box renderer");
            var rendererHandle = TessApi.Native.WordStrBoxRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}