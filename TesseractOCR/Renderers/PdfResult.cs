//
// PdfResult.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2027 Kees van Spelde
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
using TesseractOCR.Helpers;
using TesseractOCR.Interop;

namespace TesseractOCR.Renderers;

/// <summary>
///     Renders the result in PDF format
/// </summary>
public sealed class PdfResult : Result
{
    #region Fields
    /// <summary>
    ///     Pointer to the font directory handle.
    /// </summary>
    private IntPtr _fontDirectoryHandle;
    #endregion

    #region Constructor
    /// <summary>
    ///     Initializes a PDF renderer.
    /// </summary>
    /// <remarks>
    ///     The renderer creates a searchable PDF by placing recognized text over the source page images. When
    ///     <paramref name="textOnly"/> is true, the page images are omitted and the PDF contains only the text layer.
    ///     The font data directory is used to embed the fonts required by the PDF output.
    /// </remarks>
    /// <example>
    ///     The output is a PDF document. In a PDF viewer, a page appears as the scanned image with an aligned,
    ///     selectable OCR text layer; with <paramref name="textOnly"/> enabled, only the text layer is present.
    /// </example>
    /// <param name="outputFilename">The base path for the output PDF file.</param>
    /// <param name="fontDirectory">The path to the directory containing PDF font data.</param>
    /// <param name="textOnly">true to create a text-only PDF; otherwise, false.</param>
    public PdfResult(string outputFilename, string fontDirectory, bool textOnly)
    {
        Logger.LogInformation("Create PDF renderer");

        var fontDirectoryHandle = Marshal.StringToHGlobalAnsi(fontDirectory);
        var rendererHandle = TessApi.Native.PDFRendererCreate(outputFilename, fontDirectoryHandle, textOnly ? 1 : 0);

        Initialize(rendererHandle);
    }
    #endregion

    #region Dispose
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // dispose of font
        if (_fontDirectoryHandle != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_fontDirectoryHandle);
            _fontDirectoryHandle = IntPtr.Zero;
        }
    }
    #endregion
}