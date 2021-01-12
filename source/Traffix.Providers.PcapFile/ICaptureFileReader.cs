using PacketDotNet;
using System;

namespace Traffix.Providers.PcapFile
{
    public interface ICaptureFileReader : IDisposable
    {
        LinkLayers LinkLayer { get; }
        long Position { get; }

        void Close();
        bool GetNextFrame(out RawFrame frame, bool readData = true);
    }
}