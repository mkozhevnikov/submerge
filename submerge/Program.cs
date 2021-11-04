﻿using System;
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
        bool hasSwitched = false;
        while ((outputLine = reader.ReadLine()) is not null) {
            if (outputLine.StartsWith("Switched") || outputLine.StartsWith("Already")) {
                hasSwitched = true;
                break;
            }
        }

        if (!hasSwitched) {
            Console.WriteLine("Switching branch was not successfull");
            return;
        }

        const string logCmd = "log --oneline";
        RunProcess(gitCmd, $"{logCmd} .", out reader);

        var commitLog = new List<(string, string)>();
        while ((outputLine = reader.ReadLine()) is not null) {
            var commitInfo = outputLine.Split(" ", 2);
            commitLog.Add((commitInfo[0], commitInfo[1]));
            Console.WriteLine(commitInfo[1]);
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
