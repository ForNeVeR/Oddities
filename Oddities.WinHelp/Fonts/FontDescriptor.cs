using JetBrains.Annotations;

namespace Oddities.WinHelp.Fonts;

[PublicAPI]
[Flags]
public enum FontAttributes : byte
{
    Normal = 0x00,
    Bold = 0x01,
    Italic = 0x02,
    Underline = 0x04,
    Strikethrough = 0x08,
    DoubleUnderline = 0x10,
    SmallCaps = 0x20
}

[PublicAPI]
public struct RgbTriple
{
    public byte B;
    public byte G;
    public byte R;
}

[PublicAPI]
public struct FontDescriptor
{
    public FontAttributes Attributes;
    public byte HalfPoints;
    public byte FontFamily;
    public byte FontName;
    public byte UnknownZero;
    public RgbTriple ScrollingRegionColor;
    public RgbTriple NonScrollingRegionColor;

    public static FontDescriptor Read(BinaryReader input)
    {
        FontDescriptor descriptor;
        descriptor.Attributes = (FontAttributes)input.ReadByte();
        descriptor.HalfPoints = input.ReadByte();
        descriptor.FontFamily = input.ReadByte();
        descriptor.FontName = input.ReadByte();
        descriptor.UnknownZero = input.ReadByte();
        descriptor.ScrollingRegionColor.B = input.ReadByte();
        descriptor.ScrollingRegionColor.G = input.ReadByte();
        descriptor.ScrollingRegionColor.R = input.ReadByte();
        descriptor.NonScrollingRegionColor.B = input.ReadByte();
        descriptor.NonScrollingRegionColor.G = input.ReadByte();
        descriptor.NonScrollingRegionColor.R = input.ReadByte();
        return descriptor;
    }
}
