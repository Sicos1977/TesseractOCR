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
using ImageMagick;
using TesseractOCR.Pix;

namespace TesseractOCRPdfImageMagick;

/// <summary>
///     Renders PDF pages into Leptonica images using ImageMagick and Ghostscript.
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
    ///     The caller's stream remains open. Rendering reads from the beginning and may change its position.
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
    private static IEnumerable<Image> RenderStream(Stream stream, int dpi, string? password, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        stream.Position = 0;
        var settings = new MagickReadSettings
        {
            Format = MagickFormat.Pdf,
            Density = new Density(dpi),
            BackgroundColor = MagickColors.White
        };
        settings.SetDefine(MagickFormat.Pdf, "stop-on-error", true);
        if (password != null)
            settings.SetDefine("authenticate", password);

        using var pages = new MagickImageCollection();
        pages.Read(stream, settings);
        if (pages.Count == 0)
            throw new InvalidDataException("The PDF did not contain any rendered pages.");
        for (var index = 0; ; index++)
        {
            token.ThrowIfCancellationRequested();
            if (index == pages.Count)
                yield break;
            var page = pages[index];
            page.BackgroundColor = MagickColors.White;
            page.Alpha(AlphaOption.Remove);
            var image = Image.LoadFromMemory(page.ToByteArray(MagickFormat.Png24));
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
