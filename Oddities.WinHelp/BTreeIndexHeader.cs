using System.Text;
using JetBrains.Annotations;

namespace Oddities.WinHelp;

[PublicAPI]
public struct BTreeIndexHeader
{
    public ushort Unused;
    public short NEntries;
    public short PreviousPage;
    public short NextPage;
    public DirectoryIndexEntry[] Entries;

    public static BTreeIndexHeader Load(BinaryReader data, Encoding fileNameEncoding)
    {
        BTreeIndexHeader header;
        header.Unused = data.ReadUInt16();
        header.NEntries = data.ReadInt16();
        header.PreviousPage = data.ReadInt16();
        header.NextPage = data.ReadInt16();

        header.Entries = new DirectoryIndexEntry[header.NEntries];
        for (var i = 0; i < header.NEntries; ++i)
            header.Entries[i] = DirectoryIndexEntry.Load(data, fileNameEncoding);

        return header;
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
