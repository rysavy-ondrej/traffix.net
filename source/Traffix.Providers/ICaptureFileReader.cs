using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.IO;

namespace Traffix.Providers.PcapFile
{
    /// <summary>
    /// Defines possible states of capture file reader.
    /// </summary>
    public enum ReadingState 
    { 
        /// <summary>
        /// The reader is open but it is necessary to call MoveNext to advance the pointer to the first frame.
        /// </summary>
        NotStarted,
        /// <summary>
        /// The reader is open and it is possible to retrieve the frame from <see cref="IEnumerator.Current"/> property.
        /// </summary>
        Success,
        /// <summary>
        /// The reader finished the file. While it is still open it is not possible to retrieve the frame from it.
        /// Call <see cref="IEnumerator.Reset()"/> to start reading again. 
        /// </summary>
        Finished, 
        /// <summary>
        /// The reader is closed. It is not possible to read any data from it.
        /// </summary>
        Closed }
    /// <summary>
    /// The common interface for all implementation of packet file readers.
    /// </summary>
    public interface ICaptureFileReader : IEnumerator<RawCapture>, IDisposable
    {
        /// <summary>
        /// Gets the link layer of the frames.
        /// </summary>
        LinkLayers LinkLayer { get; }
        /// <summary>
        /// Gets the underlaying stream. It throw exception if the operation is not supported (<see cref="CanAccessStream"/> == false).
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        /// Gets a value indicating whether the current reader provides underlying stream.
        /// </summary>
        bool CanAccessStream { get; }

        /// <summary>
        /// Closes the reader.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets the state of the reader.
        /// </summary>
        ReadingState State { get; }

        /// <summary>
        /// Gets the next frame from the capture file. 
        /// </summary>
        /// <param name="frame">The next frame from the capture file.</param>
        /// <param name="readData">Set to true if frame should be readed oncluding its bytes. false when only frame header should be retrieved.</param>
        /// <returns>True if the next frame has been read. False if there are no more frames to read.</returns>
        bool GetNextFrame(out RawCapture frame, bool readData = true);
    }
}