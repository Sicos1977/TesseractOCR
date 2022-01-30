//
// Paragraphs.cs
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
    }
}
