using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.Interop;
using TesseractOCR.Loggers;
using Image = TesseractOCR.Pix.Image;

namespace TesseractOCR.Layout
{
    public class EnumeratorBase
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

        /// <summary>
        ///     The <see cref="PageIteratorLevel"/>
        /// </summary>
        protected PageIteratorLevel PageIteratorLevel;

        /// <summary>
        ///     Flag to check if we are doing our first enumeration
        /// </summary>
        private bool _first = true;
        #endregion

        #region Properties
        /// <summary>
        ///     Returns the text for the <see cref="Block"/>
        /// </summary>
        public string Text => TessApi.ResultIteratorGetUTF8Text(IteratorHandleRef, PageIteratorLevel);

        /// <summary>
        ///     Returns the confidence for the <see cref="Block"/>
        /// </summary>
        /// <returns></returns>
        public float Confidence => TessApi.Native.ResultIteratorGetConfidence(IteratorHandleRef, PageIteratorLevel);

        /// <summary>
        ///     Returns a binary (gray) <see cref="Pix.Image"/> at the current <see cref="PageIteratorLevel"/>
        /// </summary>
        /// <returns>The <see cref="Pix.Image"/> or <c>null</c> when it fails</returns>
        public Image BinaryImage => Image.Create(TessApi.Native.PageIteratorGetBinaryImage(IteratorHandleRef, PageIteratorLevel));

        ///// <summary>
        /////     Returns a <see cref="Pix.Image"/> from what is seen at the current <see cref="PageIteratorLevel"/>>
        ///// </summary>
        //public Image Image => Image.Create(TessApi.Native.PageIteratorGetImage(IteratorHandle, PageIteratorLevel, 0, PageRef.Image.Handle, out _, out _))
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Symbol"/> in the <see cref="Word"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Symbol"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return true;
            }

            var result = TessApi.Native.PageIteratorNext(IteratorHandleRef, PageIteratorLevel) == Constants.True;

            Logger.LogInformation(result
                ? $"Moving to next '{PageIteratorLevel}' element"
                : $"At final '{PageIteratorLevel}' element");

            return result;
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Symbol"/> in the <see cref="Word"/>
        /// </summary>
        public void Reset()
        {
            Logger.LogInformation($"Resetting to the first '{PageIteratorLevel}' element");
            _first = true;
            TessApi.Native.PageIteratorBegin(IteratorHandleRef);
        }
        #endregion

        #region Dispose
        /// <summary>
        ///     Does not do a thing, we have to implement it because of the IEnumerator interface
        /// </summary>
        public void Dispose()
        {
            // We have to implement this method because of the IEnumerator interface
            // but we have nothing to do here so just ignore it
        }
        #endregion
    }
}
