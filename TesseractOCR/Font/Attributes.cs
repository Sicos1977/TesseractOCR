//
// FontAttributes.cs
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

namespace TesseractOCR.Font
{
    /// <summary>
    ///     This struct is the return type of ResultIterator.GetWordFontAttributes().
    ///     We can't use FontInfo directly because there are properties here that are not
    ///     accounted for in FontInfo (smallcaps, underline, etc.)  Because of the caching
    ///     scheme we're using for FontInfo objects, we can't simply augment that class since
    ///     these extra properties are not accounted for by the FontInfo's unique ID.
    /// </summary>
    public sealed class Attributes
    {
        #region Properties
        /// <summary>
        ///     Returns the font information
        /// </summary>
        public Info FontInfo { get; }

        /// <summary>
        ///     Returns <c>true</c> when the text is underlined
        /// </summary>
        public bool IsUnderlined { get; }

        /// <summary>
        ///     Returns <c>true</c> when the font is small caps
        /// </summary>
        public bool IsSmallCaps { get; }
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates this object and sets all it's needed properties
        /// </summary>
        /// <param name="fontInfo"></param>
        /// <param name="isUnderlined"></param>
        /// <param name="isSmallCaps"></param>
        internal Attributes(Info fontInfo, bool isUnderlined, bool isSmallCaps)
        {
            FontInfo = fontInfo;
            IsUnderlined = isUnderlined;
            IsSmallCaps = isSmallCaps;
        }
        #endregion
    }
}