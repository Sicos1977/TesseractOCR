//
// MarshalHelper.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2023 Kees van Spelde
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

using System;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Interop
{
    internal static unsafe class MarshalHelper
    {
        #region Fields
        private static readonly Encoding DefaultEncoding = new UTF8Encoding(false);
        #endregion

        #region StringToPtr
        internal static IntPtr StringToPtr(string value, Encoding encoding = null)
        {
            encoding = encoding ?? DefaultEncoding;
            var length = encoding.GetByteCount(value);

            // The encoded value is null terminated that's the reason for the '+1'.
            var encodedValue = new byte[length + 1];
            encoding.GetBytes(value, 0, value.Length, encodedValue, 0);
            var handle = Marshal.AllocHGlobal(new IntPtr(encodedValue.Length));
            Marshal.Copy(encodedValue, 0, handle, encodedValue.Length);
            return handle;
        }
        #endregion

        #region PtrToString
        internal static string PtrToString(IntPtr handle, Encoding encoding = null)
        {
            encoding = encoding ?? DefaultEncoding;
            if (IntPtr.Zero == handle) return null;
            var length = StrLength(handle);
            return new string((sbyte*)handle.ToPointer(), 0, length, encoding);
        }
        #endregion

        #region StrLength
        /// <summary>
        ///		Gets the number of bytes in a null terminated byte array.
        /// </summary>
        internal static int StrLength(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return 0;

            var ptr = (byte*)handle.ToPointer();
            var length = 0;
            while (*(ptr + length) != 0)
            {
                length++;
            }
            return length;
        }
        #endregion
    }
}
