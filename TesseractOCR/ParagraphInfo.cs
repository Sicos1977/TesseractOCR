//
// ParagraphInfo.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
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

using TesseractOCR.Enums;

namespace TesseractOCR
{
    /// <summary>
    ///     Returns information about a <see cref="Layout.Paragraph"/>
    /// </summary>
    public readonly struct ParagraphInfo
    {
        #region Properties
        /// <summary>
        ///     <see cref="ParagraphJustification"/>
        /// </summary>
        public ParagraphJustification Justification { get; }

        /// <summary>
        ///     Returns <c>true</c> if the <see cref="Layout.Paragraph"/> is a list item
        /// </summary>
        public bool IsListItem { get; }

        /// <summary>
        ///     Returns <c>true</c> when the letter is a crown
        /// </summary>
        /// <remarks>
        ///     Big letters as shown in supplement one are to be called Crown Letters
        /// </remarks>
        public bool IsCrown { get; }

        /// <summary>
        ///     Returns <c>true</c> when the this is the first line ident
        /// </summary>
        public int FirstLineIdent { get; }
        #endregion

        #region Constructor
        internal ParagraphInfo(ParagraphJustification justification, bool isListItem, bool isCrown, int firstLineIdent)
        {
            Justification = justification;
            IsListItem = isListItem;
            IsCrown = isCrown;
            FirstLineIdent = firstLineIdent;
        }
        #endregion
    }
}