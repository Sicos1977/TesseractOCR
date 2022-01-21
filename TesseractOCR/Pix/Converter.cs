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

using System.Drawing;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Pix
{
    /// <summary>
    /// Handles converting between different image formats supported by DotNet.
    /// </summary>
    public static class Converter
    {
        #region Fields
        private static readonly ToPixConverter BitmapConverter = new ToPixConverter();
        // ReSharper disable once InconsistentNaming
        private static readonly ToBitmapConverter PixConverter_ = new ToBitmapConverter();
        #endregion

        #region ToBitmap
        /// <summary>
        /// Converts the specified <paramref name="pix"/> to a Bitmap.
        /// </summary>
        /// <param name="pix">The source image to be converted.</param>
        /// <returns>The converted pix as a <see cref="Bitmap"/>.</returns>
        public static Bitmap ToBitmap(Image pix)
        {
            return PixConverter_.Convert(pix);
        }
        #endregion

        #region ToPix
        /// <summary>
        /// Converts the specified <paramref name="img"/> to a Pix.
        /// </summary>
        /// <param name="img">The source image to be converted.</param>
        /// <returns>The converted bitmap image as a <see cref="Image"/>.</returns>
        public static Image ToPix(Bitmap img)
        {
            return BitmapConverter.Convert(img);
        }
        #endregion
    }
}
