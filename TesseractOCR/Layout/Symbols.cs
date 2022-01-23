//
// Symbols.cs
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
    ///     All the <see cref="Symbols"/> in the <see cref="Word"/>
    /// </summary>
    public sealed class Symbols : IEnumerable<Symbol>
    {
        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Symbol> GetEnumerator()
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
    ///     A single <see cref="Symbol"/> in the <see cref="Word"/>
    /// </summary>
    public sealed class Symbol : IEnumerator
    {
        #region Properties
        /// <summary>
        ///     Returns the current <see cref="Symbol"/>
        /// </summary>
        public object Current { get; }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Symbol"/> in the <see cref="Word"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Symbol"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Symbol"/> in the <see cref="Word"/>
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
