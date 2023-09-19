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
        return new()
        {
            FormatSize = input.ReadCompressedUInt16(),
            Flags = input.ReadByte(),
            DataSize = input.ReadCompressedUInt16()
        };
    }
}
