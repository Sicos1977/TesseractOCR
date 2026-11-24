//
// WordStrBoxResult.cs
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
///     Renders the result in WORD STRING BOX format
/// </summary>

public sealed class WordStrBoxResult : Result
{
    /// <summary>
    ///     Initializes a renderer that writes word string box data to a file.
    /// </summary>
    /// <remarks>
    ///     The renderer writes Tesseract's WordStr box format, which stores recognized words with their bounding-box
    ///     coordinates and page number. Unlike character box output, each box represents a whole word.
    /// </remarks>
    /// <example>
    ///     <code>Hello 10 20 80 40 0</code>
    /// </example>
    /// <param name="outputFilename">The base path for the output file.</param>
    public WordStrBoxResult(string outputFilename)
    {
        Logger.LogInformation("Create word string box renderer");
        var rendererHandle = TessApi.Native.WordStrBoxRendererCreate(outputFilename);
        Initialize(rendererHandle);
    }
}