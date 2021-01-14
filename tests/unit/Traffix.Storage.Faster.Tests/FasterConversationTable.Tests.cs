﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            //var pcapPath = Path.GetFullPath(@"data\PCAP\modbus.pcap");
            var pcapPath = Path.GetFullPath(@"C:\Users\user\Captures\sorting_station_ver2.pcap");

            var dbPath = Path.GetFullPath(@"c:\temp\0001\");
            if (Directory.Exists(dbPath)) Directory.Delete(dbPath, true);
            sw.Start();
            var flowTable = FasterConversationTable.Create(dbPath, framesCapacity:1700000);
            var frameNumber = 0;
            using (var loader = flowTable.GetStreamer())
            using (var pcapReader = new SharpPcapReader(pcapPath))
            {
                while (pcapReader.GetNextFrame(out var rawFrame))
                {
                    loader.AddFrame(rawFrame, ++frameNumber);
                }
                loader.Close();
            }
            
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Convs= {flowTable.Conversations.Count()} [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Frames= {flowTable.FramesCount} /{frameNumber} [{sw.Elapsed}]");
            sw.Restart();
            flowTable.SaveChanges();
            Console.WriteLine($"--- COMMITED --- [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Frames= {flowTable.FramesCount} / {frameNumber} [{sw.Elapsed}]");

            flowTable.Dispose();
        }

        private void PrintConversations(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ConversationKey, IConversationValue>> conversations)
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
            var sw = new Stopwatch();
            sw.Start();
            var flowTable = OpenTable();
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Convs= {flowTable.ConversationsCount} [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Frames= {flowTable.FramesCount}  [{sw.Elapsed}]");
        }

        public FasterConversationTable OpenTable()
        {
            var dbPath = Path.GetFullPath(@"c:\temp\0001\");
            var table = FasterConversationTable.Open(dbPath);


            return table;
        }
    }
}
