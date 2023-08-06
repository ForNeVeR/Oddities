using JetBrains.Annotations;

namespace Oddities.MRB;

[PublicAPI]
public record struct ShgFileHeader(

     short ObjectCount,
     uint[] ObjectOffsets
    )
{
    public static ShgFileHeader Read(BinaryReader input)
    {
        Span<byte> magic = stackalloc byte[2];
        input.BaseStream.ReadExactly(magic);
        if (!magic.SequenceEqual("lp"u8)) throw new Exception("Unexpected magic bytes");

        ShgFileHeader header = new ShgFileHeader();
        header.ObjectCount = input.ReadInt16();
        header.ObjectOffsets = new uint[header.ObjectCount];
        for (var i = 0; i < header.ObjectCount; i++)
        {
            header.ObjectOffsets[i] = input.ReadUInt32();
        }
        return header;
    }


}

