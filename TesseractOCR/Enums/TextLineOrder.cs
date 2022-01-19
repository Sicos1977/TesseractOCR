//
// TextLineOrder.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
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

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Enums
{
    /// <summary>
    ///     The text lines are read in the given sequence.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         For example in English the order is top-to-bottom. Chinese vertical text lines
    ///         are read right-to-left. While Mongolian is written in vertical columns
    ///         like Chinese but read left-to-right.
    ///     </para>
    ///     <para>
    ///         Note that only some combinations makes sense for example <see cref="WritingDirection.LeftToRight" /> implies
    ///         <see cref="TopToBottom" />.
    ///     </para>
    /// </remarks>
    public enum TextLineOrder
    {
        /// <summary>
        ///     The text lines form vertical columns ordered left to right.
        /// </summary>
        LeftToRight,

        /// <summary>
        ///     The text lines form vertical columns ordered right to left.
        /// </summary>
        RightToLeft,

        /// <summary>
        ///     The text lines form horizontal columns ordered top to bottom.
        /// </summary>
        TopToBottom
    }
}