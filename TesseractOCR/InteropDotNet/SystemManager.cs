//
// SystemManager.cs
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

namespace TesseractOCR.InteropDotNet
{
    #region Internal enum OperatingSystem
    internal enum OperatingSystem
    {
        Windows,

        Unix,

        MacOSX,

        Unknown
    }
    #endregion

    internal static class SystemManager
    {
        #region GetPlatformName
        public static string GetPlatformName()
        {
            return IntPtr.Size == sizeof(int) ? "x86" : "x64";
        }
        #endregion

        #region GetOperatingSystem
        public static OperatingSystem GetOperatingSystem()
        {
#if(NETCORE || NETSTANDARD)
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OperatingSystem.Windows;
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OperatingSystem.Unix;
            if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OperatingSystem.MacOSX;
            
            return OperatingSystem.Unknown;
#else
            var pid = (int)Environment.OSVersion.Platform;
            switch (pid)
            {
                case (int)PlatformID.Win32NT:
                case (int)PlatformID.Win32S:
                case (int)PlatformID.Win32Windows:
                case (int)PlatformID.WinCE:
                    return OperatingSystem.Windows;

                case (int)PlatformID.Unix:
                case 128:
                    return OperatingSystem.Unix;

                case (int)PlatformID.MacOSX:
                    return OperatingSystem.MacOSX;

                default:
                    return OperatingSystem.Unknown;
            }
#endif
        }
        #endregion
    }
}