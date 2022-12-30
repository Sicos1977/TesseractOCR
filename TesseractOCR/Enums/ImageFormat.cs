//
// ImageFormat.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2023 Kees van Spelde
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

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Enums
{
    /// <summary>
    ///		The image format
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        ///		The image format is unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///		The image format is BMP
        /// </summary>
        Bmp = 1,

        /// <summary>
        ///		The image format is JPG
        /// </summary>
        JfifJpeg = 2,

        /// <summary>
        ///		The image format is PNG
        /// </summary>
        Png = 3,

        /// <summary>
        ///		The image format is TIFF
        /// </summary>
        Tiff = 4,

        /// <summary>
        ///		The image format is TIFF pack bits
        /// </summary>
        TiffPackBits = 5,

        /// <summary>
        ///		The image format is TIFF RLE
        /// </summary>
        TiffRle = 6,

        /// <summary>
        ///		The image format is TIFF with G3 fax compression
        /// </summary>
        TiffG3 = 7,

        /// <summary>
        ///		The image format is TIFF with G4 fax compression
        /// </summary>
        TiffG4 = 8,

        /// <summary>
        ///		The image format is TIFF with LZW compression
        /// </summary>
        TiffLzw = 9,

        /// <summary>
        ///		The image format is TIFF with ZIP compression
        /// </summary>
        TifZip = 10,

        /// <summary>
        ///		The image format is PNM
        /// </summary>
        Pnm = 11,

        /// <summary>
        ///		The image format is postscript
        /// </summary>
        Ps = 12,

        /// <summary>
        ///		The image format is GIF
        /// </summary>
        Gif = 13,

        /// <summary>
        ///		The image format is JP2
        /// </summary>
        Jp2 = 14,

        /// <summary>
        ///		The image format is WebP
        /// </summary>
        WebP = 15,

        /// <summary>
        ///		The image format is LPDF
        /// </summary>
        Lpdf = 16,

        /// <summary>
        ///		Default format
        /// </summary>
        Default = 17,

        /// <summary>
        ///		The image format is SPIX
        /// </summary>
        Spix = 18
    }
}
