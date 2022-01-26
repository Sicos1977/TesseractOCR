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
using TesseractOCR.Interop;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Blocks"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Blocks : IEnumerable
    {
        private readonly HandleRef _handleRef;

        #region GetEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Block GetEnumerator()
        {
            return new Block(_handleRef);
        }
        #endregion

        #region Constructor
        internal Blocks(HandleRef handleRef)
        {
            _handleRef = handleRef;
            TessApi.Native.PageIteratorBegin(_handleRef);
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Block"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Block : IEnumerator
    {
        #region Fields
        private HandleRef _handleRef;
        #endregion

        #region Properties
        /// <summary>
        ///     All the available <see cref="Paragraphs"/> in this <see cref="Block"/>
        /// </summary>
        public Paragraphs Paragraphs { get; }

        /// <summary>
        ///     Returns the current <see cref="Block"/>
        /// </summary>
        public object Current 
        {
            get
            {
                try
                {
                    return _blocks[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
        #endregion

        #region Constructor
        internal Block(HandleRef handleRef)
        {
            _handleRef = handleRef;
            
            TessApi.Native.PageIteratorNext(handleRef, iteratorLevel) != 0;
        }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Block"/> on the <see cref="Page"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Line"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            _position++;
            return _position < _blocks.Count;
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Block"/> on the <see cref="Page"/>
        /// </summary>
        public void Reset()
        {
            TessApi.Native.PageIteratorBegin(_handleRef);
        }
        #endregion
    }
}
