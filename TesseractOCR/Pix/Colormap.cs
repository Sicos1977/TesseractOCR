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
using System.Runtime.InteropServices;
using TesseractOCR.Interop;
// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Pix
{
    /// <summary>
    ///     Represents a colormap.
    /// </summary>
    /// <remarks>
    ///     Once the colormap is assigned to a pix it is owned by that pix and will be disposed off automatically
    ///     when the pix is disposed off.
    /// </remarks>
    public sealed class Colormap : IDisposable
    {
        #region Properties
        internal HandleRef Handle { get; private set; }

        public int Depth => LeptonicaApi.Native.pixcmapGetDepth(Handle);

        public int Count => LeptonicaApi.Native.pixcmapGetCount(Handle);

        public int FreeCount => LeptonicaApi.Native.pixcmapGetFreeCount(Handle);

        public Color this[int index]
        {
            get
            {
                if (LeptonicaApi.Native.pixcmapGetColor32(Handle, index, out var color) == 0)
                    return Color.FromRgb((uint)color);
                throw new InvalidOperationException("Failed to retrieve color.");
            }
            set
            {
                if (LeptonicaApi.Native.pixcmapResetColor(Handle, index, value.Red, value.Green, value.Blue) != 0)
                    throw new InvalidOperationException("Failed to reset color.");
            }
        }
        #endregion

        #region Constructor
        internal Colormap(IntPtr handle)
        {
            Handle = new HandleRef(this, handle);
        }
        #endregion

        #region Create
        public static Colormap Create(int depth)
        {
            if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8))
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be 1, 2, 4, or 8 bpp.");

            var handle = LeptonicaApi.Native.pixcmapCreate(depth);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
            return new Colormap(handle);
        }
        #endregion

        #region CreateLinear
        public static Colormap CreateLinear(int depth, int levels)
        {
            if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8))
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be 1, 2, 4, or 8 bpp.");
            if (levels < 2 || levels > 2 << depth)
                throw new ArgumentOutOfRangeException(nameof(levels), "Depth must be 2 and 2^depth (inclusive).");

            var handle = LeptonicaApi.Native.pixcmapCreateLinear(depth, levels);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
            return new Colormap(handle);
        }
        #endregion

        #region CreateLinear
        public static Colormap CreateLinear(int depth, bool firstIsBlack, bool lastIsWhite)
        {
            if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8))
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be 1, 2, 4, or 8 bpp.");

            var handle = LeptonicaApi.Native.pixcmapCreateRandom(depth, firstIsBlack ? 1 : 0, lastIsWhite ? 1 : 0);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
            return new Colormap(handle);
        }
        #endregion

        #region AddColor
        public bool AddColor(Color color)
        {
            return LeptonicaApi.Native.pixcmapAddColor(Handle, color.Red, color.Green, color.Blue) == 0;
        }
        #endregion

        #region AddNewColor
        public bool AddNewColor(Color color, out int index)
        {
            return LeptonicaApi.Native.pixcmapAddNewColor(Handle, color.Red, color.Green, color.Blue, out index) == 0;
        }
        #endregion

        #region AddNearestColor
        public bool AddNearestColor(Color color, out int index)
        {
            return LeptonicaApi.Native.pixcmapAddNearestColor(Handle, color.Red, color.Green, color.Blue,
                out index) == 0;
        }
        #endregion

        #region AddBlackOrWhite
        public bool AddBlackOrWhite(int color, out int index)
        {
            return LeptonicaApi.Native.pixcmapAddBlackOrWhite(Handle, color, out index) == 0;
        }
        #endregion

        #region SetBlackOrWhite
        public bool SetBlackOrWhite(bool setBlack, bool setWhite)
        {
            return LeptonicaApi.Native.pixcmapSetBlackAndWhite(Handle, setBlack ? 1 : 0, setWhite ? 1 : 0) == 0;
        }
        #endregion

        #region IsUsableColor
        public bool IsUsableColor(Color color)
        {
            if (LeptonicaApi.Native.pixcmapUsableColor(Handle, color.Red, color.Green, color.Blue, out var usable) == 0)
                return usable == 1;
            throw new InvalidOperationException("Failed to detect if color was usable or not.");
        }
        #endregion

        #region Clear
        public void Clear()
        {
            if (LeptonicaApi.Native.pixcmapClear(Handle) != 0)
                throw new InvalidOperationException("Failed to clear color map.");
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            var tmpHandle = Handle.Handle;
            LeptonicaApi.Native.pixcmapDestroy(ref tmpHandle);
            Handle = new HandleRef(this, IntPtr.Zero);
        }
        #endregion
    }
}