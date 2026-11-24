# TesseractOCRPdf

Optional PDF input for `TesseractOCR.Engine` and `TesseractOCR.EnginePool`.

```powershell
dotnet add package TesseractOCRPdf
dotnet add package TesseractOCR.Languages.English
```

```csharp
using System;
using System.IO;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCRPdf;

var dataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
using var engine = new Engine(dataPath, Language.English);
var pages = engine.ProcessPdf("invoice.pdf");
foreach (var text in pages)
	Console.WriteLine(text);

using var pool = new EnginePool(dataPath, Language.English, maximumEngines: 2);
using var stream = File.OpenRead("invoice.pdf");
var pooledPages = await pool.ProcessPdfAsync(stream, dpi: 300);
```

Both methods accept a filename, `byte[]`, or readable, seekable `Stream`. Results contain one plain-text entry per page (including blank pages), in PDF order. All pages are rasterized and OCR'd, even when a PDF already contains selectable text. Optional arguments: `dpi` (positive, default 300), `pageSegMode`, `password` for encrypted PDFs, and `cancellationToken`. Invalid PDFs, wrong passwords, I/O and native dependency failures propagate as exceptions, not empty results. Streams remain open, are read from the beginning, and may have their position changed. Do not modify input bytes or streams during processing.

For image preprocessing, layout, confidence or other OCR output, render pages yourself:

```csharp
foreach (var image in PdfImage.RenderPages("invoice.pdf", dpi: 300))
{
	using (image)
	using (var page = engine.Process(image))
		Console.WriteLine($"{page.MeanConfidence}: {page.Text}");
}
```

Dispose every rendered `Pix.Image`. Enumeration is lazy; keep input streams open and dispose the enumerator when stopping early (`foreach` does this automatically). Extension methods dispose all their own images and OCR pages, never the caller's engine, pool or stream. Only one page image is retained at a time; text results are collected in memory. Do not use an `Engine` concurrently. Multiple documents can share an `EnginePool` from separate workers, but each document processes its pages sequentially. Waiting for pool capacity is asynchronous; rendering and OCR execute synchronously on the calling worker. Cancellation is checked between native operations and while waiting for pool capacity; it cannot interrupt native rendering or OCR already in progress.

## Dependencies and platforms

SkiaSharp cannot read/render PDF documents by itself. This package uses [PDFtoImage](https://github.com/sungaila/PDFtoImage) (MIT), which renders with PDFium and returns SkiaSharp bitmaps. Pages are composited on white, including annotations and form values, converted losslessly in memory to `Pix.Image`, and given the requested DPI. No temporary image files or `System.Drawing.Common` are used. PDFium serializes native rendering within a process; an engine pool does not make PDF rendering parallel.

The package targets .NET Framework 4.8 and .NET Standard 2.1 (.NET Core 3.0+ / modern .NET). PDFtoImage supplies its PDFium and SkiaSharp native runtime dependencies transitively. Actual platform support is the intersection with TesseractOCR: bundled OCR binaries support Windows x86/x64/ARM64 and glibc Linux x64/ARM64. See the main repository README for Tesseract native runtime prerequisites and language data. macOS requires separately supplied compatible Tesseract/Leptonica binaries and is not verified here.

Installing only `TesseractOCR` does **not** bring in PDFtoImage, PDFium or SkiaSharp. These dependencies are confined to this optional package. This package requires TesseractOCR 5.5.3 or later, which includes `EnginePool`; publish the updated core package before the PDF package. From the repository root, build both packages with:

```powershell
dotnet pack TesseractOCR\TesseractOCR.csproj -c Release -p:GeneratePackageOnBuild=false
dotnet pack TesseractOCRPdf\TesseractOCRPdf.csproj -c Release
```

Run PDF regression tests with `dotnet test TesseractOCRPdf.Tests\TesseractOCRPdf.Tests.csproj -c Release`. The test project targets .NET 10; packing the library builds both .NET Framework 4.8 and .NET Standard 2.1 assemblies.
