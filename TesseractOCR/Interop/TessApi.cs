//
// BaseApi.cs
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
using System.Runtime.InteropServices;
using System.Text;
using TesseractOCR.Enums;
using TesseractOCR.Internal;
using TesseractOCR.InteropDotNet;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace TesseractOCR.Interop
{
    /// <summary>
    ///     The exported tesseract api signatures.
    /// </summary>
    /// <remarks>
    ///     Please note this is only public for technical reasons (you can't proxy a internal interface).
    ///     It should be considered an internal interface and is NOT part of the public api and may have
    ///     breaking changes between releases.
    ///
    ///     API URL: https://github.com/tesseract-ocr/tesseract/blob/main/include/tesseract/capi.h
    /// </remarks>
    public interface ITessApiSignatures
    {
        #region General free function
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessVersion")] 
        IntPtr GetVersion();

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteTextArray")]
        void DeleteTextArray(IntPtr arr);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteIntArray")] 
        void DeleteIntArray(IntPtr arr);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteText")]
        void DeleteText(IntPtr textPtr);
        #endregion

        #region Renderer API
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessTextRendererCreate")]
        IntPtr TextRendererCreate(string outputbase);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate")]
        IntPtr HOcrRendererCreate(string outputbase);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate2")]
        IntPtr HOcrRendererCreate2(string outputbase, int font_info);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessAltoRendererCreate")]
        IntPtr AltoRendererCreate(string outputbase);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessTsvRendererCreate")]
        IntPtr TsvRendererCreate(string outputbase);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPDFRendererCreate")]
        IntPtr PDFRendererCreate(string outputbase, IntPtr datadir, int textonly);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessUnlvRendererCreate")]
        IntPtr UnlvRendererCreate(string outputbase);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBoxTextRendererCreate")]
        IntPtr BoxTextRendererCreate(string outputbase);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessLSTMBoxRendererCreate")]
        IntPtr LSTMBoxRendererCreate(string outputbase);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessWordStrBoxRendererCreate")]
        IntPtr WordStrBoxRendererCreate(string outputbase);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteResultRenderer")]
        void DeleteResultRenderer(HandleRef renderer);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererInsert")]
        void ResultRendererInsert(HandleRef renderer, HandleRef next);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererNext")]
        IntPtr ResultRendererNext(HandleRef renderer);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererBeginDocument")]
        int ResultRendererBeginDocument(HandleRef renderer, IntPtr titlePtr);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererAddImage")]
        int ResultRendererAddImage(HandleRef renderer, HandleRef api);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererEndDocument")]
        int ResultRendererEndDocument(HandleRef renderer);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererExtention")]
        IntPtr ResultRendererExtention(HandleRef renderer);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererTitle")]
        IntPtr ResultRendererTitle(HandleRef renderer);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererImageNum")]
        int ResultRendererImageNum(HandleRef renderer);
        #endregion

        #region Base API
        /// <summary>
        ///     Creates a new BaseApi instance
        /// </summary>
        /// <returns></returns>
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPICreate")]
        IntPtr BaseApiCreate();

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDelete")]
        void BaseApiDelete(HandleRef ptr);

        // TODO: Add support for : TESS_API size_t TessBaseAPIGetOpenCLDevice(TessBaseAPI* handle, void** device);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputName")]
        void BaseApiSetInputName(HandleRef handle, string name);

        // TODO : New in Tesseract 5
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetInputName")]
        string BaseAPIGetInputName(HandleRef handle);

        // TODO: Check if I can pass in an image like this
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputImage")]
        void TessBaseAPISetInputImage(HandleRef handle, Pix.Image pix);

        // TODO: Check if I can get an image like this
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetInputImage")]
        Pix.Image BaseAPIGetInputImage(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetSourceYResolution")]
        int BaseAPIGetSourceYResolution(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDatapath")]
        IntPtr BaseApiGetDatapath(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetOutputName")]
        void BaseApiSetOutputName(HandleRef handle, string name);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetVariable")]
        int BaseApiSetVariable(HandleRef handle, string name, IntPtr valPtr);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetDebugVariable")]
        int BaseApiSetDebugVariable(HandleRef handle, string name, IntPtr valPtr);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIntVariable")]
        int BaseApiGetIntVariable(HandleRef handle, string name, out int value);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetBoolVariable")]
        int BaseApiGetBoolVariable(HandleRef handle, string name, out int value);
        
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDoubleVariable")]
        int BaseApiGetDoubleVariable(HandleRef handle, string name, out double value);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetStringVariable")]
        IntPtr BaseApiGetStringVariableInternal(HandleRef handle, string name);

        // TODO: No idea yet how to do this
        // TODO: Add support for : TESS_API void TessBaseAPIPrintVariables(const TessBaseAPI *handle, FILE *fp);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIPrintVariablesToFile")]
        int BaseApiPrintVariablesToFile(HandleRef handle, string filename);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit1")]
        int BaseApiInit1(HandleRef handle, string datapath, string language, EngineMode mode, string[] configs, int configs_size);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit2")]
        int BaseApiInit2(HandleRef handle, string datapath, string language, EngineMode mode);
        
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit3")]
        int BaseApiInit3(HandleRef handle, string datapath, string language);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit4")]
        int BaseApiInit4(HandleRef handle, string datapath, string language, int mode, string[] configs, int configs_size, string[] vars_vec, string[] vars_values, UIntPtr vars_vec_size, bool set_only_non_debug_params);

        // TODO: Add support for : TESS_API const char* TessBaseAPIGetInitLanguagesAsString(const TessBaseAPI* handle);

        // TODO: Add support for : TESS_API char** TessBaseAPIGetLoadedLanguagesAsVector(const TessBaseAPI* handle);

        // TODO: Add support for : TESS_API char** TessBaseAPIGetAvailableLanguagesAsVector(const TessBaseAPI* handle);

        // TODO: Add support for : TESS_API void TessBaseAPIInitForAnalysePage(TessBaseAPI* handle);

        // TODO: Add support for : TESS_API void TessBaseAPIReadConfigFile(TessBaseAPI* handle, const char* filename);

        // TODO: Add support for : TESS_API void TessBaseAPIReadDebugConfigFile(TessBaseAPI* handle, const char* filename);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetPageSegMode")]
        void BaseApiSetPageSegMode(HandleRef handle, PageSegMode mode);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetPageSegMode")]
        PageSegMode BaseApiGetPageSegMode(HandleRef handle);

        // TODO: Add support for : TESS_API char* TessBaseAPIRect(TessBaseAPI* handle, const unsigned char* imagedata, int bytes_per_pixel, int bytes_per_line,int left, int top, int width, int height);

        // TODO: Add support for : TESS_API void TessBaseAPIClearAdaptiveClassifier(TessBaseAPI* handle);

        // TODO: Add support for : TESS_API void TessBaseAPISetImage(TessBaseAPI *handle, const unsigned char* imagedata, int width, int height, int bytes_per_pixel, int bytes_per_line);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetImage2")]
        void BaseApiSetImage(HandleRef handle, HandleRef pixHandle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetSourceResolution")]
        void BaseAPISetSourceResolution(HandleRef handle, int ppi);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetRectangle")]
        void BaseApiSetRectangle(HandleRef handle, int left, int top, int width, int height);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetThresholdedImage")]
        IntPtr BaseApiGetThresholdedImage(HandleRef handle);

        // TODO: Add support for : TESS_API struct Boxa *TessBaseAPIGetRegions(TessBaseAPI* handle, struct Pixa **pixa);
        
        // TODO: Add support for : TESS_API struct Boxa *TessBaseAPIGetTextlines(TessBaseAPI* handle, struct Pixa **pixa, int** blockids);
        
        // TODO: Add support for : TESS_API struct Boxa *TessBaseAPIGetTextlines1(TessBaseAPI* handle, BOOL raw_image, int raw_padding, struct Pixa **pixa, int** blockids, int** paraids);
        
        // TODO: Add support for : TESS_API struct Boxa *TessBaseAPIGetStrips(TessBaseAPI* handle, struct Pixa **pixa, int** blockids);
        
        // TODO: Add support for : TESS_API struct Boxa *TessBaseAPIGetWords(TessBaseAPI* handle, struct Pixa **pixa);
        
        // TODO: Add support for : TESS_API struct Boxa *TessBaseAPIGetConnectedComponents(TessBaseAPI* handle, struct Pixa **cc);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetComponentImages")]
        IntPtr BaseApiGetComponentImages(HandleRef handle, PageIteratorLevel level, int text_only, IntPtr pixa, IntPtr blockids);

        // TODO: Add support for : TESS_API struct Boxa *TessBaseAPIGetComponentImages1(TessBaseAPI* handle, TessPageIteratorLevel level, BOOL text_only, BOOL raw_image, int raw_padding, struct Pixa **pixa, int** blockids, int** paraids);

        // TODO: Add support for : TESS_API int TessBaseAPIGetThresholdedImageScaleFactor(const TessBaseAPI* handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAnalyseLayout")]
        IntPtr BaseApiAnalyseLayout(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIRecognize")]
        int BaseApiRecognize(HandleRef handle, HandleRef monitor);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIProcessPages")]
        int BaseApiProcessPages(HandleRef handle, string filename, string retry_config, int timeout_millisec, HandleRef renderer);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIProcessPage")]
        int BaseApiProcessPage(HandleRef handle, Pix.Image pix, int page_index, string filename, string retry_config, int timeout_millisec, HandleRef renderer);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIterator")]
        IntPtr BaseApiGetIterator(HandleRef handle);

        // TODO: Add support for : TESS_API TessMutableIterator *TessBaseAPIGetMutableIterator(TessBaseAPI* handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUTF8Text")]
        IntPtr BaseApiGetUTF8TextInternal(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetHOCRText")]
        IntPtr BaseApiGetHOcrTextInternal(HandleRef handle, int pageNum);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetAltoText")]
        IntPtr BaseApiGetAltoTextInternal(HandleRef handle, int pageNum);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetTsvText")]
        IntPtr BaseApiGetTsvTextInternal(HandleRef handle, int pageNum);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetBoxText")]
        IntPtr BaseApiGetBoxTextInternal(HandleRef handle, int pageNum);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetLSTMBoxText")]
        IntPtr BaseApiGetLstmBoxTextInternal(HandleRef handle, int pageNum);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetWordStrBoxText")]
        IntPtr BaseApiGetWordStrBoxTextInternal(HandleRef handle, int pageNum);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUNLVText")]
        IntPtr BaseApiGetUnlvTextInternal(HandleRef handle);
        
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIMeanTextConf")]
        int BaseApiMeanTextConf(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAllWordConfidences")]
        int BaseAPIAllWordConfidences(HandleRef handle);

        // TODO: Add support for : TESS_API BOOL TessBaseAPIAdaptToWordStr(TessBaseAPI *handle, TessPageSegMode mode, const char* wordstr);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClear")]
        void BaseApiClear(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIEnd")]
        void BaseAPIEnd(HandleRef handle);

        // TODO: Add support for : TESS_API int TessBaseAPIIsValidWord(TessBaseAPI *handle, const char *word);

        // TODO: Add support for : TESS_API BOOL TessBaseAPIGetTextDirection(TessBaseAPI *handle, int *out_offset, float* out_slope);

        // TODO: Add support for : TESS_API const char *TessBaseAPIGetUnichar(TessBaseAPI *handle, int unichar_id);

        // TODO: Add support for : TESS_API void TessBaseAPIClearPersistentCache(TessBaseAPI *handle);

        // Call TessDeleteText(*best_script_name) to free memory allocated by this function
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDetectOrientationScript")]
        int TessBaseAPIDetectOrientationScript(HandleRef handle, out int orient_deg, out float orient_conf, out IntPtr script_name, out float script_conf);

        // TODO: Add support for : TESS_API void TessBaseAPISetMinOrientationMargin(TessBaseAPI *handle, double margin);

        // TODO: Add support for : TESS_API int TessBaseAPINumDawgs(const TessBaseAPI *handle);

        // TODO: Add support for : TESS_API TessOcrEngineMode TessBaseAPIOem(const TessBaseAPI *handle);

        // TODO: Add support for : TESS_API void TessBaseGetBlockTextOrientations(TessBaseAPI *handle, int** block_orientation, bool** vertical_writing);
        #endregion

        #region Page iterator
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorDelete")]
        void PageIteratorDelete(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorCopy")]
        IntPtr PageIteratorCopy(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBegin")]
        void PageIteratorBegin(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorNext")]
        int PageIteratorNext(HandleRef handle, PageIteratorLevel level);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
        int PageIteratorIsAtBeginningOf(HandleRef handle, PageIteratorLevel level);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
        int PageIteratorIsAtFinalElement(HandleRef handle, PageIteratorLevel level, PageIteratorLevel element);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBoundingBox")]
        int PageIteratorBoundingBox(HandleRef handle, PageIteratorLevel level, out int left, out int top, out int right, out int bottom);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBlockType")]
        PolyBlockType PageIteratorBlockType(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetBinaryImage")]
        IntPtr PageIteratorGetBinaryImage(HandleRef handle, PageIteratorLevel level);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetImage")]
        IntPtr PageIteratorGetImage(HandleRef handle, PageIteratorLevel level, int padding, HandleRef originalImage, out int left, out int top);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBaseline")]
        int PageIteratorBaseline(HandleRef handle, PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorOrientation")]
        void PageIteratorOrientation(HandleRef handle, out Orientation orientation, out WritingDirection writing_direction, out TextLineOrder textLineOrder, out float deskew_angle);

        // TODO: Add support for : TESS_API void TessPageIteratorParagraphInfo(TessPageIterator* handle, TessParagraphJustification* justification, BOOL* is_list_item, BOOL* is_crown, int* first_line_indent);
        #endregion

        #region Result iterator
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorDelete")]
        void ResultIteratorDelete(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorCopy")]
        IntPtr ResultIteratorCopy(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetPageIterator")]
        IntPtr ResultIteratorGetPageIterator(HandleRef handle);

        // TODO: Add support for : TESS_API const TessPageIterator *TessResultIteratorGetPageIteratorConst(const TessResultIterator* handle);

        /// <summary>
        ///     Native API call to TessResultIteratorGetChoiceIterator
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetChoiceIterator")]
        IntPtr ResultIteratorGetChoiceIterator(HandleRef handle);

        // TODO: Add support for : TESS_API BOOL TessResultIteratorNext(TessResultIterator* handle, TessPageIteratorLevel level);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetUTF8Text")]
        IntPtr ResultIteratorGetUTF8TextInternal(HandleRef handle, PageIteratorLevel level);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorConfidence")]
        float ResultIteratorGetConfidence(HandleRef handle, PageIteratorLevel level);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordRecognitionLanguage")]
        IntPtr ResultIteratorWordRecognitionLanguageInternal(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordFontAttributes")]
        IntPtr ResultIteratorWordFontAttributes(HandleRef handle, out bool isBold, out bool isItalic, out bool isUnderlined, out bool isMonospace, out bool isSerif, out bool isSmallCaps, out int pointSize, out int fontId);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsFromDictionary")]
        bool ResultIteratorWordIsFromDictionary(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsNumeric")]
        bool ResultIteratorWordIsNumeric(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSuperscript")]
        bool ResultIteratorSymbolIsSuperscript(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSubscript")]
        bool ResultIteratorSymbolIsSubscript(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsDropcap")]
        bool ResultIteratorSymbolIsDropcap(HandleRef handle);
        #endregion

        #region Choice iterator
        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorDelete")]
        void ChoiceIteratorDelete(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorNext")]
        int ChoiceIteratorNext(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorGetUTF8Text")]
        IntPtr ChoiceIteratorGetUTF8TextInternal(HandleRef handle);

        [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorConfidence")]
        float ChoiceIteratorGetConfidence(HandleRef handle);
        #endregion

        #region Progress monitor
        // TODO: Add support for : TESS_API ETEXT_DESC *TessMonitorCreate();
        // TODO: Add support for : TESS_API void TessMonitorDelete(ETEXT_DESC *monitor);
        // TODO: Add support for : TESS_API void TessMonitorSetCancelFunc(ETEXT_DESC *monitor, TessCancelFunc cancelFunc);
        // TODO: Add support for : TESS_API void TessMonitorSetCancelThis(ETEXT_DESC *monitor, void *cancelThis);
        // TODO: Add support for : TESS_API void *TessMonitorGetCancelThis(ETEXT_DESC *monitor);
        // TODO: Add support for : TESS_API void TessMonitorSetProgressFunc(ETEXT_DESC *monitor, TessProgressFunc progressFunc);
        // TODO: Add support for : TESS_API int TessMonitorGetProgress(ETEXT_DESC *monitor);
        // TODO: Add support for : TESS_API void TessMonitorSetDeadlineMSecs(ETEXT_DESC *monitor, int deadline);
        #endregion
    }

    internal static class TessApi
    {
        #region Fields
        private static ITessApiSignatures native;
        #endregion

        #region Properties
        public static ITessApiSignatures Native
        {
            get
            {
                if (native == null)
                    Initialize();
                return native;
            }
        }
        #endregion

        #region BaseApiGetVersion
        public static string BaseApiGetVersion()
        {
            var versionHandle = Native.GetVersion();
            if (versionHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(versionHandle, Encoding.UTF8);
            return result;
        }
        #endregion

        #region BaseApiGetHOcrText
        public static string BaseApiGetHOcrText(HandleRef handle, int pageNum)
        {
            var txtHandle = Native.BaseApiGetHOcrTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            Native.DeleteText(txtHandle);
            return result;
        }
        #endregion

        #region BaseApiGetAltoText
        public static string BaseApiGetAltoText(HandleRef handle, int pageNum)
        {
            var txtHandle = Native.BaseApiGetAltoTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            Native.DeleteText(txtHandle);
            return result;
        }
        #endregion

        #region BaseApiGetTsvText
        public static string BaseApiGetTsvText(HandleRef handle, int pageNum)
        {
            var txtHandle = Native.BaseApiGetTsvTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            Native.DeleteText(txtHandle);
            return result;
        }
        #endregion

        #region BaseApiGetBoxText
        public static string BaseApiGetBoxText(HandleRef handle, int pageNum)
        {
            var txtHandle = Native.BaseApiGetBoxTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            Native.DeleteText(txtHandle);
            return result;
        }
        #endregion

        #region BaseApiGetLSTMBoxText
        public static string BaseApiGetLSTMBoxText(HandleRef handle, int pageNum)
        {
            var txtHandle = Native.BaseApiGetLstmBoxTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            Native.DeleteText(txtHandle);
            return result;
        }
        #endregion

        #region BaseApiGetWordStrBoxText
        public static string BaseApiGetWordStrBoxText(HandleRef handle, int pageNum)
        {
            var txtHandle = Native.BaseApiGetWordStrBoxTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            Native.DeleteText(txtHandle);
            return result;
        }
        #endregion

        #region BaseApiGetUNLVText
        public static string BaseApiGetUnlvText(HandleRef handle)
        {
            var txtHandle = Native.BaseApiGetUnlvTextInternal(handle);
            if (txtHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            Native.DeleteText(txtHandle);
            return result;
        }
        #endregion

        #region BaseApiGetStringVariable
        public static string BaseApiGetStringVariable(HandleRef handle, string name)
        {
            var resultHandle = Native.BaseApiGetStringVariableInternal(handle, name);
            return resultHandle != IntPtr.Zero ? MarshalHelper.PtrToString(resultHandle, Encoding.UTF8) : null;
        }
        #endregion

        #region BaseApiGetUTF8Text
        public static string BaseApiGetUTF8Text(HandleRef handle)
        {
            var txtHandle = Native.BaseApiGetUTF8TextInternal(handle);
            if (txtHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            Native.DeleteText(txtHandle);
            return result;
        }
        #endregion

        #region BaseApiInit
        public static int BaseApiInit(HandleRef handle, string datapath, string language, int mode,
            IEnumerable<string> configFiles, IDictionary<string, object> initialValues, bool setOnlyNonDebugParams)
        {
            Guard.Require("handle", handle.Handle != IntPtr.Zero, "Handle for BaseApi, created through BaseApiCreate is required");
            Guard.RequireNotNullOrEmpty("language", language);
            Guard.RequireNotNull("configFiles", configFiles);
            Guard.RequireNotNull("initialValues", initialValues);

            var configFilesArray = new List<string>(configFiles).ToArray();
            var varNames = new string[initialValues.Count];
            var varValues = new string[initialValues.Count];
            var i = 0;

            foreach (var pair in initialValues)
            {
                Guard.Require("initialValues", !string.IsNullOrEmpty(pair.Key), "Variable must have a name");
                Guard.Require("initialValues", pair.Value != null, "Variable '{0}': The type '{1}' is not supported", pair.Key, pair.Value?.GetType());
                
                varNames[i] = pair.Key;

                if (TessConvert.TryToString(pair.Value, out var varValue))
                    varValues[i] = varValue;
                else
                    throw new ArgumentException(
                        $"Variable '{pair.Key}': The type '{pair.Value?.GetType()}' is not supported",
                        nameof(initialValues));
                i++;
            }

            return Native.BaseApiInit4(handle, datapath, language, mode,
                configFilesArray, configFilesArray.Length,
                varNames, varValues, new UIntPtr((uint)varNames.Length), setOnlyNonDebugParams);
        }
        #endregion

        #region BaseApiSetDebugVariable
        public static int BaseApiSetDebugVariable(HandleRef handle, string name, string value)
        {
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
                return Native.BaseApiSetDebugVariable(handle, name, valuePtr);
            }
            finally
            {
                if (valuePtr != IntPtr.Zero) Marshal.FreeHGlobal(valuePtr);
            }
        }
        #endregion

        #region BaseApiSetVariable
        public static int BaseApiSetVariable(HandleRef handle, string name, string value)
        {
            var valuePtr = IntPtr.Zero;

            try
            {
                valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
                return Native.BaseApiSetVariable(handle, name, valuePtr);
            }
            finally
            {
                if (valuePtr != IntPtr.Zero) Marshal.FreeHGlobal(valuePtr);
            }
        }
        #endregion

        #region Initialize
        public static void Initialize()
        {
            if (native != null) return;
            LeptonicaApi.Initialize();
            Helper.SetPath();
            native = InteropRuntimeImplementer.CreateInstance<ITessApiSignatures>();
        }
        #endregion

        #region ResultIteratorWordRecognitionLanguage
        public static string ResultIteratorWordRecognitionLanguage(HandleRef handle)
        {
            // per docs (ltrresultiterator.h:118 as of 4897796 in github:tesseract-ocr/tesseract)
            // this return value should *NOT* be deleted.
            var txtHandle =
                Native.ResultIteratorWordRecognitionLanguageInternal(handle);

            return txtHandle != IntPtr.Zero
                ? MarshalHelper.PtrToString(txtHandle, Encoding.UTF8)
                : null;
        }
        #endregion

        #region ResultIteratorGetUTF8Text
        public static string ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level)
        {
            var txtHandle = Native.ResultIteratorGetUTF8TextInternal(handle, level);
            if (txtHandle == IntPtr.Zero) return null;
            var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            Native.DeleteText(txtHandle);
            return result;
        }
        #endregion

        #region ChoiceIteratorGetUTF8Text
        /// <summary>
        ///     Returns the null terminated UTF-8 encoded text string for the current choice
        /// </summary>
        /// <remarks>
        ///     NOTE: Unlike LTRResultIterator::GetUTF8Text, the return points to an
        ///     internal structure and should NOT be delete[]ed to free after use.
        /// </remarks>
        /// <param name="choiceIteratorHandle"></param>
        /// <returns>string</returns>
        internal static string ChoiceIteratorGetUTF8Text(HandleRef choiceIteratorHandle)
        {
            Guard.Require("choiceIteratorHandle", choiceIteratorHandle.Handle != IntPtr.Zero, "ChoiceIterator Handle cannot be a null IntPtr and is required");
            var txtChoiceHandle = Native.ChoiceIteratorGetUTF8TextInternal(choiceIteratorHandle);
            return MarshalHelper.PtrToString(txtChoiceHandle, Encoding.UTF8);
        }
        #endregion
    }
}