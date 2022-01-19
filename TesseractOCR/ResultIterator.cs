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
using TesseractOCR.Enums;
using TesseractOCR.Interop;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR
{
    public sealed class ResultIterator : PageIterator
    {
        #region Fields
        private readonly Dictionary<int, FontInfo> _fontInfoCache = new Dictionary<int, FontInfo>();
        #endregion

        #region Constructor
        internal ResultIterator(Page page, IntPtr handle) : base(page, handle)
        {
        }
        #endregion

        #region GetConfidence
        public float GetConfidence(PageIteratorLevel level)
        {
            VerifyNotDisposed();

            return Handle.Handle == IntPtr.Zero ? 0f : TessApi.Native.ResultIteratorGetConfidence(Handle, level);
        }
        #endregion

        #region GetText
        public string GetText(PageIteratorLevel level)
        {
            VerifyNotDisposed();

            return Handle.Handle == IntPtr.Zero ? string.Empty : TessApi.ResultIteratorGetUTF8Text(Handle, level);
        }
        #endregion

        #region GetWordFontAttributes
        public FontAttributes GetWordFontAttributes()
        {
            VerifyNotDisposed();

            if (Handle.Handle == IntPtr.Zero)
                return null;

            // per docs (ltrresultiterator.h:104 as of 4897796 in github:tesseract-ocr/tesseract)
            // this return value points to an internal table and should not be deleted.
            var nameHandle =
                TessApi.Native.ResultIteratorWordFontAttributes(
                    Handle,
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
        #endregion

        #region GetWordRecognitionLanguage
        public string GetWordRecognitionLanguage()
        {
            VerifyNotDisposed();

            return Handle.Handle == IntPtr.Zero ? null : TessApi.ResultIteratorWordRecognitionLanguage(Handle);
        }
        #endregion

        #region GetWordIsFromDictionary
        public bool GetWordIsFromDictionary()
        {
            VerifyNotDisposed();
            return Handle.Handle != IntPtr.Zero && TessApi.Native.ResultIteratorWordIsFromDictionary(Handle);
        }
        #endregion

        #region GetWordIsNumeric
        public bool GetWordIsNumeric()
        {
            VerifyNotDisposed();
            return Handle.Handle != IntPtr.Zero && TessApi.Native.ResultIteratorWordIsNumeric(Handle);
        }
        #endregion

        #region GetSymbolIsSuperscript
        public bool GetSymbolIsSuperscript()
        {
            VerifyNotDisposed();
            return Handle.Handle != IntPtr.Zero && TessApi.Native.ResultIteratorSymbolIsSuperscript(Handle);
        }
        #endregion

        #region GetSymbolIsSubscript
        public bool GetSymbolIsSubscript()
        {
            VerifyNotDisposed();
            return Handle.Handle != IntPtr.Zero && TessApi.Native.ResultIteratorSymbolIsSubscript(Handle);
        }
        #endregion

        #region GetSymbolIsDropcap
        public bool GetSymbolIsDropcap()
        {
            VerifyNotDisposed();
            return Handle.Handle != IntPtr.Zero && TessApi.Native.ResultIteratorSymbolIsDropcap(Handle);
        }
        #endregion

        #region GetChoiceIterator
        /// <summary>
        ///     Gets an instance of a choice iterator using the current symbol of interest. The ChoiceIterator allows a one-shot
        ///     iteration over the
        ///     choices for this symbol and after that is is useless.
        /// </summary>
        /// <returns>an instance of a Choice Iterator</returns>
        public ChoiceIterator GetChoiceIterator()
        {
            var choiceIteratorHandle = TessApi.Native.ResultIteratorGetChoiceIterator(Handle);
            return choiceIteratorHandle == IntPtr.Zero ? null : new ChoiceIterator(choiceIteratorHandle);
        }
        #endregion
    }
}