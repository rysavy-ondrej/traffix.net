using System;
using System.Collections.Generic;
using System.IO;
using Traffix.Providers.PcapFile;

namespace Traffix.Interactive
{
    public sealed class CaptureFileOperations
    {
        private Interactive _interactive;

        internal CaptureFileOperations(Interactive interactive)
        {
            this._interactive = interactive;
        }

        /// <summary>
        /// Writes a collection of raw <paramref name="frames"/> to PCAP file at the given <paramref name="path"/>.
        /// </summary>
        /// <param name="frames">The source frames.</param>
        /// <param name="path">The path of the pcap file to create.</param>
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
        /// Reads a collection of all frames from the given file.
        /// </summary>
        /// <param name="path">The path of the pcap file to read from.</param>
        /// <returns>A collection of all frames from the given file.</returns>
        public IEnumerable<RawFrame> ReadFromFile(string path)
        {
            using var reader = OpenRead(path);
            while(reader.GetNextFrame(out var frame))
            {
                yield return frame;
            }
        }

        /// <summary>
        /// Creates the capture file reader.
        /// </summary>
        /// <param name="path">The source pcap file to read.</param>
        /// <param name="useManaged">Use the managed reader implementation. If false it uses SharpPcap and external library.</param>
        /// <returns>The capture reader.</returns>
        public ICaptureFileReader OpenRead(string path, bool useManaged = true)
        {
            if (useManaged)
            {
                return new ManagedPcapReader(new FileInfo(path).OpenRead());
            }
            else
            {
                return new SharpPcapReader(path);
            }
        }
        /// <summary>
        /// Opens PCAP file at the given <paramref name="path"/> for writing. 
        /// </summary>
        /// <param name="path">The path of the file to create.</param>
        /// <returns>A new instance of capture file writer.</returns>
        public ICaptureFileWriter OpenWrite(string path)
        {
            return new SharpPcapWriter(path);
        }

        /// <summary>
        /// Reads up to the specified <paramref name="count"/> of frames using the given <paramref name="reader"/> 
        /// </summary>
        /// <param name="reader">The pcap reader.</param>
        /// <param name="count">Number of frames to read.</param>
        /// <returns>A collection of <see cref="RawFrame"/> objects.</returns>
        public IEnumerable<RawFrame> Take(ICaptureFileReader reader, int count)
        {
            if (reader.State == ReadingState.Finished) yield break;
            if (reader.State == ReadingState.Closed) throw new InvalidOperationException("Cannot read from closed reader.");
            for (int i = 0; i < count; i++)
            {
                if (reader.GetNextFrame(out var frame, true))
                {
                    yield return frame;
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Returns frames from a sequence as long as a specified condition is true. 
        /// </summary>
        /// <param name="reader">The reader to get elements from.</param>
        /// <param name="predicate">A function to test each source frame for a condition; 
        /// the second parameter of the function represents the index of the source frame in the scope of current operation.</param>
        /// <returns>The enumeration that contains elements satisfying the predicate. 
        /// The reader position is on the first frame after this sequence. It is thus 
        /// possible to contiune with reading next frames.</returns>
        public IEnumerable<RawFrame> TakeWhile(ICaptureFileReader reader, Func<RawFrame,int, bool> predicate)
        {
            var index = 0;
            // read first frame if necessary...
            if (reader.State == ReadingState.NotStarted) reader.MoveNext();
            if (reader.State == ReadingState.Finished) yield break;
            if (reader.State == ReadingState.Closed) throw new InvalidOperationException("Cannot read from closed reader.");
            do
            {
                if (predicate.Invoke(reader.Current, index))
                {
                    yield return reader.Current;
                    index++;
                }
                else
                {
                    yield break;
                }
            } while (reader.MoveNext());
        }
    }
}