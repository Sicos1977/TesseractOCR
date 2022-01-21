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

namespace TesseractOCR.Iterators
{
    /// <summary>
    ///     Represents an object that can iterate over Tesseract's page structure.
    /// </summary>
    /// <remarks>
    ///     The iterator points to Tesseract's internal page structure and is only valid while the Engine instance that created
    ///     it exists and has not been subjected to a call to Recognize since the iterator was created.
    /// </remarks>
    public class Page : DisposableBase
    {
        #region Fields
        /// <summary>
        ///     <see cref="HandleRef"/>
        /// </summary>
        protected readonly HandleRef HandleRef;

        /// <summary>
        ///     <see cref="Page"/>
        /// </summary>
        protected readonly TesseractOCR.Page PageRef;
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

                if (HandleRef.Handle == IntPtr.Zero)
                    return false;

                return TessApi.Native.PageIteratorIsAtBeginningOf(HandleRef, Level) != 0;
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

                if (HandleRef.Handle == IntPtr.Zero)
                    return false;
                return TessApi.Native.PageIteratorIsAtFinalElement(HandleRef, Level, Element) != 0;

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

                return HandleRef.Handle != IntPtr.Zero
                    ? TessApi.Native.PageIteratorBlockType(HandleRef)
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

                return HandleRef.Handle != IntPtr.Zero
                    ? Image.Create(TessApi.Native.PageIteratorGetBinaryImage(HandleRef, Level))
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

                return HandleRef.Handle != IntPtr.Zero
                    ? Image.Create(TessApi.Native.PageIteratorGetImage(HandleRef, Level, 0, PageRef.Image.Handle, out _, out _))
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

                if (HandleRef.Handle != IntPtr.Zero &&
                    TessApi.Native.PageIteratorBoundingBox(HandleRef, Level, out var x1, out var y1, out var x2, out var y2) != 0)
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

            if (HandleRef.Handle != IntPtr.Zero && TessApi.Native.PageIteratorBaseline(HandleRef, level, out var x1, out var y1, out var x2, out var y2) != 0)
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

                if (HandleRef.Handle == IntPtr.Zero)
                    return new ElementProperties(Orientation.PageUp, TextLineOrder.TopToBottom,
                        WritingDirection.LeftToRight, 0f);

                TessApi.Native.PageIteratorOrientation(HandleRef, out var orientation, out var writingDirection,
                    out var textLineOrder,
                    out var deskewAngle);

                return new ElementProperties(orientation, textLineOrder, writingDirection, deskewAngle);
            }
        }
        #endregion

        #region Constructor
        internal Page(TesseractOCR.Page page, IntPtr handle)
        {
            PageRef = page;
            HandleRef = new HandleRef(this, handle);
        }
        #endregion

        #region Begin
        /// <summary>
        ///     Moves the iterator to the start of the page
        /// </summary>
        public void Begin()
        {
            VerifyNotDisposed();
            if (HandleRef.Handle != IntPtr.Zero) TessApi.Native.PageIteratorBegin(HandleRef);
        }
        #endregion

        #region NextLevel
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

            if (HandleRef.Handle == IntPtr.Zero)
                return false;

            var result = TessApi.Native.PageIteratorNext(HandleRef, level) != 0;
            
            if (result)
            {
                Loggers.Logger.LogInformation($"Moved to the next '{level}' level");
                Level = level;
                Element = level;
            }
            else
                Loggers.Logger.LogInformation($"There is no next '{level}' level");

            return result;
        }
        #endregion

        #region NextElement
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
            if (HandleRef.Handle != IntPtr.Zero) TessApi.Native.PageIteratorDelete(HandleRef);
        }
        #endregion
    }
}