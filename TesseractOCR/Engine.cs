//
// TesseractEngine.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2022 Kees van Spelde
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
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Extensions.Logging;
using TesseractOCR.Exceptions;
using TesseractOCR.Enums;
using TesseractOCR.Helpers;
using TesseractOCR.Interop;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace TesseractOCR
{
    /// <summary>
    ///     The tesseract OCR engine
    /// </summary>
    public class Engine : DisposableBase
    {
        #region Fields
        private HandleRef _handle;
        private int _processCount;
        internal HandleRef Handle => _handle;
        #endregion

        #region Properties
        /// <summary>
        ///     Returns the Tesseract version
        /// </summary>
        public string Version => TessApi.BaseApiGetVersion();

        /// <summary>
        ///     Gets or sets default <see cref="PageSegMode" /> mode used by one of the Process methods
        /// </summary>
        public PageSegMode DefaultPageSegMode { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> using the <see cref="EngineMode.Default" /> mode.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        public Engine(string dataPath, Language language) : this(dataPath, language, EngineMode.Default, Array.Empty<string>(), new Dictionary<string, object>(), false)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> using the <see cref="EngineMode.Default" /> mode.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="logger">When set then logging is written to this <see cref="ILogger"/> interface</param>
        public Engine(string dataPath, Language language, ILogger logger) : this(dataPath, language, EngineMode.Default, Array.Empty<string>(), new Dictionary<string, object>(), false, logger)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="configFile" />
        ///     using the <see cref="EngineMode.Default">Default Engine Mode</see>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        ///     <para>
        ///         Note: That the config files MUST be encoded without the BOM using unix end of line characters.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="configFile">
        ///     An optional tesseract configuration file that is encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        public Engine(string dataPath, Language language, string configFile) : this(dataPath, language, EngineMode.Default, configFile != null ? new[] { configFile } : Array.Empty<string>(), new Dictionary<string, object>(), false)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="configFile" />
        ///     using the <see cref="EngineMode.Default">Default Engine Mode</see>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        ///     <para>
        ///         Note: That the config files MUST be encoded without the BOM using unix end of line characters.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="configFile">
        ///     An optional tesseract configuration file that is encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        /// <param name="logger">When set then logging is written to this <see cref="ILogger"/> interface</param>
        public Engine(string dataPath, Language language, string configFile, ILogger logger) : this(dataPath, language, EngineMode.Default, configFile != null ? new[] { configFile } : Array.Empty<string>(), new Dictionary<string, object>(), false, logger)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="configFiles" />
        ///     using the <see cref="EngineMode.Default">Default Engine Mode</see>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="configFiles">
        ///     An optional sequence of tesseract configuration files to load, encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        public Engine(string dataPath, Language language, IEnumerable<string> configFiles) : this(dataPath, language, EngineMode.Default, configFiles, new Dictionary<string, object>(), false)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="configFiles" />
        ///     using the <see cref="EngineMode.Default">Default Engine Mode</see>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="configFiles">
        ///     An optional sequence of tesseract configuration files to load, encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        /// <param name="logger">When set then logging is written to this <see cref="ILogger"/> interface</param>
        public Engine(string dataPath, Language language, IEnumerable<string> configFiles, ILogger logger) : this(dataPath, language, EngineMode.Default, configFiles, new Dictionary<string, object>(), false, logger)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="engineMode" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="engineMode">The <see cref="EngineMode" /> value to use when initializing the tesseract engine.</param>
        public Engine(string dataPath, Language language, EngineMode engineMode) : this(dataPath, language, engineMode, Array.Empty<string>(), new Dictionary<string, object>(), false)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="engineMode" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="engineMode">The <see cref="EngineMode" /> value to use when initializing the tesseract engine.</param>
        /// <param name="logger">When set then logging is written to this <see cref="ILogger"/> interface</param>
        public Engine(string dataPath, Language language, EngineMode engineMode, ILogger logger) : this(dataPath, language, engineMode, Array.Empty<string>(), new Dictionary<string, object>(), false, logger)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="engineMode" /> and
        ///     <paramref name="configFile" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        ///     <para>
        ///         Note: That the config files MUST be encoded without the BOM using unix end of line characters.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="engineMode">The <see cref="EngineMode" /> value to use when initializing the tesseract engine.</param>
        /// <param name="configFile">
        ///     An optional tesseract configuration file that is encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        public Engine(string dataPath, Language language, EngineMode engineMode, string configFile) : this(dataPath, language, engineMode, configFile != null ? new[] { configFile } : Array.Empty<string>(), new Dictionary<string, object>(), false)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="engineMode" /> and
        ///     <paramref name="configFile" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        ///     <para>
        ///         Note: That the config files MUST be encoded without the BOM using unix end of line characters.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="engineMode">The <see cref="EngineMode" /> value to use when initializing the tesseract engine.</param>
        /// <param name="configFile">
        ///     An optional tesseract configuration file that is encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        /// <param name="logger">When set then logging is written to this <see cref="ILogger"/> interface</param>
        public Engine(string dataPath, Language language, EngineMode engineMode, string configFile, ILogger logger) : this(dataPath, language, engineMode, configFile != null ? new[] { configFile } : Array.Empty<string>(), new Dictionary<string, object>(), false, logger)
        {
        }


        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="engineMode" /> and
        ///     <paramref name="configFiles" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="engineMode">The <see cref="EngineMode" /> value to use when initializing the tesseract engine.</param>
        /// <param name="configFiles">
        ///     An optional sequence of tesseract configuration files to load, encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        public Engine(string dataPath, Language language, EngineMode engineMode, IEnumerable<string> configFiles) : this(dataPath, language, engineMode, configFiles, new Dictionary<string, object>(), false)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="engineMode" /> and
        ///     <paramref name="configFiles" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="engineMode">The <see cref="EngineMode" /> value to use when initializing the tesseract engine.</param>
        /// <param name="configFiles">
        ///     An optional sequence of tesseract configuration files to load, encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        /// <param name="logger">When set then logging is written to this <see cref="ILogger"/> interface</param>
        public Engine(string dataPath, Language language, EngineMode engineMode, IEnumerable<string> configFiles, ILogger logger) : this(dataPath, language, engineMode, configFiles, new Dictionary<string, object>(), false, logger)
        {
        }


        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="engineMode" /> and
        ///     <paramref name="configFiles" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="engineMode">The <see cref="EngineMode" /> value to use when initializing the tesseract engine</param>
        /// <param name="configFiles">
        ///     An optional sequence of tesseract configuration files to load, encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        /// <param name="initialOptions"></param>
        /// <param name="setOnlyNonDebugVariables"></param>
        public Engine(string dataPath, Language language, EngineMode engineMode, IEnumerable<string> configFiles, IDictionary<string, object> initialOptions, bool setOnlyNonDebugVariables)
        {
            DefaultPageSegMode = PageSegMode.Auto;
            _handle = new HandleRef(this, TessApi.Native.BaseApiCreate());

            Initialize(dataPath, language, engineMode, configFiles, initialOptions, setOnlyNonDebugVariables);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Engine" /> with the specified <paramref name="engineMode" /> and
        ///     <paramref name="configFiles" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="dataPath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="dataPath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="dataPath">
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the
        ///     <c>TESSDATA_PREFIX</c> environment variable is defined.
        /// </param>
        /// <param name="language">The <see cref="Language"/> to load</param>
        /// <param name="engineMode">The <see cref="EngineMode" /> value to use when initializing the tesseract engine</param>
        /// <param name="configFiles">
        ///     An optional sequence of tesseract configuration files to load, encoded using UTF8 without BOM
        ///     with Unix end of line characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </param>
        /// <param name="initialOptions"></param>
        /// <param name="setOnlyNonDebugVariables"></param>
        /// <param name="logger">When set then logging is written to this <see cref="ILogger"/> interface</param>
        public Engine(string dataPath, Language language, EngineMode engineMode, IEnumerable<string> configFiles, IDictionary<string, object> initialOptions, bool setOnlyNonDebugVariables, ILogger logger)
        {
            DefaultPageSegMode = PageSegMode.Auto;
            _handle = new HandleRef(this, TessApi.Native.BaseApiCreate());

            if (logger != null)
                Logger.LoggerInterface = logger;

            Initialize(dataPath, language, engineMode, configFiles, initialOptions, setOnlyNonDebugVariables);
        }
        #endregion

        #region Process
        /// <summary>
        ///     Processes the specific image.
        /// </summary>
        /// <remarks>
        ///     You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="pageSegMode">The page layout analysis method to use.</param>
        public Page Process(Pix.Image image, PageSegMode? pageSegMode = null)
        {
            return Process(image, null, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        ///     Processes a specified region in the image using the specified page layout analysis mode.
        /// </summary>
        /// <remarks>
        ///     You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="region">The image region to process.</param>
        /// <param name="pageSegMode">The page layout analysis method to use.</param>
        /// <returns>A result iterator</returns>
        public Page Process(Pix.Image image, Rect region, PageSegMode? pageSegMode = null)
        {
            return Process(image, null, region, pageSegMode);
        }

        /// <summary>
        ///     Processes the specific image.
        /// </summary>
        /// <remarks>
        ///     You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="pageSegMode">The page layout analysis method to use.</param>
        public Page Process(Pix.Image image, string inputName, PageSegMode? pageSegMode = null)
        {
            return Process(image, inputName, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        ///     Processes a specified region in the image using the specified page layout analysis mode
        /// </summary>
        /// <remarks>
        ///     You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file</param>
        /// <param name="region">The image region to process</param>
        /// <param name="pageSegMode">The page layout analysis method to use.</param>
        /// <returns>A result iterator</returns>
        public Page Process(Pix.Image image, string inputName, Rect region, PageSegMode? pageSegMode = null)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            if (region.X1 < 0 || region.Y1 < 0 || region.X2 > image.Width || region.Y2 > image.Height)
            {
                const string message = "The image region to be processed must be within the image bounds";
                Logger.LogError(message);
                throw new ArgumentException(message, nameof(region));
            }

            if (_processCount > 0)
                throw new InvalidOperationException("Only one image can be processed at once. Please make sure you dispose of the page once your finished with it");

            _processCount++;

            var actualPageSegmentMode = pageSegMode ?? DefaultPageSegMode;
            TessApi.Native.BaseApiSetPageSegMode(_handle, actualPageSegmentMode);
            TessApi.Native.BaseApiSetImage(_handle, image.Handle);
            
            if (!string.IsNullOrEmpty(inputName)) 
                TessApi.Native.BaseApiSetInputName(_handle, inputName);
            
            var page = new Page(this, image, inputName, region, actualPageSegmentMode, 1);
            return page;
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (_handle.Handle == IntPtr.Zero) return;
            TessApi.Native.BaseApiDelete(_handle);
            _handle = new HandleRef(this, IntPtr.Zero);
        }
        #endregion

        #region GetTessDataPrefix
        private string GetTessDataPrefix()
        {
            try
            {
                return Environment.GetEnvironmentVariable("TESSDATA_PREFIX");
            }
            catch (SecurityException exception)
            {
                Logger.LogError($"Failed to detect if the environment variable 'TESSDATA_PREFIX' is set: {exception.Message}");
                return null;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        ///     Initializes the Tesseract engine
        /// </summary>
        /// <param name="dataPath">The language data path (e.g. tessdata)</param>
        /// <param name="language">The language to use</param>
        /// <param name="engineMode"><see cref="EngineMode"/></param>
        /// <param name="configFiles">The config files to load</param>
        /// <param name="initialValues"></param>
        /// <param name="setOnlyNonDebugVariables"></param>
        /// <exception cref="TesseractException"></exception>
        private void Initialize(string dataPath, Language language, EngineMode engineMode, IEnumerable<string> configFiles, IDictionary<string, object> initialValues, bool setOnlyNonDebugVariables)
        {
            // Do some minor processing on datapath to fix some common errors (this basically mirrors what tesseract does as of 3.04)
            if (!string.IsNullOrEmpty(dataPath))
            {
                // Remove any excess whitespace
                dataPath = dataPath.Trim();

                dataPath = dataPath.TrimEnd('/');
                dataPath = dataPath.TrimEnd('\\');
            }

            var languageString = LanguageHelper.EnumToString(language);

            Logger.LogInformation($"Initializing Tesseract engine, using data path '{dataPath}', language '{languageString}' and engine mode '{engineMode}'");

            if (setOnlyNonDebugVariables)
                Logger.LogInformation("Setting only no debug variables");

            if (TessApi.BaseApiInit(_handle, dataPath, languageString, (int)engineMode,
                    configFiles ?? new List<string>(),
                    initialValues ?? new Dictionary<string, object>(), setOnlyNonDebugVariables) == Constants.False)
            {
                Logger.LogInformation("Tesseract engine initialized");
                return;
            }

            // Special case logic to handle cleaning up as init has already released the handle if it fails
            _handle = new HandleRef(this, IntPtr.Zero);
            GC.SuppressFinalize(this);


            const string message = "Failed to initialize Tesseract engine";
            Logger.LogInformation(message);
            throw new TesseractException(message);
        }
        #endregion

        #region Public class PageDisposalHandle
        /// <summary>
        ///     Ties the specified pix to the lifecycle of a page.
        /// </summary>
        public class PageDisposalHandle
        {
            #region Fields
            private readonly Page _page;
            private readonly Pix.Image _pix;
            #endregion

            #region PageDisposalHandle
            public PageDisposalHandle(Page page, Pix.Image pix)
            {
                _page = page;
                _pix = pix;
                page.Disposed += OnPageDisposed;
            }
            #endregion

            #region OnPageDisposed
            private void OnPageDisposed(object sender, EventArgs e)
            {
                _page.Disposed -= OnPageDisposed;
                // dispose the pix when the page is disposed.
                _pix.Dispose();
            }
            #endregion
        }
        #endregion

        #region BaseApiSetDebugVariable
        public bool SetDebugVariable(string name, string value)
        {
            return TessApi.BaseApiSetDebugVariable(_handle, name, value) != 0;
        }
        #endregion

        #region SetVariable
        /// <summary>
        ///     Sets the value of a string variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool SetVariable(string name, string value)
        {
            Logger.LogInformation($"Setting variable '{name}' to '{value}'");
            return TessApi.BaseApiSetVariable(_handle, name, value) != 0;
        }
        #endregion

        #region SetVariable
        /// <summary>
        ///     Sets the value of a boolean variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool SetVariable(string name, bool value)
        {
            var strEncodedValue = value ? "TRUE" : "FALSE";
            Logger.LogInformation($"Setting variable '{name}' to '{strEncodedValue}'");
            return TessApi.BaseApiSetVariable(_handle, name, strEncodedValue) != 0;
        }

        /// <summary>
        ///     Sets the value of a integer variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool SetVariable(string name, int value)
        {
            var strEncodedValue = value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
            Logger.LogInformation($"Setting variable '{name}' to '{strEncodedValue}'");
            return TessApi.BaseApiSetVariable(_handle, name, strEncodedValue) != 0;
        }

        /// <summary>
        ///     Sets the value of a double variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool SetVariable(string name, double value)
        {
            var strEncodedValue = value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
            Logger.LogInformation($"Setting variable '{name}' to '{strEncodedValue}'");
            return TessApi.BaseApiSetVariable(_handle, name, strEncodedValue) != 0;
        }
        #endregion

        #region TryGetBoolVariable
        /// <summary>
        ///     Attempts to retrieve the value for a boolean variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns><c>true</c> if successful, otherwise <c>false</c>.</returns>
        public bool TryGetBoolVariable(string name, out bool value)
        {
            if (TessApi.Native.BaseApiGetBoolVariable(_handle, name, out var val) == Constants.True)
            {
                value = val != 0;
                Logger.LogInformation($"Returned variable '{name}' with value ''{val}");
                return true;
            }

            Logger.LogError($"Could not get bool variable '{name}'");

            value = false;
            return false;
        }
        #endregion

        #region TryGetDoubleVariable
        /// <summary>
        ///     Attempts to retrieve the value for a double variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool TryGetDoubleVariable(string name, out double value)
        {
            var result = TessApi.Native.BaseApiGetDoubleVariable(_handle, name, out value) == Constants.True;

            if (result)
            {
                Logger.LogInformation($"Returned double variable '{name}' with value ''{value}");
                return true;
            }
            
            Logger.LogError($"Could not get double variable '{name}'");
            return false;
        }
        #endregion

        #region TryGetIntVariable
        /// <summary>
        ///     Attempts to retrieve the value for an integer variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool TryGetIntVariable(string name, out int value)
        {
            var result = TessApi.Native.BaseApiGetIntVariable(_handle, name, out value) == Constants.True;

            if (result)
            {
                Logger.LogInformation($"Returned int variable '{name}' with value ''{value}");
                return true;
            }

            Logger.LogError($"Could not get int variable '{name}'");
            return false;
        }
        #endregion

        #region TryGetStringVariable
        /// <summary>
        ///     Attempts to retrieve the value for a string variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool TryGetStringVariable(string name, out string value)
        {
            value = TessApi.BaseApiGetStringVariable(_handle, name);
            
            var result = value != null;

            if (result)
            {
                Logger.LogInformation($"Returned string variable '{name}' with value ''{value}");
                return true;
            }

            Logger.LogError($"Could not get string variable '{name}'");
            return false;
        }
        #endregion

        #region TryPrintVariablesToFile
        /// <summary>
        ///     Attempts to print the variables to the file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool TryPrintVariablesToFile(string filename)
        {
            var result = TessApi.Native.BaseApiPrintVariablesToFile(_handle, filename) == Constants.True;

            if (result)
            {
                Logger.LogInformation($"Printed variable to file '{filename}'");
                return true;
            }

            Logger.LogError($"Could not print variables to file '{filename}'");
            return false;
        }
        #endregion Config
    }
}