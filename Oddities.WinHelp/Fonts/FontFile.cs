using JetBrains.Annotations;

namespace Oddities.WinHelp.Fonts;

[PublicAPI]
public class FontFile
{
    private readonly BinaryReader _data;
    private readonly FontHeader _header;
    public FontFile(BinaryReader data, FontHeader header)
    {
        _data = data;
        _header = header;
    }

    public static FontFile Load(BinaryReader stream)
    {
        var header = FontHeader.Read(stream);
        return new FontFile(stream, header);
    }

    public FontDescriptor[] ReadDescriptors()
    {
        _data.BaseStream.Position = _header.DescriptorsOffset;
        var result = new FontDescriptor[_header.NumDescriptors];
        for (var i = 0; i < _header.NumDescriptors; ++i)
        {
            result[i] = FontDescriptor.Read(_data);
        }
        return result;
    }
}
