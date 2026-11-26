//
// PdfImageExtensions.cs
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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

#if IMAGEMAGICK
namespace TesseractOCRPdfImageMagick;
#else
namespace TesseractOCRPdfSkiaSharp;
#endif

/// <summary>
///     Optional PDF recognition for TesseractOCR engines.
/// </summary>
public static class PdfExtensions
{
    #region ProcessPdf
    /// <summary>
    ///     Recognizes a PDF file and returns one text entry per page, in document order.
    /// </summary>
    /// <remarks>
    ///     Do not use the engine concurrently. Native rendering and OCR cannot be interrupted mid-page.
    /// </remarks>
    public static IReadOnlyList<string> ProcessPdf(this Engine engine, string filename, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        ValidateEngine(engine);
        return Process(engine, PdfImage.RenderPages(filename, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }
    #endregion

    #region ProcessPdf
    /// <summary>
    ///     Recognizes PDF bytes and returns one text entry per page, in document order.
    /// </summary>
    /// <remarks>
    ///     Do not use the engine concurrently. Native rendering and OCR cannot be interrupted mid-page.
    /// </remarks>
    public static IReadOnlyList<string> ProcessPdf(this Engine engine, byte[] pdf, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        ValidateEngine(engine);
        return Process(engine, PdfImage.RenderPages(pdf, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }
    #endregion

    #region ProcessPdf
    /// <summary>
    ///     Recognizes a readable, seekable PDF stream and returns text in page order. Leaves the stream open.
    /// </summary>
    /// <remarks>
    ///     Rendering reads from the beginning of the PDF and may change its position. Do not use the engine concurrently.
    ///     Native rendering and OCR cannot be interrupted mid-page.
    /// </remarks>
    public static IReadOnlyList<string> ProcessPdf(this Engine engine, Stream pdfStream, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        ValidateEngine(engine);
        return Process(engine, PdfImage.RenderPages(pdfStream, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }
    #endregion

    #region ProcessPdfAsync
    /// <summary>
    ///     Recognizes a PDF file using the pool and returns one text entry per page, in document order.
    /// </summary>
    /// <remarks>
    ///     Pages are recognized sequentially. Concurrent documents share the pool.
    ///     Waiting for an engine is asynchronous; rendering and native OCR run synchronously and cannot be interrupted mid-page.
    /// </remarks>
    public static Task<IReadOnlyList<string>> ProcessPdfAsync(this EnginePool pool, string filename, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool));
        return ProcessAsync(pool, PdfImage.RenderPages(filename, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }
    #endregion

    #region ProcessPdfAsync
    /// <summary>
    ///     Recognizes PDF bytes using the pool and returns one text entry per page, in document order.
    /// </summary>
    /// <remarks>
    ///     Pages are processed sequentially; concurrent documents share the pool. Native work cannot be interrupted mid-page.
    /// </remarks>
    public static Task<IReadOnlyList<string>> ProcessPdfAsync(this EnginePool pool, byte[] pdf, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool));
        return ProcessAsync(pool, PdfImage.RenderPages(pdf, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }
    #endregion

    #region ProcessPdfAsync
    /// <summary>
    ///     Recognizes a readable, seekable PDF stream using the pool, in page order. Leaves the stream open.
    /// </summary>
    /// <remarks>
    ///     Rendering reads from the beginning of the PDF and may change its position. Keep the stream open until completion.
    ///     Pages are processed sequentially; concurrent documents share the pool. Native work cannot be interrupted mid-page.
    /// </remarks>
    public static Task<IReadOnlyList<string>> ProcessPdfAsync(this EnginePool pool, Stream pdfStream, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool));
        return ProcessAsync(pool, PdfImage.RenderPages(pdfStream, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }
    #endregion

    #region ValidateEngine
    private static void ValidateEngine(Engine engine)
    {
        if (engine == null)
            throw new ArgumentNullException(nameof(engine));
        if (engine.IsDisposed)
            throw new ObjectDisposedException(nameof(Engine));
    }
    #endregion

    #region Process
    private static IReadOnlyList<string> Process(Engine engine, IEnumerable<Image> images, PageSegMode? mode, CancellationToken token)
    {
        var texts = new List<string>();
        foreach (var image in images)
        {
            using (image)
            {
                token.ThrowIfCancellationRequested();
                using var page = engine.Process(image, mode);
                texts.Add(page.Text);
                token.ThrowIfCancellationRequested();
            }
        }
        return texts.AsReadOnly();
    }
    #endregion

    #region ProcessAsync
    private static async Task<IReadOnlyList<string>> ProcessAsync(EnginePool pool, IEnumerable<Image> images, PageSegMode? mode,
        CancellationToken token)
    {
        var texts = new List<string>();
        foreach (var image in images)
        {
            using (image)
            {
                texts.Add(await pool.ProcessAsync(image, mode, token).ConfigureAwait(false));
                token.ThrowIfCancellationRequested();
            }
        }
        return texts.AsReadOnly();
    }
    #endregion
}