//
// LibraryLoader.cs
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TesseractOCR.Helpers;
using static System.String;
using File = System.IO.File;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.InteropDotNet;

/// <summary>
///     Loads the needed library
/// </summary>
public class LibraryLoader
{
    #region Fields
    /// <summary>
    ///     Provides the library loader logic.
    /// </summary>
    private readonly ILibraryLoaderLogic _logic;

    /// <summary>
    ///     Object used to synchronize access to shared state.
    /// </summary>
    private readonly object _syncLock = new();

    /// <summary>
    ///     Maps assembly names to their loaded handles.
    /// </summary>
    private readonly Dictionary<string, IntPtr> _loadedAssemblies = new();

    /// <summary>
    ///     The singleton instance of the library loader.
    /// </summary>
    private static LibraryLoader _instance;

    /// <summary>
    ///     Uses an alternative search path
    /// </summary>
    public string CustomSearchPath { get; set; }
    #endregion

    #region Properties
    /// <summary>
    ///     A singleton to the <see cref="LibraryLoader"/> class
    /// </summary>
    public static LibraryLoader Instance
    {
        get
        {
            if (_instance != null) return _instance;

            var operatingSystem = SystemManager.GetOperatingSystem();

            const string notSupported = "Unsupported operation system";

            switch (operatingSystem)
            {
                case OperatingSystem.Windows:
                    Logger.LogInformation("Current OS is Windows");
                    _instance = new LibraryLoader(new WindowsLibraryLoaderLogic());
                    break;

                case OperatingSystem.Unix:
                    Logger.LogInformation("Current OS is Unix");
                    _instance = new LibraryLoader(new UnixLibraryLoaderLogic());
                    break;

                case OperatingSystem.MacOSX:
                    Logger.LogInformation("Current OS is MacOSX");
                    _instance = new LibraryLoader(new UnixLibraryLoaderLogic());
                    break;

                case OperatingSystem.Unknown:
                default:
                    Logger.LogError(notSupported);
                    throw new NotSupportedException(notSupported);
            }

            return _instance;
        }
    }
    #endregion

    #region Constructor
    private LibraryLoader(ILibraryLoaderLogic logic)
    {
        _logic = logic;
    }
    #endregion

    #region LoadLibrary
    /// <summary>
    ///     Loads the library and returns a pointer to it
    /// </summary>
    /// <param name="fileName">The full path to the library to load</param>
    /// <param name="platformName">The name of the platform</param>
    /// <returns></returns>
    /// <exception cref="DllNotFoundException"></exception>
    public IntPtr LoadLibrary(string fileName, string platformName = null)
    {
        fileName = FixUpLibraryName(fileName);
        lock (_syncLock)
        {
            if (_loadedAssemblies.TryGetValue(fileName, out var library)) 
                return library;
            
            if (platformName == null)
                platformName = SystemManager.GetPlatformName();
            
            Logger.LogInformation($"Current platform is {platformName}");

            var dllHandle = CheckCodeBase(fileName, platformName);
            
            if (dllHandle == IntPtr.Zero)
                dllHandle = CheckExecutingAssemblyDomain(fileName, platformName);
            
            if (dllHandle == IntPtr.Zero)
                dllHandle = CheckCurrentAppDomain(fileName, platformName);
            
            if (dllHandle == IntPtr.Zero)
                dllHandle = CheckWorkingDirectory(fileName, platformName);

            if (dllHandle == IntPtr.Zero)
                dllHandle = CheckCustomSearchPath(fileName, platformName);

            if (dllHandle != IntPtr.Zero)
                _loadedAssemblies[fileName] = dllHandle;
            else
            {
                var error = $"Failed to find library '{fileName}' for platform {platformName}";
                Logger.LogError(error);
                throw new DllNotFoundException(error);
            }

            return _loadedAssemblies[fileName];
        }
    }
    #endregion

    #region CheckCodeBase
    private IntPtr CheckCodeBase(string fileName, string platformName)
    {
        if (SystemManager.GetOperatingSystem() == OperatingSystem.Unix)
            return IntPtr.Zero;

        var assemblyLocation = Assembly.GetExecutingAssembly().GetName().CodeBase;

        if (IsNullOrEmpty(assemblyLocation))
        {
            Logger.LogInformation("Code base was empty");
            return IntPtr.Zero;
        }

        assemblyLocation = assemblyLocation.Substring(8);
        var baseDirectory = Path.GetDirectoryName(assemblyLocation);
        return InternalLoadLibrary(baseDirectory, platformName, fileName);
    }
    #endregion

    #region CheckExecutingAssemblyDomain
    private IntPtr CheckExecutingAssemblyDomain(string fileName, string platformName)
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;

        if (IsNullOrEmpty(assemblyLocation))
        {
            Logger.LogInformation("Executing assembly location was empty");
            return IntPtr.Zero;
        }

        var baseDirectory = Path.GetDirectoryName(assemblyLocation);
        return InternalLoadLibrary(baseDirectory, platformName, fileName);
    }
    #endregion

    #region CheckCurrentAppDomain
    private IntPtr CheckCurrentAppDomain(string fileName, string platformName)
    {
        var appBase = AppDomain.CurrentDomain.BaseDirectory;

        if (IsNullOrEmpty(appBase))
        {
            Logger.LogInformation("App domains current domain base was empty");
            return IntPtr.Zero;
        }

        var baseDirectory = Path.GetFullPath(appBase);
        return InternalLoadLibrary(baseDirectory, platformName, fileName);
    }
    #endregion

    #region CheckWorkingDirectory
    private IntPtr CheckWorkingDirectory(string fileName, string platformName)
    {
        var currentDirectory = Environment.CurrentDirectory;

        if (IsNullOrEmpty(currentDirectory))
        {
            Logger.LogInformation("Current directory was empty");
            return IntPtr.Zero;
        }

        var baseDirectory = Path.GetFullPath(currentDirectory);
        return InternalLoadLibrary(baseDirectory, platformName, fileName);
    }
    #endregion

    #region CheckCustomSearchPath
    /// <summary>
    /// Loads a library from the custom search path.
    /// </summary>
    /// <param name="fileName">The name of the library file to load.</param>
    /// <param name="platformName">The platform name used to locate the library.</param>
    /// <returns>A handle to the loaded library, or <c>IntPtr.Zero</c> if the custom search path is not defined or the
    /// library could not be loaded.</returns>
    private IntPtr CheckCustomSearchPath(string fileName, string platformName)
    {
        var baseDirectory = CustomSearchPath;
        if (!IsNullOrEmpty(baseDirectory))
        {
            Logger.LogInformation($"Checking custom search location '{baseDirectory}' for '{fileName}' on platform {platformName}.");
            return InternalLoadLibrary(baseDirectory, platformName, fileName);
        }

        Logger.LogInformation("Custom search path is not defined, skipping.");
        return IntPtr.Zero;
    }
    #endregion

    #region InternalLoadLibrary
    private IntPtr InternalLoadLibrary(string baseDirectory, string platformName, string fileName)
    {
        if (SystemManager.GetOperatingSystem() == OperatingSystem.Unix)
        {
            if (platformName != "x64" && platformName != "arm64")
                throw new PlatformNotSupportedException($"Linux architecture '{platformName}' is not supported.");

            var nativeDirectory = Path.Combine(baseDirectory, "runtimes", "linux-" + platformName, "native");
            var runtimePath = Path.Combine(nativeDirectory, fileName);
            if (File.Exists(runtimePath))
                return _logic.LoadLibrary(runtimePath);

            var outputPath = Path.Combine(baseDirectory, fileName);
            return File.Exists(outputPath) ? _logic.LoadLibrary(outputPath) : IntPtr.Zero;
        }

        if (SystemManager.GetOperatingSystem() == OperatingSystem.Windows)
        {
            var runtimeDirectory = Path.Combine(baseDirectory, "runtimes", "win-" + platformName, "native");
            var runtimeLibrary = ResolveLibraryPath(runtimeDirectory, fileName);
            if (runtimeLibrary != null)
                return _logic.LoadLibrary(runtimeLibrary);

            var legacyLibrary = ResolveLibraryPath(Path.Combine(baseDirectory, platformName), fileName);
            if (legacyLibrary != null)
                return _logic.LoadLibrary(legacyLibrary);

            var outputLibrary = ResolveLibraryPath(baseDirectory, fileName);
            return outputLibrary != null ? _logic.LoadLibrary(outputLibrary) : IntPtr.Zero;
        }

        var platformDirectory = Path.Combine(baseDirectory, platformName);
        var fullPath = Path.Combine(platformDirectory, fileName);

        Logger.LogDebug($"Trying to load file from '{fullPath}'");

        var libraryPath = ResolveLibraryPath(platformDirectory, fileName);

        return libraryPath != null
            ? _logic.LoadLibrary(libraryPath)
            : IntPtr.Zero;
    }
    #endregion

    #region ResolveLibraryPath
    private static string ResolveLibraryPath(string platformDirectory, string fileName)
    {
        var fullPath = Path.Combine(platformDirectory, fileName);
        if (File.Exists(fullPath))
            return fullPath;

        if (SystemManager.GetOperatingSystem() != OperatingSystem.Windows ||
            !Directory.Exists(platformDirectory) ||
            !fileName.StartsWith("leptonica-", StringComparison.OrdinalIgnoreCase) ||
            !fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            return null;

        var candidates = Directory.GetFiles(platformDirectory, "leptonica-*.dll");
        if (candidates.Length == 0)
            return null;

        Array.Sort(candidates, StringComparer.OrdinalIgnoreCase);
        var libraryPath = candidates[candidates.Length - 1];
        Logger.LogInformation($"Using native library '{Path.GetFileName(libraryPath)}' in place of '{fileName}'.");
        return libraryPath;
    }
    #endregion

    #region FreeLibrary
    /// <summary>
    ///     Frees all the resources for the library with the given <paramref name="fileName"/>
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool FreeLibrary(string fileName)
    {
        fileName = FixUpLibraryName(fileName);

        lock (_syncLock)
        {
            if (!IsLibraryLoaded(fileName))
            {
                Logger.LogError($"Failed to free library '{fileName}' because it is not loaded");
                return false;
            }

            if (!_logic.FreeLibrary(_loadedAssemblies[fileName])) return false;
            _loadedAssemblies.Remove(fileName);
            return true;
        }
    }
    #endregion

    #region GetProcAddress
    /// <summary>
    ///     Returns the pointer address for the given procedure
    /// </summary>
    /// <param name="dllHandle"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public IntPtr GetProcAddress(IntPtr dllHandle, string name)
    {
        return _logic.GetProcAddress(dllHandle, name);
    }
    #endregion

    #region IsLibraryLoaded
    /// <summary>
    ///     Returns <c>true</c> when the library with the given <paramref name="fileName"/> has been loaded
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool IsLibraryLoaded(string fileName)
    {
        fileName = FixUpLibraryName(fileName);
        lock (_syncLock)
        {
            return _loadedAssemblies.ContainsKey(fileName);
        }
    }
    #endregion

    #region FixUpLibraryName
    /// <summary>
    ///     Fixes up the library name for the current platform.
    /// </summary>
    /// <param name="fileName">The name of the library file.</param>
    /// <returns>The fixed-up library name.</returns>
    private string FixUpLibraryName(string fileName)
    {
        return _logic.FixUpLibraryName(fileName);
    }
    #endregion
}