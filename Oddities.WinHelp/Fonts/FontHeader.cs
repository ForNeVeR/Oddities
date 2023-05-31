using JetBrains.Annotations;

namespace Oddities.WinHelp.Fonts;

[PublicAPI]
public struct FontHeader
{
    public ushort NumFonts;
    public ushort NumDescriptors;
    public ushort DefDescriptor;
    public ushort DescriptorsOffset;

    public static FontHeader Read(BinaryReader input)
    {
        FontHeader header;
        header.NumFonts = input.ReadUInt16();
        header.NumDescriptors = input.ReadUInt16();
        header.DefDescriptor = input.ReadUInt16();
        header.DescriptorsOffset = input.ReadUInt16();
        return header;
    }
}
