using System;
using System.Drawing;
using System.Drawing.Imaging;
using TesseractOCR.Enums;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace TesseractOCR.Pix
{
    /// <summary>
    ///     Tesseract drawing extensions
    /// </summary>
    public static class DrawingExtensions
    {
        #region Process
        /// <summary>
        ///     Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="System.Diagnostics.Process" /> instead. This is because
        ///     this method must convert the bitmap to a pix for processing which will add additional overhead.
        ///     Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="engine"><see cref="TesseractEngine"/></param>
        /// <param name="image">The image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this TesseractEngine engine, Bitmap image, PageSegMode? pageSegMode = null)
        {
            return engine.Process(image, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        ///     Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="System.Diagnostics.Process" /> instead. This is because
        ///     this method must convert the bitmap to a pix for processing which will add additional overhead.
        ///     Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="engine"><see cref="TesseractEngine"/></param>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this TesseractEngine engine, Bitmap image, string inputName,
            PageSegMode? pageSegMode = null)
        {
            return engine.Process(image, inputName, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        ///     Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="System.Diagnostics.Process" /> instead. This is because
        ///     this method must convert the bitmap to a pix for processing which will add additional overhead.
        ///     Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="engine"><see cref="TesseractEngine"/></param>
        /// <param name="image">The image to process.</param>
        /// <param name="region">The region of the image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this TesseractEngine engine, Bitmap image, Rect region,
            PageSegMode? pageSegMode = null)
        {
            return engine.Process(image, null, region, pageSegMode);
        }

        /// <summary>
        ///     Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="System.Diagnostics.Process" /> instead. This is because
        ///     this method must convert the bitmap to a pix for processing which will add additional overhead.
        ///     Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="engine"><see cref="TesseractEngine"/></param>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="region">The region of the image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this TesseractEngine engine, Bitmap image, string inputName, Rect region,
            PageSegMode? pageSegMode = null)
        {
            var pix = Converter.ToPix(image);
            pix.ImageName = inputName;
            var page = engine.Process(pix, region, pageSegMode);
            // ReSharper disable once ObjectCreationAsStatement
            new TesseractEngine.PageDisposalHandle(page, pix);
            return page;
        }
        #endregion

        #region ToColor
        /// <summary>
        ///     
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static System.Drawing.Color ToColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }
        #endregion

        #region PixColor
        /// <summary>
        ///     Convert a system.drawing.color to a pix color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToPixColor(this System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }
        #endregion

        #region GetBPP
        /// <summary>
        ///     gets the number of Bits Per Pixel (BPP)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static int GetBPP(this Bitmap bitmap)
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
                default:
                    throw new ArgumentException($"The bitmaps pixel format of {bitmap.PixelFormat} was not recognized.",
                        nameof(bitmap));
            }
        }
        #endregion
    }
}