using JetBrains.Annotations;

namespace Oddities.WinHelp.Topics;

[PublicAPI]
[Flags]
public enum ParagraphSetup
{
    SpaceBefore = 0x00020000,
    SpaceAfter = 0x00040000,
    LineSpacingBefore = 0x00080000,
    LeftMarginIndent = 0x00100000,
    RightMarginIndent = 0x00200000,
    FirstLineIndent = 0x00400000,
    ParagraphBorder = 0x01000000,
    TabSettingInformation = 0x02000000,
    RightJustify = 0x04000000,
    CenterJustify = 0x08000000,
    NoWrap = 0x10000000
}

[PublicAPI]
[Flags]
public enum ParagraphBorder
{
    DottedBorder = 0x80,
    DoubleBorder = 0x40,
    ThickBorder = 0x20,
    RightBorder = 0x10,
    BottomBorder = 0x08,
    LeftBorder = 0x04,
    TopBorder = 0x02,
    BoxedBorder = 0x01
}

[PublicAPI]
public enum BitmapAlignment
{
    Current,
    Left,
    Right
}

[PublicAPI]
public struct ParagraphSettings
{
    public ParagraphSetup Setup;
    public ParagraphBorder? Border;

    public static ParagraphSettings Load(BinaryReader data)
    {
        ParagraphSettings settings;
        settings.Setup = (ParagraphSetup)data.ReadInt32();
        if ((settings.Setup & ParagraphSetup.ParagraphBorder) != 0)
        {
            var header = data.ReadByte();
            if (header != 0x01) throw new Exception($"Paragraph border header: {header:x}, expected: 0x01.");
            settings.Border = (ParagraphBorder)data.ReadByte();
            var footer = data.ReadByte();
            if (footer != 0x51) throw new Exception($"Paragraph border footer: {footer:x}, expected: 0x51.");
        }
        else
        {
            settings.Border = null;
        }

        return settings;
    }
}

[PublicAPI]
public interface IParagraphItem {}

[PublicAPI]
public record struct ParagraphText(string Text) : IParagraphItem;

[PublicAPI]
public record struct FontChange(ushort FontDescriptor) : IParagraphItem;
[PublicAPI]
public record struct NewLine : IParagraphItem;
[PublicAPI]
public record struct NewParagraph : IParagraphItem;
[PublicAPI]
public record struct Tab : IParagraphItem;

[PublicAPI]
public record struct Bitmap(BitmapAlignment Alignment, ushort Number) : IParagraphItem
{
    public static Bitmap Read(BinaryReader input, BitmapAlignment alignment)
    {
        var subtype = input.ReadByte();
        if (subtype != 0x22) throw new Exception("Bitmap types other than 0x22 are not supported.");
        if (input.ReadByte() != 0x08)  throw new Exception("Bitmap should've been followed by a byte 0x08.");
        if (input.ReadByte() != 0x80)  throw new Exception("Bitmap should've been followed by a byte 0x08.");
        if (input.ReadByte() != 0x02)  throw new Exception("Bitmap should've been followed by a byte 0x02.");

        var embedFlag = input.ReadUInt16();
        if (embedFlag == 1) throw new Exception("Embedded bitmaps are not supported, yet.");

        var bitmapNumber = input.ReadUInt16();

        return new Bitmap(alignment, bitmapNumber);
    }
}

[PublicAPI]
public record ParagraphItems(
    ParagraphSettings Settings,
    IReadOnlyList<IParagraphItem> Items
);
