//
// Pix.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TesseractOCR.Exceptions;
using TesseractOCR.Enums;
using TesseractOCR.Helpers;
using TesseractOCR.Internal;
using TesseractOCR.Interop;
using Math = System.Math;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Pix
{
    /// <summary>
    ///     Wrapper for a Leptonica <see cref="Pix"/> image
    /// </summary>
    public sealed unsafe class Image : DisposableBase, IEquatable<Image>
    {
        #region Constants
        private const float Deg2Rad = (float)(Math.PI / 180.0);

        /// <summary>
        ///     Skew Defaults
        /// </summary>
        public const int DefaultBinarySearchReduction = 2; // binary search part

        /// <summary>
        ///     Default binary threshold
        /// </summary>
        public const int DefaultBinaryThreshold = 130;

        /// <summary>
        ///     A small angle, in radians, for threshold checking. Equal to about 0.06 degrees.
        /// </summary>
        private const float VerySmallAngle = 0.001F;

        /// <summary>
        ///     HMT (with just misses) for speckle up to 2x2
        ///     "oooo"
        ///     "oC o"
        ///     "o  o"
        ///     "oooo"
        /// </summary>
        public const string SEL_STR2 = "oooooC oo  ooooo";

        /// <summary>
        ///     HMT (with just misses) for speckle up to 3x3
        ///     "oC  o"
        ///     "o   o"
        ///     "o   o"
        ///     "ooooo"
        /// </summary>
        public const string SEL_STR3 = "ooooooC  oo   oo   oooooo";
        #endregion

        #region Fields
        private static readonly List<int> AllowedDepths = new List<int> { 1, 2, 4, 8, 16, 32 };

        /// <summary>
        ///     Used to lookup <see cref="Image"/> formats by extension.
        /// </summary>
        private static readonly Dictionary<string, ImageFormat> ImageFormatLookup = new Dictionary<string, ImageFormat>
        {
            { ".jpg", ImageFormat.JfifJpeg },
            { ".jpeg", ImageFormat.JfifJpeg },
            { ".gif", ImageFormat.Gif },
            { ".tif", ImageFormat.Tiff },
            { ".tiff", ImageFormat.Tiff },
            { ".png", ImageFormat.Png },
            { ".bmp", ImageFormat.Bmp }
        };

        private Colormap _colormap;
        private HandleRef _handle;
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the name of the <see cref="Image"/> being ocr'd.
        /// </summary>
        /// <remarks>
        ///     This is also used for some of the more advanced functionality such as identifying the associated UZN file if
        ///     present.
        /// </remarks>
        public string ImageName { get; set; }

        /// <summary>
        ///     Returns the colormap for the <see cref="Image"/>
        /// </summary>
        public Colormap Colormap
        {
            get => _colormap;
            set
            {
                if (value != null)
                {
                    if (LeptonicaApi.Native.pixSetColormap(_handle, value.Handle) == 0) _colormap = value;
                }
                else
                {
                    if (LeptonicaApi.Native.pixDestroyColormap(_handle) == 0) _colormap = null;
                }
            }
        }

        /// <summary>
        ///     Returns the depth of the <see cref="Image"/>
        /// </summary>
        public int Depth { get; }

        /// <summary>
        ///     Returns the height of the <see cref="Image"/>
        /// </summary>
        public int Height { get; }

        /// <summary>
        ///     Returns the width of the <see cref="Image"/>
        /// </summary>
        public int Width { get; }

        /// <summary>
        ///     Gets or sets the X-resolution for the <see cref="Image"/> in DPI
        /// </summary>
        public int XRes
        {
            get => LeptonicaApi.Native.pixGetXRes(_handle);
            set => LeptonicaApi.Native.pixSetXRes(_handle, value);
        }

        /// <summary>
        ///     Gets or sets the Y-resolution for the <see cref="Image"/> in DPI
        /// </summary>
        public int YRes
        {
            get => LeptonicaApi.Native.pixGetYRes(_handle);
            set => LeptonicaApi.Native.pixSetYRes(_handle, value);
        }

        /// <summary>
        ///     Returns the <see cref="HandleRef"/> to the <see cref="Image"/>
        /// </summary>
        public HandleRef Handle => _handle;
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates a new pix instance using an existing handle to a pix structure.
        /// </summary>
        /// <remarks>
        ///     Note that the resulting instance takes ownership of the data structure.
        /// </remarks>
        /// <param name="handle"></param>
        private Image(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentNullException(nameof(handle));

            _handle = new HandleRef(this, handle);
            Width = LeptonicaApi.Native.pixGetWidth(_handle);
            Height = LeptonicaApi.Native.pixGetHeight(_handle);
            Depth = LeptonicaApi.Native.pixGetDepth(_handle);

            var colorMapHandle = LeptonicaApi.Native.pixGetColormap(_handle);
            if (colorMapHandle != IntPtr.Zero) _colormap = new Colormap(colorMapHandle);
        }
        #endregion

        #region LoadFromFile
        /// <summary>
        ///     Loads the <see cref="Image"/> object from a file
        /// </summary>
        /// <param name="filename">The file name with it's full path</param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public static Image LoadFromFile(string filename)
        {
            Logger.LogInformation($"Loading image from file '{filename}'");

            var pixHandle = LeptonicaApi.Native.pixRead(filename);

            if (pixHandle == IntPtr.Zero)
            {
                const string message = "Failed to load image from file";
                Logger.LogError(message);
                throw new IOException(message);
            }

            var r = Create(pixHandle);
            r.ImageName = filename;
            Logger.LogInformation("Image loaded");
            return r;
        }
        #endregion

        #region LoadFromMemory
        /// <summary>
        ///     Loads an image from a MemoryStream
        /// </summary>
        /// <param name="memoryStream">The memory stream</param>
        /// <returns><see cref="Image"/></returns>
        /// <exception cref="IOException"></exception>
        public static Image LoadFromMemory(MemoryStream memoryStream)
        {
            Logger.LogInformation("Loading image from memory stream");
            var buffer = memoryStream.GetBuffer();
            return LoadFromMemoryInternal(buffer, 0, buffer.Length);
        }

        /// <summary>
        ///     Loads an image from a byte array
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <returns><see cref="Image"/></returns>
        /// <exception cref="IOException"></exception>
        public static Image LoadFromMemory(byte[] bytes)
        {
            Logger.LogInformation("Loading image from byte array");
            return LoadFromMemoryInternal(bytes, 0 , bytes.Length);
        }

        /// <summary>
        ///     Loads an image from a byte array
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <param name="offset">The offset</param>
        /// <param name="length">The length</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Image LoadFromMemory(byte[] bytes, int offset, int length)
        {
            if (offset < 0 || offset > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (length < 0 || length > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (offset + length > bytes.Length)
                throw new ArgumentException("Offset + length is greater then the byte array length");

            return LoadFromMemoryInternal(bytes, offset, length);
        }

        #endregion

        #region LoadFromMemoryInternal
        /// <summary>
        ///     Loads the <see cref="Image"/> object from a byte array
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <param name="offset">The offset</param>
        /// <param name="length">The amount of bytes to read</param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public static Image LoadFromMemoryInternal(byte[] bytes, int offset, int length)
        {
            IntPtr handle;

            fixed (byte* ptr = bytes)
                handle = LeptonicaApi.Native.pixReadMem(ptr + offset, length);

            if (handle == IntPtr.Zero)
            {
                const string message = "Failed to load image from memory";
                Logger.LogError(message);
                throw new IOException(message);
            }

            Logger.LogInformation("Image loaded");
            return Create(handle);
        }
        #endregion

        #region LoadMultiPageTiffFromMemory
        
        /// <summary>
        ///     Loads a multi-page TIFF <see cref="Image"/> from a byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offSet">0 for the first image</param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public static Image LoadMultiPageTiffFromMemory(byte[] bytes, ref int offSet)
        {
            Logger.LogInformation("Loading multi-page TIFF image from memory");

            IntPtr handle;

            fixed (byte* ptr = bytes)
                
            { 
                handle = LeptonicaApi.Native.pixReadMemFromMultipageTiff(ptr, bytes.Length, ref offSet);
            }

            if (handle == IntPtr.Zero)
            {
                const string message = "Failed to load multi page image from memory";
                Logger.LogError(message);
                throw new IOException(message);
            }

            Logger.LogInformation("Image loaded");
            return Create(handle);
        }
        
        /// <summary>
        ///     Enumerate in-memory multipage tiff
        ///     Will only hold a copy of a single page at a time.
        ///     Image will be disposed immediately after each iteration step.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static IEnumerable<Image> EnumerateInMemTiff(byte[] bytes)
        {
            var offSet = 0;
            do
            {
                using (var pix = LoadMultiPageTiffFromMemory(bytes, ref offSet))
                {
                    yield return pix;
                }
            } while (offSet != 0);
        }
        #endregion

        #region LoadTiffFromMemory

        /// <summary>
        ///     Loads a TIFF <see cref="Image"/> from a byte array
        ///     Supports multipage images
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="pageIndex">select page via index. default 0 for first page</param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public static Image LoadTiffFromMemory(byte[] bytes, int pageIndex = 0)
        {
            Logger.LogInformation("Loading TIFF image from memory");

            IntPtr handle;

            fixed (byte* ptr = bytes)
            {
                handle = LeptonicaApi.Native.pixReadMemTiff(ptr, bytes.Length, pageIndex);
            }

            if (handle == IntPtr.Zero)
            {
                const string message = "Failed to load image from memory";
                Logger.LogError(message);
                throw new IOException(message);
            }

            Logger.LogInformation("Image loaded");
            return Create(handle);
        }
        #endregion

        #region Save
        /// <summary>
        ///     Saves the <see cref="Image"/> to the specified file
        /// </summary>
        /// <param name="filename">The path to the file</param>
        /// <param name="imageFormat">
        ///     The format to use when saving the image, if not specified the file extension is used to guess the
        ///     format
        /// </param>
        public void Save(string filename, ImageFormat? imageFormat = null)
        {
            Logger.LogInformation($"Saving image to '{filename}' with the format '{imageFormat}'");

            ImageFormat actualFormat;

            if (!imageFormat.HasValue)
            {
                var extension = Path.GetExtension(filename).ToLowerInvariant();

                if (!ImageFormatLookup.TryGetValue(extension, out actualFormat))
                {
                    Logger.LogInformation("Couldn't find matching format, perhaps there is no extension or it's not recognized, fallback to default");
                    actualFormat = ImageFormat.Default;
                }
            }
            else
                actualFormat = imageFormat.Value;

            if (LeptonicaApi.Native.pixWrite(filename, _handle, actualFormat) != 0)
            {
                const string message = "Failed to save image";
                Logger.LogError(message);
                throw new IOException("Failed to save image");
            }

            Logger.LogInformation("Image saved");
        }
        #endregion

        #region Clone
        /// <summary>
        ///     Increments this pix's reference count and returns a reference to the same pix data.
        /// </summary>
        /// <remarks>
        ///     A "clone" is simply a reference to an existing pix. It is implemented this way because
        ///     image can be large and hence expensive to copy and extra handles need to be made with a simple
        ///     policy to avoid double frees and memory leaks.
        ///     The general usage protocol is:
        ///     <list type="number">
        ///         <item>Whenever you want a new reference to an existing <see cref="Image" /> call <see cref="Clone" />.</item>
        ///         <item>
        ///             Always call <see cref="Dispose" /> on all references. This decrements the reference count and
        ///             will destroy the pix when the reference count reaches zero.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <returns>The pix with it's reference count incremented.</returns>
        public Image Clone()
        {
            Logger.LogInformation("Cloning image");
            var clonedHandle = LeptonicaApi.Native.pixClone(_handle);
            Logger.LogInformation("Image cloned");
            return new Image(clonedHandle);
        }
        #endregion Clone

        #region Scaling
        /// <summary>
        ///     Scales the current pix by the specified <paramref name="scaleX" /> and <paramref name="scaleY" /> factors returning
        ///     a new <see cref="Image" /> of the same depth.
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <returns>The scaled image.</returns>
        /// <remarks>
        ///     <para>
        ///         This function scales 32 bpp RGB; 2, 4 or 8 bpp palette color;
        ///         2, 4, 8 or 16 bpp gray; and binary images.
        ///     </para>
        ///     <para>
        ///         When the input has palette color, the color map is removed and
        ///         the result is either 8 bpp gray or 32 bpp RGB, depending on whether
        ///         the color map has color entries.  Images with 2, 4 or 16 bpp are
        ///         converted to 8 bpp.
        ///     </para>
        ///     <para>
        ///         Because Scale() is meant to be a very simple interface to a
        ///         number of scaling functions, including the use of unsharp masking,
        ///         the type of scaling and the sharpening parameters are chosen
        ///         by default. Grayscale and color images are scaled using one
        ///         of four methods, depending on the scale factors:
        ///         <list type="number">
        ///             <item>
        ///                 <description>
        ///                     antialiased subsampling (lowpass filtering followed by
        ///                     subsampling, implemented here by area mapping), for scale factors
        ///                     less than 0.2
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     antialiased subsampling with sharpening, for scale factors
        ///                     between 0.2 and 0.7.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     linear interpolation with sharpening, for scale factors between
        ///                     0.7 and 1.4.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     linear interpolation without sharpening, for scale factors >= 1.4.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///         One could use subsampling for scale factors very close to 1.0,
        ///         because it preserves sharp edges.  Linear interpolation blurs
        ///         edges because the dest pixels will typically straddle two src edge
        ///         pixels.  Subsampling removes entire columns and rows, so the edge is
        ///         not blurred.  However, there are two reasons for not doing this.
        ///         First, it moves edges, so that a straight line at a large angle to
        ///         both horizontal and vertical will have noticeable kinks where
        ///         horizontal and vertical rasters are removed.  Second, although it
        ///         is very fast, you get good results on sharp edges by applying
        ///         a sharpening filter.
        ///     </para>
        ///     <para>
        ///         For images with sharp edges, sharpening substantially improves the
        ///         image quality for scale factors between about 0.2 and about 2.0.
        ///         pixScale() uses a small amount of sharpening by default because
        ///         it strengthens edge pixels that are weak due to anti-aliasing.
        ///         The default sharpening factors are:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description><![CDATA[for scaling factors < 0.7:   sharpfract = 0.2    sharpwidth = 1]]></description>
        ///             </item>
        ///             <item>
        ///                 <description>for scaling factors >= 0.7:  sharpfract = 0.4    sharpwidth = 2</description>
        ///             </item>
        ///         </list>
        ///         The cases where the sharpening half width is 1 or 2 have special
        ///         implementations and are about twice as fast as the general case.
        ///     </para>
        ///     <para>
        ///         However, sharpening is computationally expensive, and one needs
        ///         to consider the speed-quality tradeoff:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     For upscaling of RGB images, linear interpolation plus default
        ///                     sharpening is about 5 times slower than upscaling alone.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     For downscaling, area mapping plus default sharpening is
        ///                     about 10 times slower than downscaling alone.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///         When the scale factor is larger than 1.4, the cost of sharpening,
        ///         which is proportional to image area, is very large compared to the
        ///         incremental quality improvement, so we cut off the default use of
        ///         sharpening at 1.4.  Thus, for scale factors greater than 1.4,
        ///         pixScale() only does linear interpolation.
        ///     </para>
        ///     <para>
        ///         In many situations you will get a satisfactory result by scaling
        ///         without sharpening: call pixScaleGeneral() with @sharpfract = 0.0.
        ///         Alternatively, if you wish to sharpen but not use the default
        ///         value, first call pixScaleGeneral() with @sharpfract = 0.0, and
        ///         then sharpen explicitly using pixUnsharpMasking().
        ///     </para>
        ///     <para>
        ///         Binary images are scaled to binary by sampling the closest pixel,
        ///         without any low-pass filtering (averaging of neighboring pixels).
        ///         This will introduce aliasing for reductions.  Aliasing can be
        ///         prevented by using pixScaleToGray() instead.
        ///     </para>
        ///     <para>
        ///         Warning: implicit assumption about RGB component order for LI color scaling
        ///     </para>
        /// </remarks>
        public Image Scale(float scaleX, float scaleY)
        {
            Logger.LogInformation($"Scaling image to x '{scaleX}' and y '{scaleY}'");
            var result = LeptonicaApi.Native.pixScale(_handle, scaleX, scaleY);

            if (result == IntPtr.Zero)
            {
                const string message = "Failed to scale image";
                Logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            Logger.LogInformation("Image scaled");

            return new Image(result);
        }
        #endregion

        #region Create
        /// <summary>
        ///     Creates an empty <see cref="Image"/> object
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        /// <param name="depth">The pixel depth</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static Image Create(int width, int height, int depth)
        {
            if (!AllowedDepths.Contains(depth))
            {
                const string message = "Depth must be 1, 2, 4, 8, 16, or 32 bits";
                Logger.LogError(message);
                throw new ArgumentException(message, nameof(depth));
            }

            if (width <= 0)
            {
                const string message = "Width must be greater than zero";
                Logger.LogError(message);
                throw new ArgumentException(message, nameof(width));
            }

            if (height <= 0)
            {
                const string message = "Height must be greater than zero";
                Logger.LogError(message);
                throw new ArgumentException(message, nameof(width));
            }

            var handle = LeptonicaApi.Native.pixCreate(width, height, depth);

            if (handle == IntPtr.Zero)
            {
                const string message = "Failed to create pix, this normally occurs because the requested image size is too large, please check Standard Error Output";
                Logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            return Create(handle);
        }

        /// <summary>
        ///     Creates the <see cref="Image"/> object from a pointer
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Image Create(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
                return new Image(handle);

            const string message = "Pix handle must not be zero (null)";
            Logger.LogError(message);
            throw new ArgumentException(message, nameof(handle));
        }
        #endregion

        #region GetData
        /// <summary>
        ///     Returns the data of the Image object
        /// </summary>
        /// <returns></returns>
        public Data GetData()
        {
            return new Data(this);
        }
        #endregion

        #region Equals
        /// <summary>
        ///     Returns <c>true</c> when both objects are equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((Image)obj);
        }

        /// <summary>
        ///     Compares an <see cref="Image"/> object to another <see cref="Image"/> object and returns <c>true</c> when they are equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="TesseractException"></exception>
        public bool Equals(Image other)
        {
            if (other == null) return false;

            if (LeptonicaApi.Native.pixEqual(Handle, other.Handle, out var same) != 0)
                throw new TesseractException("Failed to compare pix");

            return same != 0;
        }
        #endregion

        #region GetHashCode
        /// <summary>
        ///     Generate a hashcode. Calls superclass, just avoiding a warning
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
        #endregion

        #region BinarizeOtsuAdaptiveThreshold
        /// <summary>
        ///     Binarization of the input image based on the passed parameters and the Otsu method
        /// </summary>
        /// <param name="sx"> sizeX Desired tile X dimension; actual size may vary.</param>
        /// <param name="sy"> sizeY Desired tile Y dimension; actual size may vary.</param>
        /// <param name="smoothx"> smoothX Half-width of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="smoothy"> smoothY Half-height of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="scorefract"> scoreFraction Fraction of the max Otsu score; typ. 0.1 (use 0.0 for standard Otsu).</param>
        /// <returns>The binarized image.</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image BinarizeOtsuAdaptiveThreshold(int sx, int sy, int smoothx, int smoothy, float scorefract)
        {
            Guard.Verify(Depth == 8, "Image must have a depth of 8 bits per pixel to be binarized using Otsu.");
            Guard.Require(nameof(sx), sx >= 16, "The sx parameter must be greater than or equal to 16");
            Guard.Require(nameof(sy), sy >= 16, "The sy parameter must be greater than or equal to 16");

            Logger.LogInformation("Binarizing image with Otsu adaptive threshold");

            var result = LeptonicaApi.Native.pixOtsuAdaptiveThreshold(_handle, sx, sy, smoothx, smoothy, scorefract, out var ppixth, out var ppixd);

            if (ppixth != IntPtr.Zero)
            {
                // Free memory held by ppixth, an array of threshold values found for each tile
                LeptonicaApi.Native.pixDestroy(ref ppixth);
            }

            if (result == 1)
            {
                const string message = "Failed to binarize image with Otsu adaptive threshold";
                Logger.LogError(message);
                throw new LeptonicaException(message);
            }

            Logger.LogInformation("Image binarized with Otsu adaptive threshold");

            return new Image(ppixd);
        }
        #endregion

        #region BinarizeSauvola
        /// <summary>
        ///     Binarization of the input image using the Sauvola local thresholding method.
        ///     Note: The source image must be 8 bpp grayscale; not colormapped.
        /// </summary>
        /// <remarks>
        ///     <list type="number">
        ///         <listheader>Notes</listheader>
        ///         <item>
        ///             The window width and height are 2 * <paramref name="whsize" /> + 1. The minimum value for
        ///             <paramref name="whsize" /> is 2; typically it is >= 7.
        ///         </item>
        ///         <item>The local statistics, measured over the window, are the average and standard deviation.</item>
        ///         <item>
        ///             The measurements of the mean and standard deviation are performed inside a border of (
        ///             <paramref name="whsize" /> + 1) pixels.
        ///             If source pix does not have these added border pixels, use <paramref name="addborder" /> = <c>True</c> to
        ///             add it here; otherwise use
        ///             <paramref name="addborder" /> = <c>False</c>.
        ///         </item>
        ///         <item>
        ///             The Sauvola threshold is determined from the formula:  t = m * (1 - k * (1 - s / 128)) where t = local
        ///             threshold, m = local mean,
        ///             k = <paramref name="factor" />, and s = local standard deviation which is maximized at 127.5 when half the
        ///             samples are 0 and the other
        ///             half are 255.
        ///         </item>
        ///         <item>
        ///             The basic idea of Niblack and Sauvola binarization is that the local threshold should be less than the
        ///             median value,
        ///             and the larger the variance, the closer to the median it should be chosen. Typical values for k are between
        ///             0.2 and 0.5.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="whsize">the window half-width for measuring local statistics.</param>
        /// <param name="factor">
        ///     The factor for reducing threshold due to variances greater than or equal to zero (0). Typically
        ///     around 0.35.
        /// </param>
        /// <param name="addborder">If <c>True</c> add a border of width (<paramref name="whsize" /> + 1) on all sides.</param>
        /// <returns>The binarized image</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image BinarizeSauvola(int whsize, float factor, bool addborder)
        {
            Guard.Verify(Depth == 8, "Source image must be 8bpp");
            Guard.Verify(Colormap == null, "Source image must not be color mapped.");
            Guard.Require(nameof(whsize), whsize >= 2, "The window half-width (whsize) must be greater than 2.");
            var maxWhSize = Math.Min((Width - 3) / 2, (Height - 3) / 2);
            Guard.Require(nameof(whsize), whsize < maxWhSize, "The window half-width (whsize) must be less than {0} for this image.", maxWhSize);
            Guard.Require(nameof(factor), factor >= 0, "Factor must be greater than zero (0).");

            Logger.LogInformation("Binarizing image with Sauvola");

            var result = LeptonicaApi.Native.pixSauvolaBinarize(_handle, whsize, factor, addborder ? 1 : 0, out var ppixm,
                out var ppixsd, out var ppixth, out var ppixd);

            // Free memory held by other unused pix's
            if (ppixm != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref ppixm);
            if (ppixsd != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref ppixsd);
            if (ppixth != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref ppixth);

            if (result == 1)
            {
                const string message = "Failed to binarize image with Sauvola";
                Logger.LogError(message);
                throw new LeptonicaException(message);
            }

            Logger.LogInformation("Image binarized with Sauvola");

            return new Image(ppixd);
        }
        #endregion

        #region BinarizeSauvolaTiled
        /// <summary>
        ///     Binarization of the input image using the Sauvola local thresholding method on tiles
        ///     of the source image.
        ///     Note: The source image must be 8 bpp grayscale; not colormapped.
        /// </summary>
        /// <remarks>
        ///     A tiled version of Sauvola can become necessary for large source images (over 16M pixels) because:
        ///     * The mean value accumulator is a uint32, overflow can occur for an image with more than 16M pixels.
        ///     * The mean value accumulator array for 16M pixels is 64 MB. While the mean square accumulator array for 16M pixels
        ///     is 128 MB.
        ///     Using tiles reduces the size of these arrays.
        ///     * Each tile can be processed independently, in parallel, on a multi-core processor.
        /// </remarks>
        /// <param name="whsize">The window half-width for measuring local statistics</param>
        /// <param name="factor">
        ///     The factor for reducing threshold due to variances greater than or equal to zero (0). Typically
        ///     around 0.35.
        /// </param>
        /// <param name="nx">The number of tiles to subdivide the source image into on the x-axis.</param>
        /// <param name="ny">The number of tiles to subdivide the source image into on the y-axis.</param>
        /// <returns>THe binarized image</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image BinarizeSauvolaTiled(int whsize, float factor, int nx, int ny)
        {
            Guard.Verify(Depth == 8, "Source image must be 8bpp");
            Guard.Verify(Colormap == null, "Source image must not be color mapped.");
            Guard.Require(nameof(whsize), whsize >= 2, "The window half-width (whsize) must be greater than 2.");
            var maxWhSize = Math.Min((Width - 3) / 2, (Height - 3) / 2);
            Guard.Require(nameof(whsize), whsize < maxWhSize, "The window half-width (whsize) must be less than {0} for this image.", maxWhSize);
            Guard.Require(nameof(factor), factor >= 0, "Factor must be greater than zero (0).");

            Logger.LogInformation("Binarizing image with Sauvola local thresholding method");

            var result = LeptonicaApi.Native.pixSauvolaBinarizeTiled(_handle, whsize, factor, nx, ny, out var ppixth, out var ppixd);

            // Free memory held by other unused pix's
            if (ppixth != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref ppixth);

            if (result == 1)
            {
                const string message = "Failed to binarize image with Sauvola local thresholding method";
                Logger.LogError(message);
                throw new LeptonicaException(message);
            }

            Logger.LogInformation("Image binarized with Sauvola local thresholding method");

            return new Image(ppixd);
        }
        #endregion

        #region ConvertRGBToGray
        /// <summary>
        ///     Conversion from RBG to 8bpp grayscale using the specified weights. Note red, green, blue weights should add up to 1.0.
        /// </summary>
        /// <param name="rwt">Red weight</param>
        /// <param name="gwt">Green weight</param>
        /// <param name="bwt">Blue weight</param>
        /// <returns>The Grayscale pix</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image ConvertRGBToGray(float rwt, float gwt, float bwt)
        {
            Guard.Verify(Depth == 32, "The source image must have a depth of 32 (32 bpp).");
            Guard.Require(nameof(rwt), rwt >= 0, "All weights must be greater than or equal to zero; red was not.");
            Guard.Require(nameof(gwt), gwt >= 0, "All weights must be greater than or equal to zero; green was not.");
            Guard.Require(nameof(bwt), bwt >= 0, "All weights must be greater than or equal to zero; blue was not.");

            Logger.LogInformation("Converting RGB image to grayscale");

            var resultPixHandle = LeptonicaApi.Native.pixConvertRGBToGray(_handle, rwt, gwt, bwt);

            if (resultPixHandle == IntPtr.Zero)
            {
                const string message = "Failed to convert image to grayscale";
                Logger.LogError(message);
                throw new LeptonicaException(message);
            }

            Logger.LogInformation("Converted RGB image to grayscale");

            return new Image(resultPixHandle);
        }

        /// <summary>
        ///     Conversion from RBG to 8bpp grayscale.
        /// </summary>
        /// <returns>The Grayscale pix.</returns>
        public Image ConvertRGBToGray()
        {
            return ConvertRGBToGray(0, 0, 0);
        }
        #endregion

        #region RemoveLines
        /// <summary>
        ///     Removes horizontal lines from a grayscale image.
        ///     The algorithm is based on Leptonica <code>lineremoval.c</code> example.
        ///     See <a href="http://www.leptonica.com/line-removal.html">line-removal</a>.
        /// </summary>
        /// <param name="whiteTresh">Threshold value for white pixels.</param>
        /// <param name="blackTresh">Threshold value for black pixels.</param>
        /// <returns>Image with lines removed</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image RemoveLines(int whiteTresh = 127, int blackTresh = 127)
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            IntPtr pix1, pix2, pix3, pix4, pix5, pix6, pix7, pix8, pix9;
            pix1 = pix2 = pix3 = pix4 = pix5 = pix6 = pix7 = pix8 = pix9 = IntPtr.Zero;

            try
            {
                // Threshold to binary, extracting much of the lines

                Logger.LogInformation("Remove lines from image");

                pix1 = LeptonicaApi.Native.pixThresholdToBinary(_handle, 170);

                // Find the skew angle and deskew using an interpolated
                // rotator for anti-aliasing (to avoid jaggies)

                LeptonicaApi.Native.pixFindSkew(new HandleRef(this, pix1), out var angle, out _);

                pix2 = LeptonicaApi.Native.pixRotateAMGray(_handle, Deg2Rad * angle, 255);

                // Extract the lines to be removed
                pix3 = LeptonicaApi.Native.pixCloseGray(new HandleRef(this, pix2), 51, 1);

                // Solidify the lines to be removed
                pix4 = LeptonicaApi.Native.pixErodeGray(new HandleRef(this, pix3), 1, 5);

                // Clean the background of those lines
                pix5 = LeptonicaApi.Native.pixThresholdToValue(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix4), whiteTresh, 255);

                pix6 = LeptonicaApi.Native.pixThresholdToValue(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix5), blackTresh, 0);

                // Get paint-through mask for changed pixels */
                pix7 = LeptonicaApi.Native.pixThresholdToBinary(new HandleRef(this, pix6), whiteTresh);

                // Add the inverted, cleaned lines to orig. Because the background was cleaned, the inversion is 0,
                // so when you add, it doesn't lighten those pixels. It only lightens (to white) the pixels in the lines! */
                LeptonicaApi.Native.pixInvert(new HandleRef(this, pix6), new HandleRef(this, pix6));

                pix8 = LeptonicaApi.Native.pixAddGray(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix2), new HandleRef(this, pix6));

                pix9 = LeptonicaApi.Native.pixOpenGray(new HandleRef(this, pix8), 1, 9);

                LeptonicaApi.Native.pixCombineMasked(new HandleRef(this, pix8), new HandleRef(this, pix9), new HandleRef(this, pix7));

                if (pix8 == IntPtr.Zero)
                {
                    const string message = "Failed to remove lines from image";
                    Logger.LogError(message);
                    throw new LeptonicaException(message);
                }

                Logger.LogInformation("Lines removed from image");

                return new Image(pix8);
            }
            finally
            {
                // Destroy any created intermediate pix's, regardless of if the process failed for any reason.
                if (pix1 != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref pix1);
                if (pix2 != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref pix2);
                if (pix3 != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref pix3);
                if (pix4 != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref pix4);
                if (pix5 != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref pix5);
                if (pix6 != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref pix6);
                if (pix7 != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref pix7);
                if (pix8 != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref pix7);
                if (pix9 != IntPtr.Zero) LeptonicaApi.Native.pixDestroy(ref pix9);
            }
        }
        #endregion

        #region Despeckle
        /// <summary>
        ///     Reduces speckle noise in image. The algorithm is based on Leptonica
        ///     <code>speckle_reg.c</code> example demonstrating morphological method of
        ///     removing speckle.
        /// </summary>
        /// <param name="selStr">hit-miss sels in 2D layout; SEL_STR2 and SEL_STR3 are predefined values</param>
        /// <param name="selSize">2 for 2x2, 3 for 3x3</param>
        /// <returns>Despeckled image</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image Despeckle(string selStr, int selSize)
        {
            Logger.LogInformation("Despeckle image");

            /*  Normalize for rapidly varying background */
            var pix1 = LeptonicaApi.Native.pixBackgroundNormFlex(_handle, 7, 7, 1, 1, 10);

            /* Remove the background */
            var pix2 = LeptonicaApi.Native.pixGammaTRCMasked(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix1),
                new HandleRef(this, IntPtr.Zero), 1.0f, 100, 175);

            /* Binarize */
            var pix3 = LeptonicaApi.Native.pixThresholdToBinary(new HandleRef(this, pix2), 180);

            /* Remove the speckle noise up to selSize x selSize */
            var sel1 = LeptonicaApi.Native.selCreateFromString(selStr, selSize + 2, selSize + 2, "speckle" + selSize);
            var pix4 = LeptonicaApi.Native.pixHMT(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix3),
                new HandleRef(this, sel1));
            var sel2 = LeptonicaApi.Native.selCreateBrick(selSize, selSize, 0, 0, SelType.SelHit);
            var pix5 = LeptonicaApi.Native.pixDilate(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix4),
                new HandleRef(this, sel2));
            var pix6 = LeptonicaApi.Native.pixSubtract(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix3),
                new HandleRef(this, pix5));

            LeptonicaApi.Native.selDestroy(ref sel1);
            LeptonicaApi.Native.selDestroy(ref sel2);
            LeptonicaApi.Native.pixDestroy(ref pix1);
            LeptonicaApi.Native.pixDestroy(ref pix2);
            LeptonicaApi.Native.pixDestroy(ref pix3);
            LeptonicaApi.Native.pixDestroy(ref pix4);
            LeptonicaApi.Native.pixDestroy(ref pix5);

            if (pix6 == IntPtr.Zero)
            {
                const string message = "Failed to despeckle image";
                Logger.LogError(message);
                throw new LeptonicaException(message);
            }

            Logger.LogInformation("Image despeckled");

            return new Image(pix6);
        }
        #endregion

        #region Deskew
        /// <summary>
        ///     Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise
        ///     returns clone of original image.
        /// </summary>
        /// <remarks>
        ///     This binarizes if necessary and finds the skew angle.  If the
        ///     angle is large enough and there is sufficient confidence,
        ///     it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image Deskew()
        {
            return Deskew(DefaultBinarySearchReduction, out _);
        }

        /// <summary>
        ///     Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise
        ///     returns clone of original image
        /// </summary>
        /// <remarks>
        ///     This binarizes if necessary and finds the skew angle.  If the
        ///     angle is large enough and there is sufficient confidence,
        ///     it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <param name="scew">The scew angle and confidence</param>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image Deskew(out Scew scew)
        {
            return Deskew(DefaultBinarySearchReduction, out scew);
        }

        /// <summary>
        ///     Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise
        ///     returns clone of original image
        /// </summary>
        /// <remarks>
        ///     This binarizes if necessary and finds the skew angle.  If the
        ///     angle is large enough and there is sufficient confidence,
        ///     it returns a deskewed image; otherwise, it returns a clone
        /// </remarks>
        /// <param name="redSearch">The reduction factor used by the binary search, can be 1, 2, or .</param>
        /// <param name="scew">The scew angle and confidence</param>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image Deskew(int redSearch, out Scew scew)
        {
            return Deskew(ScewSweep.Default, redSearch, DefaultBinaryThreshold, out scew);
        }

        /// <summary>
        ///     Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise
        ///     returns clone of original image
        /// </summary>
        /// <remarks>
        ///     This binarizes if necessary and finds the skew angle.  If the
        ///     angle is large enough and there is sufficient confidence,
        ///     it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <param name="sweep">linear sweep parameters</param>
        /// <param name="redSearch">The reduction factor used by the binary search, can be 1, 2, or 4.</param>
        /// <param name="thresh">The threshold value used for binarizing the image.</param>
        /// <param name="scew">The scew angle and confidence</param>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image Deskew(ScewSweep sweep, int redSearch, int thresh, out Scew scew)
        {
            Logger.LogInformation("Getting image scew");

            var resultPixHandle = LeptonicaApi.Native.pixDeskewGeneral(_handle, sweep.Reduction, sweep.Range,
                sweep.Delta, redSearch, thresh, out var pAngle, out var pConf);

            if (resultPixHandle == IntPtr.Zero)
            {
                const string message = "Failed to deskew image";
                Logger.LogError(message);
                throw new LeptonicaException(message);
            }

            Logger.LogInformation($"Image scew '{pAngle}', confidence '{pConf}'");

            scew = new Scew(pAngle, pConf);
            return new Image(resultPixHandle);
        }
        #endregion

        #region Rotate
        /// <summary>
        ///     Creates a new image by rotating this image about it's centre.
        /// </summary>
        /// <remarks>
        ///     Please note the following:
        ///     <list type="bullet">
        ///         <item>
        ///             Rotation will bring in either white or black pixels, as specified by <paramref name="fillColor" /> from
        ///             the outside as required.
        ///         </item>
        ///         <item>Above 20 degrees, sampling rotation will be used if shear was requested.</item>
        ///         <item>Colormaps are removed for rotation by area map and shear.</item>
        ///         <item>
        ///             The resulting image can be expanded so that no image pixels are lost. To invoke expansion,
        ///             input the original width and height. For repeated rotation, use of the original width and height allows
        ///             expansion to stop at the maximum required size which is a square of side = sqrt(w*w + h*h).
        ///         </item>
        ///     </list>
        ///     <para>
        ///         Please note there is an implicit assumption about RGB component ordering.
        ///     </para>
        /// </remarks>
        /// <param name="angleInRadians">The angle to rotate by, in radians; clockwise is positive.</param>
        /// <param name="method">The rotation method to use.</param>
        /// <param name="fillColor">The fill color to use for pixels that are brought in from the outside.</param>
        /// <param name="width">The original width; use 0 to avoid embedding</param>
        /// <param name="height">The original height; use 0 to avoid embedding</param>
        /// <returns>The image rotated around it's centre</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image Rotate(float angleInRadians, RotationMethod method = RotationMethod.AreaMap, RotationFill fillColor = RotationFill.White, int? width = null, int? height = null)
        {
            if (width == null) width = Width;
            if (height == null) height = Height;

            if (Math.Abs(angleInRadians) < VerySmallAngle)
                return Clone();

            var rotations = 2 * angleInRadians / Math.PI;

            Logger.LogInformation("Rotating image around its centre");

            var resultHandle = Math.Abs(rotations - Math.Floor(rotations)) < VerySmallAngle
                ? LeptonicaApi.Native.pixRotateOrth(_handle, (int)rotations)
                : LeptonicaApi.Native.pixRotate(_handle, angleInRadians, method, fillColor, width.Value, height.Value);

            if (resultHandle == IntPtr.Zero)
            {
                const string message = "Failed to rotate image around its centre";
                Logger.LogError(message);
                throw new LeptonicaException(message);
            }

            Logger.LogInformation("Image rotated around its centre");

            return new Image(resultHandle);
        }
        #endregion

        #region Rotate90
        /// <summary>
        ///     90 degree rotation.
        /// </summary>
        /// <param name="direction">1 = clockwise,  -1 = counter-clockwise</param>
        /// <returns>Rotated image</returns>
        /// <exception cref="LeptonicaException">Raised when something fails</exception>
        public Image Rotate90(RotationDirection direction)
        {
            var resultHandle = LeptonicaApi.Native.pixRotate90(_handle, (int) direction);

            Logger.LogInformation($"Rotating image 90 degrees {direction}");

            if (resultHandle == IntPtr.Zero)
            {
                const string message = "Failed to rotate image 90 degrees";
                Logger.LogError(message);
                throw new LeptonicaException(message);
            }

            Logger.LogInformation($"Image rotated 90 degrees {direction}");

            return new Image(resultHandle);
        }
        #endregion

        #region Blend
        /// <summary>
        ///     Blends this image with the given <paramref name="imageToBlendWith"/>
        /// </summary>
        /// <param name="imageToBlendWith"><see cref="Image"/></param>
        /// <param name="x">origin [UL corner] of <paramref name="imageToBlendWith"/> relative to the origin of <see cref="Image"/> can be  is smaller 0</param>
        /// <param name="y">origin [UL corner] of <paramref name="imageToBlendWith"/> relative to the origin of <see cref="Image"/> can be  is smaller 0</param>
        /// <param name="fraction"></param>
        /// <returns></returns>
        public Image Blend(Image imageToBlendWith, int x, int y, float fraction)
        {
            if (imageToBlendWith == null) 
                throw new ArgumentNullException(nameof(imageToBlendWith));

            Logger.LogInformation("Blending image");

            var result = LeptonicaApi.Native.pixBlend(_handle, imageToBlendWith.Handle, x, y, fraction);

            Logger.LogInformation("Image blend");

            return result == IntPtr.Zero ? null : new Image(result);
        }
        #endregion

        #region Dispose
        /// <summary>
        ///     Disposes the <see cref="Image"/> object
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            var tmpHandle = _handle.Handle;
            LeptonicaApi.Native.pixDestroy(ref tmpHandle);
            _handle = new HandleRef(this, IntPtr.Zero);
        }
        #endregion
    }
}