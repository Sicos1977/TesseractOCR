//
// EnumerableBase.cs
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

using System.Runtime.InteropServices;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     Base enumerable
    /// </summary>
    public class EnumerableBase
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by Tesseract engine
        /// </summary>
        protected HandleRef EngineHandleRef;
        
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        protected HandleRef IteratorHandleRef;

        /// <summary>
        ///     <see cref="Pix.Image"/>
        /// </summary>
        protected HandleRef ImageHandleRef;
        #endregion
    }
}