//
// TextLines.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2021-2022 Kees van Spelde
//
// Licensed under the Apache License, Version 2.0 (the "License");
//
// - You may not use this file except in compliance with the License.
// - You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="TextLines"/> in the <see cref="Paragraph"/>
    /// </summary>
    public sealed class TextLines : IEnumerable<TextLine>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;
        #endregion

        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<TextLine> GetEnumerator()
        {
            return new TextLine(_iteratorHandle);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        internal TextLines(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="TextLine"/> in the <see cref="Paragraph"/>
    /// </summary>
    public sealed class TextLine : IEnumerator<TextLine>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;

        private bool _first = true;
        #endregion

        #region Properties
        /// <summary>
        ///     Returns the current <see cref="TextLine"/> object
        /// </summary>
        public object Current => this;

        /// <summary>
        ///     Returns the current <see cref="TextLine"/> object
        /// </summary>
        TextLine IEnumerator<TextLine>.Current => this;

        /// <summary>
        ///     All the available <see cref="Words"/> in this <see cref="TextLine"/>
        /// </summary>
        public Words Words => new Words(_iteratorHandle);

        /// <summary>
        ///     Returns the text for the <see cref="TextLine"/>
        /// </summary>
        public string Text => TessApi.ResultIteratorGetUTF8Text(_iteratorHandle, PageIteratorLevel.TextLine);

        /// <summary>
        ///     Returns the confidence for the <see cref="TextLine"/>
        /// </summary>
        /// <returns></returns>
        public float Confidence => TessApi.Native.ResultIteratorGetConfidence(_iteratorHandle, PageIteratorLevel.TextLine);
        #endregion

        #region Constructor
        internal TextLine(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="TextLine"/> in the <see cref="Paragraph"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="TextLine"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return true;
            }

            if (TessApi.Native.PageIteratorIsAtFinalElement(_iteratorHandle, PageIteratorLevel.Paragraph, PageIteratorLevel.TextLine) != Constants.False)
            {
                Logger.LogInformation($"At final '{PageIteratorLevel.TextLine}' element");
                return false;
            }

            var result = TessApi.Native.PageIteratorNext(_iteratorHandle, PageIteratorLevel.TextLine) != Constants.False;

            if (result)
                Logger.LogInformation($"Moving to next '{PageIteratorLevel.TextLine}' element");

            return result;
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="TextLine"/> in the <see cref="Paragraph"/>
        /// </summary>
        public void Reset()
        {
            Logger.LogInformation($"Resetting to first '{PageIteratorLevel.TextLine}' element");
            _first = true;
            TessApi.Native.PageIteratorBegin(_iteratorHandle);
        }
        #endregion

        #region Dispose
        /// <summary>
        ///     Does not do a thing, we have to implement it because of the <see cref="IEnumerator"/> interface
        /// </summary>
        public void Dispose()
        {
            // We have to implement this method because of the IEnumerator interface
            // but we have nothing to do here so just ignore it
        }
        #endregion
    }
}
