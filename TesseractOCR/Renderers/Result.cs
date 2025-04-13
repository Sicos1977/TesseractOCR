//
// ResultIterator.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2025 Kees van Spelde
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
using System.IO;
using System.Runtime.InteropServices;
using TesseractOCR.Enums;
using TesseractOCR.Internal;
using TesseractOCR.Interop;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Represents a native result renderer (e.g. text, pdf, etc).
    /// </summary>
    /// <remarks>
    ///     Note that the ResultRenderer is explicitly responsible for managing the
    ///     renderer hierarchy. This gets around a number of difficult issues such
    ///     as keeping track of what the next renderer is and how to manage the memory.
    /// </remarks>
    public abstract class Result : DisposableBase, IResult
    {
        #region Fields
        private HandleRef _handle;
        private IDisposable _currentDocumentHandle;
        #endregion

        #region Properties
        protected HandleRef Handle => _handle;

        /// <summary>
        ///     Returns the page number
        /// </summary>
        public int PageNumber
        {
            get
            {
                VerifyNotDisposed();

                return TessApi.Native.ResultRendererImageNum(Handle);
            }
        }
        #endregion

        #region Constructor
        protected Result()
        {
            _handle = new HandleRef(this, IntPtr.Zero);
        }
        #endregion

        #region AddPage
        /// <summary>
        ///     Add the page to the current document.
        /// </summary>
        /// <param name="page"></param>
        /// <returns><c>True</c> if the page was successfully added to the result renderer; otherwise false.</returns>
        public bool AddPage(Page page)
        {
            Guard.RequireNotNull("page", page);
            VerifyNotDisposed();

            page.Recognize();

            return TessApi.Native.ResultRendererAddImage(Handle, page.Engine.Handle) != 0;
        }


        /// <inheritdoc />
        public bool ProcessPages(byte[] data, Engine engine)
        {
            var tmpFile = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tmpFile, data);
                return ProcessPages(tmpFile, engine);
            }
            finally
            {
                File.Delete(tmpFile);
            }
        }
        
        /// <inheritdoc />
        public bool ProcessPages(string imgFilePath, Engine engine)
        {
            // config and timeout are not set either when tesseract is called from cmd line
            return TessApi.Native.BaseApiProcessPages(engine.Handle, imgFilePath, null, 0, _handle) != 0;
        }
        #endregion

        #region BeginDocument
        /// <summary>
        ///     Begins a new document with the specified <paramref name="title" />.
        /// </summary>
        /// <param name="title">The (ANSI) title of the new document.</param>
        /// <returns>A handle that when disposed of ends the current document.</returns>
        public IDisposable BeginDocument(string title)
        {
            Guard.RequireNotNull("title", title);
            VerifyNotDisposed();
            Guard.Verify(_currentDocumentHandle == null,
                "Cannot begin document \"{0}\" as another document is currently being processed which must be dispose off first.",
                title);

            var titlePtr = Marshal.StringToHGlobalAnsi(title);

            if (TessApi.Native.ResultRendererBeginDocument(Handle, titlePtr) == 0)
            {
                // Release the pointer first before throwing an error.
                Marshal.FreeHGlobal(titlePtr);

                throw new InvalidOperationException($"Failed to begin document \"{title}\".");
            }

            _currentDocumentHandle = new EndDocumentOnDispose(this, titlePtr);
            return _currentDocumentHandle;
        }
        #endregion

        #region IEnumerable
        /// <summary>
        ///     Creates renderers for specified output formats.
        /// </summary>
        /// <param name="outputbase"></param>
        /// <param name="dataPath">The directory containing the pdf font data, normally same as your tessdata directory.</param>
        /// <param name="outputFormats"></param>
        /// <returns></returns>
        public static IEnumerable<IResult> CreateRenderers(string outputbase, string dataPath,
            List<RenderFormat> outputFormats)
        {
            var renderers = new List<IResult>();

            foreach (var format in outputFormats)
            {
                IResult renderer = null;

                switch (format)
                {
                    case RenderFormat.Text:
                        renderer = CreateTextRenderer(outputbase);
                        break;

                    case RenderFormat.Hocr:
                        renderer = CreateHOcrRenderer(outputbase);
                        break;

                    case RenderFormat.Pdf:
                    case RenderFormat.PdfTextonly:
                        var textonly = format == RenderFormat.PdfTextonly;
                        renderer = CreatePdfRenderer(outputbase, dataPath, textonly);
                        break;

                    case RenderFormat.Box:
                        renderer = CreateBoxRenderer(outputbase);
                        break;

                    case RenderFormat.Unlv:
                        renderer = CreateUnlvRenderer(outputbase);
                        break;

                    case RenderFormat.Alto:
                        renderer = CreateAltoRenderer(outputbase);
                        break;

                    case RenderFormat.Tsv:
                        renderer = CreateTsvRenderer(outputbase);
                        break;

                    case RenderFormat.LstmBox:
                        renderer = CreateLstmBoxRenderer(outputbase);
                        break;

                    case RenderFormat.WordStrBox:
                        renderer = CreateWordStrBoxRenderer(outputbase);
                        break;
                }

                renderers.Add(renderer);
            }

            return renderers;
        }
        #endregion

        #region CreateTextRenderer
        /// <summary>
        ///     Creates a <see cref="IResult">result renderer</see> that render that generates UTF-8 encoded text
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the text file to be generated without the file extension.</param>
        /// <returns></returns>
        public static IResult CreateTextRenderer(string outputFilename)
        {
            return new TextResult(outputFilename);
        }
        #endregion

        #region CreateHOcrRenderer
        /// <summary>
        ///     Creates a <see cref="IResult">result renderer</see> that render that generates a HOCR
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the hocr file to be generated without the file extension.</param>
        /// <param name="fontInfo">Determines if the generated HOCR file includes font information or not.</param>
        /// <returns></returns>
        public static IResult CreateHOcrRenderer(string outputFilename, bool fontInfo = false)
        {
            return new HOcrResult(outputFilename, fontInfo);
        }
        #endregion

        #region CreatePdfRenderer
        /// <summary>
        ///     Creates a <see cref="IResult">result renderer</see> that render that generates a searchable
        ///     pdf file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The filename of the pdf file to be generated without the file extension.</param>
        /// <param name="fontDirectory">The directory containing the pdf font data, normally same as your tessdata directory.</param>
        /// <param name="textonly">skip images if set</param>
        /// <returns></returns>
        public static IResult CreatePdfRenderer(string outputFilename, string fontDirectory, bool textonly)
        {
            return new PdfResult(outputFilename, fontDirectory, textonly);
        }
        #endregion

        #region CreateUnlvRenderer
        /// <summary>
        ///     Creates a <see cref="IResult">result renderer</see> that render that generates a unlv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResult CreateUnlvRenderer(string outputFilename)
        {
            return new UnlvResult(outputFilename);
        }
        #endregion

        #region CreateBoxRenderer
        /// <summary>
        ///     Creates a <see cref="IResult">result renderer</see> that render that generates a box text file from
        ///     tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the box file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResult CreateBoxRenderer(string outputFilename)
        {
            return new BoxResult(outputFilename);
        }
        #endregion

        #region CreateAltoRenderer
        /// <summary>
        ///     Creates a <see cref="IResult">result renderer</see> that render that generates an Alto
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the Alto file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResult CreateAltoRenderer(string outputFilename)
        {
            return new AltoResult(outputFilename);
        }
        #endregion

        #region CreateTsvRenderer
        /// <summary>
        ///     Creates a <see cref="IResult">result renderer</see> that render that generates a Tsv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the Tsv file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResult CreateTsvRenderer(string outputFilename)
        {
            return new TsvResult(outputFilename);
        }
        #endregion

        #region CreateLstmBoxRenderer
        /// <summary>
        ///     Creates a <see cref="IResult">result renderer</see> that render that generates a box
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResult CreateLstmBoxRenderer(string outputFilename)
        {
            return new LstmBoxResult(outputFilename);
        }
        #endregion

        #region CreateWordStrBoxRenderer
        /// <summary>
        ///     Creates a <see cref="IResult">result renderer</see> that render that generates a unlv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResult CreateWordStrBoxRenderer(string outputFilename)
        {
            return new WordStrBoxResult(outputFilename);
        }
        #endregion

        #region Initialize
        /// <summary>
        ///     Initialize the render to use the specified native result renderer.
        /// </summary>
        /// <param name="handle"></param>
        protected void Initialize(IntPtr handle)
        {
            Guard.Require("handle", handle != IntPtr.Zero, "Handle must be initialized");
            Guard.Verify(_handle.Handle == IntPtr.Zero, "Result renderer has already been initialized");

            _handle = new HandleRef(this, handle);
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!disposing) return;
                // Ensure that if the renderer has an active document when disposed it too is disposed off.
                if (_currentDocumentHandle == null) return;
                _currentDocumentHandle.Dispose();
                _currentDocumentHandle = null;
            }
            finally
            {
                if (_handle.Handle != IntPtr.Zero)
                {
                    TessApi.Native.DeleteResultRenderer(_handle);
                    _handle = new HandleRef(this, IntPtr.Zero);
                }
            }
        }
        #endregion

        #region Private class EndDocumentOnDispose
        /// <summary>
        ///     Ensures the renderer's EndDocument when disposed off.
        /// </summary>
        private class EndDocumentOnDispose : DisposableBase
        {
            #region Constructor
            public EndDocumentOnDispose(Result renderer, IntPtr titlePtr)
            {
                _renderer = renderer;
                _titlePtr = titlePtr;
            }
            #endregion

            #region Dispose
            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (!disposing) return;
                    Guard.Verify(Equals(_renderer._currentDocumentHandle, this),
                        "Expected the Result Render's active document to be this document.");

                    // End the renderer
                    TessApi.Native.ResultRendererEndDocument(_renderer._handle);
                    _renderer._currentDocumentHandle = null;
                }
                finally
                {
                    // free title ptr
                    if (_titlePtr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(_titlePtr);
                        _titlePtr = IntPtr.Zero;
                    }
                }
            }
            #endregion

            #region Fields
            private readonly Result _renderer;
            private IntPtr _titlePtr;
            #endregion
        }
        #endregion
    }
}