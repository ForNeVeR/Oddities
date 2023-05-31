using JetBrains.Annotations;

namespace Oddities.WinHelp.Topics;

[PublicAPI]
public struct TopicBlockHeader
{
    public int LastParagraph;
    public int TopicData;
    public int LastTopicHeader;

    public static TopicBlockHeader Load(BinaryReader input)
    {
        TopicBlockHeader header;
        header.LastParagraph = input.ReadInt32();
        header.TopicData = input.ReadInt32();
        header.LastTopicHeader = input.ReadInt32();
        return header;
    }
}
