using TesseractOCR.Interop;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in TSV format
    /// </summary>

    public sealed class TsvResult : Result
    {
        public TsvResult(string outputFilename)
        {
            var rendererHandle = TessApi.Native.TsvRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}