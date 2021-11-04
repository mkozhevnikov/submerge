using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

class Program
{
    static async Task Main(string[] args)
    {
        var command = ArgsParser.CreateCommand(args);
        command.Handler = CommandHandler.Create<string, string, string>(PrintParsedParams);

        await command.InvokeAsync(args);
    }

    public static void PrintParsedParams(string @base, string head, string name)
    {
        Console.WriteLine($"The base branch is '{@base}'");
        const string gitCmd = "git";
        const string checkoutCmd = "checkout";
        RunProcess(gitCmd, $"{checkoutCmd} {@base}", out var reader);

        string outputLine;
        do {
            outputLine = reader.ReadLine();
            if (outputLine == null) break;
        } while (outputLine.StartsWith("Switched") || outputLine.StartsWith("Already"));

        const string logCmd = "log --oneline";
        RunProcess(gitCmd, $"{logCmd} .", out reader);

        var commitLog = new List<(string, string)>();
        while ((outputLine = reader.ReadLine()) is not null) {
            var commitInfo = outputLine.Split(" ", 2);
            commitLog.Add((commitInfo[0], commitInfo[1]));
            Console.WriteLine(commitInfo[1]);
        }
    }

    public static void RunProcess(string cmd, string arg, out StreamReader reader)
    {
        using var process = new Process();
        process.StartInfo.FileName = cmd;
        process.StartInfo.Arguments = arg;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();

        reader = process.StandardOutput;
        process.WaitForExit();
    }
}
