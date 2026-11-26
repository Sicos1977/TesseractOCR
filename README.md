![image](https://user-images.githubusercontent.com/6692947/150184680-1ae82d62-891e-4dbd-b52b-e975c57f9761.png)


What is TesseractOCR
====================

TesseractOCR is a .NET wrapper for Tesseract OCR. The native API bindings target **Tesseract 5.5.4**, and the bundled native image-processing library is **Leptonica 1.87.0**. The Windows native binaries are named `tesseract55.dll` and `leptonica-1.87.0.dll` (the Tesseract DLL name identifies the 5.5 series).

For concurrent OCR with reusable engines, see [Recognize images in parallel with EnginePool](#recognize-images-in-parallel-with-enginepool).

## Install and prepare language data

Install the NuGet package:

```powershell
dotnet add package TesseractOCR
```

Tesseract also needs trained language data. Choose one of these options:

- **NuGet:** Find the `TesseractOCR.Languages.*` packages on [nuget.org](https://www.nuget.org/packages?q=TesseractOCR.Languages). For example, install English with:

  ```powershell
  dotnet add package TesseractOCR.Languages.English
  ```

  In projects using PackageReference, the language package copies its model to `tessdata` in the build and publish output. Pass `Path.Combine(AppContext.BaseDirectory, "tessdata")` as the engine data path. These packages contain `tessdata_fast` models and support the LSTM engine only, not the legacy engine. The `TesseractOCR.Languages.Osd` package supplies orientation and script detection data, not a recognition language.

  To use another output location, set `TesseractOCRLanguageDataDirectory` in `Directory.Build.props` before NuGet imports the package, and update the engine data path accordingly. The property is relative to the build and publish output directories; see the language package README for an example.

- **Manual download:** Download the required `.traineddata` files from Tesseract's [tessdata](https://github.com/tesseract-ocr/tessdata) or [tessdata_fast](https://github.com/tesseract-ocr/tessdata_fast) repository, place them in a directory such as `tessdata`, and pass that directory as the engine data path. Choose data compatible with the selected engine mode; `tessdata_fast` supports LSTM only.

In the examples below, `dataPath` is the directory containing the `.traineddata` files (for example, `./tessdata`).

## Microsoft Visual C++ runtimes

The Windows native DLLs are compiled with Visual Studio 2026, so you need these C++ runtimes on your computer:

- X86: https://aka.ms/vs/18/release/vc_redist.x86.exe
- X64: https://aka.ms/vs/18/release/vc_redist.x64.exe
- ARM64: https://aka.ms/vs/18/release/vc_redist.arm64.exe

Windows native binaries are included for x86, x64 and arm64 under the NuGet runtime asset paths `runtimes/win-x86/native`, `runtimes/win-x64/native` and `runtimes/win-arm64/native`. NuGet selects the appropriate files when publishing for a Windows runtime identifier. The loader also accepts the existing architecture-specific directories for applications that deploy the libraries manually.

## Linux native libraries

The package includes glibc-based `linux-x64` and `linux-arm64` Tesseract 5.5.4 and Leptonica 1.87.0 libraries from [conda-forge](https://conda-forge.org/). Their other native dependencies are **not included**. In particular, they require recent conda-forge builds of the C++ runtime, libarchive, curl, and image codecs; standard distribution packages may not satisfy these requirements. These binaries do not support musl-based distributions such as Alpine.

One way to provide the matching dependencies is to create a conda-forge environment and start the application with its library directory on the dynamic linker search path:

```bash
conda create -n tesseract-native -c conda-forge tesseract=5.5.4 leptonica=1.87.0
conda activate tesseract-native
export LD_LIBRARY_PATH="$CONDA_PREFIX/lib${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
dotnet MyApplication.dll
```

Set `LD_LIBRARY_PATH` before starting the process. Install the environment for the same architecture as the application. The NuGet package supplies only the two OCR libraries; conda supplies their dependencies and their own licenses. Native library licenses are included in the NuGet package under `native-licenses`. The builds are conda-forge `tesseract` builds `h00735b0_0` (x64) and `hb27457c_0` (ARM64), and `leptonica` builds `hd4727f9_2` (x64) and `h775db3d_2` (ARM64).

## Recognize a page

```csharp
using System;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

var dataPath = @"./tessdata";
var imagePath = @"./invoice.png";voeg

using var engine = new Engine(dataPath, Language.English, EngineMode.Default);
using var image = Image.LoadFromFile(imagePath);
using var page = engine.Process(image);

Console.WriteLine($"Mean confidence: {page.MeanConfidence}");
Console.WriteLine(page.Text);
```

For multiple images, reuse engines safely with [EnginePool](#recognize-images-in-parallel-with-enginepool) instead of sharing one `Engine` between concurrent scans.

## Recognize PDF documents (optional packages)

Choose one PDF renderer without adding its dependencies to the base package:

- [TesseractOCRPdfSkiaSharp](TesseractOCRPdfSkiaSharp/README.md) uses PDFium and SkiaSharp. It replaces the `TesseractOCRPdf` package and retains its `TesseractOCRPdf` namespace.
- [TesseractOCRPdfImageMagick](TesseractOCRPdfImageMagick/README.md) uses ImageMagick and requires a separately installed Ghostscript, which has its own licensing requirements. Its namespace is `TesseractOCRPdfImageMagick`.

For example, with the SkiaSharp package:

```powershell
dotnet add package TesseractOCRPdfSkiaSharp
```

```csharp
using TesseractOCRPdf;

var pages = engine.ProcessPdf("invoice.pdf", dpi: 300);
var pooledPages = await pool.ProcessPdfAsync("invoice.pdf", dpi: 300);
```

Both backends accept files, byte arrays and readable, seekable streams, and return one text entry per page in document order. Streams remain open. `PdfImage.RenderPages` exposes disposable `Pix.Image` pages for preprocessing or advanced OCR output. Import only the chosen backend's namespace for extension methods. See the package READMEs for platform requirements, cancellation and resource ownership; ImageMagick renders the entire document before yielding the first page.

## Recognize images in parallel with EnginePool

`EnginePool` belongs to this package and pools `TesseractOCR.Engine`, not the separate Charles Weld `Tesseract` package's `TesseractEngine`. Existing `Engine` constructors and `Process` overloads remain available. Create one pool for a group of scans or for the application lifetime, reuse it across workers, and dispose it after the workers finish. Creating a pool for each image prevents engine reuse.

The pool bounds both active and retained engines; requests wait asynchronously when all engines are busy. `maximumEngines` must be positive and defaults to `Environment.ProcessorCount` when omitted. Engines are created lazily and exclusively used for one scan at a time. Each page is disposed before its engine is returned and adaptive classifier state is cleared between scans. Failed scans discard their engine. `ProcessAsync` returns recognized plain text; optionally pass a `PageSegMode` to select the page layout for a scan.

To observe pool activity, pass an `ILogger` as the third argument, for example `new EnginePool(dataPath, Language.English, logger, maximumEngines: workers)`. Configure the logger to enable `Debug`: pool creation and closure, engine creation and disposal, and acquisition and return are logged at that level. Engine IDs correlate events within a process. This logger is used for pool events only and is not passed to the existing thread-local engine logger. With no logger, pool logging is disabled.

Set `OMP_THREAD_LIMIT` at process startup, before any Tesseract or OpenMP initialization. It is process-wide and may affect other OpenMP libraries; setting it later may have no effect. Tune the pool size for available memory and workload rather than assuming more engines are always faster.

The following example targets .NET 6 or later (`Parallel.ForEachAsync`); the library itself also supports .NET Standard. Each worker owns and disposes its image. Exceptions propagate from `Parallel.ForEachAsync`, and disposal occurs even on failure:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

Environment.SetEnvironmentVariable("OMP_THREAD_LIMIT", "1");
string dataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
string[] paths = { "invoice1.bmp", "invoice2.bmp", "invoice3.bmp" };
string[] results = new string[paths.Length];
int workers = Environment.ProcessorCount;

using var pool = new EnginePool(dataPath, Language.English, EngineMode.LstmOnly, workers);
using var cancellation = new CancellationTokenSource();
await Parallel.ForEachAsync(Enumerable.Range(0, paths.Length),
    new ParallelOptions { MaxDegreeOfParallelism = workers, CancellationToken = cancellation.Token },
    async (index, token) =>
    {
        using var image = Image.LoadFromFile(paths[index]);
        results[index] = await pool.ProcessAsync(image, cancellationToken: token);
    });

for (int i = 0; i < paths.Length; i++)
    Console.WriteLine($"{paths[i]}: {results[i]}");
```

The example uses BMP files. Image format support depends on the native Leptonica build.

`ProcessAsync` waits asynchronously for capacity, but recognition itself runs synchronously on the calling worker. Cancellation stops waiting and is checked before recognition; it cannot interrupt a native scan already in progress. Do not share or dispose an image during a scan. Disposing the pool rejects new and waiting requests and disposes idle engines immediately; scans already running may finish and dispose their engines on return. Await all workers before disposing the pool when shutdown must wait for them. The pool is intended for independent documents and does not expose mutable engines or `Page` objects. For custom engine variables, result iterators, or other output formats, continue to use the existing `Engine` API with your own lifetime management.

## Choose an OCR engine and page segmentation mode

`EngineMode` selects the recognition engine when you create an `Engine`. `PageSegMode` tells Tesseract what layout to expect in each image; pass it to `Process` or set `Engine.DefaultPageSegMode`.

### Engine modes

| Mode | What it does |
| --- | --- |
| `TesseractOnly` | Uses the legacy OCR engine. Choose this only when your language data includes legacy engine data. |
| `LstmOnly` | Uses the newer neural-network (LSTM) engine. This is the recommended choice for current language data, including `tessdata_fast` models. |
| `TesseractAndLstm` | Uses the combined legacy and LSTM engine. The language data must support both engines; LSTM-only models cannot be used with this mode. |
| `Default` | Lets Tesseract select its default engine mode. This currently uses the LSTM engine. |

### Page segmentation modes

| Mode | What it assumes or does |
| --- | --- |
| `OsdOnly` | Detects page orientation and script only; it does not recognize text. |
| `AutoOsd` | Automatically segments the page, detects orientation and script, and recognizes text. |
| `AutoOnly` | Analyses the page layout automatically, without orientation/script detection or text recognition. |
| `Auto` | Automatically segments the page and recognizes text, without orientation/script detection. This is the default. |
| `SingleColumn` | A single column of text, with variable-sized text. |
| `SingleBlockVertText` | A single block of vertically aligned text. |
| `SingleBlock` | A single, uniform block of text. |
| `SingleLine` | A single line of text. |
| `SingleWord` | A single word. |
| `CircleWord` | A single word arranged in a circle. |
| `SingleChar` | A single character. |
| `SparseText` | Finds as much text as possible, without assuming a particular reading order. |
| `SparseTextOsd` | Finds sparse text and also detects orientation and script. |
| `RawLine` | A single text line, bypassing Tesseract-specific line-recognition heuristics. |

OSD means orientation and script detection, and requires suitable `osd` language data. For example, use a single-line mode for a cropped line rather than asking automatic page segmentation to interpret it as a full page:

```csharp
using var engine = new Engine(dataPath, Language.English, EngineMode.LstmOnly);
using var image = Image.LoadFromFile(imagePath);
using var page = engine.Process(image, PageSegMode.SingleLine);
```

`PageSegMode.Count` is an internal enum sentinel, not a mode to pass to `Process`.

## Prepare images for OCR

The image API can convert to grayscale (`ConvertToGray`), normalize an uneven background (`NormalizeBackground` or `NormalizeBackgroundFlexible`), normalize contrast (`NormalizeContrast`), filter noise (`MedianFilter`), invert pixels, binarize using a fixed threshold (`Binarize`), and crop or add/remove borders. Binary images also support `Dilate`, `Erode`, `Open`, and `Close`. `RemoveAlpha` composites 32-bit RGBA images on white; for other images it returns a shared reference.

Each result must be disposed. Test preprocessing choices against your own images: Tesseract already performs preprocessing, and extra operations can reduce accuracy.

```csharp
using System;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

using var image = Image.LoadFromFile(@"./invoice.png");
using var gray = image.ConvertToGray();
using var normalized = gray.NormalizeBackground();
using var binary = normalized.Binarize(160);
using var bordered = binary.AddBorder(10);
using var engine = new Engine(@"./tessdata", Language.English);
using var page = engine.Process(bordered);
Console.WriteLine(page.Text);
```

## Inspect page layout

A recognized page exposes blocks, paragraphs, text lines, words, and symbols. Use their bounding boxes and confidence values when you need more than the combined text:

```csharp
using System;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

using var engine = new Engine(@"./tessdata", Language.English);
using var image = Image.LoadFromFile(@"./invoice.png");
using var page = engine.Process(image);

foreach (var block in page.Layout)
{
    Console.WriteLine($"Block (confidence {block.Confidence}): {block.Text}");

    foreach (var paragraph in block.Paragraphs)
    foreach (var line in paragraph.TextLines)
    foreach (var word in line.Words)
    {
        var box = word.BoundingBox;
        Console.WriteLine($"{word.Text}: confidence {word.Confidence}, " +
                          $"box {box?.X1},{box?.Y1} - {box?.X2},{box?.Y2}");
    }
}
```

## Write OCR output with a renderer

Renderers write OCR results to common text, document, and training formats. The output path passed to a renderer is a base path; Tesseract adds the format-specific extension.

This example writes recognized text to a `.txt` file:

```csharp
using System;
using System.IO;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;
using TesseractOCR.Renderers;

var dataPath = @"./tessdata";
var imagePath = @"./invoice.png";
var outputBase = @"./output/invoice"; // Do not include an extension
Directory.CreateDirectory(@"./output");

using var engine = new Engine(dataPath, Language.English);
using var image = Image.LoadFromFile(imagePath);
using var page = engine.Process(image);
using var renderer = Result.CreateTextRenderer(outputBase);

using (renderer.BeginDocument(Path.GetFileNameWithoutExtension(imagePath)))
{
    if (!renderer.AddPage(page))
        throw new InvalidOperationException("Failed to add the page to the renderer.");
}
```

Choose a renderer based on the output you need:

| Renderer / format | Output |
| --- | --- |
| `Text` | UTF-8 plain text for straightforward text extraction. |
| `Hocr` | HTML with hOCR markup, word positions, and confidence; the renderer can optionally include font information. |
| `Pdf` | Searchable PDF with the source page image and an OCR text layer. |
| `PdfTextonly` | PDF containing only the OCR text layer, without page images. |
| `Unlv` | Legacy Tesseract UNLV text format. |
| `Box` | Character-level bounding boxes, commonly used by OCR training workflows. |
| `Alto` | ALTO XML with structured page layout and recognized text. |
| `Page` | PAGE XML with page regions, lines, words, and recognized text. |
| `Tsv` | Tab-separated page, layout, word, bounding-box, and confidence data. |
| `LstmBox` | Box output for Tesseract LSTM training workflows. |
| `WordStrBox` | Word-level box output, with one box for each recognized word. |

Other formats can be created directly with `Result.CreateHOcrRenderer(outputBase)`, `Result.CreatePdfRenderer(outputBase, dataPath, textonly: false)`, or `Result.CreateTsvRenderer(outputBase)`.

To write several formats from one OCR pass, combine the renderers with `AggregateResult`. It forwards document and page operations and disposes its child renderers:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;
using TesseractOCR.Renderers;

var dataPath = @"./tessdata";
var imagePath = @"./invoice.png";
var outputBase = @"./output/invoice";
Directory.CreateDirectory(@"./output");

using var engine = new Engine(dataPath, Language.English);
using var image = Image.LoadFromFile(imagePath);
using var page = engine.Process(image);
using var renderer = new AggregateResult(Result.CreateRenderers(
    outputBase,
    dataPath,
    new List<RenderFormat> { RenderFormat.Text, RenderFormat.Hocr, RenderFormat.Tsv }));

using (renderer.BeginDocument(Path.GetFileNameWithoutExtension(imagePath)))
{
    if (!renderer.AddPage(page))
        throw new InvalidOperationException("Failed to add the page to one or more renderers.");
}
```

For more examples, see the [examples on the project wiki](https://github.com/Sicos1977/TesseractOCR/wiki/examples.md).

Supported input formats
=======================

Tesseract uses the bundled Leptonica 1.87.0 library to read images. Supported formats depend on the codecs included in the native Leptonica build and include:

- PNG (libpng and zlib)
- JPEG (libjpeg or libjpeg-turbo)
- TIFF (libtiff and zlib)
- JPEG 2000 (OpenJPEG)
- GIF (giflib)
- WebP (libwebp)
- BMP and PNM

Images are loaded through the library's Leptonica-backed `Pix.Image` API. `System.Drawing.Image` is not supported; using Leptonica keeps image loading cross-platform. To save a WebP image, specify the format explicitly: `image.Save("output.webp", ImageFormat.WebP)` (`ImageFormat` is in `TesseractOCR.Enums`).

Logging
=======

TesseractOCR accepts the Microsoft `ILogger` interface, so you can use any compatible logging provider. Built-in logger implementations are available in the `TesseractOCR.Loggers` namespace. For example, pass the built-in console logger to the engine:

```csharp
using var logger = new TesseractOCR.Loggers.Console();
using var engine = new Engine(@"./tessdata", Language.English, logger: logger);
```

In Visual Studio's Package Manager Console, install the package with:

```powershell
Install-Package TesseractOCR
```


## License Information

* Copyright 2012-2019 Charles Weld (https://github.com/charlesw)
* Copyright 2021-2027 Kees van Spelde

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Core Team
=========
* [Sicos1977](https://github.com/sicos1977) (Kees van Spelde)