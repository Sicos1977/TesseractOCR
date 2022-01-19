using TesseractOCR.Interop;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in BOX format
    /// </summary>
    public sealed class BoxResult : Result
    {
        public BoxResult(string outputFilename)
        {
            var rendererHandle = TessApi.Native.BoxTextRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}