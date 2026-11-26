//
// PdfImage.cs
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
using PDFtoImage;
using SkiaSharp;
using TesseractOCR.Pix;

namespace TesseractOCRPdfSkiaSharp;

/// <summary>
///     Renders PDF pages into Leptonica images without temporary files.
/// </summary>
public static class PdfImage
{
    #region RenderPages
    /// <summary>
    ///     Renders a PDF file in page order. Dispose every returned image.
    /// </summary>
    /// <remarks>
    ///     The file stays open during enumeration. Dispose the enumerator when stopping early.
    /// </remarks>
    public static IEnumerable<Image> RenderPages(string filename, int dpi = 300, string? password = null,
        CancellationToken cancellationToken = default)
    {
        if (filename == null)
            throw new ArgumentNullException(nameof(filename));
        ValidateDpi(dpi);
        cancellationToken.ThrowIfCancellationRequested();
        return RenderFile(filename, dpi, password, cancellationToken);
    }
    #endregion

    #region RenderPages
    /// <summary>
    ///     Renders PDF bytes in page order. Dispose every returned image.
    /// </summary>
    public static IEnumerable<Image> RenderPages(byte[] pdf, int dpi = 300, string? password = null,
        CancellationToken cancellationToken = default)
    {
        if (pdf == null)
            throw new ArgumentNullException(nameof(pdf));
        ValidateDpi(dpi);
        cancellationToken.ThrowIfCancellationRequested();
        return RenderBytes(pdf, dpi, password, cancellationToken);
    }
    #endregion

    #region RenderPages
    /// <summary>
    ///     Renders a readable, seekable PDF stream in page order. Dispose every returned image.
    /// </summary>
    /// <remarks>
    ///     The caller's stream remains open. Rendering reads from the beginning of the PDF and may change its position.
    ///     Keep the stream open and do not modify it until enumeration ends.
    /// </remarks>
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
    #endregion

    #region ValidateDpi
    private static void ValidateDpi(int dpi)
    {
        if (dpi <= 0)
            throw new ArgumentOutOfRangeException(nameof(dpi), "DPI must be positive.");
    }
    #endregion

    #region RenderFile
    private static IEnumerable<Image> RenderFile(string filename, int dpi, string? password, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        using var stream = File.OpenRead(filename);
        foreach (var image in RenderStream(stream, dpi, password, token))
            yield return image;
    }
    #endregion

    #region RenderBytes
    private static IEnumerable<Image> RenderBytes(byte[] pdf, int dpi, string? password, CancellationToken token)
    {
        using var stream = new MemoryStream(pdf, false);
        foreach (var image in RenderStream(stream, dpi, password, token))
            yield return image;
    }
    #endregion

    #region RenderStream
    /// <summary>
    ///     Renders the PDF pages in a stream as PNG-backed images.
    /// </summary>
    /// <remarks>Rendered images include annotations and form fill.</remarks>
    /// <param name="stream">The stream containing the PDF document; it remains open after rendering.</param>
    /// <param name="dpi">The resolution, in dots per inch, used for the rendered images.</param>
    /// <param name="password">The password used to open the PDF, or <see langword="null"/> if it is not password-protected.</param>
    /// <param name="token">A token used to cancel rendering.</param>
    /// <returns>An enumerable of rendered page images. The caller is responsible for disposing of each image.</returns>
    /// <exception cref="IOException">PNG encoding fails for a rendered page.</exception>
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
                using var encoded = bitmap!.Encode(SKEncodedImageFormat.Png, 100);
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
    #endregion
}