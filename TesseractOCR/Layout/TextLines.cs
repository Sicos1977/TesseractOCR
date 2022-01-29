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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="TextLines"/> in the <see cref="Paragraph"/>
    /// </summary>
    public sealed class TextLines : EnumerableBase, IEnumerable<TextLine>
    {
        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<TextLine> GetEnumerator()
        {
            return new TextLine(IteratorHandleRef, ImageHandleRef);
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
        internal TextLines(HandleRef iteratorHandleRef, HandleRef imageHandleRef)
        {
            IteratorHandleRef = iteratorHandleRef;
            ImageHandleRef = imageHandleRef;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="TextLine"/> in the <see cref="Paragraph"/>
    /// </summary>
    public sealed class TextLine : EnumeratorBase, IEnumerator<TextLine>
    {
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
        public Words Words => new Words(IteratorHandleRef, ImageHandleRef);
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates this object
        /// </summary>
        /// <param name="iteratorHandleRef">A handle reference to the page iterator</param>
        /// <param name="imageHandleRef">A handle reference to the <see cref="Pix.Image"/></param>
        internal TextLine(HandleRef iteratorHandleRef, HandleRef imageHandleRef)
        {
            IteratorHandleRef = iteratorHandleRef;
            ImageHandleRef = imageHandleRef;
            PageIteratorLevel = PageIteratorLevel.TextLine;
        }
        #endregion
    }
}
