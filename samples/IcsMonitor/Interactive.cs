using FASTER.core;
using IcsMonitor.Modbus;
using Microsoft.ML;
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

        /// <summary>
        /// Creates an interactive object.
        /// </summary>
        /// <param name="rootDirectory">Root directory. It can be used to access data.</param>
        public Interactive(DirectoryInfo rootDirectory = null)
        {
#if LogSupport
            ConfigureLogger();
#endif
            ConfigureDirectories(rootDirectory ?? new DirectoryInfo(Directory.GetCurrentDirectory()));
        }

        /// <summary>
        /// Creates a conversation table from the given collection of <paramref name="frames"/>. 
        /// </summary>
        /// <param name="frames">Source frames used to populate conversation table.</param>
        /// <param name="conversationTablePath">The path to folder where conversation table is to be saved.</param>
        /// <param name="token">The cancellation token for interrupting the operation.</param>
        /// <returns>Newly created conversation table.</returns>
        public FasterConversationTable CreateConversationTable(IEnumerable<RawFrame> frames, string conversationTablePath, CancellationToken? token = null)
        {
            var flowTable = FasterConversationTable.Create(conversationTablePath, 100000);
            using (var loader = flowTable.GetStreamer())
            {
                foreach (var frame in frames)
                {
                    loader.AddFrame(frame, frame.Number);
                    if (token?.IsCancellationRequested ?? false) break;
                }
            }
            flowTable.SaveChanges();
            return flowTable;
        }


        /// <summary>
        /// Computes a collection of conversation tables by splitting the input frames to specified time intervals.
        /// <para/>
        /// Each table is computed for a single time interval given by <paramref name="timeInterval"/> time span. 
        /// It is thus possible to consider a use case when IPFIX monitoring is used for fixed time intervals, e.g., 1 minute.
        /// In this interval the conversations are computed and can be further analyzed.
        /// </summary>
        /// <param name="frames">An input collection of frames.</param>
        /// <param name="conversationTablePath">The path to conversation table files.</param>
        /// <param name="timeInterval">The time interval used for splitting the input frames in conversation tables.</param>
        /// <param name="token">Cancellation token used to cancel the operation.</param>
        /// <returns></returns>
        public IEnumerable<FasterConversationTable> CreateConversationTables(IEnumerable<RawFrame> frames, string conversationTablePath, TimeSpan timeInterval, CancellationToken? token = null)
        {
            FasterConversationTable CreateConversationTable(int index, out FasterConversationTable.FrameStreamer streamer)
            {
                var flowTablePath = Path.Combine(conversationTablePath, index.ToString("D4"));
                var flowTable = FasterConversationTable.Create(flowTablePath, 100000);
                streamer = flowTable.GetStreamer();
                return flowTable;
            }

            long? startWindowTicks = null;
            int flowTableCount = 0;
            var flowTable = CreateConversationTable(++flowTableCount, out var loader);
            foreach (var frame in frames)
            {
                if (startWindowTicks is null) startWindowTicks = frame.Ticks;
                if (frame.Ticks > startWindowTicks + timeInterval.Ticks)
                {
                    loader.Dispose();
                    flowTable.SaveChanges();
                    yield return flowTable;

                    flowTable = CreateConversationTable(++flowTableCount, out loader);
                    startWindowTicks += timeInterval.Ticks;
                }

                loader.AddFrame(frame, frame.Number);
                if (token?.IsCancellationRequested ?? false) break;
            }
            loader.Dispose();
            flowTable.SaveChanges();
            yield return flowTable;
        }

        /// <summary>
        /// Get all packets from the conversation <paramref name="table"/>.
        /// </summary>
        /// <param name="table">The conversation table.</param>
        /// <returns>A collection of packets.</returns>
        public IEnumerable<(long Ticks, Packet Packet)> GetAllPackets(FasterConversationTable table)
        {
            static FasterConversationTable.ProcessingResult<(long, Packet)> GetPacket(FrameMetadata meta, SpanByte bytes)
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
        /// Writes a collection of raw <paramref name="frames"/> to PCAP file at the given <paramref name="path"/>.
        /// </summary>
        /// <param name="frames">The source frames.</param>
        /// <param name="path">the path of the pcap file to create.</param>
        public void WriteFramesToFile(IEnumerable<RawFrame> frames, string path)
        {
            var writer = new SharpPcapWriter(path);
            foreach (var frame in frames)
            {
                writer.WriteFrame(frame);
            }
            writer.Close();
        }

        /// <summary>
        /// Creates the capture file reader.
        /// </summary>
        /// <param name="path">The source pcap file to read.</param>
        /// <param name="useManaged">Use the managed reader implementation. If false it uses SharpPcap and external library.</param>
        /// <returns>The capture reader.</returns>
        public ICaptureFileReader OpenCaptureFile(string path, bool useManaged = true)
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

        /// <summary>
        /// Reads up to the specified <paramref name="count"/> of frames using the given <paramref name="reader"/> 
        /// <para/>
        /// Read all frames if <paramref name="count"/> is not specified.
        /// </summary>
        /// <param name="reader">The pcap reader.</param>
        /// <param name="count">Number of frames to read.</param>
        /// <returns>A collection of <see cref="RawFrame"/> objects.</returns>
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

        /// <summary>
        /// Tests if <paramref name="packet"/> belongs to the given conversation specified by its <paramref name="conversationKey"/>.
        /// </summary>
        /// <param name="table">The conversation table providing <see cref="FasterConversationTable.GetFlowKey(Packet)"/> operation.</param>
        /// <param name="conversationKey">The conversation key.</param>
        /// <param name="packet">The packet.</param>
        /// <returns>true if the packet belongs to the conversation; false otherwise</returns>
        public bool ContainsPacket(FasterConversationTable table, FlowKey conversationKey, Packet packet)
        {
            var packetKey = table.GetFlowKey(packet);
            return conversationKey.EqualsOrReverse(packetKey);
        }

        /// <summary>
        /// Creates the ICS dataset from the source PCAP file given by <paramref name="inputFile"/> name. 
        /// <para/>
        /// This is all-in-one method that reads pcap file, computes conversation tables and extract ICS 
        /// conversations using provided processor. 
        /// </summary>
        /// <typeparam name="TFlowData">The type of the dataset. </typeparam>
        /// <param name="inputFile">The source pcap file.</param>
        /// <param name="timeInterval">The time interval for conversation window.</param>
        /// <param name="frameFilter">Teh filter expression used to select frames to be inserted in conversation tables.</param>
        /// <param name="processor">The conversation processor to produce results in the dataset.</param>
        /// <returns>The dataset computed for the input data using the given conversation processor.</returns>
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
                var reader = OpenCaptureFile(inputFile, false);
                frames = GetNextFrames(reader).ToList();
                var firstFrame = frames.First();
                tables = CreateConversationTables(frames.Where(frameFilter), dbTableDirectory, timeInterval, CancellationToken.None).ToList();
                reader.Close();
            }
            var conversations = tables.Select(table => new ConversationTable<TFlowData>(table.ProcessConversations(table.ConversationKeys, processor))).ToList();
            return new IcsDataset<TFlowData>(conversations, frames);
        }

        /// <summary>
        /// Computes a Modbus Compact IPFIX dataset for the given source pcpa file. 
        /// </summary>
        /// <param name="inputFile">The name of the input PCAP file.</param>
        /// <param name="timeInterval">The time interval used to set window size for collecting flows.</param>
        /// <returns>The Modbus ICS Dataset object.</returns>
        public IcsDataset<ModbusFlowData.Compact> ComputeModbusDataset(string inputFile, TimeSpan timeInterval)
        {
            static bool FrameFilter(RawFrame frame)
            {
                var tcp = frame.GetTcpPacket();
                return tcp?.SourcePort == 502 || tcp?.DestinationPort == 502;
            }

            var processor = new ModbusBiflowProcessor();
            var transform = ConversationRecord<ModbusFlowData>.TransformTo(x => new ModbusFlowData.Compact(x));
            var compactProcessor = processor.Transform(r => transform.Invoke(r));
            return ComputeDataset(inputFile, timeInterval, FrameFilter, compactProcessor);
        }

        /// <summary>
        /// Saves the <paramref name="dataview"/> to TSV file.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="dataview">The dataview to save.</param>
        /// <param name="path">the pth of the target file.</param>
        public void SaveToTsv(MLContext context, IDataView dataview, string path)
        {
            using var stream = File.Create(path);
            context.Data.SaveAsText(dataview, stream);
        }
        /// <summary>
        /// Loads the data view from the TSV file.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="path">The path to source file.</param>
        /// <returns>The data view loaded.</returns>
        public IDataView LoadFromTsv(MLContext context, string path)
        {
            return context.Data.LoadFromTextFile(path);
        }


        private static string GetHash(string input)
        {
            using SHA256 hashAlgorithm = SHA256.Create();
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

        /// <summary>
        /// Cleans  the environment by deleting objects created during <see cref="Interactive"/> lifetime.
        /// </summary>
        public void CleanUp()
        {
            try
            {
                this.TempDirectory.Delete(true);
            }
            catch (Exception e)
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
    }
}
