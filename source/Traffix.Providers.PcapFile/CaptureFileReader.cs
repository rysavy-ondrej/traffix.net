using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.IO;

namespace Traffix.Providers.PcapFile
{

    public class SharpPcapWriter
    {
        private string _path;
        CaptureFileWriterDevice _device;

        public SharpPcapWriter(string path)
        {
            _path = path;
            _device = new CaptureFileWriterDevice(path);
            
        }

        public void Close()
        {
            _device.Close();
        }

        public void WriteFrame(RawFrame frame)
        {
            var cap = new RawCapture(frame.LinkLayer, TicksToUnix(frame.Ticks), frame.Data);
            _device.Write(cap);
        }

        private PosixTimeval TicksToUnix(long ticks)
        {  
            return new PosixTimeval(new DateTime(ticks));
        }
    }


    public class SharpPcapReader : ICaptureFileReader
    {
        ICaptureDevice _device;
        int _frameNumber;
        long _frameOffset;
        public SharpPcapReader(string captureFile)
        {
            // Get an offline device
            _device = new CaptureFileReaderDevice(captureFile);

            // Open the device
            _device.Open();
            
        }

        public LinkLayers LinkLayer => _device.LinkType;

        public long Position => throw new NotImplementedException();

        public void Close()
        {
            _device.Close();
        }

        public bool GetNextFrame(out RawFrame frame, bool readData = true)
        {
            var capture = _device.GetNextPacket();
           
            if (capture != null)
            {
                frame = new RawFrame(LinkLayer, _frameNumber, GetTicksFromPosixTimeval(capture.Timeval), _frameOffset, capture.Data.Length, capture.Data.Length)
                {
                    Data = capture.Data
                };
                _frameNumber++;
                _frameOffset += capture.Data.Length;
                return true;
            }
            else
            {
                frame = default;
                return false;
            }
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _device.Close();
                    _device = null;
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

        internal static long GetTicksFromPosixTimeval(PosixTimeval timeval)
        {
            return timeval.Date.Ticks;
        }
    }

    public class CaptureFileReader : ICaptureFileReader
    {
        private Stream _stream;
        private readonly long _length;
        private int _frameNumber;
        /// <summary>
        /// Creates a new reader device for the given data stream.
        /// </summary>
        /// <param name="stream">A stream containing pcap data.</param>
        /// <param name="length">A real length of the stream. If none is specified the length is taken from the stream.</param>
        public CaptureFileReader(Stream stream, long? length = null)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _length = length ?? stream.Length;
            Open();
        }

        public LinkLayers LinkLayer { get; private set; } = LinkLayers.Null;

        public long Position => _stream.Position;

        const int PACKET_HEADER_LENGTH = 16;

        /// <summary>
        /// Reads the next available frame from the stream. 
        /// </summary>
        /// <param name="rawFrame">Fills the structure with an information about the frame.</param>
        /// <param name="frameData">If not null, it will be used as a buffer to read data. If null a new byte array will be created and populated with frame data.</param>
        /// <param name="readData">If set to true, the data bytes of the frame will be read. Otherwise, the bytes are skipped.</param>
        /// <returns>True if the frame was sucessfully read or false, if there is not any frame available.</returns>
        public bool GetNextFrame(out RawFrame frame, bool readData = true)
        {
            if (GetNextFrameHeader(out frame))
            {
                if (readData)
                {
                    var buffer = new byte[frame.IncludedLength];
                    ReadFrameBytes(new Span<byte>(buffer));
                    frame.Data = buffer;
                }
                else
                {
                    SkipFrameBytes(frame.IncludedLength);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Reads the header of the next frame in the capture file.
        /// <para/>
        /// It is always necessary to read also frame bytes before next frame can be read.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool GetNextFrameHeader(out RawFrame frame)
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
        public int ReadFrameBytes(Span<byte> buffer)
        {
            return _stream.Read(buffer);
        }

        public int SkipFrameBytes(int byteCount)
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

        /// <summary>
        /// Open causes that a header of the capture files is read. It is up to the caller to 
        /// position the stream to the proper location.
        /// </summary>
        private void Open()
        {
            ReadHeader();
        }

        const int PCAP_FILE_HEADER_SIZE = 4 + 2 + 2 + 4 + 4 + 4 + 4;
        const int MAGIC_NUMBER_OFFSET = 0;
        const int VERSION_MAJOR_OFFSET = 4;
        const int VERSION_MINOR_OFFSET = 6;
        const int THIS_ZONE_OFFSET = 8;
        const int SIG_FIGS_OFFSET = 12;
        const int SNAP_LEN_OFFSET = 16;
        const int NETWORK_TYPE_OFFSET = 20;
        void ReadHeader()
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
