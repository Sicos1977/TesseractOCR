//
// PixToBitmapConverter.cs
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

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace TesseractOCR.Pix
{
    public class PixToBitmapConverter
    {
        #region Convert
        public Bitmap Convert(Image pix, bool includeAlpha = false)
        {
            var pixelFormat = GetPixelFormat(pix);
            var depth = pix.Depth;
            var img = new Bitmap(pix.Width, pix.Height, pixelFormat);

            if (pix.XRes > 1 && pix.YRes > 1)
                img.SetResolution(pix.XRes, pix.YRes);

            BitmapData imgData = null;

            try
            {
                // TODO: Set X and Y resolution

                // Transfer pixel data
                if ((pixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
                    TransferPalette(pix, img);

                // transfer data
                var pixData = pix.GetData();
                imgData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly,
                    pixelFormat);

                switch (depth)
                {
                    case 32:
                        TransferData32(pixData, imgData, includeAlpha ? 0 : 255);
                        break;
                    case 16:
                        TransferData16(pixData, imgData);
                        break;
                    case 8:
                        TransferData8(pixData, imgData);
                        break;
                    case 1:
                        TransferData1(pixData, imgData);
                        break;
                }

                return img;
            }
            catch (Exception)
            {
                img.Dispose();
                throw;
            }
            finally
            {
                if (imgData != null) img.UnlockBits(imgData);
            }
        }
        #endregion

        #region TransferData32
        private static unsafe void TransferData32(Data pixData, BitmapData imgData, int alphaMask)
        {
            var height = imgData.Height;
            var width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                var imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                var pixLine = (uint*)pixData.PixData + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    var pixVal = Color.FromRgba(pixLine[x]);
                    var pixelPtr = imgLine + (x << 2);

                    pixelPtr[0] = pixVal.Blue;
                    pixelPtr[1] = pixVal.Green;
                    pixelPtr[2] = pixVal.Red;
                    pixelPtr[3] = (byte)(alphaMask | pixVal.Alpha); // Allow user to include alpha or not
                }
            }
        }
        #endregion

        #region TransferData16
        private static unsafe void TransferData16(Data pixData, BitmapData imgData)
        {
            var height = imgData.Height;
            var width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                var pixLine = (uint*)pixData.PixData + y * pixData.WordsPerLine;
                var imgLine = (ushort*)imgData.Scan0 + y * imgData.Stride;

                for (var x = 0; x < width; x++)
                {
                    var pixVal = (ushort)Data.GetDataTwoByte(pixLine, x);

                    imgLine[x] = pixVal;
                }
            }
        }
        #endregion

        #region TransferData8
        private static unsafe void TransferData8(Data pixData, BitmapData imgData)
        {
            var height = imgData.Height;
            var width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                var pixLine = (uint*)pixData.PixData + y * pixData.WordsPerLine;
                var imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;

                for (var x = 0; x < width; x++)
                {
                    var pixVal = (byte)Data.GetDataByte(pixLine, x);

                    imgLine[x] = pixVal;
                }
            }
        }
        #endregion

        #region TransferData1
        private static unsafe void TransferData1(Data pixData, BitmapData imgData)
        {
            var height = imgData.Height;
            var width = imgData.Width / 8;

            for (var y = 0; y < height; y++)
            {
                var pixLine = (uint*)pixData.PixData + y * pixData.WordsPerLine;
                var imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;

                for (var x = 0; x < width; x++)
                {
                    var pixVal = (byte)Data.GetDataByte(pixLine, x);

                    imgLine[x] = pixVal;
                }
            }
        }
        #endregion

        #region TransferPalette
        private static void TransferPalette(Image pix, System.Drawing.Image img)
        {
            var pallet = img.Palette;
            var maxColors = pallet.Entries.Length;
            var lastColor = maxColors - 1;
            var colormap = pix.Colormap;

            if (colormap != null && colormap.Count <= maxColors)
            {
                var colormapCount = colormap.Count;
                for (var i = 0; i < colormapCount; i++)
                    pallet.Entries[i] = colormap[i].ToColor();
            }
            else
            {
                for (var i = 0; i < maxColors; i++)
                {
                    var value = (byte)(i * 255 / lastColor);
                    pallet.Entries[i] = System.Drawing.Color.FromArgb(value, value, value);
                }
            }

            // This is required to force the palette to update!
            img.Palette = pallet;
        }
        #endregion

        #region GetPixelFormat
        private static PixelFormat GetPixelFormat(Image pix)
        {
            switch (pix.Depth)
            {
                case 1: return PixelFormat.Format1bppIndexed;
                case 8: return PixelFormat.Format8bppIndexed;
                case 16: return PixelFormat.Format16bppGrayScale;
                case 32: return PixelFormat.Format32bppArgb;
                default: throw new InvalidOperationException($"Pix depth {pix.Depth} is not supported.");
            }
        }
        #endregion
    }
}