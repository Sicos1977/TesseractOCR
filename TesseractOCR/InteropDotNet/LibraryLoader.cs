﻿//
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

namespace TesseractOCR.InteropDotNet
{
    /// <summary>
    ///     Loads the needed library
    /// </summary>
    public class LibraryLoader
    {
        #region Fields
        private readonly ILibraryLoaderLogic _logic;
        private readonly object _syncLock = new object();
        private readonly Dictionary<string, IntPtr> _loadedAssemblies = new Dictionary<string, IntPtr>();
        private static LibraryLoader _instance;

        /// <summary>
        ///     Uses an alternative search path
        /// </summary>
        public string CustomSearchPath { get; set; }
        #endregion

        /// <summary>
        ///     A singleton to the <see cref="LibraryLoader"/> class
        /// </summary>
        #region Properties
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

        private IntPtr CheckCustomSearchPath(string fileName, string platformName)
        {
            var baseDirectory = CustomSearchPath;
            if (!IsNullOrEmpty(baseDirectory))
            {
                Logger.LogInformation($"Checking custom search location '{baseDirectory}' for '{fileName}' on platform {platformName}.");
                return InternalLoadLibrary(baseDirectory, platformName, fileName);
            }
            else
            {
                Logger.LogInformation("Custom search path is not defined, skipping.");
                return IntPtr.Zero;
            }
        }


        #region InternalLoadLibrary
        private IntPtr InternalLoadLibrary(string baseDirectory, string platformName, string fileName)
        {
            var fullPath = Path.Combine(baseDirectory, Path.Combine(platformName, fileName));

            Logger.LogDebug($"Trying to load file from '{fullPath}'");

            return
                File.Exists(fullPath) ? 
                    _logic.LoadLibrary(fullPath) : 
                    IntPtr.Zero;
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
        private string FixUpLibraryName(string fileName)
        {
            return _logic.FixUpLibraryName(fileName);
        }
        #endregion
    }
}