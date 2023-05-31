using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Oxage.Wmf;

namespace Oddities.MRB;

[PublicAPI]
public class MrbFile
{
    private readonly BinaryReader _input;
    private readonly ShgFileHeader _header;

    public MrbFile(BinaryReader input, ShgFileHeader header)
    {
        _input = input;
        _header = header;
    }

    public static MrbFile Load(BinaryReader input)
    {
        var header = ShgFileHeader.Read(input);
        return new MrbFile(input, header);
    }

    public short ImageCount => _header.ObjectCount;

    public ShgImageHeader ReadImage(int index)
    {
        var offset = _header.ObjectOffsets[index];
        _input.BaseStream.Position = offset;
        return ShgImageHeader.Read(_input);
    }

    public WmfDocument ReadWmfDocument(ShgImageHeader imageHeader)
    {
        if (imageHeader.Type != ImageType.Wmf) throw new Exception("Only WMF images are supported.");

        _input.BaseStream.Position = imageHeader.DataOffset;
        var metafileHeader = ShgMetafileHeader.Read(_input);

        using var result = new MemoryStream();
        using var writer = new BinaryWriter(result, Encoding.UTF8, leaveOpen: true);

        // Write WMF file header:
        writer.Write(new byte[] { 0xD7, 0xCD, 0xC6, 0x9A }); // magic
        writer.Write((ushort)0); // no idea

        writer.Write((ushort)0); // left
        writer.Write((ushort)0); // top
        writer.Write(metafileHeader.Width);
        writer.Write(metafileHeader.Height);
        writer.Write((ushort)2540); // default DPI for WMF
        writer.Write(0); // reserved

        var checksum = CalculateChecksum(result);
        result.Position = result.Length;
        writer.Write(checksum);

        if (imageHeader.Compression != CompressionType.Rle)
            throw new Exception($"Compression type {imageHeader.Compression} is not supported.");

        DecompressRle(metafileHeader.CompressedDataSize, writer);

        result.Position = 0;
        var doc = new WmfDocument();
        doc.Load(result);
        return doc;

        ushort CalculateChecksum(Stream stream)
        {
            var position = stream.Position;
            stream.Position = 0L;

            const int length = 20;

            Span<byte> underChecksum = stackalloc byte[length];
            stream.ReadExactly(underChecksum);

            var grouped = MemoryMarshal.Cast<byte, ushort>(underChecksum);

            ushort sum = 0;
            for (var i = 0; i < length / 2; ++i)
            {
                sum ^= grouped[i];
            }

            stream.Position = position;
            return sum;
        }
    }

    public void DecompressRle(uint compressedDataSize, BinaryWriter output)
    {
        var bytesRead = 0;
        while (bytesRead < compressedDataSize)
        {
            var count = _input.ReadByte();
            ++bytesRead;
            if ((count & 0x80) != 0)
            {
                count -= 0x80;
                while (count-- > 0)
                {
                    var data = _input.ReadByte();
                    ++bytesRead;
                    output.Write(data);
                }
            }
            else
            {
                count = (byte)(count & 0x7F);
                var data = _input.ReadByte();
                ++bytesRead;
                for (var i = 0; i < count; ++i)
                    output.Write(data);
            }
        }
    }
}
