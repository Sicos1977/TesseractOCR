//
// TessConvert.cs
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

using System.Globalization;

namespace TesseractOCR.Internal
{
    /// <summary>
    ///     Utility helpers to handle converting variable values.
    /// </summary>
    internal static class TessConvert
    {
        #region TryToString
        public static bool TryToString(object value, out string result)
        {
            switch (value)
            {
                case bool b:
                    result = ToString(b);
                    break;

                case decimal value1:
                    result = ToString(value1);
                    break;

                case double d:
                    result = ToString(d);
                    break;

                case float f:
                    result = ToString(f);
                    break;

                case short s:
                    result = ToString(s);
                    break;

                case int i:
                    result = ToString(i);
                    break;

                case long l:
                    result = ToString(l);
                    break;

                case ushort value1:
                    result = ToString(value1);
                    break;

                case uint u:
                    result = ToString(u);
                    break;

                case ulong value1:
                    result = ToString(value1);
                    break;

                case string s:
                    result = s;
                    break;

                default:
                    result = null;
                    return false;
            }

            return true;
        }
        #endregion

        #region ToString
        public static string ToString(bool value)
        {
            return value ? "TRUE" : "FALSE";
        }

        public static string ToString(decimal value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string ToString(double value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string ToString(float value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string ToString(short value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string ToString(int value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string ToString(long value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string ToString(ushort value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string ToString(uint value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string ToString(ulong value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }
        #endregion
    }
}