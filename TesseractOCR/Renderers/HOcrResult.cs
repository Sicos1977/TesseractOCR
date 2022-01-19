using TesseractOCR.Interop;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in HOCR format
    /// </summary>
    public sealed class HOcrResult : Result
    {
        public HOcrResult(string outputFilename, bool fontInfo = false)
        {
            var rendererHandle = TessApi.Native.HOcrRendererCreate2(outputFilename, fontInfo ? 1 : 0);
            Initialize(rendererHandle);
        }
    }
}