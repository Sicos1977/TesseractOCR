using System;
using System.Runtime.InteropServices;
using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Renders the result in PDF format
    /// </summary>
    public sealed class PdfResult : Result
    {
        #region Fields
        private IntPtr _fontDirectoryHandle;
        #endregion

        #region Constructor
        public PdfResult(string outputFilename, string fontDirectory, bool textOnly)
        {
            Logger.LogInformation("Create PDF renderer");

            var fontDirectoryHandle = Marshal.StringToHGlobalAnsi(fontDirectory);
            var rendererHandle = TessApi.Native.PDFRendererCreate(outputFilename, fontDirectoryHandle, textOnly ? 1 : 0);

            Initialize(rendererHandle);
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // dispose of font
            if (_fontDirectoryHandle != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_fontDirectoryHandle);
                _fontDirectoryHandle = IntPtr.Zero;
            }
        }
        #endregion
    }
}