using System.CommandLine;

public class ArgsParser
{
    public static RootCommand CreateCommand(string[] args)
    {
        var rootCmd = new RootCommand(
            "Creates a branch based on difference between base and head for a directory"
        );

        Option<string> NewOption(string[] aliases, string description, bool isRequired = true) =>
            new Option<string>(aliases, description) {
                IsRequired = isRequired
            };
        
        rootCmd.Add(NewOption(
            new[] { "-b", "--base" },
            "base branch for which a new branch will be created"
        ));
        rootCmd.Add(NewOption(
            new[] { "-h", "--head" },
            "branch which will be compared with a base branch"
        ));
        rootCmd.Add(NewOption(
            new[] { "-n", "--name" },
            "name of the result branch"
        ));
        return rootCmd;
    }
}
