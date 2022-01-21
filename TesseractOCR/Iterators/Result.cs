//
// ResultIterator.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
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
using System.Collections.Generic;
using System.Text;
using TesseractOCR.Interop;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Iterators
{
    public sealed class Result : Page
    {
        #region Fields
        private readonly Dictionary<int, FontInfo> _fontInfoCache = new Dictionary<int, FontInfo>();
        #endregion

        #region Properties
        /// <summary>
        ///     Returns the confidence for the given <see cref="Page.Level"/>
        /// </summary>
        /// <returns></returns>
        public float Confidence
        {
            get
            {
                VerifyNotDisposed();
                return HandleRef.Handle == IntPtr.Zero ? 0f : TessApi.Native.ResultIteratorGetConfidence(HandleRef, Level);
            }
        }

        /// <summary>
        ///     Returns the text for the current <see cref="Page.Level"/>
        /// </summary>
        public string Text
        {
            get
            {
                VerifyNotDisposed();

                return HandleRef.Handle == IntPtr.Zero ? string.Empty : TessApi.ResultIteratorGetUTF8Text(HandleRef, Level);
            }
        }

        /// <summary>
        ///     Returns <c>true</c> when the word is returned from a Tesseract dictionary
        /// </summary>
        /// <returns></returns>
        public bool WordIsFromDictionary
        {
            get
            {
                VerifyNotDisposed();
                return HandleRef.Handle != IntPtr.Zero && TessApi.Native.ResultIteratorWordIsFromDictionary(HandleRef);
            }
        }

        /// <summary>
        ///     Returns the <see cref="FontAttributes"/> for the word
        /// </summary>
        public FontAttributes WordFontAttributes
        {
            get
            {
                VerifyNotDisposed();

                if (HandleRef.Handle == IntPtr.Zero)
                    return null;

                // Per docs (ltrresultiterator.h:104 as of 4897796 in github:tesseract-ocr/tesseract)
                // this return value points to an internal table and should not be deleted.
                var nameHandle =
                    TessApi.Native.ResultIteratorWordFontAttributes(
                        HandleRef,
                        out var isBold, out var isItalic, out var isUnderlined,
                        out var isMonospace, out var isSerif, out var isSmallCaps,
                        out var pointSize, out var fontId);

                // This can happen in certain error conditions
                if (nameHandle == IntPtr.Zero)
                    return null;

                if (_fontInfoCache.TryGetValue(fontId, out var fontInfo))
                    return new FontAttributes(fontInfo, isUnderlined, isSmallCaps, pointSize);

                var fontName = MarshalHelper.PtrToString(nameHandle, Encoding.UTF8);
                fontInfo = new FontInfo(fontName, fontId, isItalic, isBold, isMonospace, isSerif);
                _fontInfoCache.Add(fontId, fontInfo);

                return new FontAttributes(fontInfo, isUnderlined, isSmallCaps, pointSize);
            }
        }

        /// <summary>
        ///     Returns <c>true</c> when the word is numeric
        /// </summary>
        /// <returns></returns>
        public bool WordIsNumeric
        {
            get
            {
                VerifyNotDisposed();
                return HandleRef.Handle != IntPtr.Zero && TessApi.Native.ResultIteratorWordIsNumeric(HandleRef);
            }
        }

        /// <summary>
        ///     Returns the language for the recognized word
        /// </summary>
        /// <returns></returns>
        public string WordLanguage
        {
            get
            {
                VerifyNotDisposed();
                return HandleRef.Handle == IntPtr.Zero ? null : TessApi.ResultIteratorWordRecognitionLanguage(HandleRef);
            }
        }

        /// <summary>
        ///     Returns <c>true</c> when the symbol is in superscript
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     A subscript or superscript is a character (such as a number or letter) that is set slightly below or
        ///     above the normal line of type, respectively. It is usually smaller than the rest of the text.
        ///     Subscripts appear at or below the baseline, while superscripts are above. Subscripts and superscripts
        ///     are perhaps most often used in formulas, mathematical expressions, and specifications of chemical
        ///     compounds and isotopes, but have many other uses as well.
        /// </remarks>
        public bool SymbolIsSuperscript
        {
            get
            {
                VerifyNotDisposed();
                return HandleRef.Handle != IntPtr.Zero && TessApi.Native.ResultIteratorSymbolIsSuperscript(HandleRef);
            }
        }

        /// <summary>
        ///     Returns <c>true</c> when the symbol is dropcap
        /// </summary>
        /// <remarks>
        ///     A Drop Cap is the initial letter of a paragraph which sits within the margins and runs several
        ///     lines deep into the paragraph, indenting some normal-sized text in these lines
        /// </remarks>
        public bool SymbolIsDropcap
        {
            get
            {
                VerifyNotDisposed();
                return HandleRef.Handle != IntPtr.Zero && TessApi.Native.ResultIteratorSymbolIsDropcap(HandleRef);
            }
        }

        /// <summary>
        ///     Returns an instance of a choice iterator using the current symbol of interest. The <see cref="ChoiceIterator"/>
        ///     allows a one-shot iteration over the choices for this symbol and after that is is useless
        /// </summary>
        /// <returns>An instance of a <see cref="ChoiceIterator"/></returns>
        public Choice ChoiceIterator
        {
            get
            {
                var choiceIteratorHandle = TessApi.Native.ResultIteratorGetChoiceIterator(HandleRef);
                return choiceIteratorHandle == IntPtr.Zero ? null : new Choice(choiceIteratorHandle);
            }
        }
        #endregion

        #region Constructor
        internal Result(TesseractOCR.Page page, IntPtr handle) : base(page, handle)
        {
        }
        #endregion
    }
}