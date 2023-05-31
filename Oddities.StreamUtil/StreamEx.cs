namespace Oddities.StreamUtil;

internal static class StreamEx
{
    public static ushort ReadCompressedUInt16(this BinaryReader input)
    {
        checked
        {
            var value = (ushort)input.ReadByte();
            if ((value & 1) != 0)
            {
                var highByte = input.ReadByte();
                value |= (ushort)(highByte << 8);
            }

            return (ushort)(value / 2);
        }
    }

    public static uint ReadCompressedUInt32(this BinaryReader input)
    {
        checked
        {
            var value = (uint)input.ReadUInt16();
            if ((value & 1) != 0)
            {
                var highUInt16Le = input.ReadUInt16();
                value |= (uint)(highUInt16Le << 16);
            }

            return (ushort)(value / 2);
        }
    }
}
