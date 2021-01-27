using Microsoft.ML;
using System.Collections.Generic;
using System.Linq;
using Traffix.Providers.PcapFile;

namespace IcsMonitor
{
    public partial class Interactive
    {
        /// <summary>
        /// The class collects necessary information of the single ICS dataset.
        /// <para/>
        /// The ICS dataset consists of time base, conversation tables and raw frames. 
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

            /// <summary>
            /// Gets the <see cref="IDataView"/> instance from the given conversation table.
            /// <para/>
            /// It uses <see cref="ConversationRecordExtensions.ToDataFrame{T}(IEnumerable{ConversationRecord{T}})"/> method to create <see cref="IDataView"/> from
            /// strongly type collection of conversation records. 
            /// </summary>
            /// <param name="table">The conversation table.</param>
            /// <returns>The <see cref="IDataView"/> instance with conversations from the <paramref name="table"/>.</returns>
            public IDataView GetConversationDataView(ConversationTable<TData> table)
            {
                return table.AsEnumerable().ToDataFrame();
            }
        }
    }
}
