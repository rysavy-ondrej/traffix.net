using ConsoleAppFramework;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Traffix.Hosting.Console;
using Traffix.Providers.PcapFile;

namespace IcsMonitor
{
    public partial class Program : TraffixConsoleApp
    {

        /// <summary>
        /// Export the payload of tcp segments. Each payload is put in a single binary file created in the target zip archive.  
        /// </summary>
        /// <param name="inputFile">The name of the input pcap file.</param>
        /// <param name="port">The port number of the TCP packet to be included.</param>
        /// <param name="outputFile">The name of the output zip file. If not specified the name is the same as the inpout name but with zip extension.</param>
        [Command("Export-TcpPayload")]
        public void ExportTcpPayload(
            string inputFile,
            int? port = null,
            string outputFile = null)
        {
            using var stream = new FileInfo(inputFile).OpenRead();
            using var captureReader = new CaptureFileReader(stream);
            using var payloadArchive = ZipFile.Open(outputFile, ZipArchiveMode.Update);
            int packetNumber = 0;
            while (captureReader.GetNextFrame(out var frame))
            {
                packetNumber++;
                var packet = Packet.ParsePacket(frame.LinkLayer, frame.Data);
                var tcpPacket = packet.Extract<TcpPacket>();
                if (tcpPacket != null)
                {
                    if (port == null || tcpPacket.SourcePort == port || tcpPacket.DestinationPort == port )
                    {
                        WriteNewFile(payloadArchive, packetNumber, tcpPacket.PayloadData);
                    }
                }
            }
        }

        private void WriteNewFile(ZipArchive payloadArchive, int packetNumber, ReadOnlySpan<byte> payloadData)
        {
            var entry = payloadArchive.CreateEntry($"{packetNumber.ToString("D8")}.bin");
            using var writer = new BinaryWriter(entry.Open());
            writer.Write(payloadData);
        }
    }
}
