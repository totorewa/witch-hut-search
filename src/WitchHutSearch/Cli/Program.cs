using CliFx;

namespace WitchHutSearch.Cli;

public class Program
{
    public static async Task<int> Main()
        => await new CliApplicationBuilder()
            .AddCommand<WitchHutSearchCommand>()
            .SetExecutableName("WitchHutSearch" + (OperatingSystem.IsWindows() ? ".exe" : ""))
            .SetTitle("Witch Hut Search")
            .Build()
            .RunAsync();
}