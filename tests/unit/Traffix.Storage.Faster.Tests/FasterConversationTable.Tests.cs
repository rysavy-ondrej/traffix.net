using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Traffix.Providers.PcapFile;

namespace Traffix.Storage.Faster.Tests
{
    [TestClass]
    public class FasterConversationTableTests
    {
        string testdir = "..";
        /// <summary>
        /// Loads the table from the given pcap file.
        /// </summary>
        [TestMethod]
        public void CreateTable()
        {
            var sw = new Stopwatch();
            var pcapPath = Path.GetFullPath(@"data\PCAP\modbus.pcap");
            var dbPath = Path.GetFullPath(@"c:\temp\0001\");
            if (Directory.Exists(dbPath)) Directory.Delete(dbPath, true);
            sw.Start();
            var flowTable = FasterConversationTable.Create(dbPath);
            var frames = flowTable.LoadFromStream(File.OpenRead(pcapPath), System.Threading.CancellationToken.None);
            
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            sw.Restart();
            PrintConversations(flowTable.Conversations);
            Console.WriteLine($"Frames= {flowTable.Frames.Count()} / {flowTable.WrittenFrames} /{frames} [{sw.Elapsed}]");
            sw.Restart();
            flowTable.Commit();
            
            Console.WriteLine($"--- COMMITED --- [{sw.Elapsed}]");
            sw.Restart();
            PrintConversations(flowTable.Conversations);
            Console.WriteLine($"Frames= {flowTable.Frames.Count()} / {flowTable.WrittenFrames} / {frames} [{sw.Elapsed}]");

            flowTable.Dispose();
        }

        private void PrintConversations(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<IConversationKey, IConversationValue>> conversations)
        {
            foreach (var c in conversations)
            {
                Console.WriteLine(c.Key);
            }
        }


        /// <summary>
        /// Open existing table.
        /// </summary>
        [TestMethod]

        public void ReadExistingTable()
        {
            var flowTable = OpenTable();
            Console.WriteLine("--- LOADED ---");
            PrintConversations(flowTable.Conversations);
            Console.WriteLine("--- FRAMES ---");
            Console.WriteLine($"Frames= {flowTable.Frames.Count()}");
        }

        public FasterConversationTable OpenTable()
        {
            var dbPath = Path.GetFullPath(@"c:\temp\0001\");
            var table = FasterConversationTable.Open(dbPath);


            return table;
        }
    }
}
