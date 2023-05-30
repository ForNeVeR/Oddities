Oddities [![Status Zero][status-zero]][andivionian-status-classifier]
========
This repository groups several .NET libraries supporting old and obscure data formats.

Currently, the following data formats are supported:
- [NE (New Executable)][wikipedia.ne], 16-bit `.exe` Windows binary.
- [DIB (Device-Independent Bitmap)][microsoft.dib], encountered as part of the BMP format and in the NE resource table.
- [Windows Help File format][docs.winhelp] (aka WinHelp aka `.hlp`) and accompanying formats (often stored in a `.hlp` file):
  - [MRB (Multi-Resolution Bitmap)][file-info.mrb],
  - [SHG (Segmented Hyper-Graphic)][file-info.shg].

If you encounter a case not handled by the library, don't hesitate to [open an issue][issues]! 

Read the corresponding sections below for each part of the library suite.

Documentation
-------------
- [Contributor Guide][docs.contributing]
- [License (MIT)][docs.license]
- [Code of Conduct (adapted from the Contributor Covenant)][docs.code-of-conduct]

Oddities.DIB
------------
This library provides some helpful functions to work with the [DIB (Device-Independent Bitmap)][microsoft.dib] image format, often encountered as part of bigger bitmap formats or in resource sections of executable files, such as NE and PE (Portable Executable).

Here's an example of DIB data processing using the library:

```csharp
byte[] DibSample(byte[] input)
{
    var dib = new Dib(input);
    var (w, h) = (dib.Width, dib.Height);
    var (r, g, b) = dib.GetPixel(0, 0);
    return dib.AsBmp(); // full bitmap data, may be saved to a BMP file
}
```

### Known Limitations
- Only 1, 4, and 8-bit palettes are supported.

Oddities.MRB
------------
This library supports reading of [MRB (Multi-Resolution Bitmap)][file-info.mrb] and [SHG (Segmented Hyper-Graphic)][file-info.shg] image formats. Most often, these files are encountered during reading of `.HLP` files using Oddities.WilHelp.

This library has been extracted into a separate package mainly because of different dependencies: it uses a third-party package to read WMF files (that may be part of SHG).

Here's a complex example of how to read an MRB file, extract SHG from it, then extract WMF from SHG, and finally extract a DIB from said SHG (that nesting did actually happen!):

```csharp
Dib ExtractDibFromWmfInsideMrb(string mrbPath)
{
    using var stream = new FileStream(mrbPath, FileMode.Open);
    var file = MrbFile.Load(stream);
    if (file.ImageCount != 1) throw new Exception("Too many images in the file");
    var image = file.ReadImage(0);
    WmfDocument wmf = file.ReadWmfDocument(image);

    // Now, for example, let's extract the DIB record from the file, in case the file only stores a single bitmap:
    var record = wmf.Records.OfType<WmfStretchDIBRecord>().Single();
    using var dibStream = new MemoryStream();
    var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
    record.DIB.Write(writer);
    return new Dib(dibStream.ToArray());
}
```

### Known Limitations
- Only WMF data is supported inside MRB, no BMP support.
- Only RLE compression is supported.

Oddities.NE
-----------
Library supporting the [NE (New Executable)][wikipedia.ne] file format (usually stored as `.exe`), the 16-bit executable file format used in Windows 1.0–3.x and some other operating systems.

The library is currently focused on reading the resource sections of NE files.

See [Microsoft's documentation on the format][microsoft.ne].

Here's an example function that will read all the resources of type `32770u` from a NE file and wrap every resource into a DIB.

```csharp
Dib[] ReadImagesFromNeFile(string nePath)
{
    using var stream = new FileStream(nePath, FileMode.Open, FileAccess.Read);
    var neFile = NeFile.ReadFrom(stream);
    var resources = neFile.ReadResourceTable();
    var bitmapResourceType = resources.Single(x => x.TypeId == 32770u);
    return bitmapResourceType.Resources
        .Select(neFile.ReadResourceContent)
        .Select(x => new Dib(x))
        .ToArray();
}
```

Oddities.WinHelp
----------------
This library supports reading of Windows Help files, aka WinHelp, aka `.hlp`.

Since the Windows Help format is so complex, only certain features are supported.

Here's what's supported:
- Reading the Windows Help file's HFS (Hierarchical File System) data and navigating between the files.
- Reading each HFS entry's raw contents.
- For `|bm` files, reading them using Oddities.MRB library.
- For the `|SYSTEM` file, reading and partial validation of the header.
- For the `|FONT` file, reading of the font descriptors.
- For the `|TOPIC` file, iterating over the paragraphs and dumping the text records of text paragraphs.

The following example demonstrates most of the library's capabilities:

```csharp
void DumpWinHelpFile(string hlpPath, string outDir)
{
    using var input = new FileStream(hlpPath, FileMode.Open, FileAccess.Read);
    var file = WinHelpFile.Load(input);
    var dibs = new List<Dib>();
    foreach (var entry in file.GetFiles(Encoding.UTF8))
    {
        Console.WriteLine(entry.FileName);
        var fileName = entry.FileName.Replace("|", "_");
        var outputName = Path.Combine(outDir, fileName);
        var bytes = file.ReadFile(entry);
        File.WriteAllBytes(outputName, bytes);

        if (entry.FileName.StartsWith("|bm"))
        {
            // Here, you could extract DIB from WMF images, but I'm too lazy to update the signature of
            // ExtractDibFromWmfInsideMrb to make it work with bytes. Just imagine it works.
            
            // var dib = ExtractDibFromWmfInsideMrb(bytes);
            // dibs.Add(dib);
        }
        else if (entry.FileName == "|SYSTEM")
        {
            using var stream = new MemoryStream(bytes);
            var header = SystemHeader.Load(stream);
            Console.WriteLine(" - SystemHeader ok.");
        }
        else if (entry.FileName == "|FONT")
        {
            using var stream = new MemoryStream(bytes);
            var fontFile = FontFile.Load(stream);
            Console.WriteLine(" - Font ok.");

            foreach (var descriptor in fontFile.ReadDescriptors())
                Console.WriteLine($" - - Font descriptor: {descriptor.Attributes}");
        }
        else if (entry.FileName == "|TOPIC")
        {
            using var stream = new MemoryStream(bytes);
            var topic = TopicFile.Load(stream);
            Console.WriteLine(" - Topic ok.");

            var i = 0;
            foreach (var p in topic.ReadParagraphs())
            {
                Console.WriteLine($" - Paragraph {p} ({p.DataLen1}, {p.DataLen2}) ok.");

                var out1 = outputName + $"{i}.1";
                Console.WriteLine($" - - Paragraph data: {out1}");
                File.WriteAllBytes(out1, p.ReadData1());

                var out2 = outputName + $"{i}.2";
                Console.WriteLine($" - - Paragraph data: {out2}");
                File.WriteAllBytes(out2, p.ReadData2());

                if (p.RecordType == ParagraphRecordType.TextRecord)
                {
                    var items = p.ReadItems(Encoding.GetEncoding(1251));
                    Console.WriteLine($"- - Items: {items.Settings}");
                    foreach (var item in items.Items)
                    {
                        Console.WriteLine($"- - - {item}");
                    }
                }

                ++i;
            }
        }
    }

    // Also, you may dump dibs here.
}
```

### Known Limitations
- Not supporting any image types other than `0x22` in paragraphs.
- Not supporting embedded bitmaps (only the external ones that are stored in their own HFS entries).

Project History
---------------
This project started as part of the game reimplementation [O21][o21]. The original game is an old Windows game (from the 3.1 era), and so it was necessary to implement several old Windows data formats to load the game data properly.

Since then, the data format support was extracted into a separate library suite that lives in this repository.

Acknowledgments
---------------
- For the documentation on WinHelp, MRB, and SHG, we'd like to thank:
  - Pete Davis and Mike Wallace, the authors of [Windows Undocumented File Formats][book.windows-undocumented-file-formats],
  - Manfred Winterhoff, the author of [the documentation][docs.winhelp],
  - Paul Wise and other contributors of [helpdeco][].

[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier#status-zero-
[book.windows-undocumented-file-formats]: https://a.co/d/dq5fCoj
[docs.code-of-conduct]: CODE_OF_CONDUCT.md
[docs.contributing]: CONTRIBUTING.md
[docs.license]: LICENSE.md
[docs.winhelp]: http://www.oocities.org/mwinterhoff/helpfile.htm
[file-info.mrb]: https://fileinfo.com/extension/mrb
[file-info.shg]: https://fileinfo.com/extension/shg
[helpdeco]: https://github.com/pmachapman/helpdeco
[issues]: https://github.com/ForNeVeR/Oddities/issues 
[microsoft.dib]: https://learn.microsoft.com/en-us/windows/win32/gdi/device-independent-bitmaps
[microsoft.ne]: https://jeffpar.github.io/kbarchive/kb/065/Q65122/
[o21]: https://github.com/ForNeVeR/O21
[status-zero]: https://img.shields.io/badge/status-zero-lightgrey.svg
[wikipedia.ne]: https://en.wikipedia.org/wiki/New_Executable
