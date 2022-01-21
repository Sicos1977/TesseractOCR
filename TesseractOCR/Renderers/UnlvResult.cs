using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in UNLV format
    /// </summary>
    public sealed class UnlvResult : Result
    {
        public UnlvResult(string outputFilename)
        {
            Logger.LogInformation("Create UNLV renderer");

            var rendererHandle = TessApi.Native.UnlvRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}