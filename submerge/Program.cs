using System;
using System.Threading.Tasks;
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
        Console.WriteLine($"The base param is {@base}");
        Console.WriteLine($"The head param is {head}");
        Console.WriteLine($"The name param is {name}");
    }
}
