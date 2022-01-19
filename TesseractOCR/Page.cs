//
// Page.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TesseractOCR.Exceptions;
using TesseractOCR.Enums;
using TesseractOCR.Internal;
using TesseractOCR.Interop;
// ReSharper disable UnusedMember.Global

namespace TesseractOCR
{
    public sealed class Page : DisposableBase
    {
        private static readonly TraceSource Trace = new TraceSource("Tesseract");

        #region Fields
        private bool _runRecognitionPhase;
        private Rect _regionOfInterest;
        #endregion

        #region Properties
        /// <summary>
        ///     <see cref="TesseractEngine"/>
        /// </summary>
        public TesseractEngine Engine { get; }

        /// <summary>
        ///     Gets the <see cref="Pix" /> that is being ocr'd.
        /// </summary>
        public Pix Image { get; }

        /// <summary>
        ///     Gets the name of the image being ocr'd.
        /// </summary>
        /// <remarks>
        ///     This is also used for some of the more advanced functionality such as identifying the associated UZN file if
        ///     present.
        /// </remarks>
        public string ImageName { get; }

        /// <summary>
        ///     Gets the page segmentation mode used to OCR the specified image.
        /// </summary>
        public PageSegMode PageSegmentMode { get; }

        /// <summary>
        ///     The current region of interest being parsed.
        /// </summary>
        public Rect RegionOfInterest
        {
            get => _regionOfInterest;
            set
            {
                if (value.X1 < 0 || value.Y1 < 0 || value.X2 > Image.Width || value.Y2 > Image.Height)
                    throw new ArgumentException(
                        "The region of interest to be processed must be within the image bounds.", nameof(value));

                if (_regionOfInterest == value) return;
                _regionOfInterest = value;

                // update region of interest in image
                TessApi.Native.BaseApiSetRectangle(Engine.Handle, _regionOfInterest.X1, _regionOfInterest.Y1,
                    _regionOfInterest.Width, _regionOfInterest.Height);

                // request rerun of recognition on the next call that requires recognition
                _runRecognitionPhase = false;
            }
        }
        #endregion

        #region Constructor
        internal Page(TesseractEngine engine, Pix image, string imageName, Rect regionOfInterest, PageSegMode pageSegmentMode)
        {
            Engine = engine;
            Image = image;
            ImageName = imageName;
            RegionOfInterest = regionOfInterest;
            PageSegmentMode = pageSegmentMode;
        }
        #endregion

        #region GetThresholdedImage
        /// <summary>
        ///     Gets the thresholded image that was OCR'd.
        /// </summary>
        /// <returns></returns>
        public Pix GetThresholdedImage()
        {
            Recognize();

            var pixHandle = TessApi.Native.BaseAPIGetThresholdedImage(Engine.Handle);
            if (pixHandle == IntPtr.Zero) throw new TesseractException("Failed to get thresholded image.");

            return Pix.Create(pixHandle);
        }
        #endregion

        #region AnalyseLayout
        /// <summary>
        ///     Creates a <see cref="PageIterator" /> object that is used to iterate over the page's layout as defined by the
        ///     current <see cref="RegionOfInterest" />.
        /// </summary>
        /// <returns></returns>
        public PageIterator AnalyseLayout()
        {
            Guard.Verify(PageSegmentMode != PageSegMode.OsdOnly,
                "Cannot analyse image layout when using OSD only page segmentation, please use DetectBestOrientation instead.");

            var resultIteratorHandle = TessApi.Native.BaseAPIAnalyseLayout(Engine.Handle);
            return new PageIterator(this, resultIteratorHandle);
        }
        #endregion

        #region GetIterator
        /// <summary>
        ///     Creates a <see cref="ResultIterator" /> object that is used to iterate over the page as defined by the current
        ///     <see cref="RegionOfInterest" />.
        /// </summary>
        /// <returns></returns>
        public ResultIterator GetIterator()
        {
            Recognize();
            var resultIteratorHandle = TessApi.Native.BaseApiGetIterator(Engine.Handle);
            return new ResultIterator(this, resultIteratorHandle);
        }
        #endregion

        #region GetText
        /// <summary>
        ///     Gets the page's content as plain text.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            Recognize();
            return TessApi.BaseAPIGetUTF8Text(Engine.Handle);
        }
        #endregion

        #region GetHOCRText
        /// <summary>
        ///     Gets the page's content as an HOCR text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <param name="useXHtml">True to use XHTML Output, False to HTML Output</param>
        /// <returns>The OCR'd output as an HOCR text string.</returns>
        public string GetHOCRText(int pageNum, bool useXHtml = false)
        {
            // Why Not Use 'nameof(pageNum)' instead of '"pageNum"'
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return useXHtml
                ? TessApi.BaseAPIGetHOCRText2(Engine.Handle, pageNum)
                : TessApi.BaseAPIGetHOCRText(Engine.Handle, pageNum);
        }
        #endregion

        #region GetAltoText
        /// <summary>
        ///     Gets the page's content as an Alto text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as an Alto text string.</returns>
        public string GetAltoText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return TessApi.BaseAPIGetAltoText(Engine.Handle, pageNum);
        }
        #endregion

        #region GetTsvText
        /// <summary>
        ///     Gets the page's content as a Tsv text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a Tsv text string.</returns>
        public string GetTsvText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return TessApi.BaseAPIGetTsvText(Engine.Handle, pageNum);
        }
        #endregion

        #region GetBoxText
        /// <summary>
        ///     Gets the page's content as a Box text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a Box text string.</returns>
        public string GetBoxText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return TessApi.BaseAPIGetBoxText(Engine.Handle, pageNum);
        }
        #endregion

        #region GetLSTMBoxText
        /// <summary>
        ///     Gets the page's content as a LSTMBox text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a LSTMBox text string.</returns>
        public string GetLSTMBoxText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return TessApi.BaseAPIGetLSTMBoxText(Engine.Handle, pageNum);
        }
        #endregion

        #region GetWordStrBoxText
        /// <summary>
        ///     Gets the page's content as a WordStrBox text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a WordStrBox text string.</returns>
        public string GetWordStrBoxText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return TessApi.BaseAPIGetWordStrBoxText(Engine.Handle, pageNum);
        }
        #endregion

        #region GetUNLVText
        /// <summary>
        ///     Gets the page's content as an UNLV text.
        /// </summary>
        /// <returns>The OCR'd output as an UNLV text string.</returns>
        public string GetUNLVText()
        {
            Recognize();
            return TessApi.BaseAPIGetUnlvText(Engine.Handle);
        }
        #endregion

        #region GetMeanConfidence
        /// <summary>
        ///     Get's the mean confidence that as a percentage of the recognized text.
        /// </summary>
        /// <returns></returns>
        public float GetMeanConfidence()
        {
            Recognize();
            return TessApi.Native.BaseAPIMeanTextConf(Engine.Handle) / 100.0f;
        }
        #endregion

        #region GetSegmentedRegions
        /// <summary>
        ///     Get segmented regions at specified page iterator level.
        /// </summary>
        /// <param name="pageIteratorLevel">PageIteratorLevel enum</param>
        /// <returns></returns>
        public List<Rectangle> GetSegmentedRegions(PageIteratorLevel pageIteratorLevel)
        {
            var boxArray = TessApi.Native.BaseAPIGetComponentImages(Engine.Handle, pageIteratorLevel, Constants.TRUE, IntPtr.Zero, IntPtr.Zero);
            var boxCount = LeptonicaApi.Native.boxaGetCount(new HandleRef(this, boxArray));
            var boxList = new List<Rectangle>();

            for (var i = 0; i < boxCount; i++)
            {
                var box = LeptonicaApi.Native.boxaGetBox(new HandleRef(this, boxArray), i, PixArrayAccessType.Clone);

                if (box == IntPtr.Zero)
                    continue;

                LeptonicaApi.Native.boxGetGeometry(new HandleRef(this, box), out var px, out var py, out var pw, out var ph);
                boxList.Add(new Rectangle(px, py, pw, ph));
                LeptonicaApi.Native.boxDestroy(ref box);
            }

            LeptonicaApi.Native.boxaDestroy(ref boxArray);

            return boxList;
        }
        #endregion

        #region DetectBestOrientation
        /// <summary>
        ///     Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly" />.
        /// </summary>
        /// <remarks>
        ///     If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout" /> instead as
        ///     this also provides a
        ///     deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The page orientation.</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        [Obsolete("Use DetectBestOrientation(int orientationDegrees, float confidence) that returns orientation in degrees instead.")]
        public void DetectBestOrientation(out Orientation orientation, out float confidence)
        {
            DetectBestOrientation(out int orientationDegrees, out var orientationConfidence);

            // convert angle to 0-360 (shouldn't be required but do it just o be safe).
            orientationDegrees %= 360;

            if (orientationDegrees < 0)
                orientationDegrees += 360;

            if (orientationDegrees > 315 || orientationDegrees <= 45)
                orientation = Orientation.PageUp;
            else if (orientationDegrees <= 135)
                orientation = Orientation.PageRight;
            else if (orientationDegrees <= 225)
                orientation = Orientation.PageDown;
            else
                orientation = Orientation.PageLeft;

            confidence = orientationConfidence;
        }
        #endregion

        #region DetectBestOrientation
        /// <summary>
        ///     Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly" />.
        /// </summary>
        /// <remarks>
        ///     If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout" /> instead as
        ///     this also provides a
        ///     deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The detected clockwise page rotation in degrees (0, 90, 180, or 270).</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        public void DetectBestOrientation(out int orientation, out float confidence)
        {
            DetectBestOrientationAndScript(out orientation, out confidence, out _, out _);
        }
        #endregion

        #region DetectBestOrientationAndScript
        /// <summary>
        ///     Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly" />.
        /// </summary>
        /// <remarks>
        ///     If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout" /> instead as
        ///     this also provides a
        ///     deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The detected clockwise page rotation in degrees (0, 90, 180, or 270).</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        /// <param name="scriptName">The name of the script (e.g. Latin)</param>
        /// <param name="scriptConfidence">The confidence level in the script</param>
        public void DetectBestOrientationAndScript(out int orientation, out float confidence, out string scriptName,
            out float scriptConfidence)
        {
            if (TessApi.Native.TessBaseAPIDetectOrientationScript(Engine.Handle, out var orientDeg, out var orientConf,
                    out var scriptNameHandle, out var scriptConf) != 0)
            {
                orientation = orientDeg;
                confidence = orientConf;
                scriptName = scriptNameHandle != IntPtr.Zero ? MarshalHelper.PtrToString(scriptNameHandle, Encoding.ASCII) : null;
                scriptConfidence = scriptConf;
            }
            else
                throw new TesseractException("Failed to detect image orientation.");
        }
        #endregion

        #region Recognize
        internal void Recognize()
        {
            Guard.Verify(PageSegmentMode != PageSegMode.OsdOnly, "Cannot OCR image when using OSD only page segmentation, please use DetectBestOrientation instead.");

            if (_runRecognitionPhase)
                return;

            if (TessApi.Native.BaseApiRecognize(Engine.Handle, new HandleRef(this, IntPtr.Zero)) != 0)
                throw new InvalidOperationException("Recognition of image failed.");

            _runRecognitionPhase = true;

            // Now write out the thresholded image if required to do so
            if (!Engine.TryGetBoolVariable("tessedit_write_images", out var tesseditWriteImages) || !tesseditWriteImages)
                return;

            using (var thresholdedImage = GetThresholdedImage())
            {
                var filePath = Path.Combine(Environment.CurrentDirectory, "tessinput.tif");
                try
                {
                    thresholdedImage.Save(filePath, ImageFormat.TiffG4);
                    Trace.TraceEvent(TraceEventType.Information, 2,
                        "Successfully saved the thresholded image to '{0}'", filePath);
                }
                catch (Exception error)
                {
                    Trace.TraceEvent(TraceEventType.Error, 2,
                        "Failed to save the thresholded image to '{0}'.\nError: {1}", filePath, error.Message);
                }
            }
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                TessApi.Native.BaseAPIClear(Engine.Handle);
        }
        #endregion
    }
}