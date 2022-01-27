//
// Blocks.cs
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
using TesseractOCR.Exceptions;
using TesseractOCR.Interop;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Blocks"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Blocks : IEnumerable<Block>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;
        #endregion

        #region GetEnumerator
        public IEnumerator<Block> GetEnumerator()
        {
            return new Block(_iteratorHandle);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        internal Blocks(HandleRef engineHandleRef)
        {
            var monitorHandle = new HandleRef(this, IntPtr.Zero);

            if (TessApi.Native.BaseApiRecognize(engineHandleRef, monitorHandle) != 0)
                throw new InvalidOperationException("Recognition of image failed");

            _iteratorHandle = new HandleRef(this, TessApi.Native.BaseApiGetIterator(engineHandleRef));
            TessApi.Native.PageIteratorBegin(_iteratorHandle);
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Block"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Block : IEnumerator<Block>
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
        Block IEnumerator<Block>.Current => this;

        /// <summary>
        ///     All the available <see cref="Paragraphs"/> in this <see cref="Block"/>
        /// </summary>
        public Paragraphs Paragraphs => new Paragraphs(_iteratorHandle);
        #endregion

        #region Constructor
        internal Block(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Block"/> on the <see cref="Page"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Line"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            if (TessApi.Native.PageIteratorIsAtBeginningOf(_iteratorHandle, PageIteratorLevel.Block) != 0)
                return true;

            return TessApi.Native.PageIteratorNext(_iteratorHandle, PageIteratorLevel.Block) != 0;
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Block"/> on the <see cref="Page"/>
        /// </summary>
        public void Reset()
        {
            TessApi.Native.PageIteratorBegin(_iteratorHandle);
        }
        #endregion

        public void Dispose()
        {
            TessApi.Native.PageIteratorDelete(_iteratorHandle);
        }
    }
}
