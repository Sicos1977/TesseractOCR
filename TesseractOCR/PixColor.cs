//
// PixColor.cs
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
using System.Drawing;
using System.Runtime.InteropServices;

namespace TesseractOCR
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PixColor : IEquatable<PixColor>
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
        public PixColor(byte red, byte green, byte blue, byte alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }
        #endregion

        #region PixColor
        public static PixColor FromRgba(uint value)
        {
            return new PixColor(
                (byte)(value >> 24 & 0xFF),
                (byte)(value >> 16 & 0xFF),
                (byte)(value >> 8 & 0xFF),
                (byte)(value & 0xFF));
        }
        #endregion

        #region FromRgb
        public static PixColor FromRgb(uint value)
        {
            return new PixColor(
                (byte)(value >> 24 & 0xFF),
                (byte)(value >> 16 & 0xFF),
                (byte)(value >> 8 & 0xFF));
        }
        #endregion

        #region ToRgba
        public uint ToRgba()
        {
            return (uint)(Red << 24 |
                          Green << 16 |
                          Blue << 8 |
                          Alpha);
        }
        #endregion

        #region Color
        public static explicit operator Color(PixColor color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }
        #endregion

        #region PixColor
        public static explicit operator PixColor(Color color)
        {
            return new PixColor(color.R, color.G, color.B, color.A);
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            return obj is PixColor color && Equals(color);
        }
        #endregion

        #region Equals
        public bool Equals(PixColor other)
        {
            return Red == other.Red && Blue == other.Blue && Green == other.Green && Alpha == other.Alpha;
        }
        #endregion

        #region GetHashCode
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
        public static bool operator ==(PixColor lhs, PixColor rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(PixColor lhs, PixColor rhs)
        {
            return !(lhs == rhs);
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return $"Color(0x{ToRgba():X})";
        }
        #endregion
    }
}