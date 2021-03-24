using MessagePack;

namespace Traffix.Processors
{
    [MessagePackObject]
    public struct RecordLabel
    {
        [Key("CLASS")]
        public string Class;

        [Key("SCORE")]
        public float Score;
    }
}