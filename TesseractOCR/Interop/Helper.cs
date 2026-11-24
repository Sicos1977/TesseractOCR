//
// EngineMode.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2021-2027 Kees van Spelde
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
using System.IO;
using TesseractOCR.InteropDotNet;

namespace TesseractOCR.Interop;

internal static class Helper
{
    internal static void SetPath()
    {
        if (SystemManager.GetOperatingSystem() != InteropDotNet.OperatingSystem.Windows)
            return;

        var baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var platformName = SystemManager.GetPlatformName();
        var runtimeDirectory = Path.Combine(baseDirectory, "runtimes", "win-" + platformName, "native");
        var legacyDirectory = Path.Combine(baseDirectory, platformName);
        var path = Environment.GetEnvironmentVariable("path") ?? string.Empty;

        foreach (var directory in new[] { runtimeDirectory, legacyDirectory })
        {
            if (!path.Contains(directory))
                path += ";" + directory;
        }

        Environment.SetEnvironmentVariable("path", path);
    }
}
