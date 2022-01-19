//
// WindowsLibraryLoaderLogic.cs
//
// Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//
// Copyright (c) 2014 Andrey Akinshin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Runtime.InteropServices;

namespace TesseractOCR.InteropDotNet
{
    internal class WindowsLibraryLoaderLogic : ILibraryLoaderLogic
    {
        public IntPtr LoadLibrary(string fileName)
        {
            var libraryHandle = IntPtr.Zero;

            try
            {
                LibraryLoaderTrace.TraceInformation("Trying to load native library \"{0}\"...", fileName);
                libraryHandle = WindowsLoadLibrary(fileName);
                if (libraryHandle != IntPtr.Zero)
                    LibraryLoaderTrace.TraceInformation("Successfully loaded native library \"{0}\", handle = {1}.", fileName, libraryHandle);
                else
                    LibraryLoaderTrace.TraceError("Failed to load native library \"{0}\".\r\nCheck windows event log.", fileName);
            }
            catch (Exception e)
            {
                var lastError = WindowsGetLastError();
                LibraryLoaderTrace.TraceError("Failed to load native library \"{0}\".\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", fileName, lastError, e.ToString());
            }

            return libraryHandle;
        }

        public bool FreeLibrary(IntPtr libraryHandle)
        {
            try
            {
                LibraryLoaderTrace.TraceInformation("Trying to free native library with handle {0} ...", libraryHandle);
                var isSuccess = WindowsFreeLibrary(libraryHandle);
                if (isSuccess)
                    LibraryLoaderTrace.TraceInformation("Successfully freed native library with handle {0}.", libraryHandle);
                else
                    LibraryLoaderTrace.TraceError("Failed to free native library with handle {0}.\r\nCheck windows event log.", libraryHandle);
                return isSuccess;
            }
            catch (Exception e)
            {
                var lastError = WindowsGetLastError();
                LibraryLoaderTrace.TraceError("Failed to free native library with handle {0}.\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", libraryHandle, lastError, e.ToString());
                return false;
            }
        }

        public IntPtr GetProcAddress(IntPtr libraryHandle, string functionName)
        {
            try
            {
                LibraryLoaderTrace.TraceInformation("Trying to load native function \"{0}\" from the library with handle {1}...",
                    functionName, libraryHandle);
                var functionHandle = WindowsGetProcAddress(libraryHandle, functionName);
                if (functionHandle != IntPtr.Zero)
                    LibraryLoaderTrace.TraceInformation("Successfully loaded native function \"{0}\", function handle = {1}.",
                        functionName, functionHandle);
                else
                    LibraryLoaderTrace.TraceError("Failed to load native function \"{0}\", function handle = {1}",
                        functionName, functionHandle);
                return functionHandle;
            }
            catch (Exception e)
            {
                var lastError = WindowsGetLastError();
                LibraryLoaderTrace.TraceError("Failed to free native library with handle {0}.\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", libraryHandle, lastError, e.ToString());
                return IntPtr.Zero;
            }
        }

        public string FixUpLibraryName(string fileName)
        {
            if (!String.IsNullOrEmpty(fileName) && !fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                return fileName + ".dll";
            return fileName;
        }

        [DllImport("kernel32", EntryPoint = "LoadLibrary", CallingConvention = CallingConvention.Winapi,
            SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern IntPtr WindowsLoadLibrary(string dllPath);

        [DllImport("kernel32", EntryPoint = "FreeLibrary", CallingConvention = CallingConvention.Winapi,
            SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern bool WindowsFreeLibrary(IntPtr handle);

        [DllImport("kernel32", EntryPoint = "GetProcAddress", CallingConvention = CallingConvention.Winapi,
            SetLastError = true)]
        private static extern IntPtr WindowsGetProcAddress(IntPtr handle, string procedureName);

        private static int WindowsGetLastError()
        {
            return Marshal.GetLastWin32Error();
        }
    }
}