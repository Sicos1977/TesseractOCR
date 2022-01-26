//
// Page.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
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


using System;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.Interop;

namespace TesseractOCR.Layout
{
    public sealed class Page : IDisposable
    {
        #region Fields

        private readonly HandleRef _engineHandle;
        private HandleRef _analyseLayoutHandle;
        #endregion

        #region Properties
        /// <summary>
        ///     Returns an enumerator to all the available <see cref="Blocks"/>s on the page
        /// </summary>
        public Blocks Blocks => new Blocks(_analyseLayoutHandle);
        #endregion

        /// <summary>
        ///     Creates this object
        /// </summary>
        /// <param name="engineHandle">The Tesseract engine handle</param>
        internal Page(IntPtr engineHandle)
        {
            _engineHandle = new HandleRef(this, engineHandle);
            _analyseLayoutHandle = new HandleRef(this, TessApi.Native.BaseApiAnalyseLayout(_engineHandle));

            foreach (var block in Blocks)
            {
                block.
            }
        }

        public void Dispose()
        {
        }
    }
}
