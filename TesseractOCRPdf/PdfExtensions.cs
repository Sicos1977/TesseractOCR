using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

namespace TesseractOCRPdf;

/// <summary>Optional PDF recognition for TesseractOCR engines.</summary>
public static class PdfExtensions
{
    /// <summary>Recognizes a PDF file and returns one text entry per page, in document order.</summary>
    /// <remarks>Do not use the engine concurrently. Native rendering and OCR cannot be interrupted mid-page.</remarks>
    public static IReadOnlyList<string> ProcessPdf(this Engine engine, string filename, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        ValidateEngine(engine);
        return Process(engine, PdfImage.RenderPages(filename, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }

    /// <summary>Recognizes PDF bytes and returns one text entry per page, in document order.</summary>
    /// <remarks>Do not use the engine concurrently. Native rendering and OCR cannot be interrupted mid-page.</remarks>
    public static IReadOnlyList<string> ProcessPdf(this Engine engine, byte[] pdf, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        ValidateEngine(engine);
        return Process(engine, PdfImage.RenderPages(pdf, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }

    /// <summary>Recognizes a readable, seekable PDF stream and returns text in page order. Leaves the stream open.</summary>
    /// <remarks>Rendering reads from the beginning of the PDF and may change its position. Do not use the engine concurrently.
    /// Native rendering and OCR cannot be interrupted mid-page.</remarks>
    public static IReadOnlyList<string> ProcessPdf(this Engine engine, Stream pdfStream, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        ValidateEngine(engine);
        return Process(engine, PdfImage.RenderPages(pdfStream, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }

    /// <summary>Recognizes a PDF file using the pool and returns one text entry per page, in document order.</summary>
    /// <remarks>Pages are rendered and recognized sequentially to bound image memory. Concurrent documents share the pool.
    /// Waiting for an engine is asynchronous; rendering and native OCR run synchronously and cannot be interrupted mid-page.</remarks>
    public static Task<IReadOnlyList<string>> ProcessPdfAsync(this EnginePool pool, string filename, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool));
        return ProcessAsync(pool, PdfImage.RenderPages(filename, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }

    /// <summary>Recognizes PDF bytes using the pool and returns one text entry per page, in document order.</summary>
    /// <remarks>Pages are processed sequentially; concurrent documents share the pool. Native work cannot be interrupted mid-page.</remarks>
    public static Task<IReadOnlyList<string>> ProcessPdfAsync(this EnginePool pool, byte[] pdf, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool));
        return ProcessAsync(pool, PdfImage.RenderPages(pdf, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }

    /// <summary>Recognizes a readable, seekable PDF stream using the pool, in page order. Leaves the stream open.</summary>
    /// <remarks>Rendering reads from the beginning of the PDF and may change its position. Keep the stream open until completion.
    /// Pages are processed sequentially; concurrent documents share the pool. Native work cannot be interrupted mid-page.</remarks>
    public static Task<IReadOnlyList<string>> ProcessPdfAsync(this EnginePool pool, Stream pdfStream, int dpi = 300,
        PageSegMode? pageSegMode = null, string? password = null, CancellationToken cancellationToken = default)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool));
        return ProcessAsync(pool, PdfImage.RenderPages(pdfStream, dpi, password, cancellationToken), pageSegMode, cancellationToken);
    }

    private static void ValidateEngine(Engine engine)
    {
        if (engine == null)
            throw new ArgumentNullException(nameof(engine));
        if (engine.IsDisposed)
            throw new ObjectDisposedException(nameof(Engine));
    }

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
}
