//
// BitmapToPixConverter.cs
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

namespace TesseractOCR
{
    /// <summary>
    ///     Converts a <see cref="Bitmap" /> to a <see cref="Pix" />.
    /// </summary>
    public class BitmapToPixConverter
    {
        #region Convert
        /// <summary>
        ///     Converts the specified <paramref name="img" /> to a <see cref="Pix" />.
        /// </summary>
        /// <param name="img">The source image to be converted.</param>
        /// <returns>The converted pix.</returns>
        public Pix Convert(Bitmap img)
        {
            var pixDepth = GetPixDepth(img.PixelFormat);
            var pix = Pix.Create(img.Width, img.Height, pixDepth);

            pix.XRes = (int)Math.Round(img.HorizontalResolution);
            pix.YRes = (int)Math.Round(img.VerticalResolution);

            BitmapData imgData = null;

            try
            {
                // TODO: Set X and Y resolution

                if ((img.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed) CopyColorMap(img, pix);

                // transfer data
                imgData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly,
                    img.PixelFormat);
                var pixData = pix.GetData();

                switch (imgData.PixelFormat)
                {
                    case PixelFormat.Format32bppArgb:
                        TransferDataFormat32BppArgb(imgData, pixData);
                        break;

                    case PixelFormat.Format32bppRgb:
                        TransferDataFormat32BppRgb(imgData, pixData);
                        break;

                    case PixelFormat.Format24bppRgb:
                        TransferDataFormat24BppRgb(imgData, pixData);
                        break;

                    case PixelFormat.Format8bppIndexed:
                        TransferDataFormat8BppIndexed(imgData, pixData);
                        break;

                    case PixelFormat.Format1bppIndexed:
                        TransferDataFormat1BppIndexed(imgData, pixData);
                        break;
                }

                return pix;
            }
            catch (Exception)
            {
                pix.Dispose();
                throw;
            }
            finally
            {
                if (imgData != null) img.UnlockBits(imgData);
            }
        }
        #endregion

        #region CopyColorMap
        private static void CopyColorMap(Image img, Pix pix)
        {
            var imgPalette = img.Palette;
            var imgPaletteEntries = imgPalette.Entries;
            var pixColorMap = PixColormap.Create(pix.Depth);
            try
            {
                for (var i = 0; i < imgPaletteEntries.Length; i++)
                    if (!pixColorMap.AddColor(imgPaletteEntries[i].ToPixColor()))
                        throw new InvalidOperationException($"Failed to add color map entry {i}.");

                pix.Colormap = pixColorMap;
            }
            catch (Exception)
            {
                pixColorMap.Dispose();
                throw;
            }
        }
        #endregion

        #region GetPixDepth
        private static int GetPixDepth(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    return 1;

                case PixelFormat.Format8bppIndexed:
                    return 8;

                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format24bppRgb:
                    return 32;

                default:
                    throw new InvalidOperationException($"Source bitmaps pixel format {pixelFormat} is not supported.");
            }
        }
        #endregion

        #region TransferDataFormat1BppIndexed
        private static unsafe void TransferDataFormat1BppIndexed(BitmapData imgData, PixData pixData)
        {
            var height = imgData.Height;
            var width = imgData.Width / 8;
            for (var y = 0; y < height; y++)
            {
                var imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                var pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    var pixelVal = BitmapHelper.GetDataByte(imgLine, x);
                    PixData.SetDataByte(pixLine, x, pixelVal);
                }
            }
        }
        #endregion

        #region TransferDataFormat8BppIndexed
        private static unsafe void TransferDataFormat8BppIndexed(BitmapData imgData, PixData pixData)
        {
            var height = imgData.Height;
            var width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                var imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                var pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    var pixelVal = *(imgLine + x);
                    PixData.SetDataByte(pixLine, x, pixelVal);
                }
            }
        }
        #endregion

        #region TransferDataFormat24BppRgb
        private static unsafe void TransferDataFormat24BppRgb(BitmapData imgData, PixData pixData)
        {
            var height = imgData.Height;
            var width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                var imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                var pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    var pixelPtr = imgLine + x * 3;
                    var blue = pixelPtr[0];
                    var green = pixelPtr[1];
                    var red = pixelPtr[2];
                    PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, 255));
                }
            }
        }
        #endregion

        #region TransferDataFormat32BppRgb
        private static unsafe void TransferDataFormat32BppRgb(BitmapData imgData, PixData pixData)
        {
            var height = imgData.Height;
            var width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                var imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                var pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    var pixelPtr = imgLine + (x << 2);
                    var blue = *pixelPtr;
                    var green = *(pixelPtr + 1);
                    var red = *(pixelPtr + 2);
                    PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, 255));
                }
            }
        }
        #endregion

        #region TransferDataFormat32BppArgb
        private static unsafe void TransferDataFormat32BppArgb(BitmapData imgData, PixData pixData)
        {
            var height = imgData.Height;
            var width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                var imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                var pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    var pixelPtr = imgLine + (x << 2);
                    var blue = *pixelPtr;
                    var green = *(pixelPtr + 1);
                    var red = *(pixelPtr + 2);
                    var alpha = *(pixelPtr + 3);
                    PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, alpha));
                }
            }
        }
        #endregion
    }
}