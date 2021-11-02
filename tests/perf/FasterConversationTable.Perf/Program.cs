using BenchmarkDotNet.Running;
using System;
using System.IO;
namespace FasterConversationTablePerf
{

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch(Int32.Parse(args[0]))
                {
                    case 1: BenchmarkRunner.Run<IngestPacketTraceBenchmarkFaster>();
                        break;
                    case 2:
                        BenchmarkRunner.Run<IngestPacketTraceBenchmarkObservable>();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please select a case to perftest:");
                Console.WriteLine("1: Faster");
                Console.WriteLine("2: Observable");
            }
        }
    }
}
