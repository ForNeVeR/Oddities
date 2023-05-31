using JetBrains.Annotations;
using Oddities.StreamUtil;

namespace Oddities.MRB;

[PublicAPI]
public struct ShgMetafileHeader
{
    public ushort Width;
    public ushort Height;
    public uint UncompressedDataSize;
    public uint CompressedDataSize;
    public uint HotSpotDataSize;
    public uint Unknown; // documented as dwPictureOffset in splitmrb of helpdeco
    public uint HotSpotOffset;

    public static ShgMetafileHeader Read(BinaryReader input)
    {
        ShgMetafileHeader header;
        header.Width = input.ReadUInt16();
        header.Height = input.ReadUInt16();
        header.UncompressedDataSize = input.ReadCompressedUInt32();
        header.CompressedDataSize = input.ReadCompressedUInt32();
        header.HotSpotDataSize = input.ReadCompressedUInt32();
        header.Unknown = input.ReadUInt32();
        header.HotSpotOffset = input.ReadUInt32();
        return header;
    }
}
