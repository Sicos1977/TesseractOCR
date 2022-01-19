//
// Logger.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2022 Kees van Spelde
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

//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT
using System.Diagnostics;
using System.Globalization;
// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Internal
{
    internal static class Logger
    {
        #region Fields
        private static readonly TraceSource Trace = new TraceSource("Tesseract");
        #endregion

        #region TraceInformation
        public static void TraceInformation(string format, params object[] args)
        {
            Trace.TraceEvent(TraceEventType.Information, 0, string.Format(CultureInfo.CurrentCulture, format, args));
        }
        #endregion

        #region TraceError
        public static void TraceError(string format, params object[] args)
        {
            Trace.TraceEvent(TraceEventType.Error, 0, string.Format(CultureInfo.CurrentCulture, format, args));
        }
        #endregion

        #region TraceWarning
        public static void TraceWarning(string format, params object[] args)
        {
            Trace.TraceEvent(TraceEventType.Warning, 0, string.Format(CultureInfo.CurrentCulture, format, args));
        }
        #endregion
    }
}