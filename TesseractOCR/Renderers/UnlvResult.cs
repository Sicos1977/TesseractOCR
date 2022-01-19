using TesseractOCR.Interop;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in UNLV format
    /// </summary>
    public sealed class UnlvResult : Result
    {
        public UnlvResult(string outputFilename)
        {
            var rendererHandle = TessApi.Native.UnlvRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}