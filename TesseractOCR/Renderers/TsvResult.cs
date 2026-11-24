//
// TsvResult.cs
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
///     Renders the result in TSV format
/// </summary>

public sealed class TsvResult : Result
{
    /// <summary>
    ///     Initializes a TSV renderer that writes to the specified file.
    /// </summary>
    /// <remarks>
    ///     The renderer writes tab-separated rows for Tesseract's page, block, paragraph, line, and word hierarchy.
    ///     Word-level rows include recognized text, bounding-box coordinates, and confidence, making the output convenient for data processing.
    /// </remarks>
    /// <example>
    ///     <code>level page_num block_num par_num line_num word_num left top width height conf text
    /// 5     1        1         1       1       1        100  120  150   35     96.2  Invoice</code>
    /// </example>
    /// <param name="outputFilename">The base path for the output file.</param>
    public TsvResult(string outputFilename)
    {
        Logger.LogInformation("Create TSV renderer");

        var rendererHandle = TessApi.Native.TsvRendererCreate(outputFilename);
        Initialize(rendererHandle);
    }
}