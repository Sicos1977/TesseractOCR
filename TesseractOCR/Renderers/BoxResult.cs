using TesseractOCR.Helpers;
using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in BOX format
    /// </summary>
    public sealed class BoxResult : Result
    {
        public BoxResult(string outputFilename)
        {
            Logger.LogInformation("Create box renderer");

            var rendererHandle = TessApi.Native.BoxTextRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}