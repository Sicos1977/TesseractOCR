//
// LstmBoxResult.cs
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
///     Renders the result in LSTM BOX format
/// </summary>
public sealed class LstmBoxResult : Result
{
    /// <summary>
    ///     Initializes a new LSTM box renderer that writes results to the specified file.
    /// </summary>
    /// <remarks>
    ///     The renderer writes the LSTM box format used by Tesseract's LSTM training tools. It records recognized
    ///     characters together with their image coordinates and page number, rather than producing ordinary reading text.
    /// </remarks>
    /// <example>
    ///     <code>H 10 20 24 40 0
    /// i 24 20 30 40 0</code>
    /// </example>
    /// <param name="outputFilename">The base path for the output file.</param>
    public LstmBoxResult(string outputFilename)
    {
        Logger.LogInformation("Create LSTM box renderer");

        var rendererHandle = TessApi.Native.LSTMBoxRendererCreate(outputFilename);
        Initialize(rendererHandle);
    }
}