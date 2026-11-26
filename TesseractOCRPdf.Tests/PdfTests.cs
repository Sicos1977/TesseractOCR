using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Exceptions;
#if IMAGEMAGICK
using TesseractOCRPdfImageMagick;
using PdfReadException = ImageMagick.MagickException;
#else
using PdfReadException = PDFtoImage.Exceptions.PdfException;
#endif
using Image = TesseractOCR.Pix.Image;

namespace Tesseract.Tests;

[TestClass]
public class PdfTests
{
    private static string DataPath => Path.Combine(AppContext.BaseDirectory, "tessdata");

    [TestMethod]
    public void EngineRecognizesVectorAndScannedPagesFromBytesAndCanBeReused()
    {
        using var engine = new Engine(DataPath, Language.English);
        AssertPages(engine.ProcessPdf(CreatePdf(), pageSegMode: PageSegMode.SingleLine));
        AssertPages(engine.ProcessPdf(CreatePdf(), pageSegMode: PageSegMode.SingleLine));
        Assert.IsFalse(engine.IsDisposed);
    }

    [TestMethod]
    public void EngineRecognizesStreamAndLeavesItOpen()
    {
        using var stream = new MemoryStream(CreatePdf());
        stream.Position = stream.Length;
        using var engine = new Engine(DataPath, Language.English);
        AssertPages(engine.ProcessPdf(stream, pageSegMode: PageSegMode.SingleLine));
        stream.Position = 0;
        Assert.AreEqual('%', (char)stream.ReadByte());
    }

    [TestMethod]
    public async Task EngineAndPoolRecognizeFileAndReleaseIt()
    {
        var filename = Path.Combine(Path.GetTempPath(), $"tesseract-pdf-{Guid.NewGuid():N}.pdf");
        try
        {
            File.WriteAllBytes(filename, CreatePdf());
            using var engine = new Engine(DataPath, Language.English);
            AssertPages(engine.ProcessPdf(filename, pageSegMode: PageSegMode.SingleLine));
            using var pool = new EnginePool(DataPath, Language.English, maximumEngines: 1);
            AssertPages(await pool.ProcessPdfAsync(filename, pageSegMode: PageSegMode.SingleLine));
            using var exclusive = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            Assert.AreEqual(new FileInfo(filename).Length, exclusive.Length);
        }
        finally
        {
            File.Delete(filename);
        }
    }

    [TestMethod]
    public async Task PoolRecognizesConcurrentDocumentsFromBytesAndRemainsReusable()
    {
        var pdf = CreatePdf();
        using var pool = new EnginePool(DataPath, Language.English, maximumEngines: 2);
        var tasks = Enumerable.Range(0, 4).Select(_ => Task.Run(() =>
            pool.ProcessPdfAsync(pdf, pageSegMode: PageSegMode.SingleLine))).ToArray();
        foreach (var pages in await Task.WhenAll(tasks))
            AssertPages(pages);
        using var stream = new MemoryStream(pdf);
        AssertPages(await pool.ProcessPdfAsync(stream, pageSegMode: PageSegMode.SingleLine));
        stream.Position = 0;
        Assert.AreEqual('%', (char)stream.ReadByte());
    }

    [TestMethod]
    public void BlankPagesArePreservedInResults()
    {
        using var stream = new MemoryStream();
        using (var document = SKDocument.CreatePdf(stream))
        {
            document.BeginPage(360, 120);
            document.EndPage();
            DrawPage(document, "FIRST PAGE", false);
            document.Close();
        }
        using var engine = new Engine(DataPath, Language.English);
        var pages = engine.ProcessPdf(stream.ToArray(), pageSegMode: PageSegMode.SingleLine);
        Assert.AreEqual(2, pages.Count);
        Assert.AreEqual(string.Empty, pages[0].Trim());
        Assert.AreEqual("FIRST PAGE", pages[1].Trim());
    }

    [TestMethod]
    public unsafe void RenderingPreservesDimensionsDpiWhiteBackgroundAndColors()
    {
        using var stream = new MemoryStream();
        using (var document = SKDocument.CreatePdf(stream))
        {
            var canvas = document.BeginPage(360, 120);
            using var paint = new SKPaint { Color = SKColors.Red };
            canvas.DrawRect(10, 10, 40, 40, paint);
            paint.Color = SKColors.Blue;
            canvas.DrawRect(60, 10, 40, 40, paint);
            document.EndPage();
            document.Close();
        }
        foreach (var dpi in new[] { 72, 144 })
        {
            using var pages = PdfImage.RenderPages(stream.ToArray(), dpi).GetEnumerator();
            Assert.IsTrue(pages.MoveNext());
            using var image = pages.Current;
            Assert.AreEqual(360 * dpi / 72, image.Width);
            Assert.AreEqual(120 * dpi / 72, image.Height);
            Assert.AreEqual(dpi, image.XRes);
            Assert.AreEqual(dpi, image.YRes);
            Assert.AreEqual(32, image.Depth);
            var data = image.GetData();
            var row = (uint*)data.PixData + (20 * dpi / 72) * data.WordsPerLine;
            Assert.AreEqual(0xff000000u, row[20 * dpi / 72] & 0xffffff00u);
            Assert.AreEqual(0x0000ff00u, row[70 * dpi / 72] & 0xffffff00u);
            Assert.AreEqual(0xffffff00u, row[110 * dpi / 72] & 0xffffff00u);
            Assert.IsFalse(pages.MoveNext());
        }
    }

    [TestMethod]
    public void EarlyEnumerationDisposalLeavesCallerImageAndStreamOpen()
    {
        using var stream = new MemoryStream(CreatePdf());
        Image image;
        using (var pages = PdfImage.RenderPages(stream, dpi: 72).GetEnumerator())
        {
            Assert.IsTrue(pages.MoveNext());
            image = pages.Current;
        }
        using (image)
        {
            Assert.IsFalse(image.IsDisposed);
            Assert.AreEqual(360, image.Width);
        }
        stream.Position = 0;
        Assert.AreEqual('%', (char)stream.ReadByte());
    }

    [TestMethod]
    public void CancellationBetweenPagesStopsRenderingAndLeavesStreamOpen()
    {
        using var stream = new MemoryStream(CreatePdf());
        using var cancellation = new CancellationTokenSource();
        using (var pages = PdfImage.RenderPages(stream, cancellationToken: cancellation.Token).GetEnumerator())
        {
            Assert.IsTrue(pages.MoveNext());
            using var first = pages.Current;
            cancellation.Cancel();
            Assert.ThrowsExactly<OperationCanceledException>(() => pages.MoveNext());
            Assert.IsFalse(first.IsDisposed);
        }
        Assert.IsTrue(stream.CanRead);
    }

    [TestMethod]
    public void CancellationAfterLastPageIsObservedOnNextMove()
    {
        using var stream = new MemoryStream();
        using (var document = SKDocument.CreatePdf(stream))
        {
            DrawPage(document, "FIRST PAGE", false);
            document.Close();
        }
        using var cancellation = new CancellationTokenSource();
        using var pages = PdfImage.RenderPages(stream.ToArray(), dpi: 72, cancellationToken: cancellation.Token).GetEnumerator();
        Assert.IsTrue(pages.MoveNext());
        using var image = pages.Current;
        cancellation.Cancel();
        Assert.ThrowsExactly<OperationCanceledException>(() => pages.MoveNext());
        Assert.IsFalse(image.IsDisposed);
    }

    [TestMethod]
    public async Task PreCancelledRequestsDoNotOpenMissingFilesOrInitializeEngines()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Assert.ThrowsExactly<OperationCanceledException>(() =>
            PdfImage.RenderPages("missing.pdf", cancellationToken: cancellation.Token));
        using var pool = new EnginePool("missing-tessdata", Language.English, maximumEngines: 1);
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(async () =>
            await pool.ProcessPdfAsync("missing.pdf", cancellationToken: cancellation.Token));
    }

    [TestMethod]
    public async Task InvalidPdfPropagatesForEngineAndPoolAndLeavesStreamsOpen()
    {
        var invalid = System.Text.Encoding.ASCII.GetBytes("This is not a PDF.");
        using var engine = new Engine(DataPath, Language.English);
        using var stream = new MemoryStream(invalid);
        Assert.Throws<PdfReadException>(() => engine.ProcessPdf(stream));
        Assert.IsTrue(stream.CanRead);
        using var pool = new EnginePool(DataPath, Language.English, maximumEngines: 1);
        await Assert.ThrowsAsync<PdfReadException>(async () => await pool.ProcessPdfAsync(stream));
        Assert.IsTrue(stream.CanRead);
        AssertPages(await pool.ProcessPdfAsync(CreatePdf(), pageSegMode: PageSegMode.SingleLine));
    }

    [TestMethod]
    public async Task OcrFailureDisposesRenderingButLeavesInputStreamOpen()
    {
        using var stream = new MemoryStream(CreatePdf());
        using var pool = new EnginePool("missing-tessdata", Language.English, maximumEngines: 1);
        await Assert.ThrowsExactlyAsync<TesseractException>(async () => await pool.ProcessPdfAsync(stream));
        stream.Position = 0;
        Assert.AreEqual('%', (char)stream.ReadByte());
    }

    [TestMethod]
    public void InvalidArgumentsAreRejectedBeforeRendering()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => PdfImage.RenderPages((byte[])null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => PdfImage.RenderPages((Stream)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => PdfImage.RenderPages((string)null!));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PdfImage.RenderPages(Array.Empty<byte>(), dpi: 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PdfImage.RenderPages("missing.pdf", dpi: -1));
        using var closed = new MemoryStream();
        closed.Dispose();
        Assert.ThrowsExactly<ArgumentException>(() => PdfImage.RenderPages(closed));
        Assert.ThrowsExactly<ArgumentNullException>(() => ((Engine)null!).ProcessPdf(Array.Empty<byte>()));
        Assert.ThrowsExactly<ArgumentNullException>(() => ((EnginePool)null!).ProcessPdfAsync(Array.Empty<byte>()));
        using var engine = new Engine(DataPath, Language.English);
        engine.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(() => engine.ProcessPdf(Array.Empty<byte>()));
    }

    private static void AssertPages(System.Collections.Generic.IReadOnlyList<string> pages)
    {
        Assert.AreEqual(2, pages.Count);
        Assert.AreEqual("FIRST PAGE", pages[0].Trim());
        Assert.AreEqual("SECOND PAGE", pages[1].Trim());
    }

    private static byte[] CreatePdf()
    {
        using var stream = new MemoryStream();
        using (var document = SKDocument.CreatePdf(stream))
        {
            DrawPage(document, "FIRST PAGE", false);
            DrawPage(document, "SECOND PAGE", true);
            document.Close();
        }
        return stream.ToArray();
    }

    private static void DrawPage(SKDocument document, string text, bool scanned)
    {
        var canvas = document.BeginPage(360, 120);
        using var typeface = SKTypeface.FromFamilyName("Arial");
        using var font = new SKFont(typeface, 32);
        using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        if (scanned)
        {
            using var bitmap = new SKBitmap(720, 240);
            using var imageCanvas = new SKCanvas(bitmap);
            imageCanvas.Clear(SKColors.White);
            imageCanvas.Scale(2);
            imageCanvas.DrawText(text, 25, 70, SKTextAlign.Left, font, paint);
            canvas.DrawBitmap(bitmap, new SKRect(0, 0, 360, 120), SKSamplingOptions.Default);
        }
        else
            canvas.DrawText(text, 25, 70, SKTextAlign.Left, font, paint);
        document.EndPage();
    }
}
