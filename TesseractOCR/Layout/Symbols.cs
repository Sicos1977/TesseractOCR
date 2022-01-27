//
// Symbols.cs
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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Symbols"/> in the <see cref="Word"/>
    /// </summary>
    public sealed class Symbols : IEnumerable<Symbol>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;
        #endregion

        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Symbol> GetEnumerator()
        {
            return new Symbol(_iteratorHandle);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        internal Symbols(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Symbol"/> in the <see cref="Word"/>
    /// </summary>
    public sealed class Symbol : IEnumerator<Symbol>
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
        ///     Returns the current <see cref="Symbol"/> object
        /// </summary>
        public object Current => this;

        /// <summary>
        ///     Returns the current <see cref="Symbol"/> object
        /// </summary>
        Symbol IEnumerator<Symbol>.Current => this;

        /// <summary>
        ///     Returns the text for the <see cref="Symbol"/>
        /// </summary>
        public string Text => TessApi.ResultIteratorGetUTF8Text(_iteratorHandle, PageIteratorLevel.Symbol);

        /// <summary>
        ///     Returns the confidence for the <see cref="Symbol"/>
        /// </summary>
        /// <returns></returns>
        public float Confidence => TessApi.Native.ResultIteratorGetConfidence(_iteratorHandle, PageIteratorLevel.Symbol);

        /// <summary>
        ///     Returns <c>true</c> when the <see cref="Symbol"/> is in superscript
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     A subscript or superscript is a character (such as a number or letter) that is set slightly below or
        ///     above the normal line of type, respectively. It is usually smaller than the rest of the text.
        ///     Subscripts appear at or below the baseline, while superscripts are above. Subscripts and superscripts
        ///     are perhaps most often used in formulas, mathematical expressions, and specifications of chemical
        ///     compounds and isotopes, but have many other uses as well.
        /// </remarks>
        public bool IsSuperscript => TessApi.Native.ResultIteratorSymbolIsSuperscript(_iteratorHandle);

        /// <summary>
        ///     Returns <c>true</c> when the <see cref="Symbol"/> is dropcap
        /// </summary>
        /// <remarks>
        ///     A Drop Cap is the initial letter of a paragraph which sits within the margins and runs several
        ///     lines deep into the paragraph, indenting some normal-sized text in these lines
        /// </remarks>
        public bool IsDropcap => TessApi.Native.ResultIteratorSymbolIsDropcap(_iteratorHandle);
        #endregion

        #region Constructor
        internal Symbol(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
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

            if (TessApi.Native.PageIteratorIsAtFinalElement(_iteratorHandle, PageIteratorLevel.Word, PageIteratorLevel.Symbol) != Constants.False)
            {
                Logger.LogDebug($"At final '{PageIteratorLevel.Symbol}' element");
                return false;
            }

            var result = TessApi.Native.PageIteratorNext(_iteratorHandle, PageIteratorLevel.Symbol) != Constants.False;

            if (result)
                Logger.LogDebug($"Moving to next '{PageIteratorLevel.Symbol}' element");

            return result;
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Symbol"/> in the <see cref="Word"/>
        /// </summary>
        public void Reset()
        {
            Logger.LogInformation($"Resetting to first '{PageIteratorLevel.Symbol}' element");
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
