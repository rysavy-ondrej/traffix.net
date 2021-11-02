using PacketDotNet;
using SharpPcap;
using System;

namespace Traffix.Providers.PcapFile
{
    public interface ICaptureFileWriter : IDisposable
    {
        /// <summary>
        /// Gets the link layer of frames written by this writer.
        /// </summary>
        LinkLayers LinkLayer { get; }
        /// <summary>
        /// Closes the capture file. 
        /// </summary>
        void Close();
        /// <summary>
        /// Writes a new frame.
        /// </summary>
        /// <param name="frame">The raw frame.</param>
        /// <returns>The offset of the written frame in the file.</returns>
        long WriteFrame(RawCapture frame);
    }
}