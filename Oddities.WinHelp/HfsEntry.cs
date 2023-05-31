using JetBrains.Annotations;

namespace Oddities.WinHelp;

[PublicAPI]
internal enum HfsFileType
{
    Normal = 0,
    /// <summary>Entry of the HFS itself.</summary>
    Hfs = 4
}

[PublicAPI]
internal struct HfsEntry
{
    public int ReservedSpace;
    public int UsedSpace;
    public HfsFileType FileType;

    public static HfsEntry Load(BinaryReader input) => new()
    {
        ReservedSpace = input.ReadInt32(),
        UsedSpace = input.ReadInt32(),
        FileType = (HfsFileType)input.ReadByte()
    };
}
