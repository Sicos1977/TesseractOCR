using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in LSTM BOX format
    /// </summary>
    public sealed class LstmBoxResult : Result
    {
        public LstmBoxResult(string outputFilename)
        {
            Logger.LogInformation("Create LSTM box renderer");

            var rendererHandle = TessApi.Native.LSTMBoxRendererCreate(outputFilename);
            Initialize(rendererHandle);
        }
    }
}