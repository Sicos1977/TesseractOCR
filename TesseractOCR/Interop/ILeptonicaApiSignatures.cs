using System;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.InteropDotNet;
// ReSharper disable InconsistentNaming
#pragma warning disable IDE0051
#pragma warning disable CS1591

namespace TesseractOCR.Interop;

/// <summary>
///     The exported leptonica api signatures.
/// </summary>
/// <remarks>
///     Please note this is only public for technical reasons (you can't proxy a internal interface).
///     It should be considered an internal interface and is NOT part of the public api and may have 
///     breaking changes between releases.
/// </remarks>
public interface ILeptonicaApiSignatures
{
    #region PixA
    /// <summary>Reads all pages of a multi-page TIFF file into a pix array.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaReadMultipageTiff")]
    IntPtr pixaReadMultipageTiff(string filename);

    /// <summary>Creates a pix array with the specified initial capacity.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaCreate")]
    IntPtr pixaCreate(int n);

    /// <summary>Adds a pix to a pix array using the specified copy mode.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaAddPix")]
    int pixaAddPix(HandleRef pixa, HandleRef pix, PixArrayAccessType copyflag);

    /// <summary>Gets a pix from an array using the specified access mode.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaGetPix")]
    IntPtr pixaGetPix(HandleRef pixa, int index, PixArrayAccessType accesstype);

    /// <summary>Removes the pix at the specified index from an array.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaRemovePix")]
    int pixaRemovePix(HandleRef pixa, int index);

    /// <summary>Removes all pix images from an array.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaClear")]
    int pixaClear(HandleRef pixa);

    /// <summary>Gets the number of pix images in an array.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaGetCount")]
    int pixaGetCount(HandleRef pixa);

    /// <summary>Destroys a pix array and releases its resources.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaDestroy")]
    void pixaDestroy(ref IntPtr pix);
    #endregion

    #region Pix
    /// <summary>Creates a pix image with the specified dimensions and depth.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCreate")]
    IntPtr pixCreate(int width, int height, int depth);

    /// <summary>Creates a clone that shares the source pix image data.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixClone")]
    IntPtr pixClone(HandleRef pix);

    /// <summary>Destroys a pix image and releases its resources.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDestroy")]
    void pixDestroy(ref IntPtr pix);

    /// <summary>Compares two pix images for equality.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixEqual")]
    int pixEqual(HandleRef pix1, HandleRef pix2, out int same);

    /// <summary>Gets the width of a pix image in pixels.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetWidth")]
    int pixGetWidth(HandleRef pix);

    /// <summary>Gets the height of a pix image in pixels.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetHeight")]
    int pixGetHeight(HandleRef pix);
    
    /// <summary>Gets the pixel depth of a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetDepth")]
    int pixGetDepth(HandleRef pix);

    /// <summary>Gets the horizontal resolution of a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetXRes")]
    int pixGetXRes(HandleRef pix);

    /// <summary>Gets the vertical resolution of a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetYRes")]
    int pixGetYRes(HandleRef pix);

    /// <summary>Gets the horizontal and vertical resolution of a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetResolution")]
    int pixGetResolution(HandleRef pix, out int xres, out int yres);

    /// <summary>Gets the number of 32-bit words in each image raster line.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetWpl")]
    int pixGetWpl(HandleRef pix);

    /// <summary>Sets the horizontal resolution of a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetXRes")]
    int pixSetXRes(HandleRef pix, int xres);

    /// <summary>Sets the vertical resolution of a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetYRes")]
    int pixSetYRes(HandleRef pix, int yres);

    /// <summary>Sets the horizontal and vertical resolution of a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetResolution")]
    int pixSetResolution(HandleRef pix, int xres, int yres);

    /// <summary>Scales the horizontal and vertical resolution of a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixScaleResolution")]
    int pixScaleResolution(HandleRef pix, float xscale, float yscale);

    /// <summary>Gets a pointer to the pixel data of a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetData")]
    IntPtr pixGetData(HandleRef pix);
    
    /// <summary>Gets the input format recorded for a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetInputFormat")]
    ImageFormat pixGetInputFormat(HandleRef pix);

    /// <summary>Sets the input format recorded for a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetInputFormat")]
    int pixSetInputFormat(HandleRef pix, ImageFormat inputFormat);

    /// <summary>Swaps the byte order of the pixel data in a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixEndianByteSwap")]
    int pixEndianByteSwap(HandleRef pix);

    /// <summary>Reads an image from a file.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRead")]
    IntPtr pixRead(string filename);

    /// <summary>Reads an image from a memory buffer.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixReadMem")]
    unsafe IntPtr pixReadMem(byte* data, UIntPtr size);

    /// <summary>Reads a TIFF page from a memory buffer.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixReadMemTiff")]
    unsafe IntPtr pixReadMemTiff(byte* data, UIntPtr size, int page);

    /// <summary>Reads the next page of a multi-page TIFF from memory and advances the offset.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixReadMemFromMultipageTiff")]
    unsafe IntPtr pixReadMemFromMultipageTiff(byte* data, UIntPtr size, ref UIntPtr offset);

    /// <summary>
    /// Provides array with all pages.
    /// Loops over pixReadMemFromMultipageTiff inside of leptonica
    /// </summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaReadMemMultipageTiff")]
    unsafe IntPtr pixaReadMemMultipageTiff(byte* data, UIntPtr size);

    /// <summary>
    /// Should be used to save on memory when working with large / multiple files at a time.
    /// Only a single page will be read an hold in memory
    /// </summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixReadFromMultipageTiff")]
    IntPtr pixReadFromMultipageTiff(string filename, ref UIntPtr offset);

    /// <summary>Writes a pix image to a file in the specified format.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixWrite")]
    int pixWrite(string filename, HandleRef handle, ImageFormat format);

    /// <summary>Writes a reduced copy of an image to the display output file.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDisplayWrite")]
    int pixDisplayWrite(HandleRef pixs, int reduction);

    /// <summary>Gets the colormap associated with a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetColormap")]
    IntPtr pixGetColormap(HandleRef pix);

    /// <summary>Associates a colormap with a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetColormap")]
    int pixSetColormap(HandleRef pix, HandleRef pixCmap);

    /// <summary>Removes and destroys the colormap associated with a pix image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDestroyColormap")]
    int pixDestroyColormap(HandleRef pix);
    #endregion

    #region pixconv.h functions
    /// <summary>Converts an RGB pix image to grayscale using the specified channel weights.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixConvertRGBToGray")]
    IntPtr pixConvertRGBToGray(HandleRef pix, float rwt, float gwt, float bwt);

    /// <summary>Converts a pix image to 8 bits per pixel.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixConvertTo8")]
    IntPtr pixConvertTo8(HandleRef pix, int cmapflag);
    #endregion

    #region OCR preprocessing
    /// <summary>Normalizes an image background using Leptonica's default parameters.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixBackgroundNormSimple")]
    IntPtr pixBackgroundNormSimple(HandleRef pixs, IntPtr pixim, IntPtr pixg);

    /// <summary>Normalizes local contrast in an 8-bit grayscale image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixContrastNorm")]
    IntPtr pixContrastNorm(IntPtr pixd, HandleRef pixs, int sx, int sy, int mindiff, int smoothx, int smoothy);

    /// <summary>Applies a median filter to an image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixMedianFilter")]
    IntPtr pixMedianFilter(HandleRef pixs, int width, int height);

    /// <summary>Composites an alpha channel against white.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRemoveAlpha")]
    IntPtr pixRemoveAlpha(HandleRef pixs);

    /// <summary>Adds a black or white border to an image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixAddBlackOrWhiteBorder")]
    IntPtr pixAddBlackOrWhiteBorder(HandleRef pixs, int left, int right, int top, int bottom, int operation);

    /// <summary>Removes the same number of pixels from each image edge.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRemoveBorder")]
    IntPtr pixRemoveBorder(HandleRef pixs, int pixels);

    /// <summary>Creates a native rectangle.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxCreate")]
    IntPtr boxCreate(int x, int y, int width, int height);

    /// <summary>Copies an image region specified by a rectangle.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixClipRectangle")]
    IntPtr pixClipRectangle(HandleRef pixs, HandleRef box, IntPtr clippedBox);

    /// <summary>Dilates a binary image with a rectangular structuring element.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDilateBrick")]
    IntPtr pixDilateBrick(IntPtr pixd, HandleRef pixs, int width, int height);

    /// <summary>Erodes a binary image with a rectangular structuring element.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixErodeBrick")]
    IntPtr pixErodeBrick(IntPtr pixd, HandleRef pixs, int width, int height);

    /// <summary>Opens a binary image with a rectangular structuring element.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixOpenBrick")]
    IntPtr pixOpenBrick(IntPtr pixd, HandleRef pixs, int width, int height);

    /// <summary>Closes a binary image with a rectangular structuring element.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCloseBrick")]
    IntPtr pixCloseBrick(IntPtr pixd, HandleRef pixs, int width, int height);
    #endregion

    #region Skew
    /// <summary>Deskews an image and returns the estimated skew angle and confidence.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDeskewGeneral")]
    IntPtr pixDeskewGeneral(HandleRef pix, int redSweep, float sweepRange, float sweepDelta, int redSearch, int thresh, out float pAngle, out float pConf);

    /// <summary>Finds the skew angle and confidence for an image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixFindSkew")]
    int pixFindSkew(HandleRef pixs, out float pangle, out float pconf);
    #endregion

    #region Rotation
    /// <summary>Rotates a pix image by an angle using the specified method and fill color.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotate")]
    IntPtr pixRotate(HandleRef pixs, float angle, RotationMethod type, RotationFill fillColor, int width, int height);

    /// <summary>Rotates a pix image by a multiple of 90 degrees.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotateOrth")]
    IntPtr pixRotateOrth(HandleRef pixs, int quads);

    /// <summary>Rotates a grayscale pix image by an angle using area mapping.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotateAMGray")]
    IntPtr pixRotateAMGray(HandleRef pixs, float angle, byte grayval);

    /// <summary>Rotates a pix image by 90 degrees in the specified direction.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotate90")]
    IntPtr pixRotate90(HandleRef pixs, int direction);
    #endregion

    #region Grayscale
    /// <summary>Applies grayscale morphological closing with the specified brick dimensions.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCloseGray")]
    IntPtr pixCloseGray(HandleRef pixs, int hsize, int vsize);

    /// <summary>Applies grayscale erosion with the specified brick dimensions.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixErodeGray")]
    IntPtr pixErodeGray(HandleRef pixs, int hsize, int vsize);

    /// <summary>Adds two grayscale images, optionally writing into a destination image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixAddGray")]
    IntPtr pixAddGray(HandleRef pixd, HandleRef pixs1, HandleRef pixs2);

    /// <summary>Applies grayscale morphological opening with the specified brick dimensions.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixOpenGray")]
    IntPtr pixOpenGray(HandleRef pixs, int hsize, int vsize);

    /// <summary>Combines source pixels into a destination image through a mask.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCombineMasked")]
    int pixCombineMasked(HandleRef pixd, HandleRef pixs, HandleRef pixm);
    #endregion

    #region Threshold
    /// <summary>Sets source pixels below a threshold to the specified value.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixThresholdToValue")]
    IntPtr pixThresholdToValue(HandleRef pixd, HandleRef pixs, int threshval, int setval);

    /// <summary>Thresholds a grayscale image to a binary image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixThresholdToBinary")]
    IntPtr pixThresholdToBinary(HandleRef pixs, int thresh);

    /// <summary>Inverts the pixel values of an image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixInvert")]
    IntPtr pixInvert(HandleRef pixd, HandleRef pixs);

    /// <summary>Normalizes an image background using flexible local scaling parameters.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixBackgroundNormFlex")]
    IntPtr pixBackgroundNormFlex(HandleRef pixs, int sx, int sy, int smoothx, int smoothy, int delta);

    /// <summary>Applies a gamma tone curve to an image, optionally restricted by a mask.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGammaTRCMasked")]
    IntPtr pixGammaTRCMasked(HandleRef pixd, HandleRef pixs, HandleRef pixm, float gamma, int minval, int maxval);

    /// <summary>Applies a hit-miss transform using the specified structuring element.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixHMT")]
    IntPtr pixHMT(HandleRef pixd, HandleRef pixs, HandleRef sel);

    /// <summary>Dilates a binary image using the specified structuring element.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDilate")]
    IntPtr pixDilate(HandleRef pixd, HandleRef pixs, HandleRef sel);

    /// <summary>Subtracts one image from another, optionally writing into a destination image.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSubtract")]
    IntPtr pixSubtract(HandleRef pixd, HandleRef pixs1, HandleRef pixs2);
    #endregion

    #region Sel
    /// <summary>Creates a structuring element from a text pattern.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "selCreateFromString")]
    IntPtr selCreateFromString(string text, int h, int w, string name);

    /// <summary>Creates a rectangular structuring element with the specified origin and type.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "selCreateBrick")]
    IntPtr selCreateBrick(int h, int w, int cy, int cx, SelType type);

    /// <summary>Destroys a structuring element and releases its resources.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "selDestroy")]
    void selDestroy(ref IntPtr psel);
    #endregion

    #region Binarization - src/binarize.c
    /// <summary>Computes an adaptive Otsu threshold image and its binarized result.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixOtsuAdaptiveThreshold")]
    int pixOtsuAdaptiveThreshold(HandleRef pix, int sx, int sy, int smoothx, int smoothy, float scorefract, out IntPtr ppixth, out IntPtr ppixd);

    /// <summary>Binarizes an image using the Sauvola thresholding method.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSauvolaBinarize")]
    int pixSauvolaBinarize(HandleRef pix, int whsize, float factor, int addborder, out IntPtr ppixm, out IntPtr ppixsd, out IntPtr ppixth, out IntPtr ppixd);

    /// <summary>Binarizes an image with Sauvola thresholding in tiles.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSauvolaBinarizeTiled")]
    int pixSauvolaBinarizeTiled(HandleRef pix, int whsize, float factor, int nx, int ny, out IntPtr ppixth, out IntPtr ppixd);
    #endregion

    #region Scaling - src/scale.c
    /// <summary>Scales a pix image independently along the horizontal and vertical axes.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixScale")]
    IntPtr pixScale(HandleRef pixs, float scalex, float scaley);
    #endregion

    #region Color map creation and deletion
    /// <summary>
    /// Creates a new colormap with the specified <paramref name="depth"/>.
    /// </summary>
    /// <param name="depth">The depth of the pix in bpp, can be 2, 4, or 8</param>
    /// <returns>The pointer to the color map, or null on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCreate")]
    IntPtr pixcmapCreate(int depth);

    /// <summary>
    /// Creates a new colormap of the specified <paramref name="depth"/> with random colors where the first color can optionally be set to black, and the last optionally set to white.
    /// </summary>
    /// <param name="depth">The depth of the pix in bpp, can be 2, 4, or 8</param>
    /// <param name="hasBlack">If set to 1 the first color will be black.</param>
    /// <param name="hasWhite">If set to 1 the last color will be white.</param>
    /// <returns>The pointer to the color map, or null on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCreateRandom")]
    IntPtr pixcmapCreateRandom(int depth, int hasBlack, int hasWhite);

    /// <summary>
    /// Creates a new colormap of the specified <paramref name="depth"/> with equally spaced gray color values. 
    /// </summary>
    /// <param name="depth">The depth of the pix in bpp, can be 2, 4, or 8</param>
    /// <param name="levels">The number of levels (must be between 2 and 2^<paramref name="depth"/></param>
    /// <returns>The pointer to the colormap, or null on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCreateLinear")]
    IntPtr pixcmapCreateLinear(int depth, int levels);

    /// <summary>
    /// Performs a deep copy of the color map.
    /// </summary>
    /// <param name="cmaps">The pointer to the colormap instance.</param>
    /// <returns>The pointer to the colormap, or null on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCopy")]
    IntPtr pixcmapCopy(HandleRef cmaps);

    /// <summary>
    /// Destroys and cleans up any memory used by the color map.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance, set to null on success.</param>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapDestroy")]
    void pixcmapDestroy(ref IntPtr cmap);

    // colormap metadata (depth, count, etc)

    /// <summary>
    /// Gets the number of color entries in the color map.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <returns>Returns the number of color entries in the color map, or 0 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetCount")]
    int pixcmapGetCount(HandleRef cmap);

    /// <summary>
    /// Gets the number of free color entries in the color map.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <returns>Returns the number of free color entries in the color map, or 0 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetFreeCount")]
    int pixcmapGetFreeCount(HandleRef cmap);


    /// <returns>Returns color maps depth, or 0 on error.</returns>
    /// <summary>Gets the bit depth of a colormap.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetDepth")]
    int pixcmapGetDepth(HandleRef cmap);

    /// <summary>
    /// Gets the minimum pix depth required to support the color map.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <param name="minDepth">Returns the minimum depth to support the colormap</param>
    /// <returns>Returns 0 if OK, 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetMinDepth")]
    int pixcmapGetMinDepth(HandleRef cmap, out int minDepth);

    // colormap - color addition\clearing

    /// <summary>
    /// Removes all colors from the color map by setting the count to zero.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <returns>Returns 0 if OK, 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapClear")]
    int pixcmapClear(HandleRef cmap);

    /// <summary>
    /// Adds the color to the pix color map if their is room.
    /// </summary>
    /// <returns>Returns 0 if OK, 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddColor")]
    int pixcmapAddColor(HandleRef cmap, int redValue, int greenValue, int blueValue);

    /// <summary>
    /// Adds the specified color if it doesn't already exist, returning the colors index in the data array.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <param name="redValue">The red value</param>
    /// <param name="greenValue">The green value</param>
    /// <param name="blueValue">The blue value</param>
    /// <param name="colorIndex">The index of the new color if it was added, or the existing color if it already existed.</param>
    /// <returns>Returns 0 for success, 1 for error, 2 for not enough space.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddNewColor")]
    int pixcmapAddNewColor(HandleRef cmap, int redValue, int greenValue, int blueValue, out int colorIndex);

    /// <summary>
    /// Adds the specified color if it doesn't already exist, returning the color's index in the data array.
    /// </summary>
    /// <remarks>
    /// If the color doesn't exist and there is not enough room to add a new color return the nearest color.
    /// </remarks>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <param name="redValue">The red value</param>
    /// <param name="greenValue">The green value</param>
    /// <param name="blueValue">The blue value</param>
    /// <param name="colorIndex">The index of the new color if it was added, or the existing color if it already existed.</param>
    /// <returns>Returns 0 for success, 1 for error, 2 for not enough space.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddNearestColor")]
    int pixcmapAddNearestColor(HandleRef cmap, int redValue, int greenValue, int blueValue, out int colorIndex);

    /// <summary>
    /// Checks if the color already exists or if their is enough room to add it.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <param name="redValue">The red value</param>
    /// <param name="greenValue">The green value</param>
    /// <param name="blueValue">The blue value</param>
    /// <param name="usable">Returns 1 if usable; 0 if not.</param>
    /// <returns>Returns 0 if OK, 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapUsableColor")]
    int pixcmapUsableColor(HandleRef cmap, int redValue, int greenValue, int blueValue, out int usable);

    /// <summary>
    /// Adds a color (black\white) if not already there returning it's index through <paramref name="index"/>.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <param name="color">The color to add (0 for black; 1 for white)</param>
    /// <param name="index">The index of the color.</param>
    /// <returns>Returns 0 if OK; 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddBlackOrWhite")]
    int pixcmapAddBlackOrWhite(HandleRef cmap, int color, out int index);

    /// <summary>
    /// Sets the darkest color in the colormap to black, if <paramref name="setBlack"/> is 1. 
    /// Sets the lightest color in the colormap to white if <paramref name="setWhite"/> is 1. 
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <param name="setBlack">0 for no operation; 1 to set darkest color to black</param>
    /// <param name="setWhite">0 for no operation; 1 to set lightest color to white</param>
    /// <returns>Returns 0 if OK; 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapSetBlackAndWhite")]
    int pixcmapSetBlackAndWhite(HandleRef cmap, int setBlack, int setWhite);

    // color access - color entry access

    /// <summary>
    /// Gets the color at the specified index.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <param name="index">The index of the color entry.</param>
    /// <param name="redValue">The color entry's red value.</param>
    /// <param name="greenValue">The color entry's green value.</param>
    /// <param name="blueValue">The color entry's blue value.</param>
    /// <returns>Returns 0 if OK; 1 if not accessable (caller should check).</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetColor")]
    int pixcmapGetColor(HandleRef cmap, int index, out int redValue, out int greenValue, out int blueValue);

    /// <summary>
    /// Gets the color at the specified index.
    /// </summary>
    /// <remarks>
    /// The alpha channel will always be zero as it is not used in Leptonica color maps.
    /// </remarks>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <param name="index">The index of the color entry.</param>
    /// <param name="color">The color entry as 32 bit value</param>
    /// <returns>Returns 0 if OK; 1 if not accessable (caller should check).</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetColor32")]
    int pixcmapGetColor32(HandleRef cmap, int index, out uint color);

    /// <summary>
    /// Sets a previously allocated color entry.
    /// </summary>
    /// <param name="cmap">The pointer to the colormap instance.</param>
    /// <param name="index">The index of the colormap entry</param>
    /// <param name="redValue"></param>
    /// <param name="greenValue"></param>
    /// <param name="blueValue"></param>
    /// <returns>Returns 0 if OK; 1 if not accessable (caller should check).</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapResetColor")]
    int pixcmapResetColor(HandleRef cmap, int index, int redValue, int greenValue, int blueValue);

    /// <summary>
    /// Gets the index of the color entry with the specified color, return 0 if found; 1 if not.
    /// </summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetIndex")]
    int pixcmapGetIndex(HandleRef cmap, int redValue, int greenValue, int blueValue, out int index);

    /// <summary>
    /// Determines whether the colormap contains any non-gray colors.
    /// </summary>
    /// <param name="cmap">The colormap to check.</param>
    /// <param name="hasColor">Receives 1 when the colormap contains non-gray colors; otherwise 0.</param>
    /// <returns>Returns 0 if OK; 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapHasColor")]
    int pixcmapHasColor(HandleRef cmap, out int hasColor);


    /// <summary>
    /// Returns the number of unique grey colors including black and white.
    /// </summary>
    /// <returns>Returns 0 if OK; 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCountGrayColors")]
    int pixcmapCountGrayColors(HandleRef cmap, out int ngray);

    /// <summary>
    /// Finds the index of the color entry with the rank intensity.
    /// </summary>
    /// <returns>Returns 0 if OK; 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetRankIntensity")]
    int pixcmapGetRankIntensity(HandleRef cmap, float rankVal, out int index);


    /// <summary>
    /// Finds the index of the color entry closest to the specified color.
    /// </summary>
    /// <returns>Returns 0 if OK; 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetNearestIndex")]
    int pixcmapGetNearestIndex(HandleRef cmap, int rVal, int gVal, int bVal, out int index);

    /// <summary>
    /// Finds the index of the color entry closest to the specified color.
    /// </summary>
    /// <remarks>
    /// Should only be used on gray colormaps.
    /// </remarks>
    /// <returns>Returns 0 if OK; 1 on error.</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetNearestGrayIndex")]
    int pixcmapGetNearestGrayIndex(HandleRef cmap, int val, out int index);
    #endregion

    #region Color map conversion
    /// <summary>Creates a colormap by mapping grayscale values to a specified color.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGrayToColor")]
    IntPtr pixcmapGrayToColor(uint color);

    /// <summary>Converts a color colormap to grayscale using the specified channel weights.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapColorToGray")]
    IntPtr pixcmapColorToGray(HandleRef cmaps, float redWeight, float greenWeight, float blueWeight);
    #endregion

    #region Colormap serialization
    /// <summary>Copies colormap channel values into separate arrays.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapToArrays")]
    int pixcmapToArrays(HandleRef cmap, out IntPtr redMap, out IntPtr greenMap, out IntPtr blueMap, out IntPtr alphaMap);

    /// <summary>Converts a colormap to a packed RGB table.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapToRGBTable")]
    int pixcmapToRGBTable(HandleRef cmap, out IntPtr colorTable, out int colorCount);

    /// <summary>Serializes a colormap into a memory buffer.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapSerializeToMemory")]
    int pixcmapSerializeToMemory(HandleRef cmap, int componentsPerColor, out int colorCount, out IntPtr colorData);

    /// <summary>Creates a colormap by deserializing its data from memory.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapDeserializeFromMemory")]
    unsafe IntPtr pixcmapDeserializeFromMemory(byte* colorData, int componentsPerColor, int colorCount);
    #endregion

    #region Colormap transformations 
    /// <summary>Applies a gamma tone curve to the colors in a colormap.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGammaTRC")]
    int pixcmapGammaTRC(HandleRef cmap, float gamma, int minVal, int maxVal);

    /// <summary>Adjusts the contrast of the colors in a colormap.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapContrastTRC")]
    int pixcmapContrastTRC(HandleRef cmap, float factor);

    /// <summary>Shifts the intensity of the colors in a colormap.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapShiftIntensity")]
    int pixcmapShiftIntensity(HandleRef cmap, float fraction);
    #endregion
    
    #region Box
    /// <summary>Gets the number of boxes in a box array.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxaGetCount")]
    int boxaGetCount(HandleRef boxa);

    /// <summary>Gets a box from an array using the specified access mode.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxaGetBox")]
    IntPtr boxaGetBox(HandleRef boxa, int index, PixArrayAccessType accesstype);

    /// <summary>Gets the position and dimensions of a box.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxGetGeometry")]
    int boxGetGeometry(HandleRef box, out int px, out int py, out int pw, out int ph);

    /// <summary>Destroys a box and releases its resources.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxDestroy")]
    void boxDestroy(ref IntPtr box);

    /// <summary>Destroys a box array and releases its resources.</summary>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxaDestroy")]
    void boxaDestroy(ref IntPtr box);

    #endregion

    #region pixBlend
    ///  <summary>
    ///     (1) This is a simple top-level interface.  For more flexibility,
    ///     call directly into pixBlendMask(), etc.
    ///  </summary>
    ///  <remarks>
    ///  </remarks>
    ///  <param name="pixs1">[in] - blendee</param>
    ///  <param name="pixs2">[in] - blender typ. smaller</param>
    ///  <param name="x">[in] - ,y  origin [UL corner] of pixs2 relative to the origin of pixs1 can be  is smaller 0</param>
    ///  <param name="y"></param>
    ///  <param name="fraction">[in] - blending fraction</param>
    ///   <returns>pixd blended image, or NULL on error</returns>
    [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixBlend")]
    IntPtr pixBlend(HandleRef pixs1, HandleRef pixs2, int x, int y, float fraction);
    #endregion
}