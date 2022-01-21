using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in TSV format
    /// </summary>

    public sealed class TsvResult : Result
    {
        public TsvResult(string outputFilename)
        {
            Logger.LogInformation("Create TSV renderer");

            var rendererHandle = TessApi.Native.TsvRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}