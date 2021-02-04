using Microsoft.ML;
using System.Collections.Generic;
using System.Linq;
using Traffix.Providers.PcapFile;

namespace IcsMonitor
{
    public partial class Monitor
    {
        /// <summary>
        /// The class collects necessary information of the single ICS dataset.
        /// <para/>
        /// To use data set with ML.NET it is possible to several extensions in <see cref="ConversationRecordExtensions"/> class. 
        /// </summary>
        public class IcsDataset<TData>
        {
            public IcsDataset(IEnumerable<ConversationTable<TData>> conversations, IEnumerable<RawFrame> frames)
            {
                ConversationTables = conversations.ToList();
                Frames = frames.ToList();
            }

            public long TimebaseTicks => Frames.FirstOrDefault()?.Ticks ?? 0;
            public List<ConversationTable<TData>> ConversationTables { get; private set; }
            public List<RawFrame> Frames { get; private set; }

            public DatasetStatistics Statistics =>
                new DatasetStatistics
                {
                    FramesCount = Frames.Count,
                    FirstFrame = Frames.First().Ticks,
                    LastFrame = Frames.Last().Ticks,
                    TablesCount = ConversationTables.Count,
                    AvgConversations = ConversationTables.Average(c => c.Count),
                    MaxConversation = ConversationTables.Max(c => c.Count),
                    MinConversations = ConversationTables.Min(c => c.Count)
                };
            public struct DatasetStatistics
            {
                public int FramesCount;
                public long FirstFrame;
                public long LastFrame;

                public int TablesCount;
                public double AvgConversations;
                public int MaxConversation;
                public int MinConversations;
            }
        }
    }
}
