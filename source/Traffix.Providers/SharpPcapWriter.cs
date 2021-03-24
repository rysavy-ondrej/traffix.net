using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;

namespace Traffix.Providers.PcapFile
{
    public class SharpPcapWriter : ICaptureFileWriter
    {
        private string _path;
        CaptureFileWriterDevice _device;

        public SharpPcapWriter(string path)
        {
            _path = path;
            _device = new CaptureFileWriterDevice(path);
        }

        public LinkLayers LinkLayer => throw new NotImplementedException();

        public void Close()
        {
            _device.Close();
        }

        public void Dispose()
        {
            if (_device.Opened)
            {
                Close();
            }
        }

        public long WriteFrame(RawFrame frame)
        {
            var cap = new RawCapture(frame.LinkLayer, TicksToUnix(frame.Ticks), frame.Data);
            _device.Write(cap);
            return 0;
        }

        private PosixTimeval TicksToUnix(long ticks)
        {
            return new PosixTimeval(new DateTime(ticks));
        }
    }
}
