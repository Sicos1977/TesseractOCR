//
// PixColor.cs
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

namespace TesseractOCR.Pix;

/// <summary>
///     Represents red, green, blue, and alpha color components.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct Color : IEquatable<Color>
{
    #region Properties
    /// <summary>
    ///     Returns the red byte value
    /// </summary>
    public byte Red { get; }

    /// <summary>
    ///     Returns the green byte value
    /// </summary>
    public byte Green { get; }

    /// <summary>
    ///     Returns the blue byte value
    /// </summary>
    public byte Blue { get; }

    /// <summary>
    ///     Returns the alpha byte value
    /// </summary>
    public byte Alpha { get; }
    #endregion

    #region Constructor
    /// <summary>
    ///     Creates a color from its individual components.
    /// </summary>
    /// <param name="red">The red component.</param>
    /// <param name="green">The green component.</param>
    /// <param name="blue">The blue component.</param>
    /// <param name="alpha">The alpha component; defaults to fully opaque.</param>
    public Color(byte red, byte green, byte blue, byte alpha = 255)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }
    #endregion

    #region PixColor
    /// <summary>
    ///     Creates a color from a packed RGBA value in <c>0xRRGGBBAA</c> order.
    /// </summary>
    /// <param name="value">The packed RGBA value.</param>
    /// <returns>The unpacked color.</returns>
    public static Color FromRgba(uint value)
    {
        return new Color(
            (byte)(value >> 24 & 0xFF),
            (byte)(value >> 16 & 0xFF),
            (byte)(value >> 8 & 0xFF),
            (byte)(value & 0xFF));
    }
    #endregion

    #region FromRgb
    /// <summary>
    ///     Creates an opaque color from the upper three bytes of a packed value.
    /// </summary>
    /// <param name="value">The packed value in <c>0xRRGGBBxx</c> order; the low byte is ignored.</param>
    /// <returns>The unpacked opaque color.</returns>
    public static Color FromRgb(uint value)
    {
        return new Color(
            (byte)(value >> 24 & 0xFF),
            (byte)(value >> 16 & 0xFF),
            (byte)(value >> 8 & 0xFF));
    }
    #endregion

    #region ToRgba
    /// <summary>
    ///     Packs the color components in <c>0xRRGGBBAA</c> order.
    /// </summary>
    /// <returns>The packed RGBA value.</returns>
    public uint ToRgba()
    {
        return (uint)(Red << 24 |
                      Green << 16 |
                      Blue << 8 |
                      Alpha);
    }
    #endregion

    #region Color
    /// <summary>
    ///     Converts this color to a <see cref="System.Drawing.Color"/>.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>A color with the same components.</returns>
    public static explicit operator System.Drawing.Color(Color color)
    {
        return System.Drawing.Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
    }
    #endregion

    #region PixColor
    /// <summary>
    ///     Converts a <see cref="System.Drawing.Color"/> to a pixel color.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>A color with the same components.</returns>
    public static explicit operator Color(System.Drawing.Color color)
    {
        return new Color(color.R, color.G, color.B, color.A);
    }
    #endregion

    #region Equals
    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Color color && Equals(color);
    }
    #endregion

    #region Equals
    /// <summary>
    ///     Compares all four color components with another color.
    /// </summary>
    /// <param name="other">The color to compare.</param>
    /// <returns><c>true</c> when all components match; otherwise, <c>false</c>.</returns>
    public bool Equals(Color other)
    {
        return Red == other.Red && Blue == other.Blue && Green == other.Green && Alpha == other.Alpha;
    }
    #endregion

    #region GetHashCode
    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;
        unchecked
        {
            hashCode += 1000000007 * Red.GetHashCode();
            hashCode += 1000000009 * Blue.GetHashCode();
            hashCode += 1000000021 * Green.GetHashCode();
            hashCode += 1000000033 * Alpha.GetHashCode();
        }

        return hashCode;
    }
    #endregion

    #region Operator
    /// <summary>Determines whether two colors have identical components.</summary>
    /// <param name="lhs">The first color.</param>
    /// <param name="rhs">The second color.</param>
    /// <returns><c>true</c> if the colors match; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Color lhs, Color rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>Determines whether two colors differ in any component.</summary>
    /// <param name="lhs">The first color.</param>
    /// <param name="rhs">The second color.</param>
    /// <returns><c>true</c> if the colors differ; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Color lhs, Color rhs)
    {
        return !(lhs == rhs);
    }
    #endregion

    #region ToString
    /// <inheritdoc />
    public override string ToString()
    {
        return $"Color(0x{ToRgba():X})";
    }
    #endregion
}