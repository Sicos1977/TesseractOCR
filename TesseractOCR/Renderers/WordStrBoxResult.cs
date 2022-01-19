using TesseractOCR.Interop;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in WORD STRING BOX format
    /// </summary>

    public sealed class WordStrBoxResult : Result
    {
        public WordStrBoxResult(string outputFilename)
        {
            var rendererHandle = TessApi.Native.WordStrBoxRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}