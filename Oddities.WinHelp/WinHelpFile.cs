using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace Oddities.WinHelp;

/// <remarks>
/// <para>[1]: http://www.oocities.org/mwinterhoff/helpfile.htm</para>
/// <para>[2]: P. Davis and M. Wallace — Windows Undocumented File Formats.</para>
/// </remarks>
[PublicAPI]
public struct WinHelpFile
{
    private const int Magic = 0x35F3F;

    private readonly BinaryReader _data;

    private readonly int _hfsOffset;

    /// <remarks>This is marked as "Reserved, -1" in [2], but has more documentation in [1].</remarks>
    private readonly int _firstFreeBlock;

    private readonly int _entireFileSize;

    public WinHelpFile(BinaryReader data, int hfsOffset, int firstFreeBlock, int entireFileSize)
    {
        _data = data;
        _hfsOffset = hfsOffset;
        _firstFreeBlock = firstFreeBlock;
        _entireFileSize = entireFileSize;
    }

    public static WinHelpFile Load(BinaryReader input)
    {
        var magic = input.ReadInt32();
        if (magic != Magic)
        {
            throw new Exception(
                $"Expected magic sequence {Magic.ToString("x", CultureInfo.InvariantCulture)}, " +
                $"got {magic.ToString("x", CultureInfo.InvariantCulture)}.");
        }

        var hfsOffset = input.ReadInt32();
        var firstFreeBlock = input.ReadInt32();
        var entireFileSize = input.ReadInt32();

        return new WinHelpFile(input, hfsOffset, firstFreeBlock, entireFileSize);
    }

    public DirectoryIndexEntry[] GetFiles(Encoding fileNameEncoding)
    {
        _data.BaseStream.Position = _hfsOffset;
        var hfs = HfsEntry.Load(_data);
        if (hfs.FileType != HfsFileType.Hfs) throw new Exception($"Unexpected root file entry: {hfs.FileType}.");

        var bTreeHeader = BTreeHeader.Load(_data);
        if (bTreeHeader.Magic != 0x293B) throw new Exception($"Unexpected BTreeHeader signature: {bTreeHeader.Magic}.");

        if (bTreeHeader.NLevels != 1) throw new Exception($"NLevels = {bTreeHeader.NLevels} is not expected, expected 1.");
        if (bTreeHeader.RootPage != 0) throw new Exception($"RootPage = {bTreeHeader.RootPage} is not expected, expected 0.");

        var bTreeIndexHeader = BTreeIndexHeader.Load(_data, fileNameEncoding);
        return bTreeIndexHeader.Entries;
    }

    public byte[] ReadFile(DirectoryIndexEntry entry)
    {
        _data.BaseStream.Position = entry.FileOffset;
        var file = HfsEntry.Load(_data);
        if (file.FileType != HfsFileType.Normal) throw new Exception($"Abnormal HFS entry type: {file.FileType}.");
        
        var buffer = new byte[file.UsedSpace];
        _data.BaseStream.ReadExactly(buffer);
        return buffer;
    }
}
