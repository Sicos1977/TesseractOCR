![image](https://user-images.githubusercontent.com/6692947/150184680-1ae82d62-891e-4dbd-b52b-e975c57f9761.png)


What is TesseractOCR
====================

TesseractOCR is a .NET wrapper for Tesseract OCR. The native API bindings target **Tesseract 5.5.4**, and the bundled native image-processing library is **Leptonica 1.87.0**. The Windows native binaries are named `tesseract55.dll` and `leptonica-1.87.0.dll` (the Tesseract DLL name identifies the 5.5 series).

## Install and prepare language data

Install the NuGet package:

```powershell
dotnet add package TesseractOCR
```

Tesseract also needs trained language data in a `tessdata` directory. Download the languages you need from [tessdata](https://github.com/tesseract-ocr/tessdata) or [tessdata_fast](https://github.com/tesseract-ocr/tessdata_fast). In the examples below, `dataPath` is the parent directory containing `tessdata` (for example, `./` when the directory is `./tessdata`).

## Microsoft Visual C++ runtimes

The Windows native DLLs are compiled with Visual Studio 2026, so you need these C++ runtimes on your computer:

- X86: https://aka.ms/vs/18/release/vc_redist.x86.exe
- X64: https://aka.ms/vs/18/release/vc_redist.x64.exe
- ARM64: https://aka.ms/vs/18/release/vc_redist.arm64.exe

Windows native binaries are included for x86, x64 and arm64.

## Recognize a page

```csharp
using System;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

var dataPath = @"./"; // Contains ./tessdata
var imagePath = @"./invoice.png";

using var engine = new Engine(dataPath, Language.English, EngineMode.Default);
using var image = Image.LoadFromFile(imagePath);
using var page = engine.Process(image);

Console.WriteLine($"Mean confidence: {page.MeanConfidence}");
Console.WriteLine(page.Text);
```

## Inspect page layout

A recognized page exposes blocks, paragraphs, text lines, words, and symbols. Use their bounding boxes and confidence values when you need more than the combined text:

```csharp
using System;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

using var engine = new Engine(@"./", Language.English);
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

var dataPath = @"./"; // Contains ./tessdata
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

var dataPath = @"./";
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

Images are loaded through the library's Leptonica-backed `Pix.Image` API. `System.Drawing.Image` is not supported; using Leptonica keeps image loading cross-platform.

Logging
=======

TesseractOCR accepts the Microsoft `ILogger` interface, so you can use any compatible logging provider. Built-in logger implementations are available in the `TesseractOCR.Loggers` namespace. For example, pass the built-in console logger to the engine:

```csharp
using var logger = new TesseractOCR.Loggers.Console();
using var engine = new Engine(@"./", Language.English, logger: logger);
```

In Visual Studio's Package Manager Console, install the package with:

```powershell
Install-Package TesseractOCR
```


## License Information

* Copyright 2012-2019 Charles Weld (https://github.com/charlesw)
* Copyright 2021-2025 Kees van Spelde

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
* [charlesw](https://github.com/charlesw) (Charles Weld) - Copied repro from him
