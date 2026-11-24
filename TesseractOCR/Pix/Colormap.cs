//
// PixArray.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2027 Kees van Spelde
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

namespace TesseractOCR.Pix;

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
    /// <summary>
    ///     Gets the handle to the colormap.
    /// </summary>
    internal HandleRef Handle { get; private set; }

    /// <summary>
    ///     Gets the bit depth of the colormap.
    /// </summary>
    public int Depth => LeptonicaApi.Native.pixcmapGetDepth(Handle);
    
    /// <summary>
    ///     Gets the number of colors in the colormap.
    /// </summary>  
    public int Count => LeptonicaApi.Native.pixcmapGetCount(Handle);

    /// <summary>
    ///     Gets the number of unused entries in the colormap.
    /// </summary>
    public int FreeCount => LeptonicaApi.Native.pixcmapGetFreeCount(Handle);

    /// <summary>
    ///     Gets or sets the color at the specified index.
    /// </summary>
    /// <param name="index">The index of the color.</param>
    /// <returns>The color at the specified index.</returns>
    /// <exception cref="InvalidOperationException">The color could not be retrieved or reset.</exception>
    public Color this[int index]
    {
        get
        {
            return LeptonicaApi.Native.pixcmapGetColor32(Handle, index, out var color) == 0 ? Color.FromRgb(color) : throw new InvalidOperationException("Failed to retrieve color.");
        }
        set
        {
            if (LeptonicaApi.Native.pixcmapResetColor(Handle, index, value.Red, value.Green, value.Blue) != 0)
                throw new InvalidOperationException("Failed to reset color.");
        }
    }
    #endregion

    #region Constructor
    /// <summary>
    ///     Wraps a native colormap handle.
    /// </summary>
    /// <param name="handle">The native colormap handle.</param>
    internal Colormap(IntPtr handle)
    {
        Handle = new HandleRef(this, handle);
    }
    #endregion

    #region Create
    /// <summary>
    ///     Creates a colormap with the specified bit depth.
    /// </summary>
    /// <param name="depth">The depth in bits per pixel. Valid values are 1, 2, 4, and 8.</param>
    /// <returns>A new <see cref="Colormap"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The depth is not 1, 2, 4, or 8.</exception>
    /// <exception cref="InvalidOperationException">The colormap could not be created.</exception>
    public static Colormap Create(int depth)
    {
        if (depth is not (1 or 2 or 4 or 8))
            throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be 1, 2, 4, or 8 bpp.");

        var handle = LeptonicaApi.Native.pixcmapCreate(depth);
        return handle == IntPtr.Zero ? throw new InvalidOperationException("Failed to create colormap.") : new Colormap(handle);
    }
    #endregion

    #region CreateLinear
    /// <summary>
    ///     Creates a colormap with evenly spaced grayscale levels.
    /// </summary>
    /// <param name="depth">The depth in bits per pixel: 1, 2, 4, or 8.</param>
    /// <param name="levels">The number of grayscale levels, from 2 through 2 raised to <paramref name="depth"/>.</param>
    /// <returns>A new grayscale colormap.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The depth or level count fails the managed validation.</exception>
    /// <exception cref="InvalidOperationException">The native library could not create the colormap.</exception>
    public static Colormap CreateLinear(int depth, int levels)
    {
        if (depth is not (1 or 2 or 4 or 8))
            throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be 1, 2, 4, or 8 bpp.");
        if (levels < 2 || levels > 2 << depth)
            throw new ArgumentOutOfRangeException(nameof(levels), "Depth must be 2 and 2^depth (inclusive).");

        var handle = LeptonicaApi.Native.pixcmapCreateLinear(depth, levels);
        if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
        return new Colormap(handle);
    }
    #endregion

    #region CreateLinear
    /// <summary>
    ///     Creates a colormap with random colors and optional black and white endpoints.
    /// </summary>
    /// <param name="depth">The depth in bits per pixel: 1, 2, 4, or 8.</param>
    /// <param name="firstIsBlack">Whether the first color is black.</param>
    /// <param name="lastIsWhite">Whether the last color is white.</param>
    /// <returns>A new randomly populated colormap.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The depth is not 1, 2, 4, or 8.</exception>
    /// <exception cref="InvalidOperationException">The native library could not create the colormap.</exception>
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
    /// <summary>
    ///     Adds an RGB color to the colormap.
    /// </summary>
    /// <param name="color">The color to add; its alpha component is ignored.</param>
    /// <returns><c>true</c> if the color was added; otherwise, <c>false</c>.</returns>
    public bool AddColor(Color color)
    {
        return LeptonicaApi.Native.pixcmapAddColor(Handle, color.Red, color.Green, color.Blue) == 0;
    }
    #endregion

    #region AddNewColor
    /// <summary>
    ///     Finds an existing RGB color or adds it if it is not already present.
    /// </summary>
    /// <param name="color">The color to find or add; its alpha component is ignored.</param>
    /// <param name="index">Receives the index of the existing or newly added color on success.</param>
    /// <returns><c>true</c> if the color was found or added; otherwise, <c>false</c>.</returns>
    public bool AddNewColor(Color color, out int index)
    {
        return LeptonicaApi.Native.pixcmapAddNewColor(Handle, color.Red, color.Green, color.Blue, out index) == 0;
    }
    #endregion

    #region AddNearestColor
    /// <summary>
    ///     Finds or adds an RGB color, using the nearest existing color if the colormap is full.
    /// </summary>
    /// <param name="color">The requested color; its alpha component is ignored.</param>
    /// <param name="index">Receives the index of the selected color on success.</param>
    /// <returns><c>true</c> if a color was selected; otherwise, <c>false</c>.</returns>
    public bool AddNearestColor(Color color, out int index)
    {
        return LeptonicaApi.Native.pixcmapAddNearestColor(Handle, color.Red, color.Green, color.Blue,
            out index) == 0;
    }
    #endregion

    #region AddBlackOrWhite
    /// <summary>
    ///     Finds or adds black or white in the colormap.
    /// </summary>
    /// <param name="color">The color to find or add: 0 for black or 1 for white.</param>
    /// <param name="index">Receives the index of the existing or newly added color on success.</param>
    /// <returns><c>true</c> if the color was found or added; otherwise, <c>false</c>.</returns>
    public bool AddBlackOrWhite(int color, out int index)
    {
        return LeptonicaApi.Native.pixcmapAddBlackOrWhite(Handle, color, out index) == 0;
    }
    #endregion

    #region SetBlackOrWhite
    /// <summary>
    ///     Sets the darkest and/or lightest existing colormap entry to black and/or white.
    /// </summary>
    /// <param name="setBlack">Whether to set the darkest entry to black.</param>
    /// <param name="setWhite">Whether to set the lightest entry to white.</param>
    /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
    public bool SetBlackOrWhite(bool setBlack, bool setWhite)
    {
        return LeptonicaApi.Native.pixcmapSetBlackAndWhite(Handle, setBlack ? 1 : 0, setWhite ? 1 : 0) == 0;
    }
    #endregion

    #region IsUsableColor
    /// <summary>
    ///     Checks whether an RGB color is already present or can be added to the colormap.
    /// </summary>
    /// <param name="color">The color to check; its alpha component is ignored.</param>
    /// <returns><c>true</c> if the color is present or there is room to add it; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">The native library could not check the color.</exception>
    public bool IsUsableColor(Color color)
    {
        if (LeptonicaApi.Native.pixcmapUsableColor(Handle, color.Red, color.Green, color.Blue, out var usable) == 0)
            return usable == 1;
        throw new InvalidOperationException("Failed to detect if color was usable or not.");
    }
    #endregion

    #region Clear
    /// <summary>
    ///     Removes all entries from the colormap.
    /// </summary>
    /// <exception cref="InvalidOperationException">The native library could not clear the colormap.</exception>
    public void Clear()
    {
        if (LeptonicaApi.Native.pixcmapClear(Handle) != 0)
            throw new InvalidOperationException("Failed to clear color map.");
    }
    #endregion

    #region Dispose
    /// <summary>
    ///     Releases the native colormap resources.
    /// </summary>
    public void Dispose()
    {
        var tmpHandle = Handle.Handle;
        LeptonicaApi.Native.pixcmapDestroy(ref tmpHandle);
        Handle = new HandleRef(this, IntPtr.Zero);
    }
    #endregion
}