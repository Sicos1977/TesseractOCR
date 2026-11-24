//
// PageXmlResult.cs
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

using TesseractOCR.Helpers;
using TesseractOCR.Interop;

namespace TesseractOCR.Renderers;

/// <summary>
///     Renders the result in PAGE XML format.
/// </summary>
public sealed class PageXmlResult : Result
{
    /// <summary>
    ///     Initializes a renderer that writes PAGE XML to the specified file.
    /// </summary>
    /// <remarks>
    ///     The renderer writes recognized text as PAGE XML, a structured page-description format that includes text
    ///     regions and their layout. Use it when a document-processing workflow needs OCR text associated with page structure.
    /// </remarks>
    /// <example>
    ///     <code>&lt;TextRegion id="region_1"&gt;
    ///   &lt;TextLine id="line_1"&gt;
    ///     &lt;Word id="word_1"&gt;
    ///       &lt;Unicode&gt;Invoice&lt;/Unicode&gt;
    ///     &lt;/Word&gt;
    ///   &lt;/TextLine&gt;
    /// &lt;/TextRegion&gt;</code>
    /// </example>
    /// <param name="outputFilename">The base path for the output file.</param>
    public PageXmlResult(string outputFilename)
    {
        Logger.LogInformation("Create PAGE XML renderer");

        var rendererHandle = TessApi.Native.PAGERendererCreate(outputFilename);
        Initialize(rendererHandle);
    }
}
