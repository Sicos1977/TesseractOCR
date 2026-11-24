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

namespace TesseractOCR.InteropDotNet;

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

            UnixGetLastError();
            libraryHandle = UnixLoadLibrary(fileName, RtldNow);

            if (libraryHandle != IntPtr.Zero)
                Logger.LogInformation($"Successfully loaded native library '{fileName}' with handle '{libraryHandle}'");
            else
            {
                var detail = Marshal.PtrToStringAnsi(UnixGetLastError());
                if (string.IsNullOrEmpty(detail))
                    detail = "Check the library's native dependencies and dynamic linker search path.";
                var message = $"Failed to load native library '{fileName}': {detail}";
                Logger.LogError(message);
                if (IsLinux)
                    throw new DllNotFoundException(message);
            }
        }
        catch (DllNotFoundException) when (IsLinux)
        {
            throw;
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

        try
        {
            Logger.LogDebug($"Trying to load native function '{functionName}' from the library with handle '{libraryHandle}'");
        
            var functionHandle = UnixGetProcAddress(libraryHandle, functionName);
            var errorPointer = UnixGetLastError();
        
            if (errorPointer != IntPtr.Zero)
                throw new Exception($"dlsym: {Marshal.PtrToStringAnsi(errorPointer)}");
        
            if (functionHandle != IntPtr.Zero && errorPointer == IntPtr.Zero)
                Logger.LogDebug($"Successfully loaded native function '{functionName}' with handle '{functionHandle}'");
            else
                Logger.LogError($"Failed to load native function '{functionName}', with handle '{functionHandle}', error pointer '{errorPointer}'");
        
            return functionHandle;

        }
        catch (Exception exception)
        {
            var lastError = UnixGetLastError();
            Logger.LogError($"Failed to load native function '{functionName}', last error '{lastError}', exception: {exception}");
            return IntPtr.Zero;
        }
    }
    #endregion

    #region FixUpLibraryName
    public string FixUpLibraryName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return fileName;

        if (SystemManager.GetOperatingSystem() == OperatingSystem.Unix)
        {
            if (fileName == TesseractOCR.Interop.Constants.TesseractDllName) return "libtesseract.so.5";
            if (fileName == TesseractOCR.Interop.Constants.LeptonicaDllName) return "libleptonica.so.6";
        }

        if (!fileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase))
            fileName += FileExtension;

        if (!fileName.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
            fileName = "lib" + fileName;

        return fileName;
    }
    #endregion

    #region Native methods
    private static readonly bool IsLinux = SystemManager.GetOperatingSystem() == OperatingSystem.Unix;
    private static IntPtr UnixLoadLibrary(string fileName, int flags) => IsLinux ? LinuxLoadLibrary(fileName, flags) : MacLoadLibrary(fileName, flags);
    private static int UnixFreeLibrary(IntPtr handle) => IsLinux ? LinuxFreeLibrary(handle) : MacFreeLibrary(handle);
    private static IntPtr UnixGetProcAddress(IntPtr handle, string symbol) => IsLinux ? LinuxGetProcAddress(handle, symbol) : MacGetProcAddress(handle, symbol);
    private static IntPtr UnixGetLastError() => IsLinux ? LinuxGetLastError() : MacGetLastError();

    [DllImport("libdl.so.2", EntryPoint = "dlopen")]
    private static extern IntPtr LinuxLoadLibrary(string fileName, int flags);
    [DllImport("libdl.so.2", EntryPoint = "dlclose")]
    private static extern int LinuxFreeLibrary(IntPtr handle);
    [DllImport("libdl.so.2", EntryPoint = "dlsym")]
    private static extern IntPtr LinuxGetProcAddress(IntPtr handle, string symbol);
    [DllImport("libdl.so.2", EntryPoint = "dlerror")]
    private static extern IntPtr LinuxGetLastError();

    [DllImport("libdl", EntryPoint = "dlopen")]
    private static extern IntPtr MacLoadLibrary(string fileName, int flags);
    [DllImport("libdl", EntryPoint = "dlclose")]
    private static extern int MacFreeLibrary(IntPtr handle);
    [DllImport("libdl", EntryPoint = "dlsym")]
    private static extern IntPtr MacGetProcAddress(IntPtr handle, string symbol);
    [DllImport("libdl", EntryPoint = "dlerror")]
    private static extern IntPtr MacGetLastError();
    #endregion
}