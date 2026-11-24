using System;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.InteropDotNet;
// ReSharper disable InconsistentNaming

namespace TesseractOCR.Interop;

#pragma warning disable CS1591
/// <summary>
///     The exported tesseract api signatures.
/// </summary>
/// <remarks>
///     Please note this is only public for technical reasons (you can't proxy a internal interface).
///     It should be considered an internal interface and is NOT part of the public api and may have
///     breaking changes between releases.
///
///     API URL: https://github.com/tesseract-ocr/tesseract/blob/5.5.4/include/tesseract/capi.h
/// </remarks>
public interface ITessApiSignatures
{
    #region General free function
    /// <summary>
    ///     Returns the current version
    /// </summary>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessVersion")] 
    IntPtr GetVersion();

    /// <summary>
    ///     Deallocates the memory block occupied by text array
    /// </summary>
    /// <param name="arr">The text array to free.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteTextArray")]
    void DeleteTextArray(IntPtr arr);

    /// <summary>
    ///     Deallocates the memory block occupied by integer array
    /// </summary>
    /// <param name="arr">The integer array to free.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteIntArray")]
    void DeleteIntArray(IntPtr arr);

    /// <summary>
    ///     Deallocates the memory block occupied by text array
    /// </summary>
    /// <param name="textPtr">The text pointer to free.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteText")]
    void DeleteText(IntPtr textPtr);
    #endregion

    #region Renderer API
    /// <summary>Creates a renderer that writes recognized text as plain text.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessTextRendererCreate")]
    IntPtr TextRendererCreate(string outputbase);

    /// <summary>Creates a renderer that writes recognized text in hOCR format.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate")]
    IntPtr HOcrRendererCreate(string outputbase);

    /// <summary>Creates an hOCR renderer, optionally including font information.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    /// <param name="font_info">Nonzero to include font information in the output.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate2")]
    IntPtr HOcrRendererCreate2(string outputbase, int font_info);

    /// <summary>Creates a renderer that writes recognized text in ALTO XML format.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessAltoRendererCreate")]
    IntPtr AltoRendererCreate(string outputbase);

    /// <summary>
    ///     Creates a renderer that writes recognized text in PAGE XML format.
    /// </summary>
    /// <param name="outputbase">The base name for the output files.</param>
    /// <returns>A pointer to the PAGE renderer.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPAGERendererCreate")]
    IntPtr PAGERendererCreate(string outputbase);

    /// <summary>Creates a renderer that writes recognized text in TSV format.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessTsvRendererCreate")]
    IntPtr TsvRendererCreate(string outputbase);

    /// <summary>Creates a renderer that writes searchable PDF output.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    /// <param name="datadir">The directory containing PDF resources.</param>
    /// <param name="textonly">Nonzero to produce a text-only PDF.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPDFRendererCreate")]
    IntPtr PDFRendererCreate(string outputbase, IntPtr datadir, int textonly);

    /// <summary>Creates a renderer that writes recognized text in UNLV format.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessUnlvRendererCreate")]
    IntPtr UnlvRendererCreate(string outputbase);

    /// <summary>Creates a renderer that writes recognized text in box-file format.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBoxTextRendererCreate")]
    IntPtr BoxTextRendererCreate(string outputbase);

    /// <summary>Creates a renderer that writes recognized text in LSTM box-file format.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessLSTMBoxRendererCreate")]
    IntPtr LSTMBoxRendererCreate(string outputbase);

    /// <summary>Creates a renderer that writes recognized text in WordStr box-file format.</summary>
    /// <param name="outputbase">The base name for output files.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessWordStrBoxRendererCreate")]
    IntPtr WordStrBoxRendererCreate(string outputbase);

    /// <summary>Deletes a result renderer and releases its resources.</summary>
    /// <param name="renderer">The renderer to delete.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteResultRenderer")]
    void DeleteResultRenderer(HandleRef renderer);

    /// <summary>Inserts a renderer after the specified renderer in the renderer chain.</summary>
    /// <param name="renderer">The renderer in the chain after which to insert.</param>
    /// <param name="next">The renderer to insert.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererInsert")]
    void ResultRendererInsert(HandleRef renderer, HandleRef next);

    /// <summary>Returns the next renderer in the renderer chain.</summary>
    /// <param name="renderer">The current renderer.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererNext")]
    IntPtr ResultRendererNext(HandleRef renderer);

    /// <summary>Begins a document in the output renderer.</summary>
    /// <param name="renderer">The renderer for the document.</param>
    /// <param name="titlePtr">A pointer to the document title.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererBeginDocument")]
    int ResultRendererBeginDocument(HandleRef renderer, IntPtr titlePtr);

    /// <summary>Adds the current image's recognition results to the output document.</summary>
    /// <param name="renderer">The renderer receiving the image results.</param>
    /// <param name="api">The Tesseract API instance containing the image results.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererAddImage")]
    int ResultRendererAddImage(HandleRef renderer, HandleRef api);

    /// <summary>Ends the current output document.</summary>
    /// <param name="renderer">The renderer for the document.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererEndDocument")]
    int ResultRendererEndDocument(HandleRef renderer);

    /// <summary>Returns the output file extension used by the renderer.</summary>
    /// <param name="renderer">The renderer whose output extension is requested.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererExtention")]
    IntPtr ResultRendererExtention(HandleRef renderer);

    /// <summary>Returns the title associated with the renderer.</summary>
    /// <param name="renderer">The renderer whose title is requested.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererTitle")]
    IntPtr ResultRendererTitle(HandleRef renderer);

    /// <summary>Returns the number of images processed by the renderer.</summary>
    /// <param name="renderer">The renderer whose processed image count is requested.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererImageNum")]
    int ResultRendererImageNum(HandleRef renderer);
    #endregion

    #region Base API
    /// <summary>
    ///     Creates an instance of the base class for all Tesseract APIs
    /// </summary>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPICreate")]
    IntPtr BaseApiCreate();

    /// <summary>
    ///     Disposes the TesseractAPI instance
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDelete")]
    void BaseApiDelete(HandleRef handle);

    /// <summary>
    ///     Set the name of the input file. Needed only for training and reading a UNLV zone file, and for searchable PDF output
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <param name="name">The name</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputName")]
    void BaseApiSetInputName(HandleRef handle, string name);

    /// <summary>
    ///     These functions are required for searchable PDF output. We need our hands on the input file so that we can include it in the PDF without
    ///     transcoding. If that is not possible, we need the original image. Finally, resolution metadata is stored in the PDF so we need that as well
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns>The input name</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetInputName")]
    string BaseAPIGetInputName(HandleRef handle);

    /// <summary>
    ///     Sets the input image
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <param name="pixHandle"><see cref="Pix.Image.Handle"/></param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputImage")]
    void BaseAPISetInputImage(HandleRef handle, HandleRef pixHandle);

    /// <summary>
    ///     Gets the input image
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns><see cref="Pix.Image.Handle"/></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetInputImage")]
    IntPtr BaseAPIGetInputImage(HandleRef handle);

    /// <summary>
    ///     Sets the Y-resolution for the image
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetSourceYResolution")]
    int BaseAPIGetSourceYResolution(HandleRef handle);

    /// <summary>
    ///     Gets the data path
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDatapath")]
    IntPtr BaseApiGetDatapath(HandleRef handle);

    /// <summary>
    ///     Set the name of the bonus output files. Needed only for debugging
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="name">The base name for Tesseract's auxiliary output files.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetOutputName")]
    void BaseApiSetOutputName(HandleRef handle, string name);

    /// <summary>
    ///     Set the value of an internal "parameter." Supply the name of the parameter and the value as a string, just as you would in
    ///     a config file. Returns false if the name lookup failed. E.g., SetVariable("tessedit_char_blacklist", "xyz"); to ignore
    ///     x, y and z. Or SetVariable("classify_bln_numeric_mode", "1"); to set numeric-only mode. SetVariable may be used before Init,
    ///     but settings will revert to defaults on End()
    /// </summary>
    /// <remarks>
    ///     Must be called after Init(). Only works for non-init variables (init variables should be passed to Init())
    /// </remarks>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="name">The name of the parameter to set.</param>
    /// <param name="valPtr">A pointer to the parameter value encoded as a string.</param>
    /// <returns>Nonzero if the parameter was set; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetVariable")]
    int BaseApiSetVariable(HandleRef handle, string name, IntPtr valPtr);

    /// <summary>
    ///     Sets a debug variable
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="name">The name of the debug parameter to set.</param>
    /// <param name="valPtr">A pointer to the parameter value encoded as a string.</param>
    /// <returns>Nonzero if the parameter was set; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetDebugVariable")]
    int BaseApiSetDebugVariable(HandleRef handle, string name, IntPtr valPtr);

    /// <summary>
    ///     Get the value of an internal int parameter
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="name">The name of the integer parameter to retrieve.</param>
    /// <param name="value">Receives the parameter value when it is found.</param>
    /// <returns>Nonzero if the parameter was found; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIntVariable")]
    int BaseApiGetIntVariable(HandleRef handle, string name, out int value);

    /// <summary>
    ///     Get the value of an internal bool parameter
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="name">The name of the boolean parameter to retrieve.</param>
    /// <param name="value">Receives the parameter value as a 32-bit integer when found.</param>
    /// <returns>Nonzero if the parameter was found; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetBoolVariable")]
    int BaseApiGetBoolVariable(HandleRef handle, string name, out int value);

    /// <summary>
    ///     Get the value of an internal double parameter
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="name">The name of the double parameter to retrieve.</param>
    /// <param name="value">Receives the parameter value when it is found.</param>
    /// <returns>Nonzero if the parameter was found; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDoubleVariable")]
    int BaseApiGetDoubleVariable(HandleRef handle, string name, out double value);

    /// <summary>Gets the value of an internal string parameter.</summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="name">The name of the string parameter to retrieve.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetStringVariable")]
    IntPtr BaseApiGetStringVariableInternal(HandleRef handle, string name);

    // TESS_API void TessBaseAPIPrintVariables(const TessBaseAPI *handle, FILE *fp);

    /// <summary>
    ///     Print Tesseract parameters to the given file
    /// </summary>
    /// <remarks>
    ///     Must not be the first method called after instance create
    /// </remarks>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="filename">The path of the file to which variables are written.</param>
    /// <returns>Nonzero if the variables were written successfully; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIPrintVariablesToFile")]
    int BaseApiPrintVariablesToFile(HandleRef handle, string filename);

    /// <summary>
    ///     Instances are now mostly thread-safe and totally independent, but some global parameters remain. Basically it is safe to use
    ///     multiple TessBaseAPIs in different threads in parallel, UNLESS you use SetVariable on some of the Params in classify and textord.
    ///     If you do, then the effect will be to change it for all your instances.
    ///
    ///     Start tesseract.Returns zero on success and -1 on failure. NOTE that the only members that may be called before Init are those
    ///     listed above here in the class definition.
    ///
    ///     It is entirely safe(and eventually will be efficient too) to call Init multiple times on the same instance to change language,
    ///     or just to reset the classifier.Languages may specify internally that they want to be loaded with one or more other languages,
    ///     so the ~sign is available to override that.E.g., if hin were set to load eng by default, then hin+~eng would force loading only
    ///     hin.The number of loaded languages is limited only by memory, with the caveat that loading additional languages will impact both
    ///     speed and accuracy, as there is more work to do to decide on the applicable language, and there is more chance of hallucinating
    ///     incorrect words.WARNING: On changing languages, all Tesseract parameters are reset back to their default values. (Which may vary
    ///     between languages.) If you have a rare need to set a Variable that controls initialization for a second call to Init you should
    ///     explicitly call End() and then use SetVariable before Init. This is only a very rare use case, since there are very few uses that
    ///     require any parameters to be set before Init
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="datapath">The path to the tessdata directory.</param>
    /// <param name="language">The language code or codes to load.</param>
    /// <param name="mode">The OCR engine mode.</param>
    /// <param name="configs">Configuration file names to load.</param>
    /// <param name="configs_size">The number of entries in <paramref name="configs"/>.</param>
    /// <returns>0 on success and -1 on initialization failure.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit1")]
    int BaseApiInit1(HandleRef handle, string datapath, string language, EngineMode mode, string[] configs, int configs_size);

    /// <summary>
    ///     Instances are now mostly thread-safe and totally independent, but some global parameters remain. Basically it is safe to use
    ///     multiple TessBaseAPIs in different threads in parallel, UNLESS you use SetVariable on some of the Params in classify and textord.
    ///     If you do, then the effect will be to change it for all your instances.
    ///
    ///     Start tesseract.Returns zero on success and -1 on failure. NOTE that the only members that may be called before Init are those
    ///     listed above here in the class definition.
    ///
    ///     It is entirely safe(and eventually will be efficient too) to call Init multiple times on the same instance to change language,
    ///     or just to reset the classifier.Languages may specify internally that they want to be loaded with one or more other languages,
    ///     so the ~sign is available to override that.E.g., if hin were set to load eng by default, then hin+~eng would force loading only
    ///     hin.The number of loaded languages is limited only by memory, with the caveat that loading additional languages will impact both
    ///     speed and accuracy, as there is more work to do to decide on the applicable language, and there is more chance of hallucinating
    ///     incorrect words.WARNING: On changing languages, all Tesseract parameters are reset back to their default values. (Which may vary
    ///     between languages.) If you have a rare need to set a Variable that controls initialization for a second call to Init you should
    ///     explicitly call End() and then use SetVariable before Init. This is only a very rare use case, since there are very few uses that
    ///     require any parameters to be set before Init
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="datapath">The path to the tessdata directory.</param>
    /// <param name="language">The language code or codes to load.</param>
    /// <param name="mode">The OCR engine mode.</param>
    /// <returns>0 on success and -1 on initialization failure.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit2")]
    int BaseApiInit2(HandleRef handle, string datapath, string language, EngineMode mode);

    /// <summary>
    ///     Instances are now mostly thread-safe and totally independent, but some global parameters remain. Basically it is safe to use
    ///     multiple TessBaseAPIs in different threads in parallel, UNLESS you use SetVariable on some of the Params in classify and textord.
    ///     If you do, then the effect will be to change it for all your instances.
    ///
    ///     Start tesseract.Returns zero on success and -1 on failure. NOTE that the only members that may be called before Init are those
    ///     listed above here in the class definition.
    ///
    ///     It is entirely safe(and eventually will be efficient too) to call Init multiple times on the same instance to change language,
    ///     or just to reset the classifier.Languages may specify internally that they want to be loaded with one or more other languages,
    ///     so the ~sign is available to override that.E.g., if hin were set to load eng by default, then hin+~eng would force loading only
    ///     hin.The number of loaded languages is limited only by memory, with the caveat that loading additional languages will impact both
    ///     speed and accuracy, as there is more work to do to decide on the applicable language, and there is more chance of hallucinating
    ///     incorrect words.WARNING: On changing languages, all Tesseract parameters are reset back to their default values. (Which may vary
    ///     between languages.) If you have a rare need to set a Variable that controls initialization for a second call to Init you should
    ///     explicitly call End() and then use SetVariable before Init. This is only a very rare use case, since there are very few uses that
    ///     require any parameters to be set before Init
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="datapath">The path to the tessdata directory.</param>
    /// <param name="language">The language code or codes to load.</param>
    /// <returns>0 on success and -1 on initialization failure.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit3")]
    int BaseApiInit3(HandleRef handle, string datapath, string language);

    /// <summary>
    ///     Instances are now mostly thread-safe and totally independent, but some global parameters remain. Basically it is safe to use
    ///     multiple TessBaseAPIs in different threads in parallel, UNLESS you use SetVariable on some of the Params in classify and textord.
    ///     If you do, then the effect will be to change it for all your instances.
    /// 
    ///     Start tesseract.Returns zero on success and -1 on failure. NOTE that the only members that may be called before Init are those
    ///     listed above here in the class definition.
    /// 
    ///     It is entirely safe(and eventually will be efficient too) to call Init multiple times on the same instance to change language,
    ///     or just to reset the classifier.Languages may specify internally that they want to be loaded with one or more other languages,
    ///     so the ~sign is available to override that.E.g., if hin were set to load eng by default, then hin+~eng would force loading only
    ///     hin.The number of loaded languages is limited only by memory, with the caveat that loading additional languages will impact both
    ///     speed and accuracy, as there is more work to do to decide on the applicable language, and there is more chance of hallucinating
    ///     incorrect words.WARNING: On changing languages, all Tesseract parameters are reset back to their default values. (Which may vary
    ///     between languages.) If you have a rare need to set a Variable that controls initialization for a second call to Init you should
    ///     explicitly call End() and then use SetVariable before Init. This is only a very rare use case, since there are very few uses
    ///     that require any parameters to be set before Init
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="datapath">The path to the tessdata directory.</param>
    /// <param name="language">The language code or codes to load.</param>
    /// <param name="mode">The OCR engine mode.</param>
    /// <param name="configs">Configuration file names to load.</param>
    /// <param name="configs_size">The number of entries in <paramref name="configs"/>.</param>
    /// <param name="vars_vec">Names of variables to set during initialization.</param>
    /// <param name="vars_values">Values corresponding to <paramref name="vars_vec"/>.</param>
    /// <param name="vars_vec_size">The number of entries in the variable arrays.</param>
    /// <param name="set_only_non_debug_params">Whether to set only non-debug parameters.</param>
    /// <returns>0 on success and -1 on initialization failure.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit4")]
    int BaseApiInit4(HandleRef handle, string datapath, string language, int mode, string[] configs, int configs_size, string[] vars_vec, string[] vars_values, UIntPtr vars_vec_size, bool set_only_non_debug_params);

    /// <summary>
    ///     Initializes Tesseract using a traineddata file supplied as an in-memory buffer.
    /// </summary>
    /// <param name="handle">The TesseractAPI instance.</param>
    /// <param name="data">The traineddata file contents.</param>
    /// <param name="data_size">The size of the traineddata buffer in bytes.</param>
    /// <param name="language">The language code(s) to initialize.</param>
    /// <param name="mode">The OCR engine mode.</param>
    /// <param name="configs">Configuration file names.</param>
    /// <param name="configs_size">The number of configuration files.</param>
    /// <param name="vars_vec">Variable names to set during initialization.</param>
    /// <param name="vars_values">Values corresponding to <paramref name="vars_vec"/>.</param>
    /// <param name="vars_vec_size">The number of variable names and values.</param>
    /// <param name="set_only_non_debug_params">Whether to set only non-debug parameters.</param>
    /// <returns>0 on success; -1 if initialization fails.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit5")]
    int BaseApiInit5(HandleRef handle, byte[] data, int data_size, string language, EngineMode mode, string[] configs, int configs_size, string[] vars_vec, string[] vars_values, UIntPtr vars_vec_size, bool set_only_non_debug_params);

    /// <summary>
    ///     Returns the languages string used in the last valid initialization. If the last initialization specified "deu+hin" then that
    ///     will be returned. If hin loaded eng automatically as well, then that will not be included in this list. To find the languages
    ///     actually loaded, use GetLoadedLanguagesAsVector. The returned string should NOT be deleted
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetInitLanguagesAsString")]
    string BaseAPIGetInitLanguagesAsString(HandleRef handle);

    /// <summary>
    ///     Returns the loaded languages
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetLoadedLanguagesAsVector")]
    IntPtr BaseAPIGetLoadedLanguagesAsVector(HandleRef handle);

    /// <summary>
    ///     Returns the available languages
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetAvailableLanguagesAsVector")]
    IntPtr BaseAPIGetAvailableLanguagesAsVector(HandleRef handle);

    /// <summary>
    ///     Init only for page layout analysis. Use only for calls to SetImage and AnalysePage. Calls that attempt recognition will generate an error
    /// </summary>
    /// <param name="handle">The Tesseract API instance to initialize for page layout analysis.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInitForAnalysePage")]
    void BaseAPIInitForAnalysePage(HandleRef handle);

    /// <summary>
    ///     Read a "config" file containing a set of param, value pairs. Searches the standard places: tessdata/configs, tessdata/tessconfigs and
    ///     also accepts a relative or absolute path name. Note: only non-init params will be set (init params are set by Init())
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="filename">The config file name or path to read.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIReadConfigFile")]
    void BaseAPIReadConfigFile(HandleRef handle, string filename);

    /// <summary>
    ///     Read a DEBUG "config" file containing a set of param, value pairs. Searches the standard places: tessdata/configs, tessdata/tessconfigs and
    ///     also accepts a relative or absolute path name. Note: only non-init params will be set (init params are set by Init())
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="filename">The debug config file name or path to read.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIReadDebugConfigFile")]
    void BaseAPIReadDebugConfigFile(HandleRef handle, string filename);

    /// <summary>
    ///     Set the current page segmentation mode. Defaults to PSM_SINGLE_BLOCK. The mode is stored as an IntParam
    ///     so it can also be modified by ReadConfigFile or SetVariable("tessedit_pageseg_mode", mode as string)
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="mode">The page segmentation mode to use.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetPageSegMode")]
    void BaseApiSetPageSegMode(HandleRef handle, PageSegMode mode);

    /// <summary>
    ///     Return the current page segmentation mode
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetPageSegMode")]
    PageSegMode BaseApiGetPageSegMode(HandleRef handle);

    /// <summary>
    ///     Recognize a rectangle from an image and return the result as a string
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="imagedata">The image pixel data.</param>
    /// <param name="bytes_per_pixel">The number of bytes per pixel.</param>
    /// <param name="bytes_per_line">The number of bytes per image row.</param>
    /// <param name="left">The left coordinate of the rectangle.</param>
    /// <param name="top">The top coordinate of the rectangle.</param>
    /// <param name="width">The rectangle width in pixels.</param>
    /// <param name="height">The rectangle height in pixels.</param>
    /// <returns>A pointer to the recognized UTF-8 text.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIRect")]
    IntPtr BaseAPIRect(HandleRef handle, byte[] imagedata, int bytes_per_pixel, int bytes_per_line, int left, int top, int width, int height);

    /// <summary>
    ///     Call between pages or documents etc to free up memory and forget adaptive data
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClearAdaptiveClassifier")]
    void BaseAPIClearAdaptiveClassifier(HandleRef handle);

    /// <summary>
    ///     Provide an image for Tesseract to recognize. Format is as TesseractRect above. Does not copy the image buffer, or take ownership.
    ///     The source image may be destroyed after Recognize is called, either explicitly or implicitly via one of the Get*Text functions.
    ///     SetImage clears all recognition results, and sets the rectangle to the full image, so it may be followed immediately by a GetUTF8Text,
    ///     and it will automatically perform recognition
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="imagedata">The image pixel data.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <param name="bytes_per_pixel">The number of bytes per pixel.</param>
    /// <param name="bytes_per_line">The number of bytes per image row.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetImage")]
    void BaseAPISetImage(HandleRef handle, byte[] imagedata, int width, int height, int bytes_per_pixel, int bytes_per_line);

    /// <summary>
    ///     Provide an image for Tesseract to recognize. Format is as TesseractRect above. Does not copy the image buffer, or take ownership.
    ///     The source image may be destroyed after Recognize is called, either explicitly or implicitly via one of the Get*Text functions.
    ///     SetImage clears all recognition results, and sets the rectangle to the full image, so it may be followed immediately by a
    ///     GetUTF8Text, and it will automatically perform recognition
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <param name="pixHandle"><see cref="Pix.Image.Handle"/></param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetImage2")]
    void BaseApiSetImage(HandleRef handle, HandleRef pixHandle);

    /// <summary>
    ///     Set the resolution of the source image in pixels per inch so font size information can be calculated in results. Call this after SetImage()
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="ppi">The source image resolution in pixels per inch.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetSourceResolution")]
    void BaseAPISetSourceResolution(HandleRef handle, int ppi);

    /// <summary>
    ///     Restrict recognition to a sub-rectangle of the image. Call after SetImage. Each SetRectangle clears the recognition results
    ///     so multiple rectangles can be recognized with the same image
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="left">The left coordinate of the rectangle.</param>
    /// <param name="top">The top coordinate of the rectangle.</param>
    /// <param name="width">The rectangle width in pixels.</param>
    /// <param name="height">The rectangle height in pixels.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetRectangle")]
    void BaseApiSetRectangle(HandleRef handle, int left, int top, int width, int height);

    /// <summary>
    ///     ONLY available after SetImage if you have Leptonica installed. Get a copy of the internal thresholded image from Tesseract
    /// </summary>
    /// <param name="handle">The Tesseract API instance containing the image.</param>
    /// <returns>A pointer to a copy of the thresholded image.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetThresholdedImage")]
    IntPtr BaseApiGetThresholdedImage(HandleRef handle);

    /// <summary>
    ///     Returns the page orientation gradient calculated during layout analysis.
    /// </summary>
    /// <param name="handle">The TesseractAPI instance.</param>
    /// <returns>The page orientation gradient.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetGradient")]
    float BaseApiGetGradient(HandleRef handle);

    /// <summary>
    ///     Get the result of page layout analysis as a Leptonica-style Boxa, Pixa pair, in reading order. Can be called before or after Recognize.
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="pixa">Receives the images corresponding to the returned regions.</param>
    /// <returns>A pointer to the region boxes.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetRegions")]
    IntPtr BaseAPIGetRegions(HandleRef handle, IntPtr pixa);

    /// <summary>
    ///     Get the textlines as a Leptonica-style Boxa, Pixa pair, in reading order. Can be called before or after Recognize. If blockids
    ///     is not NULL, the block-id of each line is also returned as an array of one element per line. delete [] after use. If paraids
    ///     is not NULL, the paragraph-id of each line within its block is also returned as an array of one element per line. delete [] after use.
    ///     Helper method to extract from the thresholded image(most common usage).
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="pixa">Receives the images corresponding to the text lines.</param>
    /// <param name="blockids">Receives the block ID for each line when provided.</param>
    /// <returns>A pointer to the text-line boxes.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetTextlines")]
    IntPtr BaseAPIGetTextlines(HandleRef handle, IntPtr pixa, IntPtr blockids);

    /// <summary>
    ///     Get the textlines as a Leptonica-style Boxa, Pixa pair, in reading order. Can be called before or after Recognize. If blockids
    ///     is not NULL, the block-id of each line is also returned as an array of one element per line. delete [] after use. If paraids is
    ///     not NULL, the paragraph-id of each line within its block is also returned as an array of one element per line. delete [] after use.
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="raw_image">Whether to extract line images from the original image instead of the thresholded image.</param>
    /// <param name="raw_padding">The padding in pixels around extracted original-image regions.</param>
    /// <param name="pixa">Receives the images corresponding to the text lines.</param>
    /// <param name="blockids">Receives the block ID for each line when provided.</param>
    /// <param name="paraids">Receives the paragraph ID for each line when provided.</param>
    /// <returns>A pointer to the text-line boxes.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetTextlines1")]
    IntPtr TessBaseAPIGetTextlines1(HandleRef handle, bool raw_image, int raw_padding, IntPtr pixa, IntPtr blockids, IntPtr paraids);

    /// <summary>
    ///     Get textlines and strips of image regions as a Leptonica-style Boxa, Pixa pair, in reading order. Enables downstream handling
    ///     of non-rectangular regions. Can be called before or after Recognize. If blockids is not NULL, the block-id of each line is
    ///     also returned as an array of one element per line. delete [] after use.
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="pixa">Receives the images corresponding to the text strips.</param>
    /// <param name="blockids">Receives the block ID for each strip when provided.</param>
    /// <returns>A pointer to the text-strip boxes.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetStrips")]
    IntPtr BaseAPIGetStrips(HandleRef handle, IntPtr pixa, IntPtr blockids);

    /// <summary>
    ///     Get the words as a Leptonica-style Boxa, Pixa pair, in reading order. Can be called before or after Recognize.
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="pixa">Receives the images corresponding to the words.</param>
    /// <returns>A pointer to the word boxes.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetWords")]
    IntPtr BaseAPIGetWords(HandleRef handle, IntPtr pixa);

    /// <summary>
    ///     Gets the individual connected (text) components (created after pages segmentation step, but before recognition) as a
    ///     Leptonica-style Boxa, Pixa pair, in reading order. Can be called before or after Recognize.
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="pixa">Receives the images of the connected components.</param>
    /// <returns>A pointer to the connected-component boxes.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetConnectedComponents")]
    IntPtr BaseAPIGetConnectedComponents(HandleRef handle, IntPtr pixa);

    /// <summary>
    ///     Get the given level kind of components (block, textline, word etc.) as a Leptonica-style Boxa, Pixa pair, in reading order.
    ///     Can be called before or after Recognize. If blockids is not NULL, the block-id of each component is also returned as an
    ///     array of one element per component. delete [] after use. If text_only is true, then only text components are returned.
    ///     Helper function to get binary images with no padding (most common usage).
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="level">The page iterator level of components to extract.</param>
    /// <param name="text_only">Nonzero to return only text components.</param>
    /// <param name="pixa">Receives images for the returned components.</param>
    /// <param name="blockids">Receives the block ID for each component when provided.</param>
    /// <returns>A pointer to the component boxes.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetComponentImages")]
    IntPtr BaseApiGetComponentImages(HandleRef handle, PageIteratorLevel level, int text_only, IntPtr pixa, IntPtr blockids);

    /// <summary>
    ///     Get the given level kind of components (block, textline, word etc.) as a Leptonica-style Boxa, Pixa pair, in reading order.
    ///     Can be called before or after Recognize. If blockids is not NULL, the block-id of each component is also returned as an array of
    ///     one element per component. delete [] after use. If paraids is not NULL, the paragraph-id of each component with its block is also
    ///     returned as an array of one element per component. delete [] after use. If raw_image is true, then portions of the original image
    ///     are extracted instead of the thresholded image and padded with raw_padding. If text_only is true, then only text components are returned.
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="level">The page iterator level of components to extract.</param>
    /// <param name="text_only">Nonzero to return only text components.</param>
    /// <param name="raw_image">Whether to extract regions from the original image.</param>
    /// <param name="raw_padding">The padding in pixels around original-image regions.</param>
    /// <param name="pixa">Receives images for the returned components.</param>
    /// <param name="blockids">Receives the block ID for each component when provided.</param>
    /// <param name="paraids">Receives the paragraph ID for each component when provided.</param>
    /// <returns>A pointer to the component boxes.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetComponentImages1")]
    IntPtr BaseApiGetComponentImages1(HandleRef handle, PageIteratorLevel level, int text_only, bool raw_image, int raw_padding, IntPtr pixa, IntPtr blockids, IntPtr paraids);

    /// <summary>
    ///     Scale factor from original image
    /// </summary>
    /// <param name="handle">The Tesseract API instance containing the image.</param>
    /// <returns>The scale factor applied to the original image.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetThresholdedImageScaleFactor")]
    int BaseAPIGetThresholdedImageScaleFactor(HandleRef handle);

    /// <summary>
    ///     Runs page layout analysis in the mode set by SetPageSegMode
    /// </summary>
    /// <param name="handle">The Tesseract API instance containing the image.</param>
    /// <returns>A pointer to the page iterator, or null if layout analysis fails.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAnalyseLayout")]
    IntPtr BaseApiAnalyseLayout(HandleRef handle);

    /// <summary>Recognizes the current image and stores the results in the Tesseract API instance.</summary>
    /// <param name="handle">The Tesseract API instance containing the image.</param>
    /// <param name="monitor">The progress monitor, or a null handle if no monitor is used.</param>
    /// <returns>0 on success; nonzero if recognition fails or is cancelled.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIRecognize")]
    int BaseApiRecognize(HandleRef handle, HandleRef monitor);

    /// <summary>
    ///     Recognize the image from SetAndThresholdImage, generating Tesseract internal structures. Returns 0 on success. Optional.
    ///     The Get*Text functions below will call Recognize if needed. After Recognize, the output is kept internally until the next SetImage
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="filename">The input file or page sequence to process.</param>
    /// <param name="retry_config">The retry configuration name, or null for no retry.</param>
    /// <param name="timeout_millisec">The recognition timeout in milliseconds.</param>
    /// <param name="renderer">The renderer receiving the results.</param>
    /// <returns>Nonzero if processing succeeds; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIProcessPages")]
    int BaseApiProcessPages(HandleRef handle, string filename, string retry_config, int timeout_millisec, HandleRef renderer);

    /// <summary>
    ///     Processes one image and sends the recognition results to the renderer.
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="pixHandle">The Leptonica image to process.</param>
    /// <param name="page_index">The page number to associate with this image.</param>
    /// <param name="filename">The input file name associated with the image.</param>
    /// <param name="retry_config">The retry configuration name, or null for no retry.</param>
    /// <param name="timeout_millisec">The recognition timeout in milliseconds.</param>
    /// <param name="renderer">The renderer receiving the results.</param>
    /// <returns>Nonzero if processing succeeds; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIProcessPage")]
    int BaseApiProcessPage(HandleRef handle, HandleRef pixHandle, int page_index, string filename, string retry_config, int timeout_millisec, HandleRef renderer);

    /// <summary>
    ///     Get a reading-order iterator to the results of LayoutAnalysis and/or Recognize. The returned iterator must be deleted after use.
    ///     WARNING! This class points to data held within the TessBaseAPI class, and therefore can only be used while the TessBaseAPI class
    ///     still exists and has not been subjected to a call of Init, SetImage, Recognize, Clear, End, DetectOS, or anything else that
    ///     changes the internal PAGE_RES.
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIterator")]
    IntPtr BaseApiGetIterator(HandleRef handle);

    /// <summary>
    ///     Get a mutable iterator to the results of LayoutAnalysis and/or Recognize. The returned iterator must be deleted after use.
    ///     WARNING! This class points to data held within the TessBaseAPI class, and therefore can only be used while the TessBaseAPI
    ///     class still exists and has not been subjected to a call of Init, SetImage, Recognize, Clear, End, DetectOS, or anything else
    ///     that changes the internal PAGE_RES.
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetMutableIterator")]
    IntPtr BaseAPIGetMutableIterator(HandleRef handle);

    /// <summary>Returns the recognized text for the current image as UTF-8.</summary>
    /// <param name="handle">The Tesseract API instance containing the recognition result.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUTF8Text")]
    IntPtr BaseApiGetUTF8TextInternal(HandleRef handle);

    /// <summary>Returns the recognized text for a page in hOCR format.</summary>
    /// <param name="handle">The Tesseract API instance containing the recognition result.</param>
    /// <param name="pageNum">The zero-based page number.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetHOCRText")]
    IntPtr BaseApiGetHOcrTextInternal(HandleRef handle, int pageNum);

    /// <summary>Returns the recognized text for a page in ALTO XML format.</summary>
    /// <param name="handle">The Tesseract API instance containing the recognition result.</param>
    /// <param name="pageNum">The zero-based page number.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetAltoText")]
    IntPtr BaseApiGetAltoTextInternal(HandleRef handle, int pageNum);

    /// <summary>
    ///     Returns the recognized text for a page in PAGE XML format.
    /// </summary>
    /// <param name="handle">The TesseractAPI instance.</param>
    /// <param name="pageNum">The zero-based page number.</param>
    /// <returns>A pointer to the PAGE XML text, or null if unavailable.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetPAGEText")]
    IntPtr BaseApiGetPAGETextInternal(HandleRef handle, int pageNum);

    /// <summary>Returns the recognized text for a page in TSV format.</summary>
    /// <param name="handle">The Tesseract API instance containing the recognition result.</param>
    /// <param name="pageNum">The zero-based page number.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetTsvText")]
    IntPtr BaseApiGetTsvTextInternal(HandleRef handle, int pageNum);

    /// <summary>Returns the recognized text for a page in box-file format.</summary>
    /// <param name="handle">The Tesseract API instance containing the recognition result.</param>
    /// <param name="pageNum">The zero-based page number.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetBoxText")]
    IntPtr BaseApiGetBoxTextInternal(HandleRef handle, int pageNum);

    /// <summary>Returns the recognized text for a page in LSTM box-file format.</summary>
    /// <param name="handle">The Tesseract API instance containing the recognition result.</param>
    /// <param name="pageNum">The zero-based page number.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetLSTMBoxText")]
    IntPtr BaseApiGetLstmBoxTextInternal(HandleRef handle, int pageNum);

    /// <summary>Returns the recognized text for a page in WordStr box-file format.</summary>
    /// <param name="handle">The Tesseract API instance containing the recognition result.</param>
    /// <param name="pageNum">The zero-based page number.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetWordStrBoxText")]
    IntPtr BaseApiGetWordStrBoxTextInternal(HandleRef handle, int pageNum);

    /// <summary>Returns the recognized text for the current image in UNLV format.</summary>
    /// <param name="handle">The Tesseract API instance containing the recognition result.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUNLVText")]
    IntPtr BaseApiGetUnlvTextInternal(HandleRef handle);

    /// <summary>
    ///     Returns the average word confidence for Tesseract page result
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIMeanTextConf")]
    int BaseApiMeanTextConf(HandleRef handle);

    /// <summary>
    ///     Returns an array of all word confidences, terminated by -1
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAllWordConfidences")]
    IntPtr BaseAPIAllWordConfidences(HandleRef handle);

    /// <summary>
    ///     Applies the given word to the adaptive classifier if possible. The word must be SPACE-DELIMITED UTF-8 - l i k e t h i s ,
    ///     so it can tell the boundaries of the graphemes. Assumes that SetImage/SetRectangle have been used to set the image to the
    ///     given word. The mode arg should be PSM_SINGLE_WORD or PSM_CIRCLE_WORD, as that will be used to control layout analysis.
    ///     The currently set PageSegMode is preserved. Returns false if adaption was not possible for some reason
    /// </summary>
    /// <param name="handle">The Tesseract API instance configured with the image of the word.</param>
    /// <param name="mode">The page segmentation mode used to analyze the word.</param>
    /// <param name="wordstr">The space-delimited UTF-8 word to apply to the adaptive classifier.</param>
    /// <returns>Nonzero if the word was applied successfully; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAdaptToWordStr")]
    int BaseAPIAdaptToWordStr(HandleRef handle, PageSegMode mode, string wordstr);

    /// <summary>
    ///     Free up recognition results and any stored image data, without actually freeing any recognition data that would be time-consuming to reload
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClear")]
    void BaseApiClear(HandleRef handle);

    /// <summary>
    ///     Close down tesseract and free up all memory. End() is equivalent to destructing and reconstructing your TessBaseAPI.
    ///     Once End() has been used, none of the other API functions may be used other than Init and anything declared above
    ///     it in the class definition.
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIEnd")]
    void BaseAPIEnd(HandleRef handle);

    /// <summary>
    ///     Check whether a word is valid according to Tesseract's language model
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="word">The word to check against the language model.</param>
    /// <remarks>
    ///     0 if the word is invalid, non-zero if valid
    /// </remarks>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIIsValidWord")]
    int BaseAPIIsValidWord(HandleRef handle, string word);

    /// <summary>
    ///     Gets text direction
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <param name="out_offset">Offset</param>
    /// <param name="out_slope">Slope</param>
    /// <returns>TRUE if text direction is valid</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetTextDirection")]
    int BaseAPIGetTextDirection(HandleRef handle, out int out_offset, out float out_slope);

    /// <summary>
    ///     Gets the string of the specified unichar
    /// </summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="unichar_id">The identifier of the character to retrieve.</param>
    /// <returns>The string representation of the character.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUnichar")]
    string BaseAPIGetUnichar(HandleRef handle, int unichar_id);

    /// <summary>
    ///     Clear any library-level memory caches. There are a variety of expensive-to-load constant data structures
    ///     (mostly language dictionaries) that are cached globally -- surviving the Init() and End() of individual TessBaseAPI's.
    ///     This function allows the clearing of these caches
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClearPersistentCache")]
    void BaseAPIClearPersistentCache(HandleRef handle);

    /// <summary>Detects the page orientation and writing script, returning confidence values for each result.</summary>
    /// <param name="handle">The Tesseract API instance containing the page image.</param>
    /// <param name="orient_deg">Receives the page orientation in degrees.</param>
    /// <param name="orient_conf">Receives the orientation confidence.</param>
    /// <param name="script_name">Receives a pointer to the detected script name.</param>
    /// <param name="script_conf">Receives the script confidence.</param>
    /// <returns>Nonzero if orientation and script detection succeeds; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDetectOrientationScript")]
    int BaseAPIDetectOrientationScript(HandleRef handle, out int orient_deg, out float orient_conf, out IntPtr script_name, out float script_conf);

    /// <summary>Sets the minimum margin used when detecting page orientation.</summary>
    /// <param name="handle">The Tesseract API instance.</param>
    /// <param name="margin">The minimum orientation confidence margin.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetMinOrientationMargin")]
    void BaseAPISetMinOrientationMargin(HandleRef handle, double margin);

    /// <summary>Returns the number of loaded dictionary (Dawg) models.</summary>
    /// <param name="handle">The Tesseract API instance.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPINumDawgs")]
    int BaseAPINumDawgs(HandleRef handle);

    /// <summary>
    ///     Returns the current engine mode
    /// </summary>
    /// <param name="handle">The TesseractAPI instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIOem")]
    EngineMode BaseAPIOem(HandleRef handle);

    /// <summary>
    ///     Return text orientation of each block as determined in an earlier page layout analysis operation.
    ///     Orientation is returned as the number of ccw 90-degree rotations (in [0..3]) required to make the
    ///     text in the block upright (readable). Note that this may not necessary be the block orientation preferred
    ///     for recognition (such as the case of vertical CJK text). Also returns whether the text in the block is
    ///     believed to have vertical writing direction (when in an upright page orientation).
    ///     The returned array is of length equal to the number of text blocks, which may be less than the total number of blocks.
    ///     The ordering is intended to be consistent with GetTextLines().
    /// </summary>
    /// <param name="handle">The Tesseract API instance with completed layout analysis.</param>
    /// <param name="block_orientation">Receives the orientation value for each text block.</param>
    /// <param name="vertical_writing">Receives whether each text block uses vertical writing.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseGetBlockTextOrientations")]
    void BaseGetBlockTextOrientations(HandleRef handle, out int[] block_orientation, out bool[] vertical_writing);
    #endregion

    #region Page iterator
    /// <summary>
    ///     Deletes the specified PageIterator instance
    /// </summary>
    /// <param name="handle">The TessPageIterator instance</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorDelete")]
    void PageIteratorDelete(HandleRef handle);

    /// <summary>
    ///     Creates a copy of the specified PageIterator instance
    /// </summary>
    /// <param name="handle">The page iterator to copy.</param>
    /// <returns>A pointer to the copied page iterator.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorCopy")]
    IntPtr PageIteratorCopy(HandleRef handle);

    /// <summary>
    ///     Resets the iterator to point to the start of the page
    /// </summary>
    /// <param name="handle">The TessPageIterator instance</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBegin")]
    void PageIteratorBegin(HandleRef handle);

    /// <summary>
    ///     Moves to the start of the next object at the given level in the page hierarchy, and returns false if the
    ///     end of the page was reached. NOTE (CHANGED!) that ALL PageIteratorLevel level values will visit each non-text
    ///     block at least once.Think of non text blocks as containing a single para, with at least one line, with a single
    ///     imaginary word, containing a single symbol.The bounding boxes mark out any polygonal nature of the block,
    ///     and PTIsTextType(BLockType()) is false for non-text blocks. Calls to Next with different levels may be freely
    ///     intermixed.This function iterates words in right-to-left scripts correctly, if the appropriate language has
    ///     been loaded into Tesseract
    /// </summary>
    /// <param name="handle">The page iterator to advance.</param>
    /// <param name="level">The page hierarchy level at which to advance.</param>
    /// <returns>Nonzero if another element exists at the requested level; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorNext")]
    int PageIteratorNext(HandleRef handle, PageIteratorLevel level);

    /// <summary>
    ///     Returns TRUE if the iterator is at the start of an object at the given level. Possible uses include
    ///     determining if a call to Next(RIL_WORD) moved to the start of a RIL_PARA
    /// </summary>
    /// <remarks>
    ///     1 if true
    /// </remarks>
    /// <param name="handle">The page iterator to inspect.</param>
    /// <param name="level">The hierarchy level to check.</param>
    /// <returns>Nonzero if the iterator is at the beginning of that level; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
    int PageIteratorIsAtBeginningOf(HandleRef handle, PageIteratorLevel level);

    /// <summary>
    ///     Returns whether the iterator is positioned at the last element in a given level. (e.g. the last word in a line, the last line in a block)
    /// </summary>
    /// <remarks>
    ///     1 if true
    /// </remarks>
    /// <param name="handle">The page iterator to inspect.</param>
    /// <param name="level">The hierarchy level containing the current position.</param>
    /// <param name="element">The element level whose final position is checked.</param>
    /// <returns>Nonzero if this is the final element at the specified level; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
    int PageIteratorIsAtFinalElement(HandleRef handle, PageIteratorLevel level, PageIteratorLevel element);

    /// <summary>
    ///     Returns the bounding rectangle of the current object at the given level in coordinates of the original image
    /// </summary>
    /// <param name="handle">The page iterator identifying the current object.</param>
    /// <param name="level">The hierarchy level of the object.</param>
    /// <param name="left">Receives the left coordinate of the bounding box.</param>
    /// <param name="top">Receives the top coordinate of the bounding box.</param>
    /// <param name="right">Receives the right coordinate of the bounding box.</param>
    /// <param name="bottom">Receives the bottom coordinate of the bounding box.</param>
    /// <returns>Nonzero if a bounding box is available; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBoundingBox")]
    int PageIteratorBoundingBox(HandleRef handle, PageIteratorLevel level, out int left, out int top, out int right, out int bottom);

    /// <summary>
    ///     Returns the type of the current block
    /// </summary>
    /// <param name="handle">The TessPageIterator instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBlockType")]
    PolyBlockType PageIteratorBlockType(HandleRef handle);

    /// <summary>
    ///     Returns a binary image of the current object at the given level. The position and size match the return from BoundingBoxInternal,
    ///     and so this could be upscaled with respect to the original input image. Use pixDestroy to delete the image after use. The
    ///     following methods are used to generate the images: RIL_BLOCK: mask the page image with the block polygon. RIL_TEXTLINE: Clip the
    ///     rectangle of the line box from the page image.
    /// </summary>
    /// <param name="handle">The page iterator identifying the current object.</param>
    /// <param name="level">The hierarchy level of the object image to retrieve.</param>
    /// <returns>A pointer to the binary image, or null if unavailable.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetBinaryImage")]
    IntPtr PageIteratorGetBinaryImage(HandleRef handle, PageIteratorLevel level);

    /// <summary>
    ///     Returns an image of the current object at the given level in greyscale if available in the input. To guarantee a binary image use
    ///     BinaryImage. NOTE that in order to give the best possible image, the bounds are expanded slightly over the binary connected
    ///     component, by the supplied padding, so the top-left position of the returned image is returned in (left,top). These will most
    ///     likely not match the coordinates returned by BoundingBox. If you do not supply an original image, you will get a binary one.
    ///     Use pixDestroy to delete the image after use
    /// </summary>
    /// <param name="handle">The page iterator identifying the current object.</param>
    /// <param name="level">The hierarchy level of the object image to retrieve.</param>
    /// <param name="padding">The number of pixels to add around the object.</param>
    /// <param name="originalImage">The original image to crop, or a null handle to use a binary image.</param>
    /// <param name="left">Receives the left coordinate of the returned image in the original image.</param>
    /// <param name="top">Receives the top coordinate of the returned image in the original image.</param>
    /// <returns>A pointer to the extracted image, or null if unavailable.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetImage")]
    IntPtr PageIteratorGetImage(HandleRef handle, PageIteratorLevel level, int padding, HandleRef originalImage, out int left, out int top);

    /// <summary>
    ///     Returns the baseline of the current object at the given level. The baseline is the line that passes through (x1, y1) and (x2, y2).
    ///     WARNING: with vertical text, baselines may be vertical!
    /// </summary>
    /// <param name="handle">The page iterator identifying the current object.</param>
    /// <param name="level">The hierarchy level whose baseline is requested.</param>
    /// <param name="x1">Receives the x-coordinate of the baseline start.</param>
    /// <param name="y1">Receives the y-coordinate of the baseline start.</param>
    /// <param name="x2">Receives the x-coordinate of the baseline end.</param>
    /// <param name="y2">Receives the y-coordinate of the baseline end.</param>
    /// <returns>Nonzero if a baseline is available; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBaseline")]
    int PageIteratorBaseline(HandleRef handle, PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);

    /// <summary>
    ///     Returns the orientation
    /// </summary>
    /// <param name="handle">The page iterator for the current text block.</param>
    /// <param name="orientation">Receives the page orientation.</param>
    /// <param name="writing_direction">Receives the text writing direction.</param>
    /// <param name="textLineOrder">Receives the order of text lines.</param>
    /// <param name="deskew_angle">Receives the estimated deskew angle in radians.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorOrientation")]
    void PageIteratorOrientation(HandleRef handle, out Orientation orientation, out WritingDirection writing_direction, out TextLineOrder textLineOrder, out float deskew_angle);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle">The TessPageIterator instance</param>
    /// <param name="justification"><see cref="ParagraphJustification"/></param>
    /// <param name="is_list_item">List item</param>
    /// <param name="is_crown">Very first or continuation</param>
    /// <param name="first_line_indent">First line indentation</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorParagraphInfo")]
    void PageIteratorParagraphInfo(HandleRef handle, out ParagraphJustification justification, out bool is_list_item, out bool is_crown, out int first_line_indent);
    #endregion

    #region Result iterator
    /// <summary>
    ///     Deletes the specified ResultIterator handle
    /// </summary>
    /// <param name="handle">The TessResultIterator instance</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorDelete")]
    void ResultIteratorDelete(HandleRef handle);

    /// <summary>
    ///     Creates a copy of the specified ResultIterator instance
    /// </summary>
    /// <param name="handle">The TessResultIterator instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorCopy")]
    IntPtr ResultIteratorCopy(HandleRef handle);

    /// <summary>
    ///     Gets the PageIterator of the specified ResultIterator instance
    /// </summary>
    /// <param name="handle">The TessResultIterator instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetPageIterator")]
    IntPtr ResultIteratorGetPageIterator(HandleRef handle);

    /// <summary>
    ///     Gets the PageIterator of the specified ResultIterator instance.
    /// </summary>
    /// <param name="handle">The TessResultIterator instance</param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetPageIteratorConst")]
    IntPtr TessResultIteratorGetPageIteratorConst(HandleRef handle);

    /// <summary>
    ///     Gets the choice iterator for the current result.
    /// </summary>
    /// <param name="handle">The result iterator for the current recognition result.</param>
    /// <returns>A pointer to the choice iterator, or null if none is available.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetChoiceIterator")]
    IntPtr ResultIteratorGetChoiceIterator(HandleRef handle);

    /// <summary>Advances the result iterator to the next element at the specified level.</summary>
    /// <param name="handle">The result iterator to advance.</param>
    /// <param name="level">The page hierarchy level at which to advance.</param>
    /// <returns>Nonzero if another element exists at the requested level; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorNext")]
    int ResultIteratorNext(HandleRef handle, PageIteratorLevel level);

    /// <summary>Returns the recognized text for the current result at the specified level as UTF-8.</summary>
    /// <param name="handle">The result iterator positioned at the desired result.</param>
    /// <param name="level">The hierarchy level whose text is requested.</param>
    /// <returns>A pointer to the UTF-8 text, or null if unavailable.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetUTF8Text")]
    IntPtr ResultIteratorGetUTF8TextInternal(HandleRef handle, PageIteratorLevel level);

    /// <summary>
    ///    Returns the mean confidence of the current object at the given level. The number should be interpreted as a percent probability (0.0f-100.0f) 
    /// </summary>
    /// <param name="handle">The TessResultIterator instance</param>
    /// <param name="level"><see cref="PageIteratorLevel"/></param>
    /// <returns></returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorConfidence")]
    float ResultIteratorGetConfidence(HandleRef handle, PageIteratorLevel level);

    /// <summary>Returns the recognition language associated with the current word.</summary>
    /// <param name="handle">The result iterator positioned at the word.</param>
    /// <returns>A pointer to the language code.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordRecognitionLanguage")]
    IntPtr ResultIteratorWordRecognitionLanguageInternal(HandleRef handle);

    /// <summary>
    ///     Returns the font attributes of the current word. If iterating at a higher level object than words, e.g.,
    ///     text lines, then this will return the attributes of the first word in that textline. The actual return
    ///     value is a string representing a font name. It points to an internal table and SHOULD NOT BE DELETED.
    ///     Lifespan is the same as the iterator itself, ie rendered invalid by various members of TessBaseAPI,
    ///     including Init, SetImage, End or deleting the TessBaseAPI. Point size is returned in printers points (1/72 inch)
    /// </summary>
    /// <param name="handle">The result iterator positioned at the word.</param>
    /// <param name="isBold">Receives whether the font is bold.</param>
    /// <param name="isItalic">Receives whether the font is italic.</param>
    /// <param name="isUnderlined">Receives whether the text is underlined.</param>
    /// <param name="isMonospace">Receives whether the font is monospaced.</param>
    /// <param name="isSerif">Receives whether the font is a serif font.</param>
    /// <param name="isSmallCaps">Receives whether the text uses small caps.</param>
    /// <param name="pointSize">Receives the font size in points.</param>
    /// <param name="fontId">Receives the internal font identifier.</param>
    /// <returns>A pointer to the font name.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordFontAttributes")]
    IntPtr ResultIteratorWordFontAttributes(HandleRef handle, out bool isBold, out bool isItalic, out bool isUnderlined, out bool isMonospace, out bool isSerif, out bool isSmallCaps, out int pointSize, out int fontId);

    /// <summary>
    ///     Returns TRUE if the current word was found in a dictionary
    /// </summary>
    /// <param name="handle">The TessResultIterator instance</param>
    /// <returns>1 if word is from dictionary</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsFromDictionary")]
    bool ResultIteratorWordIsFromDictionary(HandleRef handle);

    /// <summary>
    ///     Returns TRUE if the current word is numeric
    /// </summary>
    /// <param name="handle">The result iterator positioned at the word.</param>
    /// <returns>1 if word is numeric</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsNumeric")]
    bool ResultIteratorWordIsNumeric(HandleRef handle);
    
    /// <summary>
    ///     Returns TRUE if the current symbol is a superscript. If iterating at a higher level object than symbols, e.g., words, then this
    ///     will return the attributes of the first symbol in that word
    /// </summary>
    /// <param name="handle">The TessResultIterator instance</param>
    /// <returns>1 if symbol is superscript</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSuperscript")]
    bool ResultIteratorSymbolIsSuperscript(HandleRef handle);

    /// <summary>
    ///     Returns TRUE if the current symbol is a subscript. If iterating at a higher level object than symbols, e.g., words, then this
    ///     will return the attributes of the first symbol in that word
    /// </summary>
    /// <param name="handle">The TessResultIterator instance</param>
    /// <returns>1 if symbol is subscript</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSubscript")]
    bool ResultIteratorSymbolIsSubscript(HandleRef handle);

    /// <summary>
    ///     Returns TRUE if the current symbol is a dropcap. If iterating at a higher level object than symbols, e.g., words, then this
    ///     will return the attributes of the first symbol in that word
    /// </summary>
    /// <param name="handle">The TessResultIterator instance</param>
    /// <returns>1 if symbol is dropcap</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsDropcap")]
    bool ResultIteratorSymbolIsDropcap(HandleRef handle);
    #endregion

    #region Choice iterator
    /// <summary>Deletes the specified choice iterator and releases its resources.</summary>
    /// <param name="handle">The choice iterator to delete.</param>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorDelete")]
    void ChoiceIteratorDelete(HandleRef handle);

    /// <summary>Advances the choice iterator to the next choice.</summary>
    /// <param name="handle">The choice iterator to advance.</param>
    /// <returns>Nonzero if another choice exists; otherwise, zero.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorNext")]
    int ChoiceIteratorNext(HandleRef handle);

    /// <summary>Returns the UTF-8 text of the current choice.</summary>
    /// <param name="handle">The choice iterator positioned at the desired choice.</param>
    /// <returns>A pointer to the choice text.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorGetUTF8Text")]
    IntPtr ChoiceIteratorGetUTF8TextInternal(HandleRef handle);

    /// <summary>Returns the confidence of the current choice.</summary>
    /// <param name="handle">The choice iterator positioned at the desired choice.</param>
    /// <returns>The choice confidence as a percentage.</returns>
    [RuntimeDllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorConfidence")]
    float ChoiceIteratorGetConfidence(HandleRef handle);
    #endregion
}