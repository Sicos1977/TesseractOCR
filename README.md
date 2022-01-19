![image](https://user-images.githubusercontent.com/6692947/150184416-564bcffe-3080-4be0-b7a4-68a11af98687.png)

What is TesseractOCR
=========

It is a .NET wrapper for Tesseract 5.0.0 that is originally copied from Charles Weld (https://github.com/charlesw/tesseract) and modified for my own needs

How to use
============
```c#
using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
{
     using (var img = Pix.LoadFromFile(testImagePath))
     {
         using (var page = engine.Process(img))
         {
             var text = page.GetText();
             Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
             Console.WriteLine("Text (GetText): \r\n{0}", text);
             Console.WriteLine("Text (iterator):");
         }
    }
}
```

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
