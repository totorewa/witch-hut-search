using CliFx;

namespace WitchHutSearch.Cli;

public class Program
{
    public static async Task<int> Main()
        => await new CliApplicationBuilder()
            .AddCommand<WitchHutSearchCommand>()
            .SetExecutableName(Path.GetFileName(Environment.GetCommandLineArgs().First()))
            .SetTitle("Witch Hut Search")
            .Build()
            .RunAsync();
}