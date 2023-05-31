using JetBrains.Annotations;

namespace Oddities.WinHelp.Topics;

[PublicAPI]
public struct TopicFile
{
    private readonly BinaryReader _data;
    private readonly TopicBlockHeader _header;
    
    public TopicFile(BinaryReader data, TopicBlockHeader header)
    {
        _data = data;
        _header = header;
    }

    public static TopicFile Load(BinaryReader input)
    {
        var header = TopicBlockHeader.Load(input);
        
        return new(input, header);
    }

    public List<Paragraph> ReadParagraphs()
    {
        var paragraphs = new List<Paragraph>();
        var ptr = _header.TopicData;
        while (ptr != -1)
        {
            _data.BaseStream.Position = ptr;
            var paragraph = Paragraph.Load(_data);
            paragraphs.Add(paragraph);
            ptr = paragraph.NextPara;
        }

        return paragraphs;
    }
}
