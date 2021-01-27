using FASTER.core;
using IcsMonitor.Modbus;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Traffix.Core.Flows;
using Traffix.Providers.PcapFile;
using Traffix.Storage.Faster;

namespace IcsMonitor
{
    /// <summary>
    /// Interactive class exposes various API methods usable in C# interactive sessions. 
    /// </summary>
    public partial class Interactive : IDisposable
    {
        public Interactive(DirectoryInfo rootDirectory = null)
        {
            #if LogSupport
            ConfigureLogger();
            #endif
            ConfigureDirectories(rootDirectory ?? new DirectoryInfo(Directory.GetCurrentDirectory()));
        }

        public FasterConversationTable ReadToConversationTable(string pcapFile, string conversationTablePath, CancellationToken? token = null)
        {
            using var stream = new FileInfo(pcapFile).OpenRead();
            return ReadToConversationTable(stream, conversationTablePath, token);
        }
        public FasterConversationTable ReadToConversationTable(Stream stream, string conversationTablePath, CancellationToken? token = null)
        {
            var flowTable = FasterConversationTable.Create(conversationTablePath, 100000);
            // flowTable.LoadFromStream(stream, token ?? CancellationToken.None, null);
            return flowTable;
        }

        public FasterConversationTable CreateConversationTable(IEnumerable<RawFrame> frames, string conversationTablePath, CancellationToken? token = null)
        {
            var flowTable = FasterConversationTable.Create(Path.Combine(conversationTablePath), 100000);
            using (var loader = flowTable.GetStreamer())
            {
                foreach (var frame in frames)
                {
                    loader.AddFrame(frame,  frame.Number);
                    if (token?.IsCancellationRequested ?? false) break;
                }
            }
            flowTable.SaveChanges();
            return flowTable;
        }

        /// <summary>
        /// Gets the data using cnversation processor and the result function transformer.
        /// </summary>
        /// <typeparam name="Tdata"></typeparam>
        /// <typeparam name="Tout"></typeparam>
        /// <param name="table">The conversation table.</param>
        /// <param name="processor">The conversation processor used to get conversation from the conversation table.</param>
        /// <param name="resultFuncTransformer">Transform data produced by the conversation processor to the result objects.</param>
        /// <returns></returns>
        public IEnumerable<TData> GetConversations<TData>(FasterConversationTable table, IConversationProcessor<TData> processor)
        {
            return table.ProcessConversations<TData>(table.ConversationKeys, processor);
        }

        public IEnumerable<(long Ticks, Packet Packet)> GetPackets(FasterConversationTable table)
        {
            static FasterConversationTable.ProcessingResult<(long,Packet)> GetPacket(FrameMetadata meta, SpanByte bytes)
            {
                return new FasterConversationTable.ProcessingResult<(long, Packet)>
                {
                    State = FasterConversationTable.ProcessingState.Success,
                    Result = (meta.Ticks, Packet.ParsePacket((LinkLayers)meta.LinkLayer, bytes.ToByteArray()))
                };
            }
            return table.ProcessFrames<(long, Packet)>(GetPacket);
        }

        /// <summary>
        /// Reads the input file and creates a collection of conversation tables for the intervals of the specified duration.
        /// <para>
        /// This method enables to create mutliple conversation tables for given time interval. For instance if the time intevral is set to X minutes
        /// thenn every X minutes a new conversation table is created. It means that long running conversations can be split to multiple tables. 
        /// </para>
        /// </summary>
        /// <param name="reader">The packet capture reader.</param>
        /// <param name="conversationTablePath">The root folder for storing data of the conversation tables.</param>
        /// <param name="timeInterval">The time interval for capturing packets to a conversation table.</param>
        /// <param name="token">the cancellation token.</param>
        /// <returns>A collection of conversation tables.</returns>
        public IEnumerable<FasterConversationTable> ReadToConversationTables(ICaptureFileReader reader, string conversationTablePath, TimeSpan timeInterval, CancellationToken? token = null)
        {
            return PopulateConversationTables(GetNextFrames(reader), conversationTablePath, timeInterval, token);
        }


        public void WriteToFile(IEnumerable<RawFrame> frames, string path)
        {
            var writer = new SharpPcapWriter(path);
            foreach (var frame in frames)
            {
                writer.WriteFrame(frame);
            }
            writer.Close();
        }

        /// <summary>
        /// Computes a collection of conversation tables by splitting the input frames to specified time intervals.
        /// <para/>
        /// Each table is computed for a single time interval given by <paramref name="timeInterval"/> time span. 
        /// It is thus possible to consider a use case when IPFIX monitoring is used for fixed time intervals, e.g, 1 minute.
        /// In this interval the conversations are computed and can be further analyzed.
        /// </summary>
        /// <param name="frames">An input collection of frames.</param>
        /// <param name="conversationTablePath">The path to conversation table files.</param>
        /// <param name="timeInterval">The time interval used for splitting the input frames in conversation tables.</param>
        /// <param name="token">Cancellation token used to cancel the operation.</param>
        /// <returns></returns>
        public IEnumerable<FasterConversationTable> PopulateConversationTables(IEnumerable<RawFrame> frames, string conversationTablePath, TimeSpan timeInterval, CancellationToken? token = null)
        {
            long? startWindowTicks = null;
            long timeIntervalTicks = timeInterval.Ticks;
            int flowTableNum = 0;
            var flowTablePath = Path.Combine(conversationTablePath, flowTableNum.ToString("D4"));
            Console.WriteLine($">> {flowTablePath}");

            var flowTable = FasterConversationTable.Create(flowTablePath, 100000);
            var loader = flowTable.GetStreamer();
            foreach (var frame in frames)
            {
                if (startWindowTicks is null) startWindowTicks = frame.Ticks;
                if (frame.Ticks > startWindowTicks + timeIntervalTicks)
                {
                    loader.Dispose();
                    flowTable.SaveChanges();
                    yield return flowTable;
                    flowTableNum++;
                    flowTablePath = Path.Combine(conversationTablePath, flowTableNum.ToString("D4"));
                    flowTable = FasterConversationTable.Create(flowTablePath, 100000);
                    loader = flowTable.GetStreamer();
                    startWindowTicks += timeIntervalTicks;
                }

                loader.AddFrame(frame,  frame.Number);
                if (token?.IsCancellationRequested ?? false) break;
            }
            loader.Dispose();
            flowTable.SaveChanges();
            yield return flowTable;
        }

        

        /// <summary>
        /// Creates the capture file reader.
        /// </summary>
        /// <param name="path">The source pcap file to read.</param>
        /// <param name="useManaged">Use the managed reader implementation. If false it uses SharpPcap and external library.</param>
        /// <returns>The capture reader.</returns>
        public ICaptureFileReader CreateCaptureFileReader(string path, bool useManaged = true)
        {
            if (useManaged)
            {
                return new CaptureFileReader(new FileInfo(path).OpenRead());
            }
            else
            {
                return new SharpPcapReader(path);
            }
        }

        public IEnumerable<RawFrame> GetNextFrames(ICaptureFileReader reader, int count = Int32.MaxValue)
        {
            for (int i = 0; i < count; i++)
            {
                if (reader.GetNextFrame(out var frame, true))
                {
                    yield return frame;
                }
                else
                {
                    break;
                }
            }
        }

        public bool ContainsPacket(FasterConversationTable table, FlowKey conversationKey, Packet packet)
        {
            var packetKey = table.GetFlowKey(packet);
            return conversationKey.EqualsOrReverse(packetKey);
        }
                
        /// <summary>
        /// Computes the ICS dataset from the source PCAP file. 
        /// <para/>
        /// This is all-in-one method that enables to read pcap file, compute flow tables and extract ICS 
        /// conversations using provided processor. 
        /// </summary>
        /// <typeparam name="TFlowData"></typeparam>
        /// <typeparam name="TTargetData"></typeparam>
        /// <param name="inputFile"></param>
        /// <param name="timeInterval"></param>
        /// <param name="processor"></param>
        /// <param name="transformer"></param>
        /// <returns></returns>
        public IcsDataset<TFlowData> ComputeDataset<TFlowData>(string inputFile, TimeSpan timeInterval, Func<RawFrame, bool> frameFilter, IConversationProcessor<ConversationRecord<TFlowData>> processor)
        {
            // provide existing dataset or create a new one for the input file.
            // input file name is used to determine whether we already have
            // existing conversation tables or we need to compute a new set of ones.
            var inputFilePath = Path.GetFullPath(inputFile);
            var datasetHash = GetHash($"{inputFilePath}[{timeInterval}]");
            var dbTableDirectory = Path.Combine(TempDirectory.FullName, datasetHash);
            List<FasterConversationTable> tables = null;
            List<RawFrame> frames = null;
            if (Directory.Exists(dbTableDirectory))
            {
                Console.WriteLine($"Conversation table has already existed for '{inputFile}[{timeInterval}]', reading from db '{dbTableDirectory}'.");
                frames = new List<RawFrame>();
                tables = new List<FasterConversationTable>();

                foreach (var folder in Directory.GetDirectories(dbTableDirectory))
                {
                    var table = FasterConversationTable.Create(folder, 100000);
                    tables.Add(table);
                    frames.AddRange(table.GetRawFrames());
                }
                Console.WriteLine($"Frames count = {frames.Count}");
            }
            else
            {
                Directory.CreateDirectory(dbTableDirectory);
                Console.WriteLine($"Creating conversation table '{inputFile}[{timeInterval}]' at '{dbTableDirectory}'.");
                var reader = CreateCaptureFileReader(inputFile, false);
                frames = GetNextFrames(reader).ToList();
                var firstFrame = frames.First();
                tables = PopulateConversationTables(frames.Where(frameFilter), dbTableDirectory, timeInterval, CancellationToken.None).ToList();
                reader.Close();
            }
            var conversations = tables.Select(x => new ConversationTable<TFlowData>(GetConversations<ConversationRecord<TFlowData>>(x, processor))).ToList();
            return new IcsDataset<TFlowData>(conversations, frames);
        }

        /// <summary>
        /// Computes Modbus Compact IPFIX dataset. 
        /// </summary>
        /// <param name="inputFile">The name of the input PCAP file.</param>
        /// <param name="timeInterval">The time interval used to set window size for collecting flows.</param>
        /// <returns>The Modbus ICS Dataset object.</returns>
        IcsDataset<ModbusFlowData.Compact> ComputeModbusDataset(string inputFile, TimeSpan timeInterval)
        {
            var processor = new IcsMonitor.Modbus.ModbusBiflowProcessor();
            bool FrameFilter(Traffix.Providers.PcapFile.RawFrame frame)
            {
                var tcp = frame.GetTcpPacket();
                return tcp?.SourcePort == 502 || tcp?.DestinationPort == 502;
            }
            var transform = ConversationRecord<ModbusFlowData>.TransformTo<ModbusFlowData.Compact>(x => new ModbusFlowData.Compact(x));
            var compactProcessor = processor.Transform(r => transform.Invoke(r));
            return ComputeDataset<ModbusFlowData.Compact>(inputFile, timeInterval, FrameFilter, compactProcessor);
        }


        private static string GetHash(string input)
        {
            using (SHA256 hashAlgorithm = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

#if LogSupport
        private ILogger _logger;
        private ConsoleLoggerProvider _consoleLoggerProvider;
        void ConfigureLogger()
        {
            _consoleLoggerProvider = new ConsoleLoggerProvider(new LoggerOptions(), new[] { new LoggerFormatter() });
            _logger = _consoleLoggerProvider.CreateLogger("Interactive");
        }
        public ILogger Logger => _logger;

#endif
        public DirectoryInfo RootDirectory { get => _rootDirectory; set => _rootDirectory = value; }
        public DirectoryInfo TempDirectory { get => _tempDirectory; set => _tempDirectory = value; }

        DirectoryInfo _rootDirectory;
        DirectoryInfo _tempDirectory;
        private bool m_disposedValue;

        void ConfigureDirectories(DirectoryInfo rootDirectory)
        {
            _rootDirectory = rootDirectory;
            _tempDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "icsmonitor.interactive"));
            if (!_tempDirectory.Exists) _tempDirectory.Create(); 
        }

        private void CleanUp()
        {
            try
            {
                this.TempDirectory.Delete(true);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine($"Cannot delete temp directory '{this.TempDirectory.FullName}' : {e.Message}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposedValue)
            {
                if (disposing)
                {
                    CleanUp();
                }
                m_disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A memory representation of the conversation table. 
        /// </summary>
        /// <typeparam name="TData">The data type of the conversation.</typeparam>
        public class ConversationTable<TData> : List<ConversationRecord<TData>> 
        {
            /// <summary>
            /// Start time of the conversation table.
            /// </summary>
            public DateTime StartTime;
            /// <summary>
            /// The interval defining the duration of the conversation table.
            /// </summary>
            public TimeSpan Interval;

            /// <summary>
            /// Creates a new conversation table from the given collection of conversations.
            /// </summary>
            /// <param name="collection"></param>
            public ConversationTable(IEnumerable<ConversationRecord<TData>> collection) : base(collection)
            {
            }
            /// <summary>
            /// Aggregates conversations by grouping conversations using <paramref name="keySelector"/> and then by applying <paramref name="aggregator"/> function 
            /// to all conversations in the group. The result is a collection of aggregated conversations.
            /// </summary>
            /// <typeparam name="Tout">The type of the output.</typeparam>
            /// <typeparam name="TKey">The type of the keys.</typeparam>
            /// <param name="conversations">The input collection of conversations.</param>
            /// <param name="keySelector">The selector of key fields for the aggregated conversations.</param>
            /// <param name="accumulator">The initial value of the aggregation.</param>
            /// <param name="aggregator">The aggregattor function that implements a way to add a new conversation to the aggregation.</param>
            /// <returns>A collection of conversations. Number of returned items equals to a number of groups created by <paramref name="keySelector"/>.</returns>
            public IEnumerable<Tout> AggregateConversations<Tout, TKey>(Func<ConversationRecord<TData>, TKey> keySelector, Func<ConversationRecord<TData>, Tout>  getTarget, Func<Tout, Tout, Tout> aggregator)
            {
                var groups = this.GroupBy(keySelector);
                foreach (var g in groups)
                {
                    yield return g.Select(getTarget).Aggregate(aggregator);
                }
            }
        }
    }
}
