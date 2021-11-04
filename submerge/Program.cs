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
        if (!TrySwitchBranch(@base)) {
            Console.WriteLine("Switching branch was not successfull");
            return;
        }

        foreach (var commitInfo in ReadCommits()) {
            Console.WriteLine(commitInfo.Message);
        }
    }

    const string gitCmd = "git";
    const string checkoutCmd = "checkout";

    public static bool TrySwitchBranch(string branch)
    {
        RunProcess(gitCmd, $"{checkoutCmd} {branch}", out var reader);

        string outputLine;
        while ((outputLine = reader.ReadLine()) is not null) {
            if (outputLine.StartsWith("Switched") || outputLine.StartsWith("Already")) {
                Console.WriteLine("Have found keyword");
                return true;
            }
        }

        return false;
    }

    const string logCmd = "log --oneline";

    public static IEnumerable<(string Hash, string Message)> ReadCommits()
    {
        RunProcess(gitCmd, $"{logCmd} .", out var reader);

        string outputLine;
        while ((outputLine = reader.ReadLine()) is not null) {
            var commitInfo = outputLine.Split(" ", 2);
            yield return (commitInfo[0], commitInfo[1]);
        }
    }

    public static bool RunProcess(string cmd, string arg, out TextReader reader) 
    {
        using var process = new Process();
        process.StartInfo.FileName = cmd;
        process.StartInfo.Arguments = arg;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.ErrorDataReceived += new DataReceivedEventHandler(
            (sender, e) => Console.WriteLine(e.Data));

        process.Start();
        process.BeginErrorReadLine();
        var stdout = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        Console.WriteLine(stdout);
        reader = new StringReader(stdout);

        return process.ExitCode == 0;
    }
}
