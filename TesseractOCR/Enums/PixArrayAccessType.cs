//
// PixArrayAccessType.cs
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
    ///     Determines how <see cref="Pix" /> of a <see cref="PixArray" /> structure are accessed.
    /// </summary>
    public enum PixArrayAccessType
    {
        /// <summary>
        ///     Stuff it in; no copy, clone or copy-clone.
        /// </summary>
        Insert = 0,

        /// <summary>
        ///     Make/use a copy of the object.
        /// </summary>
        Copy = 1,

        /// <summary>
        ///     Make/use clone (ref count) of the object
        /// </summary>
        Clone = 2,

        /// <summary>
        ///     Make a new object and fill with with clones of each object in the array(s)
        /// </summary>
        CopyClone = 3
    }
}