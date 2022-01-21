//
// ChoiceIterator.cs
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

using System;
using System.Runtime.InteropServices;
using TesseractOCR.Interop;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR
{
    /// <summary>
    ///     Class to iterate over the classifier choices for a single symbol.
    /// </summary>
    public sealed class ChoiceIterator : DisposableBase
    {
        #region Fields
        private readonly HandleRef _handleRef;
        #endregion

        #region Properties
        /// <summary>
        ///     Returns the confidence of the current choice
        /// </summary>
        /// <remarks>
        ///     The number should be interpreted as a percent probability (0.0f - 100.0f)
        /// </remarks>
        /// <returns>float</returns>
        public float Confidence
        {
            get
            {
                VerifyNotDisposed();
                return _handleRef.Handle == IntPtr.Zero ? 0f : TessApi.Native.ChoiceIteratorGetConfidence(_handleRef);
            }
        }

        /// <summary>
        ///     Returns the text string for the current choice
        /// </summary>
        /// <returns>string</returns>
        public string Text
        {
            get
            {
                VerifyNotDisposed();
                return _handleRef.Handle == IntPtr.Zero ? string.Empty : TessApi.ChoiceIteratorGetUTF8Text(_handleRef);
            }
        }
        #endregion

        #region Constructor
        internal ChoiceIterator(IntPtr handle)
        {
            _handleRef = new HandleRef(this, handle);
        }
        #endregion

        #region Next
        /// <summary>
        ///     Moves to the next choice for the symbol and returns <c>false</c> if there are none left
        /// </summary>
        /// <returns>true|false</returns>
        public bool Next()
        {
            VerifyNotDisposed();

            if (_handleRef.Handle == IntPtr.Zero)
                return false;

            return TessApi.Native.ChoiceIteratorNext(_handleRef) != 0;
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (_handleRef.Handle != IntPtr.Zero) 
                TessApi.Native.ChoiceIteratorDelete(_handleRef);
        }
        #endregion
    }
}