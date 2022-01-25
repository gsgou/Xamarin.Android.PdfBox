Xamarin.Android.PdfBox
======================

[![NuGet Badge](https://buildstats.info/nuget/Xamarin.Android.PdfBox)](https://www.nuget.org/packages/Xamarin.Android.PdfBox)

Xamarin bindings library for [PdfBox-Android] from Tom Roush.

Allows creation of new PDF documents, manipulation of existing<br/>
documents and the ability to extract content from documents.

Usage
=====

Before calls to PDFBox are made it is **highly** recommended to initialize the library's resource loader.<br/>
Add the following line before calling PDFBox methods:

```csharp
PDFBoxResourceLoader.Init(Application.Context);
```

An example app is located in the `Sample` directory and includes examples of common tasks.

Important notes
===============

-Currently based on PdfBox-Android v2.0.13.0<br/>
-Requires API 19 or greater for full functionality

License
=======

I am not associated with either [PdfBox-Android] or [PdfBox].<br/>
All rights belong to their respective owners.

**Xamarin.Android.PdfBox** is licensed under [Apache-2.0][Apache-2.0]

[Apache-2.0]: http://www.apache.org/licenses/LICENSE-2.0.html
[Nuget]: https://www.nuget.org/packages/Xamarin.Android.PdfBox
[PdfBox]: http://pdfbox.apache.org
[PdfBox-Android]: https://github.com/TomRoush/PdfBox-Android