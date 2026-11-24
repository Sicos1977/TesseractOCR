using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PDFtoImage;
using SkiaSharp;
using TesseractOCR.Pix;

namespace TesseractOCRPdf;

/// <summary>Renders PDF pages into Leptonica images without temporary files.</summary>
public static class PdfImage
{
    /// <summary>Renders a PDF file in page order. Dispose every returned image.</summary>
    /// <remarks>The file stays open during enumeration. Dispose the enumerator when stopping early.</remarks>
    public static IEnumerable<Image> RenderPages(string filename, int dpi = 300, string? password = null,
        CancellationToken cancellationToken = default)
    {
        if (filename == null)
            throw new ArgumentNullException(nameof(filename));
        ValidateDpi(dpi);
        cancellationToken.ThrowIfCancellationRequested();
        return RenderFile(filename, dpi, password, cancellationToken);
    }

    /// <summary>Renders PDF bytes in page order. Dispose every returned image.</summary>
    public static IEnumerable<Image> RenderPages(byte[] pdf, int dpi = 300, string? password = null,
        CancellationToken cancellationToken = default)
    {
        if (pdf == null)
            throw new ArgumentNullException(nameof(pdf));
        ValidateDpi(dpi);
        cancellationToken.ThrowIfCancellationRequested();
        return RenderBytes(pdf, dpi, password, cancellationToken);
    }

    /// <summary>Renders a readable, seekable PDF stream in page order. Dispose every returned image.</summary>
    /// <remarks>The caller's stream remains open. Rendering reads from the beginning of the PDF and may change its position.
    /// Keep the stream open and do not modify it until enumeration ends.</remarks>
    public static IEnumerable<Image> RenderPages(Stream pdfStream, int dpi = 300, string? password = null,
        CancellationToken cancellationToken = default)
    {
        if (pdfStream == null)
            throw new ArgumentNullException(nameof(pdfStream));
        if (!pdfStream.CanRead || !pdfStream.CanSeek)
            throw new ArgumentException("The PDF stream must be readable and seekable.", nameof(pdfStream));
        ValidateDpi(dpi);
        cancellationToken.ThrowIfCancellationRequested();
        return RenderStream(pdfStream, dpi, password, cancellationToken);
    }

    private static void ValidateDpi(int dpi)
    {
        if (dpi <= 0)
            throw new ArgumentOutOfRangeException(nameof(dpi), "DPI must be positive.");
    }

    private static IEnumerable<Image> RenderFile(string filename, int dpi, string? password, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        using var stream = File.OpenRead(filename);
        foreach (var image in RenderStream(stream, dpi, password, token))
            yield return image;
    }

    private static IEnumerable<Image> RenderBytes(byte[] pdf, int dpi, string? password, CancellationToken token)
    {
        using var stream = new MemoryStream(pdf, false);
        foreach (var image in RenderStream(stream, dpi, password, token))
            yield return image;
    }

    private static IEnumerable<Image> RenderStream(Stream stream, int dpi, string? password, CancellationToken token)
    {
        using var pages = Conversion.ToImages(stream, leaveOpen: true, password: password,
            options: new RenderOptions(Dpi: dpi, WithAnnotations: true, WithFormFill: true)).GetEnumerator();
        while (true)
        {
            token.ThrowIfCancellationRequested();
            if (!pages.MoveNext())
                yield break;

            Image image;
            using (var bitmap = pages.Current)
            {
                token.ThrowIfCancellationRequested();
                using var encoded = bitmap.Encode(SKEncodedImageFormat.Png, 100);
                if (encoded == null)
                    throw new IOException("Failed to encode the rendered PDF page as PNG.");
                image = Image.LoadFromMemory(encoded.ToArray());
            }

            var transferred = false;
            try
            {
                image.XRes = dpi;
                image.YRes = dpi;
                token.ThrowIfCancellationRequested();
                transferred = true;
                yield return image;
            }
            finally
            {
                if (!transferred)
                    image.Dispose();
            }
        }
    }
}
