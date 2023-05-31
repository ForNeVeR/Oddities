using System.Globalization;
using JetBrains.Annotations;

namespace Oddities.WinHelp;

[PublicAPI]
public struct SystemHeader
{
    public byte Magic;
    public byte Version;
    public byte Revision;
    public byte Always0;
    public ushort Always1;
    public uint GenDate;
    public ushort Flags;

    public static SystemHeader Load(BinaryReader input)
    {
        SystemHeader header;
        header.Magic = input.ReadByte();
        header.Version = input.ReadByte();
        header.Revision = input.ReadByte();
        header.Always0 = input.ReadByte();
        header.Always1 = input.ReadByte();
        header.GenDate = input.ReadUInt32();
        header.Flags = input.ReadUInt16();

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
