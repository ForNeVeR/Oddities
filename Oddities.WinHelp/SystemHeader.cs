using System.Globalization;
using JetBrains.Annotations;

namespace Oddities.WinHelp;

[PublicAPI]
public record struct SystemHeader
(
       byte Magic,
       byte Version,
       byte Revision,
       byte Always0,
       ushort Always1,
       uint GenDate,
       ushort Flags
)

{
    public static SystemHeader Load(BinaryReader input)
    {
        SystemHeader header = new()
        {
            Magic = input.ReadByte(),
            Version = input.ReadByte(),
            Revision = input.ReadByte(),
            Always0 = input.ReadByte(),
            Always1 = input.ReadByte(),
            GenDate = input.ReadUInt32(),
            Flags = input.ReadUInt16()
        };

        if (header.Magic != 0x6C)
            throw new Exception(
                $"Magic: expected to be 0x6C, actual {header.Magic.ToString("x", CultureInfo.InvariantCulture)}");

        if (header.Version != 3)
            throw new Exception(
                $"Version: expected to be 3, actual {header.Version.ToString(CultureInfo.InvariantCulture)}");

        if (header.Always0 != 0)
            throw new Exception(
                $"Always0: expected to be 0, actual {header.Always0.ToString(CultureInfo.InvariantCulture)}");

        if (header.Always1 != 1)
            throw new Exception(
                $"Always1: expected to be 1, actual {header.Always0.ToString(CultureInfo.InvariantCulture)}");

        return header;
    }
}
