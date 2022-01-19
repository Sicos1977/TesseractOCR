using TesseractOCR.Interop;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result as Text
    /// </summary>
    public sealed class TextResult : Result
    {
        public TextResult(string outputFilename)
        {
            var rendererHandle = TessApi.Native.TextRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}