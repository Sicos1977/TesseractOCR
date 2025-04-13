//
// PageSegMode.cs
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
    ///     Represents the possible page layout analysis modes.
    /// </summary>
    public enum PageSegMode
    {
        /// <summary>
        ///     Orientation and script detection (OSD) only.
        /// </summary>
        OsdOnly = 0,

        /// <summary>
        ///     Automatic page segmentation with orientation and script detection (OSD).
        /// </summary>
        AutoOsd = 1,

        /// <summary>
        ///     Automatic page segmentation, but no OSD, or OCR.
        /// </summary>
        AutoOnly = 2,

        /// <summary>
        ///     Fully automatic page segmentation, but no OSD.
        /// </summary>
        Auto = 3,

        /// <summary>
        ///     Assume a single column of text of variable sizes.
        /// </summary>
        SingleColumn = 4,

        /// <summary>
        ///     Assume a single uniform block of vertically aligned text.
        /// </summary>
        SingleBlockVertText = 5,

        /// <summary>
        ///     Assume a single uniform block of text.
        /// </summary>
        SingleBlock = 6,

        /// <summary>
        ///     Treat the image as a single text line.
        /// </summary>
        SingleLine = 7,

        /// <summary>
        ///     Treat the image as a single word.
        /// </summary>
        SingleWord = 8,

        /// <summary>
        ///     Treat the image as a single word in a circle.
        /// </summary>
        CircleWord = 9,

        /// <summary>
        ///     Treat the image as a single character.
        /// </summary>
        SingleChar = 10,

        /// <summary>
        /// </summary>
        SparseText = 11,

        /// <summary>
        ///     Sparse text with orientation and script detection.
        /// </summary>
        SparseTextOsd = 12,

        /// <summary>
        ///     Treat the image as a single text line, bypassing hacks that are
        ///     specific to Tesseract.
        /// </summary>
        RawLine = 13,

        /// <summary>
        ///     Number of enum entries.
        /// </summary>
        Count
    }
}