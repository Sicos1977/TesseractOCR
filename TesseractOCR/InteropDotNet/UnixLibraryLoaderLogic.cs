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
using TesseractOCR.Helpers;

namespace TesseractOCR.InteropDotNet
{
    internal class UnixLibraryLoaderLogic : ILibraryLoaderLogic
    {
        #region Consts
        private const int RtldNow = 2;
        #endregion

        #region Fields
        private static readonly string FileExtension = SystemManager.GetOperatingSystem() == OperatingSystem.MacOSX ? ".dylib" : ".so";
        #endregion

        #region LoadLibrary
        public IntPtr LoadLibrary(string fileName)
        {
            var libraryHandle = IntPtr.Zero;

            try
            {
                Logger.LogInformation($"Trying to load native library '{fileName}'");
                
                libraryHandle = UnixLoadLibrary(fileName, RtldNow);
                
                if (libraryHandle != IntPtr.Zero)
                    Logger.LogInformation($"Successfully loaded native library '{fileName}' with handle '{libraryHandle}'");
                else
                    Logger.LogError($"Failed to load native library '{fileName}', set logging to debug level and check logging");
            }
            catch (Exception exception)
            {
                var lastError = UnixGetLastError();
                Logger.LogError($"Failed to load native library '{fileName}', last error '{lastError}', inner Exception: {exception}");
            }

            return libraryHandle;
        }
        #endregion

        #region FreeLibrary
        public bool FreeLibrary(IntPtr libraryHandle)
        {
            return UnixFreeLibrary(libraryHandle) != 0;
        }
        #endregion

        #region GetProcAddress
        public IntPtr GetProcAddress(IntPtr libraryHandle, string functionName)
        {
            UnixGetLastError(); // Clearing previous errors

            Logger.LogDebug($"Trying to load native function '{functionName}' from the library with handle '{libraryHandle}'");
            
            var functionHandle = UnixGetProcAddress(libraryHandle, functionName);
            var errorPointer = UnixGetLastError();
            
            if (errorPointer != IntPtr.Zero)
                throw new Exception($"dlsym: {Marshal.PtrToStringAnsi(errorPointer)}");
            
            if (functionHandle != IntPtr.Zero && errorPointer == IntPtr.Zero)
                Logger.LogDebug($"Successfully loaded native function '{functionName}' with handle '{libraryHandle}'");
            else
                Logger.LogError($"Failed to load native function '{functionName}', function handle '{libraryHandle}', error pointer '{errorPointer}'");
            
            return functionHandle;
        }
        #endregion

        #region FixUpLibraryName
        public string FixUpLibraryName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;

            if (!fileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase))
                fileName += FileExtension;

            if (!fileName.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
                fileName = "lib" + fileName;

            return fileName;
        }
        #endregion

        #region Native methods
        [DllImport("libdl", EntryPoint = "dlopen")]
        private static extern IntPtr UnixLoadLibrary(string fileName, int flags);

        [DllImport("libdl", EntryPoint = "dlclose", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int UnixFreeLibrary(IntPtr handle);

        [DllImport("libdl", EntryPoint = "dlsym", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixGetProcAddress(IntPtr handle, string symbol);

        [DllImport("libdl", EntryPoint = "dlerror", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixGetLastError();
        #endregion
    }
}