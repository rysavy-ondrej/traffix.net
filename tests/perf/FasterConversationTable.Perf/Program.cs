using BenchmarkDotNet.Running;
using System.IO;
namespace FasterConversationTablePerf
{

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<IngestPacketTraceBenchmarkObservable>();
        }
    }
}
