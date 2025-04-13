//
// EngineMode.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2025 Kees van Spelde
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

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Enums
{
    /// <summary>
    ///     The Tesseract engine mode
    /// </summary>
    public enum EngineMode
    {
        /// <summary>
        ///     Only the legacy tesseract OCR engine is used.
        /// </summary>
        TesseractOnly = 0,

        /// <summary>
        ///     Only the new LSTM-based OCR engine is used.
        /// </summary>
        LstmOnly,

        /// <summary>
        ///     Both the legacy and new LSTM based OCR engine is used.
        /// </summary>
        TesseractAndLstm,

        /// <summary>
        ///     The default OCR engine is used (currently LSTM-based OCR engine).
        /// </summary>
        Default
    }
}