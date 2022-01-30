//
// Blocks.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.Helpers;
using TesseractOCR.Interop;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Blocks"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Blocks : EnumerableBase, IEnumerable<Block>, IDisposable
    {
        #region Fields
        /// <summary>
        ///     Flag to check if we already disposed everything
        /// </summary>
        private bool _disposed;
        #endregion

        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Block> GetEnumerator()
        {
            return new Block(EngineHandleRef, IteratorHandleRef, ImageHandleRef);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates this object
        /// </summary>
        /// <param name="engineHandleRef">A handle reference to the Tesseract engine</param>
        /// <param name="imageHandleRef">A handle reference to the <see cref="Pix.Image"/></param>
        internal Blocks(HandleRef engineHandleRef, HandleRef imageHandleRef)
        {
            Logger.LogInformation("Getting iterator");
            EngineHandleRef = engineHandleRef;
            IteratorHandleRef = new HandleRef(this, TessApi.Native.BaseApiGetIterator(engineHandleRef));
            ImageHandleRef = imageHandleRef;
            Logger.LogInformation("Begin iterator");
            TessApi.Native.PageIteratorBegin(IteratorHandleRef);
        }
        #endregion

        #region Dispose
        /// <summary>
        ///     Cleans up the page iterator
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            Logger.LogInformation("Disposing page iterator");
            TessApi.Native.PageIteratorDelete(IteratorHandleRef);
            _disposed = true;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Block"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Block : EnumeratorBase, IEnumerator<Block>
    {
        #region Properties
        /// <summary>
        ///     Returns the current <see cref="Block"/> object
        /// </summary>
        public object Current => this;

        /// <summary>
        ///     Returns the current <see cref="Block"/> object
        /// </summary>
        Block IEnumerator<Block>.Current => this;

        /// <summary>
        ///     All the available <see cref="Paragraphs"/> in this <see cref="Block"/>
        /// </summary>
        public Paragraphs Paragraphs => new Paragraphs(EngineHandleRef, IteratorHandleRef, ImageHandleRef);

        /// <summary>
        ///     Returns the <see cref="PolyBlockType"/>
        /// </summary>
        /// <returns></returns>
        public PolyBlockType BlockType => TessApi.Native.PageIteratorBlockType(IteratorHandleRef);
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates this object
        /// </summary>
        /// <param name="engineHandleRef">A handle reference to the Tesseract engine</param>
        /// <param name="iteratorHandleRef">A handle reference to the page iterator</param>
        /// <param name="imageHandleRef">A handle reference to the <see cref="Pix.Image"/></param>
        internal Block(HandleRef engineHandleRef, HandleRef iteratorHandleRef, HandleRef imageHandleRef)
        {
            EngineHandleRef = engineHandleRef;
            IteratorHandleRef = iteratorHandleRef;
            ImageHandleRef = imageHandleRef;
            PageIteratorLevel = PageIteratorLevel.Block;
        }
        #endregion
    }
}
