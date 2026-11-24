# TesseractOCR documentation

TesseractOCR is a .NET wrapper for the Tesseract OCR engine. This documentation site contains the generated API reference for the library and a quick overview of how to get started.

## Project and native library versions

- **Tesseract API:** 5.5.4
- **Leptonica:** 1.87.0
- **Supported library targets:** .NET Standard 2.0 and .NET Standard 2.1

The bundled Windows native libraries are provided for x86, x64, and ARM64. The wrapper uses Leptonica's `Pix.Image` API to load images.

## Get started

Install the NuGet package and provide trained language data in a `tessdata` directory. The `dataPath` argument points to the parent directory containing `tessdata`.

```powershell
dotnet add package TesseractOCR
```

```csharp
using System;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

using var engine = new Engine(@"./", Language.English);
using var image = Image.LoadFromFile(@"./invoice.png");
using var page = engine.Process(image);

Console.WriteLine(page.Text);
Console.WriteLine($"Mean confidence: {page.MeanConfidence}");
```

Download trained language files from [tessdata](https://github.com/tesseract-ocr/tessdata) or [tessdata_fast](https://github.com/tesseract-ocr/tessdata_fast).

## Render recognized text

Renderers produce different representations of recognized pages. Create a renderer with a base output path (without an extension), begin a document, add each recognized page, then dispose the renderer to finish the output.

```csharp
using System;
using System.IO;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;
using TesseractOCR.Renderers;

var outputBase = @"./output/invoice";
Directory.CreateDirectory(@"./output");

using var engine = new Engine(@"./", Language.English);
using var image = Image.LoadFromFile(@"./invoice.png");
using var page = engine.Process(image);
using var renderer = Result.CreateTextRenderer(outputBase);

using (renderer.BeginDocument("invoice"))
{
	if (!renderer.AddPage(page))
		throw new InvalidOperationException("Failed to add the page to the renderer.");
}
```

| Format | Purpose |
| --- | --- |
| `Text` | Plain UTF-8 text. |
| `Hocr` | HTML with OCR structure, word locations, and confidence values. |
| `Pdf` | Searchable PDF with the original page image and an OCR text layer. |
| `PdfTextonly` | PDF containing only the OCR text layer. |
| `Unlv` | Legacy Tesseract text format. |
| `Box` | Character bounding boxes, commonly used for OCR training. |
| `Alto` | ALTO XML with recognized text and page layout. |
| `Page` | PAGE XML with page regions, lines, words, and text. |
| `Tsv` | Tab-separated layout, text, coordinates, and confidence values. |
| `LstmBox` | Box output for LSTM training workflows. |
| `WordStrBox` | Word-level bounding boxes. |

Use `AggregateResult` to send pages to multiple renderers in a single processing pass. See the [README](https://github.com/Sicos1977/TesseractOCR/blob/master/TesseractOCR/README.md) for a complete example and additional usage details.

## Build the documentation site

Install the [DocFX command-line tool](https://dotnet.github.io/docfx/) and run `build.bat` from this directory. The script generates API metadata from `TesseractOCR.csproj` and builds the site into `Docs/_site`. Run `serve.bat` to preview the generated site locally.

## API reference

Use the **API Reference** entry in the navigation to browse public classes, methods, properties, and their XML documentation.
