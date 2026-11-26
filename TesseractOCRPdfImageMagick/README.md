# TesseractOCRPdfImageMagick

Optional PDF input for `TesseractOCR.Engine` and `TesseractOCR.EnginePool`, rendered with ImageMagick and Ghostscript.

```powershell
dotnet add package TesseractOCRPdfImageMagick
dotnet add package TesseractOCR.Languages.English
```

```csharp
using System;
using System.IO;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCRPdfImageMagick;

var dataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
using var engine = new Engine(dataPath, Language.English);
var pages = engine.ProcessPdf("invoice.pdf", dpi: 300);
foreach (var text in pages)
	Console.WriteLine(text);

using var pool = new EnginePool(dataPath, Language.English, maximumEngines: 2);
using var stream = File.OpenRead("invoice.pdf");
var pooledPages = await pool.ProcessPdfAsync(stream);
```

Both methods accept a filename, `byte[]`, or readable, seekable `Stream`. They return one text entry per page, including blank pages, in document order. All pages are rasterized and OCR'd, even when a PDF contains selectable text. Optional arguments include positive `dpi` (default 300), `pageSegMode`, `password`, and `cancellationToken`. Invalid PDFs, incorrect passwords, I/O errors, and native dependency failures propagate as exceptions. Streams remain open, are read from the beginning, and may have their position changed. Do not modify input bytes or streams during processing.

For preprocessing or other OCR outputs, use `PdfImage.RenderPages` and dispose every returned `Pix.Image`. Enumeration starts rendering lazily, but ImageMagick renders and retains the entire document before yielding the first image, so large documents can require substantial memory and temporary disk space. Dispose the enumerator when stopping early. The extension methods dispose their images and pages, not the caller's engine, pool, or stream. Do not use an `Engine` concurrently. Pool processing is sequential per document; waiting for a pool engine is asynchronous, while rendering and OCR execute synchronously. Cancellation is checked between native operations and while waiting for the pool; it cannot interrupt a native operation in progress.

## Dependencies and platforms

[Magick.NET Q8](https://github.com/dlemstra/Magick.NET) provides ImageMagick. PDF rendering also requires a separately installed [Ghostscript](https://ghostscript.com/releases/gsdnld.html) compatible with the process architecture. On Windows, `MagickNET.SetGhostscriptDirectory` can specify its installation directory. Ghostscript has its own licensing requirements, including for commercial use; review its license before deployment. ImageMagick and Ghostscript may use temporary files. Pages are composited on white and converted to Leptonica images in memory at the requested DPI. Rendering and annotation behavior can differ from the PDFium/SkiaSharp backend.

The library targets .NET Framework 4.8 and .NET Standard 2.1. Platform support is the intersection of Magick.NET, Ghostscript, and TesseractOCR native dependencies; see the main repository README for OCR runtime requirements. Installing only `TesseractOCR` does not install these optional PDF dependencies.

Build and test from the repository root:

```powershell
dotnet pack TesseractOCRPdfImageMagick\TesseractOCRPdfImageMagick.csproj -c Release -p:GeneratePackageOnBuild=false
dotnet test TesseractOCRPdfImageMagick.Tests\TesseractOCRPdfImageMagick.Tests.csproj -c Release
```
