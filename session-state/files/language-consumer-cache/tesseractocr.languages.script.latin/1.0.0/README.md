# TesseractOCR language data

This package provides a `tessdata_fast` trained model and depends on the [TesseractOCR NuGet package](https://www.nuget.org/packages/TesseractOCR), which NuGet restores automatically when you install this language package. The model data itself does not contain the OCR engine.

When installed using PackageReference, the package's MSBuild props copy the model to the `tessdata` directory in the build and publish output. Script models are placed in `tessdata/script`. Pass the directory containing the model to `Engine`, for example:

```csharp
using System;
using System.IO;
using TesseractOCR;
using TesseractOCR.Enums;

using var engine = new Engine(Path.Combine(AppContext.BaseDirectory, "tessdata"), Language.English);
```

If you use a different location, you must configure both the copy destination and the path passed to `Engine`. To change the destination, set a relative output path in a `Directory.Build.props` file in the consuming project directory (or an ancestor directory), before NuGet imports this package's props:

```xml
<Project>
  <PropertyGroup>
    <TesseractOCRLanguageDataDirectory>ocr-data</TesseractOCRLanguageDataDirectory>
  </PropertyGroup>
</Project>
```

For this example, pass `Path.Combine(AppContext.BaseDirectory, "ocr-data")` to `Engine`. Script models remain under its `script` subdirectory. Configure the property in each consuming project that needs a different output location; do not edit the installed package because a restore can replace it. The property is intended for paths relative to the build and publish output directories.

Choose the language matching the installed package. The `osd` model is for orientation and script detection, not a recognition language. These fast models support Tesseract's LSTM engine, not its legacy engine. If the engine data path is empty, the wrapper attempts to use `TESSDATA_PREFIX`.

The models are from [tessdata_fast](https://github.com/tesseract-ocr/tessdata_fast), licensed under Apache-2.0. The source commit is recorded in the package metadata.
