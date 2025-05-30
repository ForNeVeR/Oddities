using System.Text;
using JetBrains.Annotations;

namespace Oddities.WinHelp;

[PublicAPI]
public record struct BTreeIndexHeader(
    ushort Unused,
    short NEntries,
    short PreviousPage,
    short NextPage,
    DirectoryIndexEntry[] Entries
)
{
    public static BTreeIndexHeader Load(BinaryReader data, Encoding fileNameEncoding)
    {
        var unused = data.ReadUInt16();
        var nEntries = data.ReadInt16();
        var previousPage = data.ReadInt16();
        var nextPage = data.ReadInt16();
        var entries = new DirectoryIndexEntry[nEntries];

        for (var i = 0; i < nEntries; ++i)
            entries[i] = DirectoryIndexEntry.Load(data, fileNameEncoding);

        return new BTreeIndexHeader(unused, nEntries, previousPage, nextPage, entries);
    }
}


[PublicAPI]
public struct DirectoryIndexEntry
{
    public string FileName;
    public int FileOffset;

    public static DirectoryIndexEntry Load(BinaryReader data, Encoding fileNameEncoding)
    {
        var stream = data.BaseStream;
        var start = stream.Position;
        while (data.ReadByte() != 0)
        {
        }
        var end = stream.Position;

        var buffer = new byte[end - start];
        stream.Position = start;
        stream.ReadExactly(buffer);

        return new DirectoryIndexEntry
        {
            // -1 for terminating zero
            FileName = fileNameEncoding.GetString(buffer, 0, buffer.Length - 1),
            FileOffset = data.ReadInt32()
        };
    }
}
