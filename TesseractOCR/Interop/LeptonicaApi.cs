//
// LeptonicaApi.cs
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
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.InteropDotNet;
#pragma warning disable CS1591

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Interop
{
    // TODO: Update leptonica interface

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
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaReadMultipageTiff")]
        IntPtr pixaReadMultipageTiff(string filename);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaCreate")]
        IntPtr pixaCreate(int n);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaAddPix")]
        int pixaAddPix(HandleRef pixa, HandleRef pix, PixArrayAccessType copyflag);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaGetPix")]
        IntPtr pixaGetPix(HandleRef pixa, int index, PixArrayAccessType accesstype);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaRemovePix")]
        int pixaRemovePix(HandleRef pixa, int index);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaClear")]
        int pixaClear(HandleRef pixa);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaGetCount")]
        int pixaGetCount(HandleRef pixa);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaDestroy")]
        void pixaDestroy(ref IntPtr pix);
        #endregion

        #region Pix
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCreate")]
        IntPtr pixCreate(int width, int height, int depth);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixClone")]
        IntPtr pixClone(HandleRef pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDestroy")]
        void pixDestroy(ref IntPtr pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixEqual")]
        int pixEqual(HandleRef pix1, HandleRef pix2, out int same);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetWidth")]
        int pixGetWidth(HandleRef pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetHeight")]
        int pixGetHeight(HandleRef pix);
        
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetDepth")]
        int pixGetDepth(HandleRef pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetXRes")]
        int pixGetXRes(HandleRef pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetYRes")]
        int pixGetYRes(HandleRef pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetResolution")]
        int pixGetResolution(HandleRef pix, out int xres, out int yres);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetWpl")]
        int pixGetWpl(HandleRef pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetXRes")]
        int pixSetXRes(HandleRef pix, int xres);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetYRes")]
        int pixSetYRes(HandleRef pix, int yres);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetResolution")]
        int pixSetResolution(HandleRef pix, int xres, int yres);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixScaleResolution")]
        int pixScaleResolution(HandleRef pix, float xscale, float yscale);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetData")]
        IntPtr pixGetData(HandleRef pix);
        
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetInputFormat")]
        ImageFormat pixGetInputFormat(HandleRef pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetInputFormat")]
        int pixSetInputFormat(HandleRef pix, ImageFormat inputFormat);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixEndianByteSwap")]
        int pixEndianByteSwap(HandleRef pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRead")]
        IntPtr pixRead(string filename);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixReadMem")]
        unsafe IntPtr pixReadMem(byte* data, int length);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixReadMemTiff")]
        unsafe IntPtr pixReadMemTiff(byte* data, int length, int page);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixReadMemFromMultipageTiff")]
        unsafe IntPtr pixReadMemFromMultipageTiff (byte* data, int length, ref int offset);

        /// <summary>
        /// Provides array with all pages.
        /// Loops over pixReadMemFromMultipageTiff inside of leptonica
        /// </summary>
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaReadMemMultipageTiff")]
        unsafe IntPtr pixaReadMemMultipageTiff (byte* data, int length);

        /// <summary>
        /// Should be used to save on memory when working with large / multiple files at a time.
        /// Only a single page will be read an hold in memory
        /// </summary>
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixReadFromMultipageTiff")]
        IntPtr pixReadFromMultipageTiff(string filename, ref int offset);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixWrite")]
        int pixWrite(string filename, HandleRef handle, ImageFormat format);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDisplayWrite")]
        int pixDisplayWrite(HandleRef pixs, int reduction);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetColormap")]
        IntPtr pixGetColormap(HandleRef pix);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetColormap")]
        int pixSetColormap(HandleRef pix, HandleRef pixCmap);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDestroyColormap")]
        int pixDestroyColormap(HandleRef pix);
        #endregion

        #region pixconv.h functions
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixConvertRGBToGray")]
        IntPtr pixConvertRGBToGray(HandleRef pix, float rwt, float gwt, float bwt);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixConvertTo8")]
        IntPtr pixConvertTo8(HandleRef pix, int cmapflag);
        #endregion

        #region Skew
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDeskewGeneral")]
        IntPtr pixDeskewGeneral(HandleRef pix, int redSweep, float sweepRange, float sweepDelta, int redSearch, int thresh, out float pAngle, out float pConf);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixFindSkew")]
        int pixFindSkew(HandleRef pixs, out float pangle, out float pconf);
        #endregion

        #region Rotation
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotate")]
        IntPtr pixRotate(HandleRef pixs, float angle, RotationMethod type, RotationFill fillColor, int width, int height);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotateOrth")]
        IntPtr pixRotateOrth(HandleRef pixs, int quads);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotateAMGray")]
        IntPtr pixRotateAMGray(HandleRef pixs, float angle, byte grayval);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotate90")]
        IntPtr pixRotate90(HandleRef pixs, int direction);
        #endregion

        #region Grayscale
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCloseGray")]
        IntPtr pixCloseGray(HandleRef pixs, int hsize, int vsize);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixErodeGray")]
        IntPtr pixErodeGray(HandleRef pixs, int hsize, int vsize);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixAddGray")]
        IntPtr pixAddGray(HandleRef pixd, HandleRef pixs1, HandleRef pixs2);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixOpenGray")]
        IntPtr pixOpenGray(HandleRef pixs, int hsize, int vsize);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCombineMasked")]
        int pixCombineMasked(HandleRef pixd, HandleRef pixs, HandleRef pixm);
        #endregion

        #region Threshold
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixThresholdToValue")]
        IntPtr pixThresholdToValue(HandleRef pixd, HandleRef pixs, int threshval, int setval);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixThresholdToBinary")]
        IntPtr pixThresholdToBinary(HandleRef pixs, int thresh);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixInvert")]
        IntPtr pixInvert(HandleRef pixd, HandleRef pixs);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixBackgroundNormFlex")]
        IntPtr pixBackgroundNormFlex(HandleRef pixs, int sx, int sy, int smoothx, int smoothy, int delta);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGammaTRCMasked")]
        IntPtr pixGammaTRCMasked(HandleRef pixd, HandleRef pixs, HandleRef pixm, float gamma, int minval, int maxval);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixHMT")]
        IntPtr pixHMT(HandleRef pixd, HandleRef pixs, HandleRef sel);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDilate")]
        IntPtr pixDilate(HandleRef pixd, HandleRef pixs, HandleRef sel);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSubtract")]
        IntPtr pixSubtract(HandleRef pixd, HandleRef pixs1, HandleRef pixs2);
        #endregion

        #region Sel
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "selCreateFromString")]
        IntPtr selCreateFromString(string text, int h, int w, string name);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "selCreateBrick")]
        IntPtr selCreateBrick(int h, int w, int cy, int cx, SelType type);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "selDestroy")]
        void selDestroy(ref IntPtr psel);
        #endregion

        #region Binarization - src/binarize.c
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixOtsuAdaptiveThreshold")]
        int pixOtsuAdaptiveThreshold(HandleRef pix, int sx, int sy, int smoothx, int smoothy, float scorefract, out IntPtr ppixth, out IntPtr ppixd);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSauvolaBinarize")]
        int pixSauvolaBinarize(HandleRef pix, int whsize, float factor, int addborder, out IntPtr ppixm, out IntPtr ppixsd, out IntPtr ppixth, out IntPtr ppixd);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSauvolaBinarizeTiled")]
        int pixSauvolaBinarizeTiled(HandleRef pix, int whsize, float factor, int nx, int ny, out IntPtr ppixth, out IntPtr ppixd);
        #endregion

        #region Scaling - src/scale.c
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
        /// <param name="blueValue">The color entry's blue value.</param>
        /// <param name="greenValue">The color entry's green value.</param>
        /// <returns>Returns 0 if OK; 1 if not accessable (caller should check).</returns>
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetColor")]
        int pixcmapGetColor(HandleRef cmap, int index, out int redValue, out int blueValue, out int greenValue);

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
        int pixcmapGetColor32(HandleRef cmap, int index, out int color);

        /// <summary>
        /// Sets a previously allocated color entry.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="index">The index of the colormap entry</param>
        /// <param name="redValue"></param>
        /// <param name="blueValue"></param>
        /// <param name="greenValue"></param>
        /// <returns>Returns 0 if OK; 1 if not accessable (caller should check).</returns>
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapResetColor")]
        int pixcmapResetColor(HandleRef cmap, int index, int redValue, int blueValue, int greenValue);

        /// <summary>
        /// Gets the index of the color entry with the specified color, return 0 if found; 1 if not.
        /// </summary>
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetIndex")]
        int pixcmapGetIndex(HandleRef cmap, int redValue, int blueValue, int greenValue, out int index);

        /// <summary>
        /// Returns 0 if the color exists in the color map; otherwise 1.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapHasColor")]
        int pixcmapHasColor(HandleRef cmap, int color);


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
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCountGrayColors")]
        int pixcmapGetRankIntensity(HandleRef cmap, float rankVal, out int index);


        /// <summary>
        /// Finds the index of the color entry closest to the specified color.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetNearestIndex")]
        int pixcmapGetNearestIndex(HandleRef cmap, int rVal, int bVal, int gVal, out int index);

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
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGrayToColor")]
        IntPtr pixcmapGrayToColor(int color);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapColorToGray")]
        IntPtr pixcmapColorToGray(HandleRef cmaps, float redWeight, float greenWeight, float blueWeight);
        #endregion

        #region Colormap serialization
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapColorToGray")]
        int pixcmapToArrays(HandleRef cmap, out IntPtr redMap, out IntPtr blueMap, out IntPtr greenMap);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapToRGBTable")]
        int pixcmapToRGBTable(HandleRef cmap, out IntPtr colorTable, out int colorCount);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapSerializeToMemory")]
        int pixcmapSerializeToMemory(HandleRef cmap, out int components, out int colorCount, out IntPtr colorData, out int colorDataLength);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapDeserializeFromMemory")]
        IntPtr pixcmapDeserializeFromMemory(HandleRef colorData, int colorCount, int colorDataLength);
        #endregion

        #region Colormap transformations 
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGammaTRC")]
        int pixcmapGammaTRC(HandleRef cmap, float gamma, int minVal, int maxVal);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapContrastTRC")]
        int pixcmapContrastTRC(HandleRef cmap, float factor);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapShiftIntensity")]
        int pixcmapShiftIntensity(HandleRef cmap, float fraction);
        #endregion
        
        #region Box
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxaGetCount")]
        int boxaGetCount(HandleRef boxa);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxaGetBox")]
        IntPtr boxaGetBox(HandleRef boxa, int index, PixArrayAccessType accesstype);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxGetGeometry")]
        int boxGetGeometry(HandleRef box, out int px, out int py, out int pw, out int ph);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxDestroy")]
        void boxDestroy(ref IntPtr box);

        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxaDestroy")]
        void boxaDestroy(ref IntPtr box);

        #endregion

        ///  <summary>
        /// (1) This is a simple top-level interface.  For more flexibility,
        /// call directly into pixBlendMask(), etc.
        ///  </summary>
        ///  <remarks>
        ///  </remarks>
        ///  <param name="pixs1">[in] - blendee</param>
        ///  <param name="pixs2">[in] - blender typ. smaller</param>
        ///  <param name="x">[in] - ,y  origin [UL corner] of pixs2 relative to the origin of pixs1 can be  is smaller 0</param>
        ///  <param name="fraction">[in] - blending fraction</param>
        ///   <returns>pixd blended image, or NULL on error</returns>
        [RuntimeDllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixBlend")]
        IntPtr pixBlend(HandleRef pixs1, HandleRef pixs2, int x, int y, float fraction);
    }

    internal static class LeptonicaApi
    {
        #region Fields
        private static ILeptonicaApiSignatures native;
        #endregion

        #region Properties
        public static ILeptonicaApiSignatures Native
        {
            get
            {
                if (native == null)
                    Initialize();

                return native;
            }
        }
        #endregion

        #region Initialize
        public static void Initialize()
        {
            if (native != null) return;
            Helper.SetPath();
            native = InteropRuntimeImplementer.CreateInstance<ILeptonicaApiSignatures>();
        }
        #endregion
    }
}