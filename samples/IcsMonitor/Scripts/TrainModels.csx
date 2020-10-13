using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

if (Args.Count == 2)
{
    var inputPath = Args[0];
    var modelPath = Args[1];

    var inputFiles = System.IO.Directory.EnumerateFiles(inputPath, "*.pcap", System.IO.SearchOption.AllDirectories);

    foreach (var inputFile in inputFiles)
    {
        var inputFileName = Path.GetFileName(inputFile);
        var modelFile = Path.Combine(modelPath, Path.ChangeExtension(inputFileName, "mdl.zip"));
        Console.WriteLine($"Training model from {inputFile}");
        var process = Process.Start("IcsMonitor.exe", $"Train-ModbusModel -inputFile \"{inputFile}\" -outputFile \"{modelFile}\"");
        process.WaitForExit();
        Console.WriteLine($"Done, model written to ${modelFile}.");
    }
}
else
{
    Console.Error.WriteLine("Missing arguments.");
    Console.Error.WriteLine("Usage: dotnet-script TrainModel.csx [SourcePath] [ModelFolder]");
}
