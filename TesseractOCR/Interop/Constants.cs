﻿//
// Constants.cs
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

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Interop
{
    /// <summary>
    ///     Description of Constants.
    /// </summary>
    internal static class Constants
    {
        public const string LeptonicaDllName = "leptonica-1.86.0.dll";
        public const string TesseractDllName = "tesseract55.dll";

        // Tesseract uses an int to represent true false values.
        public const int True = 1;
        public const int False = 0;
    }
}