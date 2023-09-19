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

        var objectCount = input.ReadInt16();
        var objectOffsets = new uint[objectCount];

        for (var i = 0; i < objectCount; i++)
        {
            objectOffsets[i] = input.ReadUInt32();
        }

        return new()
        {
            ObjectCount = objectCount,
            ObjectOffsets = objectOffsets,
        };
    }


}

