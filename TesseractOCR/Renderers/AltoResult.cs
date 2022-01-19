using TesseractOCR.Interop;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in ALTO format
    /// </summary>
    public sealed class AltoResult : Result
    {
        public AltoResult(string outputFilename)
        {
            var rendererHandle = TessApi.Native.AltoRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}