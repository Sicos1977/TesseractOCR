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
// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Symbols"/> in the <see cref="Word"/>
    /// </summary>
    public sealed class Symbols : EnumerableBase, IEnumerable<Symbol>
    {
        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Symbol> GetEnumerator()
        {
            return new Symbol(EngineHandleRef, IteratorHandleRef, ImageHandleRef);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates this object
        /// </summary>
        /// <param name="engineHandleRef">A handle reference to the Tesseract engine</param>
        /// <param name="iteratorHandleRef">A handle reference to the page iterator</param>
        /// <param name="imageHandleRef">A handle reference to the <see cref="Pix.Image"/></param>

        internal Symbols(HandleRef engineHandleRef, HandleRef iteratorHandleRef, HandleRef imageHandleRef)
        {
            EngineHandleRef = engineHandleRef;
            IteratorHandleRef = iteratorHandleRef;
            ImageHandleRef = imageHandleRef;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Symbol"/> in the <see cref="Word"/>
    /// </summary>
    public sealed class Symbol : EnumeratorBase, IEnumerator<Symbol>
    {
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
        public bool IsSuperscript => TessApi.Native.ResultIteratorSymbolIsSuperscript(IteratorHandleRef);

        /// <summary>
        ///     Returns <c>true</c> when the <see cref="Symbol"/> is dropcap
        /// </summary>
        /// <remarks>
        ///     A Drop Cap is the initial letter of a paragraph which sits within the margins and runs several
        ///     lines deep into the paragraph, indenting some normal-sized text in these lines
        /// </remarks>
        public bool IsDropcap => TessApi.Native.ResultIteratorSymbolIsDropcap(IteratorHandleRef);
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates this object
        /// </summary>
        /// <param name="engineHandleRef">A handle reference to the Tesseract engine</param>
        /// <param name="iteratorHandleRef">A handle reference to the page iterator</param>
        /// <param name="imageHandleRef">A handle reference to the <see cref="Pix.Image"/></param>

        internal Symbol(HandleRef engineHandleRef, HandleRef iteratorHandleRef, HandleRef imageHandleRef)
        {
            EngineHandleRef = engineHandleRef;
            IteratorHandleRef = iteratorHandleRef;
            ImageHandleRef = imageHandleRef;
            PageIteratorLevel = PageIteratorLevel.Symbol;
            LogDebug = true;
        }
        #endregion
    }
}
