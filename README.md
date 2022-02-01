![image](https://user-images.githubusercontent.com/6692947/150184680-1ae82d62-891e-4dbd-b52b-e975c57f9761.png)


What is TesseractOCR
=========

It is a .NET wrapper for Tesseract 5.0.0 that is originally copied from Charles Weld (https://github.com/charlesw/tesseract) and modified for my own needs

How to use
============

You need trained data in tessdata by language
You can get them at https://github.com/tesseract-ocr/tessdata or https://github.com/tesseract-ocr/tessdata_fast

##OCR a page

```c#
using var engine = new TesseractEngine(@"./tessdata", Language.English, EngineMode.Default)
using var img = Pix.Image.LoadFromFile(testImagePath)
using var page = engine.Process(img)
{
     Console.WriteLine("Mean confidence: {0}", page.MeanConfidence);
     Console.WriteLine("Text: \r\n{0}", page.Text);
}
```

##Iterate through the layout of a page

```c#
using var engine = CreateEngine();
using var img = LoadTestPix(TestImageFileColumn);
using var page = engine.Process(img);

foreach (var block in page.Layout)
{
    result.AppendLine($"Block confidence: {block.Confidence}");
    if (block.BoundingBox != null)
    {
        var boundingBox = block.BoundingBox.Value;
        result.AppendLine($"Block bounding box X1 '{boundingBox.X1}', Y1 '{boundingBox.Y2}', X2 " +
                          $"'{boundingBox.X2}', Y2 '{boundingBox.Y2}', width '{boundingBox.Width}', height '{boundingBox.Height}'");
    }
    result.AppendLine($"Block text: {block.Text}");

    foreach (var paragraph in block.Paragraphs)
    {
        result.AppendLine($"Paragraph confidence: {paragraph.Confidence}");
        if (paragraph.BoundingBox != null)
        {
            var boundingBox = paragraph.BoundingBox.Value;
            result.AppendLine($"Paragraph bounding box X1 '{boundingBox.X1}', Y1 '{boundingBox.Y2}', X2 " +
                              $"'{boundingBox.X2}', Y2 '{boundingBox.Y2}', width '{boundingBox.Width}', height '{boundingBox.Height}'");
        }
        var info = paragraph.Info;
        result.AppendLine($"Paragraph info justification: {info.Justification}");
        result.AppendLine($"Paragraph info is list item: {info.IsListItem}");
        result.AppendLine($"Paragraph info is crown: {info.IsCrown}");
        result.AppendLine($"Paragraph info first line ident: {info.FirstLineIdent}");
        result.AppendLine($"Paragraph text: {paragraph.Text}");
        
        foreach (var textLine in paragraph.TextLines)
        {
            if (textLine.BoundingBox != null)
            {
                var boundingBox = textLine.BoundingBox.Value;
                result.AppendLine($"Text line bounding box X1 '{boundingBox.X1}', Y1 '{boundingBox.Y2}', X2 " +
                                  $"'{boundingBox.X2}', Y2 '{boundingBox.Y2}', width '{boundingBox.Width}', height '{boundingBox.Height}'");
            }
            result.AppendLine($"Text line confidence: {textLine.Confidence}");
            result.AppendLine($"Text line text: {textLine.Text}");

            foreach (var word in textLine.Words)
            {
                result.AppendLine($"Word confidence: {word.Confidence}");
                if (word.BoundingBox != null)
                {
                    var boundingBox = word.BoundingBox.Value;
                    result.AppendLine($"Word bounding box X1 '{boundingBox.X1}', Y1 '{boundingBox.Y2}', X2 " +
                                      $"'{boundingBox.X2}', Y2 '{boundingBox.Y2}', width '{boundingBox.Width}', height '{boundingBox.Height}'");
                }
                result.AppendLine($"Word is from dictionary: {word.IsFromDictionary}");
                result.AppendLine($"Word is numeric: {word.IsNumeric}");
                result.AppendLine($"Word language: {word.Language}");
                result.AppendLine($"Word text: {word.Text}");

                foreach (var symbol in word.Symbols)
                {
                    result.AppendLine($"Symbol confidence: {symbol.Confidence}");
                    if (symbol.BoundingBox != null)
                    {
                        var boundingBox = symbol.BoundingBox.Value;
                        result.AppendLine($"Symbol bounding box X1 '{boundingBox.X1}', Y1 '{boundingBox.Y2}', X2 " +
                                          $"'{boundingBox.X2}', Y2 '{boundingBox.Y2}', width '{boundingBox.Width}', height '{boundingBox.Height}'");
                    }
                    result.AppendLine($"Symbol is superscript: {symbol.IsSuperscript}");
                    result.AppendLine($"Symbol is dropcap: {symbol.IsDropcap}");
                    result.AppendLine($"Symbol text: {symbol.Text}");
                }
            }
        }
    }
}
```

For more examples see https://github.com/Sicos1977/TesseractOCR/wiki/examples.md

Supported input formats
=======================

Tesseract uses the Leptonica library to read images with one of these formats:

- PNG - requires libpng, libz
- JPEG - requires libjpeg / libjpeg-turbo
- TIFF - requires libtiff, libz
- JPEG 2000 - requires libopenjp2
- GIF - requires libgif (giflib)
- WebP (including animated WebP) - requires libwebp
- BMP - no library required*
= PNM - no library required*
* Except Leptonica

**I have dropped support for the Windows.Drawing.Image namespace since this only works good on Windows and not on other systems. You should be fine with Leptonica**

Installing via NuGet
====================

The easiest way to install TesseractOCR is via NuGet.

In Visual Studio's Package Manager Console, simply enter the following command:

    Install-Package TesseractOCR


## License Information

* Copyright 2012-2019 Charles Weld (https://github.com/charlesw)
* Copyright 2021-2022 Kees van Spelde

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
