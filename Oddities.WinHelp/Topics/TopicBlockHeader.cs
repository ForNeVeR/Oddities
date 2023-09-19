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
        return new()
        {
            LastParagraph = input.ReadInt32(),
            TopicData = input.ReadInt32(),
            LastTopicHeader = input.ReadInt32()
        };
    }
}
