//
// AltoResult.cs
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
///     Renders the result in ALTO format
/// </summary>
public sealed class AltoResult : Result
{
    /// <summary>
    ///     Initializes an ALTO renderer that writes output to the specified file.
    /// </summary>
    /// <remarks>
    ///     The renderer writes recognized text as ALTO XML, an XML format for representing OCR text together with page,
    ///     block, line, and word layout information. Use it when downstream document-processing tools need structured text and geometry.
    /// </remarks>
    /// <example>
    ///     <code>&lt;String CONTENT="Invoice" HPOS="100" VPOS="120" WIDTH="150" HEIGHT="35" /&gt;</code>
    /// </example>
    /// <param name="outputFilename">The base path for the output file.</param>
    public AltoResult(string outputFilename)
    {
        Logger.LogInformation("Create alto renderer");

        var rendererHandle = TessApi.Native.AltoRendererCreate(outputFilename);
        Initialize(rendererHandle);
    }
}