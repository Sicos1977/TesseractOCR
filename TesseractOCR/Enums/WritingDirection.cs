//
// WritingDirection.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2025 Kees van Spelde
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
    ///     The grapheme cluster within a line of text are laid out logically in this direction,
    ///     judged when looking at the text line rotated so that Orientation is "page up".
    /// </summary>
    public enum WritingDirection
    {
        /// <summary>
        ///     The text line from the left hand side to the right hand side when the page is rotated so it's orientation is
        ///     <see cref="Orientation.PageUp" />.
        /// </summary>
        LeftToRight,

        /// <summary>
        ///     The text line from the right hand side to the left hand side when the page is rotated so it's orientation is
        ///     <see cref="Orientation.PageUp" />.
        /// </summary>
        RightToLeft,

        /// <summary>
        ///     The text line from the top to the bottom of the page when the page is rotated so it's orientation is
        ///     <see cref="Orientation.PageUp" />.
        /// </summary>
        TopToBottom
    }
}