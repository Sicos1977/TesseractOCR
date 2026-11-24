//
// TextResult.cs
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
///     Renders the result as Text
/// </summary>
public sealed class TextResult : Result
{
    /// <summary>
    ///     Initializes a text result renderer that writes output to a file.
    /// </summary>
    /// <remarks>
    ///     The renderer writes recognized text as UTF-8 plain text. It preserves the text content but does not include
    ///     structured layout details such as word bounding boxes or confidence values.
    /// </remarks>
    /// <example>
    ///     <code>Invoice number: 12345
    /// Total: $42.00</code>
    /// </example>
    /// <param name="outputFilename">The base path for the output file.</param>
    public TextResult(string outputFilename)
    {
        Logger.LogInformation("Create text renderer");

        var rendererHandle = TessApi.Native.TextRendererCreate(outputFilename);
        Initialize(rendererHandle);
    }
}