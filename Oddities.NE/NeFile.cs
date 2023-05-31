using JetBrains.Annotations;

namespace Oddities.NE;

/// <remarks>https://jeffpar.github.io/kbarchive/kb/065/Q65122/</remarks>
[PublicAPI]
public class NeFile
{
    private readonly BinaryReader _input;
    private readonly ushort _segmentedHeaderOffset;
    private readonly ushort _resourceTableOffset;
    private readonly ushort _resourceAlignmentShiftCount;

    public NeFile(
        BinaryReader input,
        ushort segmentedHeaderOffset,
        ushort resourceTableOffset,
        ushort resourceAlignmentShiftCount)
    {
        _input = input;
        _segmentedHeaderOffset = segmentedHeaderOffset;
        _resourceTableOffset = resourceTableOffset;
        _resourceAlignmentShiftCount = resourceAlignmentShiftCount;
    }

    public static NeFile ReadFrom(BinaryReader input)
    {
        var stream = input.BaseStream;
        stream.Position = 0x3CL;
        var segmentedHeaderOffset = input.ReadUInt16();
        stream.Position = segmentedHeaderOffset;

        Span<byte> signature = stackalloc byte[2];
        stream.ReadExactly(signature);
        if (!signature.SequenceEqual("NE"u8))
            throw new Exception(
                $"""Invalid NE file signature: "{(char)signature[0]}{(char)signature[1]}" instead of "NE".""");

        // Read resource table offset at header + 0x24:
        stream.Position = segmentedHeaderOffset + 0x24;
        var resourceTableOffset = input.ReadUInt16();

        // The resource entry number is supposed to be placed at header + 0x34, though it's always zero in
        // the files I examined. Let's ignore it: the resource table has its own end marker (typeId == 0).
        //
        // Notably, eXeScope calls the same field "Number of Reserved Segment".

        // Read resource alignment shift count at resource table offset:
        stream.Position = segmentedHeaderOffset + resourceTableOffset;
        var alignmentShiftCount = input.ReadUInt16();

        return new NeFile(input, segmentedHeaderOffset, resourceTableOffset, alignmentShiftCount);
    }

    public IEnumerable<NeResourceType> ReadResourceTable()
    {
        var stream = _input.BaseStream;
        stream.Position = _segmentedHeaderOffset + _resourceTableOffset + 2;
        while (true)
        {
            var typeId = _input.ReadUInt16();
            if ((typeId & (1 << 16)) != 0)
                throw new Exception($"Non-integer resource type id is not supported: {typeId}.");

            if (typeId == 0) yield break;

            var resourceCount = _input.ReadUInt16();
            stream.Position += 4; // DD Reserved

            var resources = new NeResource[resourceCount];
            for (var r = 0; r < resourceCount; ++r)
            {
                resources[r] = ReadResource();
            }

            yield return new NeResourceType(typeId, resources);
        }
    }

    public byte[] ReadResourceContent(NeResource resource)
    {
        var alignment = 1 << _resourceAlignmentShiftCount;
        var offset = resource.ContentOffsetInAlignments * alignment;
        var length = resource.ContentLength * alignment;

        _input.BaseStream.Position = offset;
        return _input.ReadBytes(length);
    }

    private NeResource ReadResource()
    {
        var offset = _input.ReadUInt16();
        var length = _input.ReadUInt16();

        // DW Flag word
        // DW Resource ID
        // DD Reserved
        _input.BaseStream.Position += 8;

        return new NeResource(offset, length);
    }
}
