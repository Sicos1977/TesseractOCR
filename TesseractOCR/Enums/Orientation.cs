//
// Orientation.cs
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

namespace TesseractOCR.Enums
{
    /// <summary>
    ///     Represents orientation that the page would need to be rotated so that .
    /// </summary>
    /// <remarks>
    ///     Orientation is defined as to what side of the page would need to correspond to the 'up' direction such that the
    ///     characters will
    ///     be read able. Another way of looking at this what direction you need to rotate you head so that "up" aligns with
    ///     Orientation,
    ///     then the characters will appear "right side up" and readable.
    ///     In short:
    ///     <list type="bullet">
    ///         <item>PageUp - Page is correctly aligned with up and no rotation is needed.</item>
    ///         <item>
    ///             PageRight - Page needs to be rotated so the right hand side is up, 90 degrees counter clockwise, to be
    ///             readable.
    ///         </item>
    ///         <item>
    ///             PageDown - Page needs to be rotated so the bottom side is up, 180 degrees counter clockwise, to be
    ///             readable.
    ///         </item>
    ///         <item>PageLeft - Page needs to be rotated so the left hand side is up, 90 degrees clockwise, to be readable.</item>
    ///     </list>
    /// </remarks>
    public enum Orientation
    {
        /// <summary>
        ///     Page is correctly aligned with up and no rotation is needed.
        /// </summary>
        PageUp,

        /// <summary>
        ///     Page needs to be rotated so the right hand side is up, 90 degrees counter clockwise, to be readable.
        /// </summary>
        PageRight,

        /// <summary>
        ///     Page needs to be rotated so the bottom side is up, 180 degrees counter clockwise, to be readable.
        /// </summary>
        PageDown,

        /// <summary>
        ///     Page needs to be rotated so the left hand side is up, 90 degrees clockwise, to be readable.
        /// </summary>
        PageLeft
    }
}