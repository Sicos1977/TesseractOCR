//
// PixConverter.cs
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
using TesseractOCR.Interop;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace TesseractOCR.Pix
{
    public unsafe class Data
    {
        #region Properties
        /// <summary>
        ///     <see cref="Pix"/>
        /// </summary>
        public Image Pix { get; }

        /// <summary>
        ///     Pointer to the data.
        /// </summary>
        public IntPtr PixData { get; }

        /// <summary>
        ///     Number of 32-bit words per line.
        /// </summary>
        public int WordsPerLine { get; }
        #endregion

        #region PixData
        internal Data(Image pix)
        {
            Pix = pix;
            PixData = LeptonicaApi.Native.pixGetData(Pix.Handle);
            WordsPerLine = LeptonicaApi.Native.pixGetWpl(Pix.Handle);
        }
        #endregion

        #region EndianByteSwap
        /// <summary>
        ///     Swaps the bytes on little-endian platforms within a word; bytes 0 and 3 swapped, and bytes `1 and 2 are swapped.
        /// </summary>
        /// <remarks>
        ///     This is required for little-endians in situations where we convert from a serialized byte order that is in raster
        ///     order,
        ///     as one typically has in file formats, to one with MSB-to-the-left in each 32-bit word, or v.v. See
        ///     <seealso href="http://www.leptonica.com/byte-addressing.html" />
        /// </remarks>
        public void EndianByteSwap()
        {
            LeptonicaApi.Native.pixEndianByteSwap(Pix.Handle);
        }
        #endregion

        #region EncodeAsRGBA
        public static uint EncodeAsRGBA(byte red, byte green, byte blue, byte alpha)
        {
            return (uint)(red << 24 |
                          green << 16 |
                          blue << 8 |
                          alpha);
        }
        #endregion

        #region GetDataBit
        /// <summary>
        ///     Gets the pixel value for a 1bpp image.
        /// </summary>
        public static uint GetDataBit(uint* data, int index)
        {
            return *(data + (index >> 5)) >> 31 - (index & 31) & 1;
        }
        #endregion

        #region GetDataDIBit
        /// <summary>
        ///     Sets the pixel value for a 1bpp image.
        /// </summary>
        public static void SetDataBit(uint* data, int index, uint value)
        {
            var wordPtr = data + (index >> 5);
            *wordPtr &= ~(0x80000000 >> (index & 31));
            *wordPtr |= value << 31 - (index & 31);
        }
        #endregion

        #region GetDataDIBit
        /// <summary>
        ///     Gets the pixel value for a 2bpp image.
        /// </summary>
        public static uint GetDataDIBit(uint* data, int index)
        {
            return *(data + (index >> 4)) >> 2 * (15 - (index & 15)) & 3;
        }
        #endregion

        #region SetDataDIBit
        /// <summary>
        ///     Sets the pixel value for a 2bpp image.
        /// </summary>
        public static void SetDataDIBit(uint* data, int index, uint value)
        {
            var wordPtr = data + (index >> 4);
            *wordPtr &= ~(0xc0000000 >> 2 * (index & 15));
            *wordPtr |= (value & 3) << 30 - 2 * (index & 15);
        }
        #endregion

        #region GetDataQBit
        /// <summary>
        ///     Gets the pixel value for a 4bpp image.
        /// </summary>
        public static uint GetDataQBit(uint* data, int index)
        {
            return *(data + (index >> 3)) >> 4 * (7 - (index & 7)) & 0xf;
        }
        #endregion

        #region SetDataQBit
        /// <summary>
        ///     Sets the pixel value for a 4bpp image.
        /// </summary>
        public static void SetDataQBit(uint* data, int index, uint value)
        {
            var wordPtr = data + (index >> 3);
            *wordPtr &= ~(0xf0000000 >> 4 * (index & 7));
            *wordPtr |= (value & 15) << 28 - 4 * (index & 7);
        }
        #endregion

        #region GetDataByte
        /// <summary>
        ///     Gets the pixel value for a 8bpp image.
        /// </summary>
        public static uint GetDataByte(uint* data, int index)
        {
            // Must do direct size comparison to detect x64 process, since in this will be jited out and results in a lot faster code (e.g. 6x faster for image conversion)
            if (IntPtr.Size == 8)
                return *(byte*)((ulong)((byte*)data + index) ^ 3);
            return *(byte*)((uint)((byte*)data + index) ^ 3);
            // Architecture types that are NOT little endian are not currently supported
            //return *((byte*)data + index);  
        }
        #endregion

        #region SetDataByte
        /// <summary>
        ///     Sets the pixel value for a 8bpp image.
        /// </summary>
        public static void SetDataByte(uint* data, int index, uint value)
        {
            // Must do direct size comparison to detect x64 process, since in this will be jited out and results in a lot faster code (e.g. 6x faster for image conversion)
            if (IntPtr.Size == 8)
                *(byte*)((ulong)((byte*)data + index) ^ 3) = (byte)value;
            else
                *(byte*)((uint)((byte*)data + index) ^ 3) = (byte)value;

            // Architecture types that are NOT little-edian are not currently supported
            // *((byte*)data + index) =  (byte)value;
        }
        #endregion

        #region GetDataTwoByte
        /// <summary>
        ///     Gets the pixel value for a 16bpp image.
        /// </summary>
        public static uint GetDataTwoByte(uint* data, int index)
        {
            // Must do direct size comparison to detect x64 process, since in this will be jited out and results in a lot faster code (e.g. 6x faster for image conversion)
            if (IntPtr.Size == 8)
                return *(ushort*)((ulong)((ushort*)data + index) ^ 2);
            return *(ushort*)((uint)((ushort*)data + index) ^ 2);
            // Architecture types that are NOT little edian are not currently supported
            // return *((ushort*)data + index);
        }
        #endregion

        #region SetDataTwoByte
        /// <summary>
        ///     Sets the pixel value for a 16bpp image.
        /// </summary>
        public static void SetDataTwoByte(uint* data, int index, uint value)
        {
            // Must do direct size comparison to detect x64 process, since in this will be jited out and results in a lot faster code (e.g. 6x faster for image conversion)
            if (IntPtr.Size == 8)
                *(ushort*)((ulong)((ushort*)data + index) ^ 2) = (ushort)value;
            else
                *(ushort*)((uint)((ushort*)data + index) ^ 2) = (ushort)value;
            // Architecture types that are NOT little edian are not currently supported
            //*((ushort*)data + index) = (ushort)value;
        }
        #endregion

        #region GetDataFourByte
        /// <summary>
        ///     Gets the pixel value for a 32bpp image.
        /// </summary>
        public static uint GetDataFourByte(uint* data, int index)
        {
            return *(data + index);
        }
        #endregion

        #region SetDataFourByte
        /// <summary>
        ///     Sets the pixel value for a 32bpp image.
        /// </summary>
        public static void SetDataFourByte(uint* data, int index, uint value)
        {
            *(data + index) = value;
        }
        #endregion
    }
}