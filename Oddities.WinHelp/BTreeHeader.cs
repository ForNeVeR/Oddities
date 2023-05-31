using JetBrains.Annotations;

namespace Oddities.WinHelp;

[PublicAPI]
[Flags]
public enum BTreeFlags : ushort
{
    Default = 0x0002,
    Directory = 0x0400
}

[PublicAPI]
public struct BTreeHeader
{
    private const int StructureSize = 16;

    public ushort Magic;
    public BTreeFlags Flags;
    public ushort PageSize;
    public unsafe fixed byte Structure[StructureSize];
    public short FirstLeaf;
    public short PageSplits;
    public short RootPage;
    public short FirstFree;
    public short TotalPages;
    public short NLevels;
    public uint TotalHfsEntries;

    public static unsafe BTreeHeader Load(BinaryReader data)
    {
        BTreeHeader header;
        header.Magic = data.ReadUInt16();
        header.Flags = (BTreeFlags)data.ReadUInt16();
        header.PageSize = data.ReadUInt16();

        var structure = new Span<byte>(header.Structure, StructureSize);
        data.BaseStream.ReadExactly(structure);

        header.FirstLeaf = data.ReadInt16();
        header.PageSplits = data.ReadInt16();
        header.RootPage = data.ReadInt16();
        header.FirstFree = data.ReadInt16();
        header.TotalPages = data.ReadInt16();
        header.NLevels = data.ReadInt16();
        header.TotalHfsEntries = data.ReadUInt32();

        return header;
    }
}
