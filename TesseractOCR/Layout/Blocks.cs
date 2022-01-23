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

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Blocks"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Blocks : IEnumerable<Block>
    {
        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Block> GetEnumerator()
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
    ///     A single <see cref="Block"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Block : IEnumerator
    {
        #region Properties
        /// <summary>
        ///     All the available <see cref="Paragraphs"/> in this <see cref="Block"/>
        /// </summary>
        public Paragraphs Paragraphs { get; }

        /// <summary>
        ///     Returns the current <see cref="Block"/>
        /// </summary>
        public object Current { get; }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Block"/> on the <see cref="Page"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Line"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Block"/> on the <see cref="Page"/>
        /// </summary>
        public void Reset()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
