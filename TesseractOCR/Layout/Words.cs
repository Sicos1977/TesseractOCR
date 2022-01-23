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

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Words"/> in the <see cref="Line"/>
    /// </summary>
    public sealed class Words : IEnumerable<Word>
    {
        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Word> GetEnumerator()
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
    ///     A single <see cref="Word"/> in the <see cref="Line"/>
    /// </summary>
    public sealed class Word : IEnumerator
    {
        #region Properties
        /// <summary>
        ///     All the available <see cref="Symbols"/> in this <see cref="Word"/>
        /// </summary>
        public Symbols Symbols { get; }

        /// <summary>
        ///     Returns the current <see cref="Word"/>
        /// </summary>
        public object Current { get; }
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
            throw new NotImplementedException();
        }
        #endregion
    }
}
