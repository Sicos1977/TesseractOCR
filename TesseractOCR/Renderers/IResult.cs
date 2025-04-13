//
// IResult.cs
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

namespace TesseractOCR.Renderers
{
    public interface IResult : IDisposable
    {
        /// <summary>
        ///     Begins a new document with the specified <paramref name="title" />.
        /// </summary>
        /// <param name="title">The title of the new document.</param>
        /// <returns>A handle that when disposed of ends the current document.</returns>
        IDisposable BeginDocument(string title);

        /// <summary>
        ///     Add the page to the current document.
        /// </summary>
        /// <param name="page"></param>
        /// <returns><c>True</c> if the page was successfully added to the result renderer; otherwise false.</returns>
        bool AddPage(Page page);

        /// <summary>
        /// Uses internal file processing. (which is must faster than adding single pages)
        /// Should behave as if the cmd line was called. Thus will deal with different file formats out of the box
        /// 
        /// Data needs to be persisted to disk as temp file before passed down.
        /// Temp files will be deleted afterwards
        /// </summary>
        /// <param name="data">
        /// The raw bytes of the image to add. tiff, multipage tiff, jpg
        /// and all other types the cmd line call of tesseract would support
        /// </param>
        /// <param name="engine">The engine to be used with the result</param>
        /// <returns></returns>
        bool ProcessPages(byte[] data, Engine engine);
        
        /// <summary>
        /// Uses internal file processing. (which is must faster than adding single pages)
        /// Should behave as if the cmd line was called. Thus will deal with different file formats out of the box
        /// 
        /// Data needs to be persisted to disk as temp file before passed down.
        /// Temp files will be deleted afterwards
        /// </summary>
        /// <param name="imgFilePath">The path to the image file to add. tiff, multipage tiff, jpg, etc.</param>
        /// <param name="engine">The engine to be used for processing</param>
        /// <returns></returns>
        bool ProcessPages(string imgFilePath, Engine engine);

        /// <summary>
        ///     Gets the current page number; returning -1 if no page has yet been added otherwise the number
        ///     of the last added page (starting from 0).
        /// </summary>
        int PageNumber { get; }
    }
}