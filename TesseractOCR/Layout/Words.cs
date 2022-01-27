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
using TesseractOCR.Interop;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Words"/> in the <see cref="Line"/>
    /// </summary>
    public sealed class Words : IEnumerable<Word>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;
        #endregion

        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Word> GetEnumerator()
        {
            return new Word(_iteratorHandle);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        internal Words(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Word"/> in the <see cref="Line"/>
    /// </summary>
    public sealed class Word : IEnumerator<Word>
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
        Word IEnumerator<Word>.Current => this;

        /// <summary>
        ///     All the available <see cref="Symbols"/> in this <see cref="Word"/>
        /// </summary>
        public Symbols Symbols { get; }
        #endregion

        #region Constructor
        internal Word(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Word"/> in the <see cref="Line"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Word"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Word"/> in the <see cref="Line"/>
        /// </summary>
        public void Reset()
        {
            TessApi.Native.PageIteratorBegin(_iteratorHandle);
        }
        #endregion

        public void Dispose()
        {
            //TessApi.Native.PageIteratorDelete(_iteratorHandle);
        }
    }
}
