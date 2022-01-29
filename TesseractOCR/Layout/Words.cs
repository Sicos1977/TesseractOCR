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

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Words"/> in the <see cref="TextLine"/>
    /// </summary>
    public sealed class Words : EnumerableBase, IEnumerable<Word>
    {
        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Word> GetEnumerator()
        {
            return new Word(IteratorHandleRef, ImageHandleRef);
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
        /// <param name="iteratorHandleRef">A handle reference to the page iterator</param>
        /// <param name="imageHandleRef">A handle reference to the <see cref="Pix.Image"/></param>
        internal Words(HandleRef iteratorHandleRef, HandleRef imageHandleRef)
        {
            IteratorHandleRef = iteratorHandleRef;
            ImageHandleRef = imageHandleRef;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Word"/> in the <see cref="TextLine"/>
    /// </summary>
    public sealed class Word : EnumeratorBase, IEnumerator<Word>
    {
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
        public Symbols Symbols => new Symbols(IteratorHandleRef, ImageHandleRef);

        /// <summary>
        ///     Returns <c>true</c> when the <see cref="Word"/> is returned from a Tesseract dictionary
        /// </summary>
        /// <returns></returns>
        public bool IsFromDictionary => TessApi.Native.ResultIteratorWordIsFromDictionary(IteratorHandleRef);

        /// <summary>
        ///     Returns <c>true</c> when the <see cref="Word"/> is numeric
        /// </summary>
        /// <returns></returns>
        public bool IsNumeric => TessApi.Native.ResultIteratorWordIsNumeric(IteratorHandleRef);

        /// <summary>
        ///     Returns the <see cref="Language"/> for the recognized <see cref="Word"/>
        /// </summary>
        /// <returns></returns>
        public Language Language
        {
            get
            {
                var value = LanguageAsString;
                return LanguageHelper.StringToEnum(value);
            }
        } 
        /// <summary>
        ///     Returns the <see cref="Language"/> as a string
        /// </summary>
        public string LanguageAsString => TessApi.ResultIteratorWordRecognitionLanguage(IteratorHandleRef);

        /// <summary>
        ///     Returns the <see cref="FontAttributes"/> for the <see cref="Word"/>
        /// </summary>
        public FontAttributes FontAttributes
        {
            get
            {
                var nameHandle =
                    TessApi.Native.ResultIteratorWordFontAttributes(
                        IteratorHandleRef,
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
        /// <summary>
        ///     Creates this object
        /// </summary>
        /// <param name="iteratorHandleRef">A handle reference to the page iterator</param>
        /// <param name="imageHandleRef">A handle reference to the <see cref="Pix.Image"/></param>
        internal Word(HandleRef iteratorHandleRef, HandleRef imageHandleRef)
        {
            IteratorHandleRef = iteratorHandleRef;
            ImageHandleRef = imageHandleRef;
            PageIteratorLevel = PageIteratorLevel.Word;
        }
        #endregion
    }
}
