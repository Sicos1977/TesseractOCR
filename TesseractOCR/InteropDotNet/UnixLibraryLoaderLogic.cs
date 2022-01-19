//
// UnixLibraryLoaderLogic.cs
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
    internal class UnixLibraryLoaderLogic : ILibraryLoaderLogic
    {
        public IntPtr LoadLibrary(string fileName)
        {
            var libraryHandle = IntPtr.Zero;

            try
            {
                LibraryLoaderTrace.TraceInformation("Trying to load native library \"{0}\"...", fileName);
                libraryHandle = UnixLoadLibrary(fileName, RTLD_NOW);
                if (libraryHandle != IntPtr.Zero)
                    LibraryLoaderTrace.TraceInformation("Successfully loaded native library \"{0}\", handle = {1}.", fileName, libraryHandle);
                else
                    LibraryLoaderTrace.TraceError("Failed to load native library \"{0}\".\r\nCheck windows event log.", fileName);
            }
            catch (Exception e)
            {
                var lastError = UnixGetLastError();
                LibraryLoaderTrace.TraceError("Failed to load native library \"{0}\".\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", fileName, lastError, e.ToString());
            }

            return libraryHandle;
        }

        public bool FreeLibrary(IntPtr libraryHandle)
        {
            return UnixFreeLibrary(libraryHandle) != 0;
        }

        public IntPtr GetProcAddress(IntPtr libraryHandle, string functionName)
        {
            UnixGetLastError(); // Clearing previous errors
            LibraryLoaderTrace.TraceInformation("Trying to load native function \"{0}\" from the library with handle {1}...",
                functionName, libraryHandle);
            var functionHandle = UnixGetProcAddress(libraryHandle, functionName);
            var errorPointer = UnixGetLastError();
            if (errorPointer != IntPtr.Zero)
                throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errorPointer));
            if (functionHandle != IntPtr.Zero && errorPointer == IntPtr.Zero)
                LibraryLoaderTrace.TraceInformation("Successfully loaded native function \"{0}\", function handle = {1}.",
                    functionName, functionHandle);
            else
                LibraryLoaderTrace.TraceError("Failed to load native function \"{0}\", function handle = {1}, error pointer = {2}",
                    functionName, functionHandle, errorPointer);
            return functionHandle;
        }

        public string FixUpLibraryName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (!fileName.EndsWith(".so", StringComparison.OrdinalIgnoreCase))
                    fileName += ".so";
                if (!fileName.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
                    fileName = "lib" + fileName;
            }
            return fileName;
        }

        const int RTLD_NOW = 2;

        [DllImport("libdl.so", EntryPoint = "dlopen")]
        private static extern IntPtr UnixLoadLibrary(String fileName, int flags);

        [DllImport("libdl.so", EntryPoint = "dlclose", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int UnixFreeLibrary(IntPtr handle);

        [DllImport("libdl.so", EntryPoint = "dlsym", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixGetProcAddress(IntPtr handle, String symbol);

        [DllImport("libdl.so", EntryPoint = "dlerror", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixGetLastError();
    }
}