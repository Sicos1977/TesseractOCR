//
// AggregateResultRenderer.cs
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
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace TesseractOCR
{
    /// <summary>
    /// Description of BitmapHelper.
    /// </summary>
    public static unsafe class BitmapHelper
    {
        #region GetBpp
        /// <summary>
        /// gets the number of Bits Per Pixel (BPP)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static int GetBpp(Bitmap bitmap)
        {
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed: return 1;
                case PixelFormat.Format4bppIndexed: return 4;
                case PixelFormat.Format8bppIndexed: return 8;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565: return 16;
                case PixelFormat.Format24bppRgb: return 24;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb: return 32;
                case PixelFormat.Format48bppRgb: return 48;
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb: return 64;
                default: throw new ArgumentException($"The bitmaps pixel format of {bitmap.PixelFormat} was not recognized.", nameof(bitmap));
            }
        }
        #endregion

        #region Bitmap Data Access
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataBit(byte* data, int index)
        {
            return (byte)(*(data + (index >> 3)) >> (index & 0x7) & 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataBit(byte* data, int index, byte value)
        {
            var wordPtr = data + (index >> 3);
            *wordPtr &= (byte)~(0x80 >> (index & 7));           // clear bit, note first pixel in the byte is most significant (1000 0000)
            *wordPtr |= (byte)((value & 1) << 7 - (index & 7));       // set bit, if value is 1
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataQBit(byte* data, int index)
        {
            return (byte)(*(data + (index >> 1)) >> 4 * (index & 1) & 0xF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataQBit(byte* data, int index, byte value)
        {
            var wordPtr = data + (index >> 1);
            *wordPtr &= (byte)~(0xF0 >> 4 * (index & 1)); // clears qbit located at index, note like bit the qbit corresponding to the first pixel is the most significant (0xF0)
            *wordPtr |= (byte)((value & 0x0F) << 4 - 4 * (index & 1)); // applys qbit to n
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataByte(byte* data, int index)
        {
            return *(data + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataByte(byte* data, int index, byte value)
        {
            *(data + index) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetDataUInt16(ushort* data, int index)
        {
            return *(data + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataUInt16(ushort* data, int index, ushort value)
        {
            *(data + index) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetDataUInt32(uint* data, int index)
        {
            return *(data + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataUInt32(uint* data, int index, uint value)
        {
            *(data + index) = value;
        }
        #endregion

        #region ConvertRgb555ToRGBA
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertRgb555ToRGBA(uint val)
        {
            var red = (val & 0x7C00) >> 10;
            var green = (val & 0x3E0) >> 5;
            var blue = val & 0x1F;

            return (red << 3 | red >> 2) << 24 |
                (green << 3 | green >> 2) << 16 |
                (blue << 3 | blue >> 2) << 8 |
                0xFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertRgb565ToRGBA(uint val)
        {
            var red = (val & 0xF800) >> 11;
            var green = (val & 0x7E0) >> 5;
            var blue = val & 0x1F;

            return (red << 3 | red >> 2) << 24 |
                (green << 2 | green >> 4) << 16 |
                (blue << 3 | blue >> 2) << 8 |
                0xFF;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertArgb1555ToRGBA(uint val)
        {
            var alpha = (val & 0x8000) >> 15;
            var red = (val & 0x7C00) >> 10;
            var green = (val & 0x3E0) >> 5;
            var blue = val & 0x1F;

            return (red << 3 | red >> 2) << 24 |
                (green << 3 | green >> 2) << 16 |
                (blue << 3 | blue >> 2) << 8 |
                (alpha << 8) - alpha; // effectively alpha * 255, only works as alpha will be either 0 or 1
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint EncodeAsRGBA(byte red, byte green, byte blue, byte alpha)
        {
            return (uint)(red << 24 |
                green << 16 |
                blue << 8 |
                alpha);
        }
        #endregion
    }
}
