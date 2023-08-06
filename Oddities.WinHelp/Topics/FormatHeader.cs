using JetBrains.Annotations;
using Oddities.StreamUtil;

namespace Oddities.WinHelp.Topics;

[PublicAPI]
public record struct FormatHeader
(
     ushort FormatSize,
     byte Flags,
     ushort DataSize
)
{
    public static FormatHeader Read(BinaryReader input)
    {
        FormatHeader header = new FormatHeader();
        header.FormatSize = input.ReadCompressedUInt16();
        header.Flags = input.ReadByte();
        header.DataSize = input.ReadCompressedUInt16();
        return header;
    }
}
