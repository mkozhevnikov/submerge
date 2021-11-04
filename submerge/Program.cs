using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
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
        Console.WriteLine($"The head branch is '{head}'");
        if (!TrySwitchBranch(head)) {
            Console.WriteLine("Switching branch was not successfull");
            return;
        }

        var headCommits = ReadCommits().ToList();

        Console.WriteLine($"The base branch is '{@base}'");
        if (!TrySwitchBranch(@base)) {
            Console.WriteLine("Switching branch was not successfull");
            return;
        }

        var baseCommits = ReadCommits().Select(c => c.Hash).ToHashSet();

        Console.WriteLine($"The result branch is '{name}'");
        if (!TrySwitchBranch(name) && !TrySwitchBranch(name, checkoutCmd + " -b")) {
            Console.WriteLine("Switching branch was not successfull");
            return;
        }

        Console.WriteLine($"head branch has {headCommits.Count} commits total");
        Console.WriteLine($"base branch has {baseCommits.Count} commits total");
        for (var i = headCommits.Count - 1; i >= 0; i--) {
            var commit = headCommits[i];
            Console.WriteLine(commit.Hash);
            if (baseCommits.Contains(commit.Hash)) {
                continue;
            }
            else {
                Console.WriteLine("Cherry-picking commit: " + commit.Message);
                CherryPick(commit.Hash);
            }
        }
    }

    const string gitCmd = "git";
    const string checkoutCmd = "checkout";

    public static bool TrySwitchBranch(string branch, string checkoutCommand = checkoutCmd)
    {
        return RunProcess(gitCmd, $"{checkoutCommand} {branch}", out var _);
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

    const string cherryPickCmd = "cherry-pick";
    public static bool CherryPick(string commitHash)
    {
        return RunProcess(gitCmd, $"{cherryPickCmd} {commitHash}", out var _);
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
