//
// PixArray.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.Internal;
using TesseractOCR.Interop;
// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Pix
{
    /// <summary>
    ///     Represents an array of <see cref="Image" />.
    /// </summary>
    public class Array : DisposableBase, IEnumerable<Image>
    {
        #region Fields
        /// <summary>
        ///     Gets the handle to the underlying PixA structure.
        /// </summary>
        private HandleRef _handle;
        private int _count;
        private readonly int _version;

        public string Filename { get; set; }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the number of <see cref="Image" /> contained in the array.
        /// </summary>
        public int Count
        {
            get
            {
                VerifyNotDisposed();
                return _count;
            }
        }
        #endregion

        #region Static Constructors
        /// <summary>
        ///     Loads the multi-page tiff located at <paramref name="filename" />.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Array LoadMultiPageTiffFromFile(string filename)
        {
            var pixaHandle = LeptonicaApi.Native.pixaReadMultipageTiff(filename);
            if (pixaHandle == IntPtr.Zero) throw new IOException($"Failed to load image '{filename}'.");

            return new Array(pixaHandle) { Filename = filename };
        }

        public static Array Create(int n)
        {
            var pixaHandle = LeptonicaApi.Native.pixaCreate(n);
            if (pixaHandle == IntPtr.Zero) throw new IOException("Failed to create PixArray");

            return new Array(pixaHandle);
        }
        #endregion

        #region Constructor
        private Array(IntPtr handle)
        {
            _handle = new HandleRef(this, handle);
            _version = 1;

            // These will need to be updated whenever the PixA structure changes (i.e. a Pix is added or removed) though at the moment that isn't a problem.
            _count = LeptonicaApi.Native.pixaGetCount(_handle);
        }
        #endregion

        #region Add
        /// <summary>
        ///     Add the specified pix to the end of the pix array.
        /// </summary>
        /// <remarks>
        ///     PixArrayAccessType.Insert is not supported as the managed Pix object will attempt to release the pix when
        ///     it goes out of scope creating an access exception.
        /// </remarks>
        /// <param name="pix">The pix to add.</param>
        /// <param name="copyflag">Determines if a clone or copy of the pix is inserted into the array.</param>
        /// <returns></returns>
        public bool Add(Image pix, PixArrayAccessType copyflag = PixArrayAccessType.Clone)
        {
            Guard.RequireNotNull("pix", pix);
            Guard.Require(nameof(copyflag), copyflag == PixArrayAccessType.Clone || copyflag == PixArrayAccessType.Copy,
                "Copy flag must be either copy or clone but was {0}.", copyflag);

            var result = LeptonicaApi.Native.pixaAddPix(_handle, pix.Handle, copyflag);
            if (result == 0) _count = LeptonicaApi.Native.pixaGetCount(_handle);
            return result == 0;
        }
        #endregion

        #region Private class PixArrayEnumerato
        /// <summary>
        ///     Handles enumerating through the <see cref="Image" /> in the PixArray.
        /// </summary>
        private class PixArrayEnumerator : DisposableBase, IEnumerator<Image>
        {
            #region Constructor
            public PixArrayEnumerator(Array array)
            {
                _array = array;
                _version = array._version;
                _items = new Image[array.Count];
                _index = 0;
                _current = null;
            }
            #endregion

            #region Disposal
            protected override void Dispose(bool disposing)
            {
                if (!disposing) return;
                for (var i = 0; i < _items.Length; i++)
                    if (_items[i] != null)
                    {
                        _items[i].Dispose();
                        _items[i] = null;
                    }
            }
            #endregion

            #region Fields
            private readonly Array _array;
            private readonly Image[] _items;
            private Image _current;
            private int _index;
            private readonly int _version;
            #endregion

            #region MoveNext
            /// <inheritdoc />
            public bool MoveNext()
            {
                VerifyArrayUnchanged();
                VerifyNotDisposed();

                if (_index < _items.Length)
                {
                    if (_items[_index] == null) _items[_index] = _array.GetPix(_index);
                    _current = _items[_index];
                    _index++;
                    return true;
                }

                _index = _items.Length + 1;
                _current = null;
                return false;
            }
            #endregion

            #region Current
            /// <inheritdoc />
            public Image Current
            {
                get
                {
                    VerifyArrayUnchanged();
                    VerifyNotDisposed();

                    return _current;
                }
            }

            /// <inheritdoc />
            object IEnumerator.Current
            {
                get
                {
                    // note: Only the non-generic requires an exception check according the MSDN docs (Generic _version just undefined if it's not currently pointing to an item). Go figure.
                    if (_index == 0 || _index == _items.Length + 1)
                        throw new InvalidOperationException(
                            "The enumerator is positioned either before the first item or after the last item .");

                    return Current;
                }
            }
            #endregion

            #region Reset
            /// <inheritdoc />
            void IEnumerator.Reset()
            {
                VerifyArrayUnchanged();
                VerifyNotDisposed();

                _index = 0;
                _current = null;
            }
            #endregion

            #region VerifyArrayUnchanged
            /// <summary>
            ///     Verifies if the array is not changed
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            private void VerifyArrayUnchanged()
            {
                if (_version != _array._version)
                    throw new InvalidOperationException(
                        "PixArray was modified; enumeration operation may not execute.");
            }
            #endregion
        }
        #endregion

        #region Remove
        /// <summary>
        ///     Removes the pix located at index.
        /// </summary>
        /// <remarks>
        ///     Notes:
        ///     * This shifts pixa[i] --> pixa[i - 1] for all i > index.
        ///     * Do not use on large arrays as the functionality is O(n).
        ///     * The corresponding box is removed as well, if it exists.
        /// </remarks>
        /// <param name="index">The index of the pix to remove.</param>
        public void Remove(int index)
        {
            Guard.Require(nameof(index), index >= 0 && index < Count, "The index {0} must be between 0 and {1}.", index,
                Count);

            VerifyNotDisposed();
            if (LeptonicaApi.Native.pixaRemovePix(_handle, index) == 0)
                _count = LeptonicaApi.Native.pixaGetCount(_handle);
        }
        #endregion

        #region Clear
        /// <summary>
        ///     Destroys ever pix in the array.
        /// </summary>
        public void Clear()
        {
            VerifyNotDisposed();
            if (LeptonicaApi.Native.pixaClear(_handle) == 0) _count = LeptonicaApi.Native.pixaGetCount(_handle);
        }
        #endregion

        #region GetPix
        /// <summary>
        ///     Gets the <see cref="Image" /> located at <paramref name="index" /> using the specified <paramref name="accessType" />
        ///     .
        /// </summary>
        /// <param name="index">The index of the pix (zero based).</param>
        /// <param name="accessType">
        ///     The <see cref="PixArrayAccessType" /> used to retrieve the <see cref="Image" />, only Clone or
        ///     Copy are allowed.
        /// </param>
        /// <returns>The retrieved <see cref="Image" />.</returns>
        public Image GetPix(int index, PixArrayAccessType accessType = PixArrayAccessType.Clone)
        {
            Guard.Require(nameof(accessType),
                accessType == PixArrayAccessType.Clone || accessType == PixArrayAccessType.Copy,
                "Access type must be either copy or clone but was {0}.", accessType);
            Guard.Require(nameof(index), index >= 0 && index < Count, "The index {0} must be between 0 and {1}.", index,
                Count);

            VerifyNotDisposed();

            var pixHandle = LeptonicaApi.Native.pixaGetPix(_handle, index, accessType);
            if (pixHandle == IntPtr.Zero)
                throw new InvalidOperationException($"Failed to retrieve pix {pixHandle}.");
            var result = Image.Create(pixHandle);
            result.ImageName = Filename;
            return result;
        }
        #endregion

        #region GetEnumerator
        /// <summary>
        ///     Returns a <see cref="IEnumerator{Pix}" /> that iterates the the array of <see cref="Image" />.
        /// </summary>
        /// <remarks>
        ///     When done with the enumerator you must call <see cref="Dispose" /> to release any unmanaged resources.
        ///     However if your using the enumerator in a foreach loop, this is done for you automatically by .Net. This also means
        ///     that any <see cref="Image" /> returned from the enumerator cannot safely be used outside a foreach loop (or after
        ///     Dispose has been
        ///     called on the enumerator). If you do indeed need the pix after the enumerator has been disposed of you must clone
        ///     it using
        ///     <see cref="Image.Clone()" />.
        /// </remarks>
        /// <returns>A <see cref="IEnumerator{Pix}" /> that iterates the the array of <see cref="Image" />.</returns>
        public IEnumerator<Image> GetEnumerator()
        {
            return new PixArrayEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PixArrayEnumerator(this);
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            var handle = _handle.Handle;
            LeptonicaApi.Native.pixaDestroy(ref handle);
            _handle = new HandleRef(this, handle);
        }
        #endregion
    }
}