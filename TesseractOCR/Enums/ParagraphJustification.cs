//
// ParagraphJustification
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2021-2023 Kees van Spelde
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
    public enum ParagraphJustification
    {
        /// <summary>
        ///     The alignment is not clearly one of the other options. This could happen for example
        ///     if there are only one or two lines of text or the text looks like source code or poetry
        /// </summary>
        Unknown,

        /// <summary>
        ///     Each line, except possibly the first, is flush to the same left tab stop
        /// </summary>
        Left,

        /// <summary>
        ///     The text lines of the paragraph are centered about a line going down through their middle of the text lines
        /// </summary>
        Center,

        /// <summary>
        ///     Each line, except possibly the first, is flush to the same right tab stop
        /// </summary>
        Right,
    }
}
