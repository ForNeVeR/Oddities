using JetBrains.Annotations;

namespace Oddities.WinHelp.Topics;

[PublicAPI]
public record struct TopicBlockHeader
(
    int LastParagraph,
    int TopicData,
    int LastTopicHeader
)
{
    public static TopicBlockHeader Load(BinaryReader input)
    {
        TopicBlockHeader header = new TopicBlockHeader();
        header.LastParagraph = input.ReadInt32();
        header.TopicData = input.ReadInt32();
        header.LastTopicHeader = input.ReadInt32();
        return header;
    }
}
