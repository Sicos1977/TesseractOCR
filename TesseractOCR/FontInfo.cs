//
// FontInfo.cs
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

namespace TesseractOCR
{
    /// <summary>
    ///     The .NET equivalent of the ccstruct/fontinfo.h
    ///     FontInfo struct. It's missing spacing info
    ///     since we don't have any way of getting it (and
    ///     it's probably not all that useful anyway)
    /// </summary>
    public class FontInfo
    {
        #region Properties
        /// <summary>
        ///     Returns the name of the font
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Returns the id of the font
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     Returns <c>true</c> when the font is in italic
        /// </summary>
        public bool IsItalic { get; }

        /// <summary>
        ///     Returns <c>true</c> when the font is bold
        /// </summary>
        public bool IsBold { get; }

        /// <summary>
        ///     Returns <c>true</c> when the font has a fixed pitch
        /// </summary>
        public bool IsFixedPitch { get; }

        /// <summary>
        ///     Returns <c>true</c> when the font is serif
        /// </summary>
        public bool IsSerif { get; }

        /// <summary>
        ///     Returns <c>true</c> when the font is fraktur
        /// </summary>
        public bool IsFraktur { get; }
        #endregion

        #region Constructor
        internal FontInfo(string name, int id, bool isItalic, bool isBold, bool isFixedPitch, bool isSerif,
            bool isFraktur = false)
        {
            Name = name;
            Id = id;

            IsItalic = isItalic;
            IsBold = isBold;
            IsFixedPitch = isFixedPitch;
            IsSerif = isSerif;
            IsFraktur = isFraktur;
        }
        #endregion
    }
}