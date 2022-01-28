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
        public Words Words => new Words(IteratorHandleRef);
        #endregion

        #region Constructor
        internal TextLine(HandleRef iteratorHandle)
        {
            IteratorHandleRef = iteratorHandle;
            PageIteratorLevel = PageIteratorLevel.TextLine;
        }
        #endregion
    }
}
