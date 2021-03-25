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

        public long WriteFrame(RawCapture frame)
        {
            _device.Write(frame);
            return 0;
        }
    }
}
