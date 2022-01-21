//
// PageIterator.cs
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
using TesseractOCR.Enums;
using TesseractOCR.Interop;
using TesseractOCR.Pix;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR
{
    /// <summary>
    ///     Represents an object that can iterate over Tesseract's page structure.
    /// </summary>
    /// <remarks>
    ///     The iterator points to Tesseract's internal page structure and is only valid while the Engine instance that created
    ///     it exists and has not been subjected to a call to Recognize since the iterator was created.
    /// </remarks>
    public class PageIterator : DisposableBase
    {
        #region Fields
        protected readonly HandleRef Handle;
        protected readonly Page Page;
        #endregion

        #region Properties 
        /// <summary>
        ///     Returns the current <see cref="PageIteratorLevel"/>
        /// </summary>
        public PageIteratorLevel Level { get; private set; }

        /// <summary>
        ///     Returns the current element <see cref="PageIteratorLevel"/>
        /// </summary>
        public PageIteratorLevel Element { get; private set; }

        /// <summary>
        ///     Returns <c>true</c> if the iterator is at the first <see cref="Element"/> at the given <see cref="Level"/>.
        /// </summary>
        /// <remarks>
        ///     A possible use is to determine if a call to next(word) moved to the start of a new paragraph.
        /// </remarks>
        /// <returns></returns>
        public bool IsAtBeginning
        {
            get
            {
                VerifyNotDisposed();

                if (Handle.Handle == IntPtr.Zero)
                    return false;

                return TessApi.Native.PageIteratorIsAtBeginningOf(Handle, Level) != 0;
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if the iterator is positioned at the last element at the given level
        /// </summary>
        /// <returns></returns>
        public bool IsAtFinal
        {
            get
            {
                VerifyNotDisposed();

                if (Handle.Handle == IntPtr.Zero)
                    return false;
                return TessApi.Native.PageIteratorIsAtFinalElement(Handle, Level, Element) != 0;

            }
        }

        /// <summary>
        ///     Returns the <see cref="PolyBlockType"/>
        /// </summary>
        public PolyBlockType BlockType
        {
            get
            {
                VerifyNotDisposed();

                return Handle.Handle != IntPtr.Zero
                    ? TessApi.Native.PageIteratorBlockType(Handle)
                    : PolyBlockType.Unknown;
            }
        }

        /// <summary>
        ///     Returns a binary (gray) <see cref="Pix.Image"/> at the current <see cref="Level"/>
        /// </summary>
        /// <returns>The <see cref="Pix.Image"/> or <c>null</c> when it fails</returns>
        public Image BinaryImage
        {
            get
            {
                VerifyNotDisposed();

                return Handle.Handle != IntPtr.Zero
                    ? Image.Create(TessApi.Native.PageIteratorGetBinaryImage(Handle, Level))
                    : null;
            }
        }

        /// <summary>
        ///     Returns an <see cref="Pix.Image"/> at the current <see cref="Level"/>
        /// </summary>
        /// <returns>The <see cref="Pix.Image"/> or <c>null</c> when it fails</returns>
        public Image Image
        {
            get
            {
                VerifyNotDisposed();

                return Handle.Handle != IntPtr.Zero
                    ? Image.Create(TessApi.Native.PageIteratorGetImage(Handle, Level, 0, Page.Image.Handle, out _, out _))
                    : null;
            }
        }

        /// <summary>
        ///     Returns the bounding <see cref="Rect"/> of the current <see cref="Element"/> at the given <see cref="Level"/>
        /// </summary>
        /// <returns>The <see cref="Rect"/> or <c>null</c> returns when it fails</returns>
        public Rect? BoundingBox
        {
            get
            {
                VerifyNotDisposed();

                if (Handle.Handle != IntPtr.Zero &&
                    TessApi.Native.PageIteratorBoundingBox(Handle, Level, out var x1, out var y1, out var x2, out var y2) != 0)
                    return Rect.FromCoords(x1, y1, x2, y2);

                return null;
            }
        }

        /// <summary>
        ///     Returns the baseline of the current <see cref="Element"/> at the current <see cref="Level"/>
        /// </summary>
        /// <remarks>
        ///     The baseline is the line that passes through (x1, y1) and (x2, y2).
        ///     WARNING: with vertical text, baselines may be vertical! Returns false if there is no baseline at the current
        ///     position.
        /// </remarks>
        /// <returns>The <see cref="Rect"/> or <c>null</c> when it fails</returns>
        public Rect? Baseline(PageIteratorLevel level)
        {
            VerifyNotDisposed();

            if (Handle.Handle != IntPtr.Zero && TessApi.Native.PageIteratorBaseline(Handle, level, out var x1, out var y1, out var x2, out var y2) != 0)
                return Rect.FromCoords(x1, y1, x2, y2);

            return null;
        }

        /// <summary>
        ///     Returns the <see cref="Element"/> orientation information that the iterator currently points too
        /// </summary>
        public ElementProperties Properties
        {
            get
            {
                VerifyNotDisposed();

                if (Handle.Handle == IntPtr.Zero)
                    return new ElementProperties(Orientation.PageUp, TextLineOrder.TopToBottom,
                        WritingDirection.LeftToRight, 0f);

                TessApi.Native.PageIteratorOrientation(Handle, out var orientation, out var writingDirection,
                    out var textLineOrder,
                    out var deskewAngle);

                return new ElementProperties(orientation, textLineOrder, writingDirection, deskewAngle);
            }
        }
        #endregion

        #region Constructor
        internal PageIterator(Page page, IntPtr handle)
        {
            Page = page;
            Handle = new HandleRef(this, handle);
        }
        #endregion

        #region Begin
        /// <summary>
        ///     Moves the iterator to the start of the page
        /// </summary>
        public void Begin()
        {
            VerifyNotDisposed();
            if (Handle.Handle != IntPtr.Zero) TessApi.Native.PageIteratorBegin(Handle);
        }
        #endregion

        #region Next
        /// <summary>
        ///     Moves to the start of the next element at the given <see cref="PageIteratorLevel"/>.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool NextLevel(PageIteratorLevel level)
        {
            VerifyNotDisposed();

            if (Handle.Handle == IntPtr.Zero)
                return false;

            var result = TessApi.Native.PageIteratorNext(Handle, level) != 0;
            
            if (result)
            {
                Loggers.Logger.LogInformation($"Moved to the next '{level}' level");
                Level = level;
            }
            else
                Loggers.Logger.LogInformation($"There is no next '{level}' level");

            return result;
        }

        /// <summary>
        ///     Moves the iterator to the next <paramref name="element" />. If the iterator is not currently pointing to the last
        ///     <paramref name="element" /> in the current <see cref="Level"/> (i.e. the last word in the paragraph).
        /// </summary>
        /// <param name="element">The <see cref="PageIteratorLevel"/></param>
        /// <returns>
        ///     Returns <c>true</c> if there is another <paramref name="element" /> to advance too and the current element is not the
        ///     last element at the given level, otherwise returns <c>false</c>.
        /// </returns>
        public bool NextElement(PageIteratorLevel element)
        {
            VerifyNotDisposed();

            var result = !IsAtFinal && NextLevel(element);
            
            if (result)
            {
                Loggers.Logger.LogInformation($"Moved to the next '{element}' element");
                Element = element;
            }
            else
                Loggers.Logger.LogInformation($"There is no next '{element}' element");

            return result;
        }
        #endregion

        #region Dispose
        /// <summary>
        ///     Disposes this object
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (Handle.Handle != IntPtr.Zero) TessApi.Native.PageIteratorDelete(Handle);
        }
        #endregion
    }
}