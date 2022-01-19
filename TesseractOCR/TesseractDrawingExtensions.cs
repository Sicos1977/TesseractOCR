using System;
using System.Drawing;
using System.Drawing.Imaging;
using TesseractOCR.Enums;

// ReSharper disable InconsistentNaming

namespace TesseractOCR
{
    /// <summary>
    ///     Tesseract drawing extensions
    /// </summary>
    public static class TesseractDrawingExtensions
    {
        #region Process
        /// <summary>
        ///     Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="TesseractEngine.Process(Pix, PageSegMode?)" /> instead. This is because
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
        ///     Please consider <see cref="TesseractEngine.Process(Pix, string, PageSegMode?)" /> instead. This is because
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
        ///     Please consider <see cref="TesseractEngine.Process(Pix, Rect, PageSegMode?)" /> instead. This is because
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
        ///     Please consider <see cref="TesseractEngine.Process(Pix, string, Rect, PageSegMode?)" /> instead. This is because
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
            var pix = PixConverter.ToPix(image);
            pix.ImageName = inputName;
            var page = engine.Process(pix, region, pageSegMode);
            // ReSharper disable once ObjectCreationAsStatement
            new TesseractEngine.PageDisposalHandle(page, pix);
            return page;
        }
        #endregion

        #region ToColor
        public static Color ToColor(this PixColor color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }
        #endregion

        #region PixColor
        public static PixColor ToPixColor(this Color color)
        {
            return new PixColor(color.R, color.G, color.B, color.A);
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