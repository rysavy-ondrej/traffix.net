using PacketDotNet;
using System;
using System.Collections;
using System.IO;

namespace Traffix.Providers.PcapFile
{
    /// <summary>
    /// Implements fully managed PCAP file reader for TCPDUMP file format.
    /// </summary>
    public class ManagedPcapReader : ICaptureFileReader
    {
        private Stream _stream;
        private readonly long _length;
        private int _frameNumber;
        ReadingState _state;
        RawFrame _current;
        /// <summary>
        /// Creates a new reader device for the given data stream.
        /// </summary>
        /// <param name="stream">A stream containing pcap data.</param>
        /// <param name="length">A real length of the stream. If none is specified the length is taken from the stream.</param>
        public ManagedPcapReader(Stream stream, long? length = null)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _length = length ?? stream.Length;
            Open();
            _state = ReadingState.NotStarted;
        }

        public LinkLayers LinkLayer { get; private set; } = LinkLayers.Null;

        /// <inheritdoc/>
        public RawFrame Current
        {
            get
            {
                switch (_state)
                {
                    case ReadingState.NotStarted: throw new InvalidOperationException("Call MoveNext first.");
                    case ReadingState.Finished: throw new InvalidOperationException("Already finished.");
                    case ReadingState.Success: return _current;
                    default: throw new InvalidOperationException("Reader is not open.");
                }
            }
        }

        /// <inheritdoc/>
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public ReadingState State => _state;

        public Stream Stream => _stream;

        public bool CanAccessStream => true;

        public bool MoveNext()
        {
            return GetNextFrameInternal(true);
        }

        public void Reset()
        {
            _stream.Position = 0;
            _state = ReadingState.NotStarted;
            Open();
        }

        const int PACKET_HEADER_LENGTH = 16;
                      
        /// <inheritdoc/>
        public bool GetNextFrame(out RawFrame frame, bool readData = true)
        {
            var result = GetNextFrameInternal(readData);
            frame = _current;
            return result;
        }
        private bool GetNextFrameInternal(bool readData)
        {
            if (_state == ReadingState.Closed) throw new InvalidOperationException("Reader is not open.");
            if (_state == ReadingState.Finished) return false;
            if (GetNextFrameHeader(out _current))
            {
                if (readData)
                {
                    var buffer = new byte[_current.IncludedLength];
                    ReadFrameBytes(new Span<byte>(buffer));
                    _current.Data = buffer;
                }
                else
                {
                    SkipFrameBytes(_current.IncludedLength);
                }
                _state = ReadingState.Success;
                return true;
            }
            else
            {
                _current = default;
                _state = ReadingState.Finished;
                return false;
            }
        }
        /// <summary>
        /// Reads the header of the next frame in the capture file.
        /// <para/>
        /// It is always necessary to read also frame bytes before next frame can be read.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private bool GetNextFrameHeader(out RawFrame frame)
        {
            frame = null;
            _frameNumber++;
            var frameOffset = _stream.Position;
            // Do we have enough data to read?
            if (_stream.Position + PACKET_HEADER_LENGTH > _length) return false;
            var (ticks, includedLength, originalLength) = ReadFrameHeader();
            frame = new RawFrame(LinkLayer, _frameNumber, ticks, frameOffset, (int)includedLength, (int)originalLength);
            return true;
        }

        /// <summary>
        /// Reads bytes for the frame from the actual position of the stream.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private int ReadFrameBytes(Span<byte> buffer)
        {
            return _stream.Read(buffer);
        }

        private int SkipFrameBytes(int byteCount)
        {
            var skippedBytes = Math.Min(_length - _stream.Position, byteCount);
            _stream.Seek(skippedBytes, SeekOrigin.Current);
            return (int)skippedBytes;
        }


        private unsafe (long Ticks, uint IncludedLength, uint OriginalLength) ReadFrameHeader()
        {
            var headerBytes = stackalloc byte[PACKET_HEADER_LENGTH];
            var header = new Span<byte>(headerBytes, PACKET_HEADER_LENGTH);
            _stream.Read(header);
            var tsSeconds = BitConverter.ToUInt32(header.Slice(0, 4));
            var tsMicroseconds = BitConverter.ToUInt32(header.Slice(4, 4));
            var ticks = UnixTimeValToTicks(tsSeconds, tsMicroseconds);
            var includedLength = BitConverter.ToUInt32(header.Slice(8, 4));
            var originalLength = BitConverter.ToUInt32(header.Slice(12, 4));
            return (ticks, includedLength, originalLength);
        }

        internal static Int64 UnixTimeValToTicks(Int64 tvSec, Int64 tvUsec)
        {

            long ticks = (tvUsec * (TimeSpan.TicksPerMillisecond / 1000)) +
                         (tvSec * TimeSpan.TicksPerSecond);
            return epochDateTimeTicks + ticks;
        }
        private static readonly long epochDateTimeTicks = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public void Close()
        {
            _stream?.Close();
        }

        const int PCAP_FILE_HEADER_SIZE = 4 + 2 + 2 + 4 + 4 + 4 + 4;
        const int MAGIC_NUMBER_OFFSET = 0;
        const int VERSION_MAJOR_OFFSET = 4;
        const int VERSION_MINOR_OFFSET = 6;
        const int THIS_ZONE_OFFSET = 8;
        const int SIG_FIGS_OFFSET = 12;
        const int SNAP_LEN_OFFSET = 16;
        const int NETWORK_TYPE_OFFSET = 20;
        void Open()
        {
            var buffer = new byte[PCAP_FILE_HEADER_SIZE];
            _stream.Read(buffer, 0, PCAP_FILE_HEADER_SIZE);
            var magicNumber = BitConverter.ToUInt32(buffer, MAGIC_NUMBER_OFFSET);
            if (magicNumber != 0xa1b2c3d4) throw new InvalidDataException("Capture file is not of supported format or version.");
            var version_major = BitConverter.ToUInt16(buffer, VERSION_MAJOR_OFFSET);
            var version_minor = BitConverter.ToUInt16(buffer, VERSION_MINOR_OFFSET);
            var thiszone = BitConverter.ToUInt32(buffer, THIS_ZONE_OFFSET);
            var sigfigs = BitConverter.ToUInt32(buffer, SIG_FIGS_OFFSET);
            var snaplen = BitConverter.ToUInt32(buffer, SNAP_LEN_OFFSET);
            LinkLayer = (LinkLayers)BitConverter.ToUInt32(buffer, NETWORK_TYPE_OFFSET);
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    this.Close();
                    _stream.Dispose();
                    _stream = null;
                }
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
