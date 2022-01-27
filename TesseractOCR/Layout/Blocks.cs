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
using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Blocks"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Blocks : IEnumerable<Block>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;
        #endregion

        #region GetEnumerator
        public IEnumerator<Block> GetEnumerator()
        {
            return new Block(_iteratorHandle);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        internal Blocks(HandleRef engineHandleRef)
        {
            var monitorHandle = new HandleRef(this, IntPtr.Zero);

            Logger.LogInformation("Analyzing page layout");

            if (TessApi.Native.BaseApiRecognize(engineHandleRef, monitorHandle) != Constants.False)
            {
                const string message = "Recognition of image failed";
                Logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            Logger.LogInformation("Getting iterator");
            _iteratorHandle = new HandleRef(this, TessApi.Native.BaseApiGetIterator(engineHandleRef));

            Logger.LogInformation("Begin iterator");
            TessApi.Native.PageIteratorBegin(_iteratorHandle);
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Block"/> on the <see cref="Page"/>
    /// </summary>
    public sealed class Block : IEnumerator<Block>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;

        private bool _first = true;
        private bool _disposed;
        #endregion

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
        public Paragraphs Paragraphs => new Paragraphs(_iteratorHandle);

        /// <summary>
        ///     Returns the text for the <see cref="Block"/>
        /// </summary>
        public string Text => TessApi.ResultIteratorGetUTF8Text(_iteratorHandle, PageIteratorLevel.Block);

        /// <summary>
        ///     Returns the confidence for the <see cref="Block"/>
        /// </summary>
        /// <returns></returns>
        public float Confidence => TessApi.Native.ResultIteratorGetConfidence(_iteratorHandle, PageIteratorLevel.Block);
        #endregion

        #region Constructor
        internal Block(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Block"/> on the <see cref="Page"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Block"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return true;
            }

            if (TessApi.Native.PageIteratorIsAtFinalElement(_iteratorHandle, PageIteratorLevel.Block, PageIteratorLevel.Block) != Constants.False)
            {
                Logger.LogInformation($"At final '{PageIteratorLevel.Block}' element");
                return false;
            }

            var result = TessApi.Native.PageIteratorNext(_iteratorHandle, PageIteratorLevel.Block) != Constants.False;
            
            if (result)
                Logger.LogInformation($"Moving to next '{PageIteratorLevel.Block}' element");

            return result;
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Block"/> on the <see cref="Page"/>
        /// </summary>
        public void Reset()
        {
            Logger.LogInformation($"Resetting to first '{PageIteratorLevel.Block}' element");
            _first = true;
            TessApi.Native.PageIteratorBegin(_iteratorHandle);
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
            TessApi.Native.PageIteratorDelete(_iteratorHandle);
            _disposed = true;
        }
        #endregion
    }
}
