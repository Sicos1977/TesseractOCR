//
// RenderFormat.cs
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

namespace TesseractOCR.Enums
{
    /// <summary>
    ///     Rendered formats supported by Tesseract.
    /// </summary>
    public enum RenderFormat
    {
        /// <summary>
        ///     To text
        /// </summary>
        Text,

        /// <summary>
        ///     To HOCR
        /// </summary>
        Hocr,

        /// <summary>
        ///     To PDF
        /// </summary>
        Pdf,

        /// <summary>
        ///     A PDF with only an invisible text layer
        /// </summary>
        PdfTextonly,

        /// <summary>
        ///     To UNLV
        /// </summary>
        Unlv,

        /// <summary>
        ///     To boxed
        /// </summary>
        Box,

        /// <summary>
        ///     To alto
        /// </summary>
        Alto,

        /// <summary>
        ///     To tsv
        /// </summary>
        Tsv,

        /// <summary>
        ///     To LSTM box
        /// </summary>
        LstmBox,

        /// <summary>
        ///     To word str box
        /// </summary>
        WordStrBox
    }
}
