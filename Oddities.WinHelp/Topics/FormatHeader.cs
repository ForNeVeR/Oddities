using JetBrains.Annotations;
using Oddities.StreamUtil;

namespace Oddities.WinHelp.Topics;

[PublicAPI]
public struct FormatHeader
{
    public ushort FormatSize;
    public byte Flags;
    public ushort DataSize;

    public static FormatHeader Read(BinaryReader input)
    {
        FormatHeader header;
        header.FormatSize = input.ReadCompressedUInt16();
        header.Flags = input.ReadByte();
        header.DataSize = input.ReadCompressedUInt16();
        return header;
    }
}
