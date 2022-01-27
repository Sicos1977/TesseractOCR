//
// Words.cs
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
using System.Text;
using TesseractOCR.Enums;
using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Words"/> in the <see cref="TextLine"/>
    /// </summary>
    public sealed class Words : IEnumerable<Word>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;
        #endregion

        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Word> GetEnumerator()
        {
            return new Word(_iteratorHandle);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        internal Words(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Word"/> in the <see cref="TextLine"/>
    /// </summary>
    public sealed class Word : IEnumerator<Word>
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
        ///     Returns the current <see cref="Word"/> object
        /// </summary>
        public object Current => this;

        /// <summary>
        ///     Returns the current <see cref="Word"/> object
        /// </summary>
        Word IEnumerator<Word>.Current => this;

        /// <summary>
        ///     All the available <see cref="Symbols"/> in this <see cref="Word"/>
        /// </summary>
        public Symbols Symbols => new Symbols(_iteratorHandle);

        /// <summary>
        ///     Returns the text for the <see cref="Word"/>
        /// </summary>
        public string Text => TessApi.ResultIteratorGetUTF8Text(_iteratorHandle, PageIteratorLevel.Word);

        /// <summary>
        ///     Returns the confidence for the <see cref="Word"/>
        /// </summary>
        /// <returns></returns>
        public float Confidence => TessApi.Native.ResultIteratorGetConfidence(_iteratorHandle, PageIteratorLevel.Word);

        /// <summary>
        ///     Returns <c>true</c> when the <see cref="Word"/> is returned from a Tesseract dictionary
        /// </summary>
        /// <returns></returns>
        public bool IsFromDictionary => TessApi.Native.ResultIteratorWordIsFromDictionary(_iteratorHandle);

        /// <summary>
        ///     Returns <c>true</c> when the <see cref="Word"/> is numeric
        /// </summary>
        /// <returns></returns>
        public bool IsNumeric => TessApi.Native.ResultIteratorWordIsNumeric(_iteratorHandle);

        /// <summary>
        ///     Returns the language for the recognized <see cref="Word"/>
        /// </summary>
        /// <returns></returns>
        public string Language => TessApi.ResultIteratorWordRecognitionLanguage(_iteratorHandle);

        /// <summary>
        ///     Returns the <see cref="FontAttributes"/> for the <see cref="Word"/>
        /// </summary>
        public FontAttributes FontAttributes
        {
            get
            {
                var nameHandle =
                    TessApi.Native.ResultIteratorWordFontAttributes(
                        _iteratorHandle,
                        out var isBold, out var isItalic, out var isUnderlined,
                        out var isMonospace, out var isSerif, out var isSmallCaps,
                        out var pointSize, out var fontId);

                // This can happen in certain error conditions
                if (nameHandle == IntPtr.Zero)
                    return null;

                var fontName = MarshalHelper.PtrToString(nameHandle, Encoding.UTF8);
                var fontInfo = new FontInfo(fontName, fontId, isItalic, isBold, isMonospace, isSerif);
                return new FontAttributes(fontInfo, isUnderlined, isSmallCaps, pointSize);
            }
        }
        #endregion

        #region Constructor
        internal Word(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Word"/> in the <see cref="Line"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Word"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return true;
            }

            if (TessApi.Native.PageIteratorIsAtFinalElement(_iteratorHandle, PageIteratorLevel.TextLine, PageIteratorLevel.Word) != Constants.False)
            {
                Logger.LogInformation($"At final '{PageIteratorLevel.Word}' element");
                return false;
            }

            var result = TessApi.Native.PageIteratorNext(_iteratorHandle, PageIteratorLevel.Word) != Constants.False;

            if (result)
                Logger.LogInformation($"Moving to next '{PageIteratorLevel.Word}' element");

            return result;
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Word"/> in the <see cref="TextLine"/>
        /// </summary>
        public void Reset()
        {
            Logger.LogInformation($"Resetting to first '{PageIteratorLevel.Word}' element");
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
            // We have implement this method because of the IEnumerator interface
            // but we have nothing to do here so just ignore it
        }
        #endregion
    }
}
