using JetBrains.Annotations;
using Oddities.StreamUtil;

namespace Oddities.MRB;

[PublicAPI]
public enum ImageType : byte
{
    Bmp = 0x06,
    Wmf = 0x08
}

[PublicAPI]
public enum CompressionType : byte
{
    None = 0,
    Rle = 1,
    Lz77 = 2
}

[PublicAPI]
public struct ShgImageHeader
{
    public ImageType Type;
    public CompressionType Compression;
    public ushort Dpi;
    public long DataOffset;

    public static ShgImageHeader Read(BinaryReader input)
    {
        ShgImageHeader header;
        header.Type = (ImageType)input.ReadByte();
        header.Compression = (CompressionType)input.ReadByte();
        header.Dpi = input.ReadCompressedUInt16();
        header.DataOffset = input.BaseStream.Position;
        return header;
    }
}
