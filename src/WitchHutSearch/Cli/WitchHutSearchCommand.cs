using System.Diagnostics;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.Extensions.Logging;
using WitchHutSearch.Cli.Validators;
using WitchHutSearch.Generator;
using WitchHutSearch.Searcher;
using WitchHutSearch.Searcher.Parameters;

namespace WitchHutSearch.Cli;

[Command]
public class WitchHutSearchCommand : ICommand
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    [CommandParameter(0, Description = "Number of huts.", Validators = new[] { typeof(HutsParameterValidator) })]
    public int Huts { get; init; }

    [CommandOption("seed", 's', Description = "Seed to search on.", IsRequired = true)]
    public long Seed { get; init; }

    [CommandOption("blocks", 'b', Description = "Number of blocks to search in each direction.")]
    public int Blocks { get; init; } = 128000;

    [CommandOption("threads", 't', Description = "Number of threads to search with.")]
    public int ThreadCount { get; init; } = Environment.ProcessorCount * 2;

    public WitchHutSearchCommand()
    {
        _loggerFactory = LoggerFactory.Create(b => b.AddConsole());
        _logger = _loggerFactory.CreateLogger<WitchHutSearchCommand>();
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var requirements = CreateSearchRequirements();
        var generator = new BiomeGenerator(unchecked((ulong)Seed));
        var collation = new HutCollation(_loggerFactory.CreateLogger<HutCollation>(), requirements);
        var worker = new SearchWorker(_loggerFactory.CreateLogger<SearchWorker>(), collation, generator);

        _logger.LogInformation("Searching for {Huts} huts on seed: {Seed}",
            requirements.Count, Seed);

        var workers = requirements.CreateSearchRanges(ThreadCount)
            .Select(r => new Thread(() => worker.Search(r)))
            .ToArray();

        foreach (var thread in workers)
            thread.Start();

        foreach (var thread in workers)
            thread.Join();

        stopwatch.Stop();
        _logger.LogInformation("Search completed - printing results");

        foreach (var hut in collation.Centres.OrderBy(c => c.DistanceFromOrigin))
            console.Output.WriteLine($"{hut.Huts} at {hut.X}, {hut.Z}");

        _logger.LogInformation("Search completed in {ElapsedSeconds} seconds",
            stopwatch.Elapsed.TotalSeconds.ToString("F4"));

        return ValueTask.CompletedTask;
    }

    public SearchRequirements CreateSearchRequirements()
    {
        const int blocksInRegion = 16 * 16;
        var regions = (int)Math.Ceiling((float)Blocks / blocksInRegion);
        return new SearchRequirements
        {
            Count = Huts,
            Range = regions
        };
    }
}