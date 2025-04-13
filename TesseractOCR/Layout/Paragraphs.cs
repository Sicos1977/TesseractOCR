﻿//
// Paragraphs.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2021-2025 Kees van Spelde
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
using TesseractOCR.Helpers;
using TesseractOCR.Interop;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Paragraphs"/> in the <see cref="Block"/>
    /// </summary>
    public sealed class Paragraphs : EnumerableBase, IEnumerable<Paragraph>
    {
        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Paragraph> GetEnumerator()
        {
            return new Paragraph(EngineHandleRef, IteratorHandleRef, ImageHandleRef);
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
        internal Paragraphs(HandleRef engineHandleRef , HandleRef iteratorHandleRef, HandleRef imageHandleRef)
        {
            EngineHandleRef = engineHandleRef;
            IteratorHandleRef = iteratorHandleRef;
            ImageHandleRef = imageHandleRef;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Paragraph"/> in the <see cref="Block"/>
    /// </summary>
    public sealed class Paragraph : EnumeratorBase, IEnumerator<Paragraph>
    {
        #region Properties
        /// <summary>
        ///     Returns the current <see cref="Paragraph"/> object
        /// </summary>
        public object Current => this;

        /// <summary>
        ///     Returns the current <see cref="Paragraph"/> object
        /// </summary>
        Paragraph IEnumerator<Paragraph>.Current => this;

        /// <summary>
        ///     All the available <see cref="TextLines"/> in this <see cref="Paragraph"/>
        /// </summary>
        public TextLines TextLines => new TextLines(EngineHandleRef, IteratorHandleRef, ImageHandleRef);

        /// <summary>
        ///     Returns <c>true</c> if the iterator is at the final <see cref="Paragraph"/> in the current <see cref="Block"/>
        /// </summary>
        /// <returns><c>true</c> when at the end</returns>
        public bool IsAtFinalElement => TessApi.Native.PageIteratorIsAtFinalElement(IteratorHandleRef, PageIteratorLevel.Block, PageIteratorLevel) == Constants.True;

        /// <summary>
        ///     Returns information about the <see cref="Paragraph"/>
        /// </summary>
        public ParagraphInfo Info
        {
            get
            {
                TessApi.Native.PageIteratorParagraphInfo(IteratorHandleRef, out var justification, out var isListItem, out var isCrown, out var firstLineIndent);
                return new ParagraphInfo(justification, isListItem, isCrown, firstLineIndent);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates this object
        /// </summary>
        /// <param name="engineHandleRef">A handle reference to the Tesseract engine</param>
        /// <param name="iteratorHandleRef">A handle reference to the page iterator</param>
        /// <param name="imageHandleRef">A handle reference to the <see cref="Pix.Image"/></param>
        internal Paragraph(HandleRef engineHandleRef, HandleRef iteratorHandleRef, HandleRef imageHandleRef)
        {
            EngineHandleRef = engineHandleRef;
            IteratorHandleRef = iteratorHandleRef;
            ImageHandleRef = imageHandleRef;
            PageIteratorLevel = PageIteratorLevel.Paragraph;
        }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Paragraph"/> in the current <see cref="Block"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Paragraph"/>, otherwise <c>false</c></returns>
        public new bool MoveNext()
        {
            if (First)
            {
                First = false;
                return true;
            }

            if (!IsAtFinalElement) return base.MoveNext();
            Logger.LogInformation($"At final '{PageIteratorLevel}' element");
            return false;
        }
        #endregion
    }
}
