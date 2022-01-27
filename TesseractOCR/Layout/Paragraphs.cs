//
// Paragraphs.cs
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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.Interop;
using TesseractOCR.Loggers;

namespace TesseractOCR.Layout
{
    /// <summary>
    ///     All the <see cref="Paragraphs"/> in the <see cref="Block"/>
    /// </summary>
    public sealed class Paragraphs : IEnumerable<Paragraph>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;
        #endregion

        #region GetEnumerator
        /// <inheritdoc />
        public IEnumerator<Paragraph> GetEnumerator()
        {
            return new Paragraph(_iteratorHandle);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        internal Paragraphs(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion
    }

    /// <summary>
    ///     A single <see cref="Paragraph"/> in the <see cref="Block"/>
    /// </summary>
    public sealed class Paragraph : IEnumerator<Paragraph>
    {
        #region Fields
        /// <summary>
        ///     Handle that is returned by TessApi.Native.BaseApiGetIterator
        /// </summary>
        private readonly HandleRef _iteratorHandle;

        private bool _first = true;
        #endregion

        #region Properties
        /// <summary>
        ///     Returns the current <see cref="Paragraph"/> object
        /// </summary>
        public object Current => this;

        /// <summary>
        ///     Returns the current <see cref="Paragraph"/> object
        /// </summary>
        Paragraph IEnumerator<Paragraph>.Current => this;

        /// <summary>
        ///     All the available <see cref="TextLines"/> in this <see cref="Paragraph"/>
        /// </summary>
        public TextLines TextLines => new TextLines(_iteratorHandle);

        /// <summary>
        ///     Returns the text for the <see cref="Paragraph"/>
        /// </summary>
        public string Text => TessApi.ResultIteratorGetUTF8Text(_iteratorHandle, PageIteratorLevel.Paragraph);

        /// <summary>
        ///     Returns the confidence for the <see cref="Paragraph"/>
        /// </summary>
        public float Confidence => TessApi.Native.ResultIteratorGetConfidence(_iteratorHandle, PageIteratorLevel.Paragraph);
        #endregion

        #region Constructor
        internal Paragraph(HandleRef iteratorHandle)
        {
            _iteratorHandle = iteratorHandle;
        }
        #endregion

        #region MoveNext
        /// <summary>
        ///     Moves to the next <see cref="Paragraph"/> in the <see cref="Block"/>
        /// </summary>
        /// <returns><c>true</c> when there is a next <see cref="Paragraph"/>, otherwise <c>false</c></returns>
        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return true;
            }

            if (TessApi.Native.PageIteratorIsAtFinalElement(_iteratorHandle, PageIteratorLevel.Block, PageIteratorLevel.Paragraph) != Constants.False)
            {
                Logger.LogInformation($"At final '{PageIteratorLevel.Paragraph}' element");
                return false;
            }

            var result = TessApi.Native.PageIteratorNext(_iteratorHandle, PageIteratorLevel.Paragraph) != Constants.False;

            if (result)
                Logger.LogInformation($"Moving to next '{PageIteratorLevel.Paragraph}' element");

            return result;
        }
        #endregion

        #region Reset
        /// <summary>
        ///     Resets the enumerator to the first <see cref="Paragraph"/> in the <see cref="Block"/>
        /// </summary>
        public void Reset()
        {
            Logger.LogInformation($"Resetting to first '{PageIteratorLevel.Paragraph}' element");
            _first = true;
            TessApi.Native.PageIteratorBegin(_iteratorHandle);
        }
        #endregion

        #region Dispose
        /// <summary>
        ///     Does not do a thing, we have to implement it because of the <see cref="IEnumerator"/> interface
        /// </summary>
        public void Dispose()
        {
            // We have to implement this method because of the IEnumerator interface
            // but we have nothing to do here so just ignore it
        }
        #endregion
    }
}
