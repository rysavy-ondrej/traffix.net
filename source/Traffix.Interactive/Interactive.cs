using PacketDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Traffix.Core.Flows;
using Traffix.Core.Processors;
using Traffix.Providers.PcapFile;
using Traffix.Storage.Faster;

namespace Traffix.Interactive
{
    /// <summary>
    /// Interactive class exposes various API methods usable in C# interactive sessions. 
    /// </summary>
    public sealed class Interactive : IDisposable
    {
        DirectoryInfo _homeDirectory;
        /// <summary>
        /// Creates an interactive object.
        /// </summary>
        /// <param name="homeDirectory">Home directory. It is used by the interactive for various I/O operations.</param>
        public Interactive(DirectoryInfo? homeDirectory = null)
        {
            _homeDirectory = homeDirectory ?? new DirectoryInfo(Directory.GetCurrentDirectory());
            Captures = new CaptureFileOperations(this);
            Packets = new PacketOperation(this);
            Conversations = new ConversationOperations(this);
            Datasets = new DatasetOperations(this);
        }
        public CaptureFileOperations Captures { get; }
        public PacketOperation Packets { get; }
        public ConversationOperations Conversations { get; }
        public DatasetOperations Datasets { get; }

        /// <summary>
        /// The handler for the log messages.
        /// </summary>
        public event EventHandler<LogEntry>? Log;

        public string GetSHA256Hash(string input)
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

        private bool m_disposedValue;
        void Dispose(bool disposing)
        {
            if (!m_disposedValue)
            {
                if (disposing)
                {
                    // Do we have something to do here?
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
