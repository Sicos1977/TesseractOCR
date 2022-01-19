using TesseractOCR.Interop;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in LSTM BOX format
    /// </summary>
    public sealed class LstmBoxResult : Result
    {
        public LstmBoxResult(string outputFilename)
        {
            var rendererHandle = TessApi.Native.LSTMBoxRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}