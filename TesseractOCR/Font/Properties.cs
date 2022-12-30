//
// Properties.cs
//
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

using TesseractOCR.Enums;

namespace TesseractOCR.Font
{
    /// <summary>
    ///     Returns font properties
    /// </summary>
    public class Properties
    {
        /// <summary>
        ///     Returns the point size of the font
        /// </summary>
        /// <remarks>
        ///     Point size is returned in printers points (1/72 inch)
        /// </remarks>
        public int PointSize { get; }

        /// <summary>
        ///     Returns other font attributes
        /// </summary>
        /// <remarks>
        ///     This information is only available when using older engine modes like
        ///     <see cref="EngineMode.TesseractOnly "/> and <see cref="EngineMode.TesseractAndLstm "/>
        /// </remarks>
        public Attributes Attributes { get; }

        internal Properties(int pointSize, Attributes attributes = null)
        {
            PointSize = pointSize;
            Attributes = attributes;
        }
    }
}
