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
        private const string WikiPageUrl = "https://github.com/Sicos1977/TesseractOCR/wiki/How-to-use-the-iLogger-interface";
        #endregion

        #region Format
        public static string Format(string messageFormat, params object[] messageArgs)
        {
            var errorMessage = string.Format(messageFormat, messageArgs);
            return string.Format(ErrorMessageFormat, errorMessage, WikiPageUrl);
        }
        #endregion
    }
}
