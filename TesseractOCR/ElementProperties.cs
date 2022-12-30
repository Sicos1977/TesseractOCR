//
// ElementProperties.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
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

namespace TesseractOCR
{
    /// <summary>
    ///     Describes the element properties at the current <see cref="PageIteratorLevel"/>
    /// </summary>
    public class ElementProperties
    {
        #region Properties
        /// <summary>
        ///     Returns the <see cref="Orientation" />
        /// </summary>
        public Orientation Orientation { get; }

        /// <summary>
        ///     Returns the <see cref="TextLineOrder" />
        /// </summary>
        public TextLineOrder TextLineOrder { get; }

        /// <summary>
        ///     Returns the <see cref="WritingDirection" />
        /// </summary>
        public WritingDirection WritingDirection { get; }

        /// <summary>
        ///     Returns the angle the page would need to be rotated to deskew the information
        /// </summary>
        public float DeskewAngle { get; }
        #endregion

        #region Constructor
        /// <summary>
        ///     The information for the element at the current <see cref="PageIteratorLevel"/>
        /// </summary>
        /// <param name="orientation"><see cref="Orientation"/></param>
        /// <param name="textLineOrder"><see cref="TextLineOrder"/></param>
        /// <param name="writingDirection"><see cref="WritingDirection"/></param>
        /// <param name="deskewAngle"><see cref="DeskewAngle"/></param>
        public ElementProperties(Orientation orientation, TextLineOrder textLineOrder, WritingDirection writingDirection, float deskewAngle)
        {
            Orientation = orientation;
            TextLineOrder = textLineOrder;
            WritingDirection = writingDirection;
            DeskewAngle = deskewAngle;
        }
        #endregion
    }
}