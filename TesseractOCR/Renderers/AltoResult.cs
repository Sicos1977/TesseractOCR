using TesseractOCR.Helpers;
using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in ALTO format
    /// </summary>
    public sealed class AltoResult : Result
    {
        public AltoResult(string outputFilename)
        {
            Logger.LogInformation("Create alto renderer");

            var rendererHandle = TessApi.Native.AltoRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}