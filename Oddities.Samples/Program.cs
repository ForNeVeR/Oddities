// See https://aka.ms/new-console-template for more information

using System.Text;
using Oddities.MRB;
using Oddities.NE;
using Oddities.Resources;
using Oddities.WinHelp;
using Oddities.WinHelp.Fonts;
using Oddities.WinHelp.Topics;
using Oxage.Wmf;
using Oxage.Wmf.Records;

Console.WriteLine("Hello, World!");

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

byte[] DibSample(byte[] input)
{
    var dib = new Dib(input);
    var (w, h) = (dib.Width, dib.Height);
    var (r, g, b) = dib.GetPixel(0, 0);
    return dib.AsBmp(); // full bitmap data, may be saved to a BMP file
}

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
