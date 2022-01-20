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
using TesseractOCR.Loggers;

namespace TesseractOCR.InteropDotNet
{
    internal class WindowsLibraryLoaderLogic : ILibraryLoaderLogic
    {
        #region LoadLibrary
        public IntPtr LoadLibrary(string fileName)
        {
            var libraryHandle = IntPtr.Zero;

            try
            {
                Logger.LogInformation($"Trying to load native library '{fileName}'");
                libraryHandle = WindowsLoadLibrary(fileName);

                if (libraryHandle != IntPtr.Zero)
                    Logger.LogInformation($"Successfully loaded native library '{fileName}' with handle '{libraryHandle}'");
                else
                    Logger.LogError($"Failed to load native library '{fileName}', check logging");
            }
            catch (Exception exception)
            {
                var lastError = WindowsGetLastError();
                Logger.LogError($"Failed to load native library '{fileName}', last error '{lastError}', inner Exception: {exception}");
            }

            return libraryHandle;
        }
        #endregion

        #region FreeLibrary
        public bool FreeLibrary(IntPtr libraryHandle)
        {
            try
            {
                Logger.LogInformation($"Trying to free native library with handle '{libraryHandle}'");
                
                var isSuccess = WindowsFreeLibrary(libraryHandle);
                
                if (isSuccess)
                    Logger.LogInformation($"Successfully freed native library with handle '{libraryHandle}'");
                else
                    Logger.LogError($"Failed to free native library with handle '{libraryHandle}', check windows event log");

                return isSuccess;
            }
            catch (Exception exception)
            {
                var lastError = WindowsGetLastError();
                Logger.LogError($"Failed to free native library with handle '{libraryHandle}', last error '{lastError}', inner Exception: {exception}");
                return false;
            }
        }
        #endregion

        #region GetProcAddress
        public IntPtr GetProcAddress(IntPtr libraryHandle, string functionName)
        {
            try
            {
                Logger.LogInformation($"Trying to load native function '{functionName}' from the library with handle '{libraryHandle}'");
                var functionHandle = WindowsGetProcAddress(libraryHandle, functionName);
                
                if (functionHandle != IntPtr.Zero)
                    Logger.LogInformation($"Successfully loaded native function '{functionName}' with handle '{functionHandle}'");
                else
                    Logger.LogError($"Failed to load native function '{functionName}' with handle '{functionHandle}'");
                return functionHandle;
            }
            catch (Exception exception)
            {
                var lastError = WindowsGetLastError();
                Logger.LogError($"Failed to free native library with handle '{libraryHandle}', last error '{lastError}', inner Exception: {exception}");
                return IntPtr.Zero;
            }
        }
        #endregion

        #region FixUpLibraryName
        public string FixUpLibraryName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && !fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                return fileName + ".dll";
            return fileName;
        }
        #endregion

        #region Native methods
        [DllImport("kernel32", EntryPoint = "LoadLibrary", CallingConvention = CallingConvention.Winapi,
            SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern IntPtr WindowsLoadLibrary(string dllPath);

        [DllImport("kernel32", EntryPoint = "FreeLibrary", CallingConvention = CallingConvention.Winapi,
            SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern bool WindowsFreeLibrary(IntPtr handle);

        [DllImport("kernel32", EntryPoint = "GetProcAddress", CallingConvention = CallingConvention.Winapi,
            SetLastError = true)]
        private static extern IntPtr WindowsGetProcAddress(IntPtr handle, string procedureName);
        #endregion

        #region WindowsGetLastError
        private static int WindowsGetLastError()
        {
            return Marshal.GetLastWin32Error();
        }
        #endregion
    }
}