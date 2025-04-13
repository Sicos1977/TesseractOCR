//
// AggregateResultRenderer.cs
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
using System.Linq;
using TesseractOCR.Helpers;
using TesseractOCR.Internal;

namespace TesseractOCR.Renderers
{
    /// <summary>
    ///     Aggregate result renderer.
    /// </summary>
    public class AggregateResult : DisposableBase, IResult
    {
        #region Fields
        private IDisposable _currentDocumentHandle;
        private List<IResult> _resultRenderers;
        #endregion

        #region Properties
        /// <summary>
        ///     Get's the child result renderers.
        /// </summary>
        public IEnumerable<IResult> ResultRenderers => _resultRenderers;

        /// <summary>
        ///     Get's the current page number.
        /// </summary>
        public int PageNumber { get; private set; } = -1;
        #endregion

        #region Constructors
        /// <summary>
        ///     Create a new aggregate result renderer with the specified child result renderers.
        /// </summary>
        /// <param name="resultRenderers">The child result renderers.</param>
        public AggregateResult(params IResult[] resultRenderers) : this((IEnumerable<IResult>)resultRenderers)
        {
        }

        /// <summary>
        ///     Create a new aggregate result renderer with the specified child result renderers.
        /// </summary>
        /// <param name="resultRenderers">The child result renderers.</param>
        public AggregateResult(IEnumerable<IResult> resultRenderers)
        {
            Guard.RequireNotNull("resultRenderers", resultRenderers);

            _resultRenderers = new List<IResult>(resultRenderers);
        }
        #endregion

        #region AddPage
        /// <summary>
        ///     Adds a page to each of the child result renderers.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool AddPage(Page page)
        {
            Guard.RequireNotNull("page", page);
            VerifyNotDisposed();

            PageNumber++;

            return ResultRenderers.All(m => m.AddPage(page));
        }

        /// <inheritdoc />
        public bool ProcessPages(byte[] data, Engine engine)
        {
            Guard.RequireNotNull("data", data);
            Guard.RequireNotNull("engine", engine);
            VerifyNotDisposed();

            return ResultRenderers.All(m => m.ProcessPages(data, engine));
        }

        /// <inheritdoc />
        public bool ProcessPages(string imgFilePath, Engine engine)
        {
            Guard.RequireNotNull("imgFilePath", imgFilePath);
            Guard.RequireNotNull("engine", engine);
            VerifyNotDisposed();

            return ResultRenderers.All(m => m.ProcessPages(imgFilePath, engine));
        }

        #endregion

        #region BeginDocument
        /// <summary>
        ///     Begins a new document with the specified title.
        /// </summary>
        /// <param name="title">The title of the document.</param>
        /// <returns></returns>
        public IDisposable BeginDocument(string title)
        {
            Guard.RequireNotNull("title", title);
            VerifyNotDisposed();
            Guard.Verify(_currentDocumentHandle == null, "Cannot begin document \"{0}\" as another document is currently being processed which must be dispose off first.", title);

            // Reset the page number
            PageNumber = -1;

            // Begin the document on each child renderer.
            var children = new List<IDisposable>();

            try
            {
                Logger.LogInformation("Begin document");
                children.AddRange(ResultRenderers.Select(m => m.BeginDocument(title)));
                _currentDocumentHandle = new EndDocumentOnDispose(this, children);

                return _currentDocumentHandle;
            }
            catch (Exception)
            {
                // Dispose of all previously created child document's iff an error occurred to prevent a memory leak.
                foreach (var child in children)
                    try
                    {
                        child.Dispose();
                    }
                    catch (Exception disposalError)
                    {
                        Logger.LogError($"Failed to dispose of child document {child}, error {disposalError}");
                    }

                throw;
            }
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
                // dispose of result renderers
                foreach (var renderer in ResultRenderers) renderer.Dispose();
                _resultRenderers = null;
            }
        }
        #endregion

        #region Private class EndDocumentOnDispose
        /// <summary>
        ///     Ensures the renderer's EndDocument when disposed off.
        /// </summary>
        private class EndDocumentOnDispose : DisposableBase
        {
            private readonly AggregateResult _renderer;
            private List<IDisposable> _children;

            public EndDocumentOnDispose(AggregateResult renderer, IEnumerable<IDisposable> children)
            {
                _renderer = renderer;
                _children = new List<IDisposable>(children);
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposing) return;

                Guard.Verify(Equals(_renderer._currentDocumentHandle, this), "Expected the Result Render's active document to be this document");

                // End the renderer
                foreach (var child in _children)
                    child.Dispose();

                _children = null;

                // Reset current handle
                _renderer._currentDocumentHandle = null;
            }
        }
        #endregion
    }
}