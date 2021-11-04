using System.CommandLine;

public class ArgsParser
{
    public static RootCommand CreateCommand(string[] args)
    {
        var rootCmd = new RootCommand(
            "Creates a branch based on difference between base and head for a directory"
        );
        rootCmd.Add(new Option<string>(
            new []{ "-b", "--base" },
            "base branch for which a new branch will be created"
        ));
        rootCmd.Add(new Option<string>(
            new []{ "-h", "--head" },
            "branch which will be compared with a base branch"
        ));
        rootCmd.Add(new Option<string>(
            new []{ "-n", "--name" },
            "name of the result branch"
        ));
        return rootCmd;
    }
}
