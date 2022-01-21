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
    ///     it exists
    ///     and has not been subjected to a call to Recognize since the iterator was created.
    /// </remarks>
    public class PageIterator : DisposableBase
    {
        #region Fields
        protected readonly HandleRef Handle;
        protected readonly Page Page;
        #endregion

        #region Constructor
        internal PageIterator(Page page, IntPtr handle)
        {
            Page = page;
            Handle = new HandleRef(this, handle);
        }
        #endregion

        #region BlockType
        public PolyBlockType BlockType
        {
            get
            {
                VerifyNotDisposed();

                return Handle.Handle == IntPtr.Zero
                    ? PolyBlockType.Unknown
                    : TessApi.Native.PageIteratorBlockType(Handle);
            }
        }
        #endregion

        #region Begin
        /// <summary>
        ///     Moves the iterator to the start of the page.
        /// </summary>
        public void Begin()
        {
            VerifyNotDisposed();
            if (Handle.Handle != IntPtr.Zero) TessApi.Native.PageIteratorBegin(Handle);
        }
        #endregion

        #region Next
        /// <summary>
        ///     Moves to the start of the next element at the given level.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool Next(PageIteratorLevel level)
        {
            VerifyNotDisposed();

            if (Handle.Handle == IntPtr.Zero)
                return false;

            return TessApi.Native.PageIteratorNext(Handle, level) != 0;
        }

        /// <summary>
        ///     Moves the iterator to the next <paramref name="element" /> iff the iterator is not currently pointing to the last
        ///     <paramref name="element" /> in the specified <paramref name="level" /> (i.e. the last word in the paragraph).
        /// </summary>
        /// <param name="level">The iterator level.</param>
        /// <param name="element">The page level.</param>
        /// <returns>
        ///     <c>True</c> iff there is another <paramref name="element" /> to advance too and the current element is not the
        ///     last element at the given level; otherwise returns <c>False</c>.
        /// </returns>
        public bool Next(PageIteratorLevel level, PageIteratorLevel element)
        {
            VerifyNotDisposed();

            var isAtFinalElement = IsAtFinalOf(level, element);
            return !isAtFinalElement && Next(element);
        }
        #endregion

        #region IsAtBeginningOf
        /// <summary>
        ///     Returns <c>True</c> if the iterator is at the first element at the given level.
        /// </summary>
        /// <remarks>
        ///     A possible use is to determine if a call to next(word) moved to the start of a new paragraph.
        /// </remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsAtBeginningOf(PageIteratorLevel level)
        {
            VerifyNotDisposed();

            if (Handle.Handle == IntPtr.Zero)
                return false;

            return TessApi.Native.PageIteratorIsAtBeginningOf(Handle, level) != 0;
        }
        #endregion

        #region IsAtFinalOf
        /// <summary>
        ///     Returns <c>True</c> if the iterator is positioned at the last element at the given level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsAtFinalOf(PageIteratorLevel level, PageIteratorLevel element)
        {
            VerifyNotDisposed();

            if (Handle.Handle == IntPtr.Zero)
                return false;
            return TessApi.Native.PageIteratorIsAtFinalElement(Handle, level, element) != 0;
        }
        #endregion

        #region GetBinaryImage
        public Image GetBinaryImage(PageIteratorLevel level)
        {
            VerifyNotDisposed();

            return Handle.Handle == IntPtr.Zero
                ? null
                : Image.Create(TessApi.Native.PageIteratorGetBinaryImage(Handle, level));
        }
        #endregion

        #region GetImage
        public Image GetImage(PageIteratorLevel level, int padding, out int x, out int y)
        {
            VerifyNotDisposed();

            if (Handle.Handle != IntPtr.Zero)
                return Image.Create(TessApi.Native.PageIteratorGetImage(Handle, level, padding, Page.Image.Handle, out x, out y));

            x = 0;
            y = 0;

            return null;
        }
        #endregion

        #region TryGetBoundingBox
        /// <summary>
        ///     Gets the bounding rectangle of the current element at the given level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool TryGetBoundingBox(PageIteratorLevel level, out Rect bounds)
        {
            VerifyNotDisposed();

            if (Handle.Handle != IntPtr.Zero && TessApi.Native.PageIteratorBoundingBox(Handle, level, out var x1, out var y1, out var x2, out var y2) != 0)
            {
                bounds = Rect.FromCoords(x1, y1, x2, y2);
                return true;
            }

            bounds = Rect.Empty;
            return false;
        }
        #endregion

        #region TryGetBaseline
        /// <summary>
        ///     Gets the baseline of the current element at the given level.
        /// </summary>
        /// <remarks>
        ///     The baseline is the line that passes through (x1, y1) and (x2, y2).
        ///     WARNING: with vertical text, baselines may be vertical! Returns false if there is no baseline at the current
        ///     position.
        /// </remarks>
        /// <param name="level"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool TryGetBaseline(PageIteratorLevel level, out Rect bounds)
        {
            VerifyNotDisposed();

            if (Handle.Handle != IntPtr.Zero && TessApi.Native.PageIteratorBaseline(Handle, level, out var x1, out var y1, out var x2, out var y2) != 0)
            {
                bounds = Rect.FromCoords(x1, y1, x2, y2);
                return true;
            }

            bounds = Rect.Empty;
            return false;
        }
        #endregion

        #region GetProperties
        /// <summary>
        ///     Gets the element orientation information that the iterator currently points too.
        /// </summary>
        public ElementProperties GetProperties()
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
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (Handle.Handle != IntPtr.Zero) TessApi.Native.PageIteratorDelete(Handle);
        }
        #endregion
    }
}