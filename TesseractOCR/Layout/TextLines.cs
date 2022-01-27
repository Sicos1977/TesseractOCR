//
// Lines.cs
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
using TesseractOCR.Enums;
using TesseractOCR.Interop;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="TextLines"/> in the <see cref="Paragraph"/>
    /// </summary>
    public sealed class TextLines : IEnumerable<Line>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;
        #endregion

        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Line> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="TextLine"/> in the <see cref="Paragraph"/>
    /// </summary>
    public sealed class TextLine : IEnumerator<TextLine>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;
        #endregion

        #region Properties
        public object Current => this;

        /// <summary>
        ///     Returns the current element
        /// </summary>
        TextLine IEnumerator<TextLine>.Current => this;

        /// <summary>
        ///     All the available <see cref="Words"/> in this <see cref="Line"/>
        /// </summary>
        public Words Words { get; }
        #endregion

        #region Constructor
        internal TextLine(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="TextLine"/> in the <see cref="Paragraph"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="TextLine"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            if (TessApi.Native.PageIteratorIsAtBeginningOf(_iteratorHandle, PageIteratorLevel.TextLine) != 0)
                return true;

            return TessApi.Native.PageIteratorNext(_iteratorHandle, PageIteratorLevel.TextLine) != 0;
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Line"/> in the <see cref="Paragraph"/>
        /// </summary>
        public void Reset()
        {
            TessApi.Native.PageIteratorBegin(_iteratorHandle);
        }

        public void Dispose()
        {
            //TessApi.Native.PageIteratorDelete(_iteratorHandle);
        }
        #endregion
    }
}
