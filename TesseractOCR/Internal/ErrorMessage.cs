//
// ErrorMessage.cs
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

namespace TesseractOCR.Internal
{
    internal static class ErrorMessage
    {
        #region Constants
        private const string ErrorMessageFormat = "{0}. See {1} for details.";
        private const string WikiUrlFormat = "https://github.com/charlesw/tesseract/wiki/Error-{0}";
        #endregion

        #region Format
        public static string Format(int errorNumber, string messageFormat, params object[] messageArgs)
        {
            var errorMessage = string.Format(messageFormat, messageArgs);
            var errorPageUrl = ErrorPageUrl(errorNumber);
            return string.Format(ErrorMessageFormat, errorMessage, errorPageUrl);
        }
        #endregion

        #region ErrorPageUrl
        public static string ErrorPageUrl(int errorNumber)
        {
            return string.Format(WikiUrlFormat, errorNumber);
        }
        #endregion
    }
}
