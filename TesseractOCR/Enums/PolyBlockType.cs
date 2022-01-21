//
// PolyBlockType.cs
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
    ///     The poly block type
    /// </summary>
    public enum PolyBlockType
    {
        /// <summary>
        ///     The type is not known yet, keep as first element
        /// </summary>
        Unknown,

        /// <summary>
        ///     The text is inside a column
        /// </summary>
        FlowingText,

        /// <summary>
        ///     The text spans more than one column
        /// </summary>
        HeadingText,

        /// <summary>
        ///     The text is in a cross-column pull-out region
        /// </summary>
        PullOutText,

        /// <summary>
        ///     The portion belongs to an equation region
        /// </summary>
        Equation,

        /// <summary>
        ///     The partition has an inline equation
        /// </summary>
        InlineEquation,

        /// <summary>
        ///     The partition belongs to a Table region
        /// </summary>
        Table,

        /// <summary>
        ///     Text line runs vertically
        /// </summary>
        VerticalText,

        /// <summary>
        ///     Text that belongs to an image
        /// </summary>
        CaptionText,

        /// <summary>
        ///     Image that lives inside a column
        /// </summary>
        FlowingImage,

        /// <summary>
        ///     Image that spans more than one column
        /// </summary>
        HeadingImage,

        /// <summary>
        ///     Image that is in a cross-column pull-out region
        /// </summary>
        PullOutImage,

        /// <summary>
        ///     Horizontal Line
        /// </summary>
        HorizontalLine,

        /// <summary>
        ///     Vertical Line
        /// </summary>
        VerticalLine,

        /// <summary>
        ///     Lies outside any column.
        /// </summary>
        Noise,

        /// <summary>
        ///     Count
        /// </summary>
        Count
    }
}