using System.Runtime.InteropServices;

namespace TesseractOCR.Layout
{
    public class EnumerableBase
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        protected HandleRef IteratorHandleRef;

        /// <summary>
        ///     <see cref="Pix.Image"/>
        /// </summary>
        protected HandleRef ImageHandleRef;
        #endregion
    }
}
