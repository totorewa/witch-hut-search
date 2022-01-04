using System.Diagnostics;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.Extensions.Logging;
using WitchHutSearch.Cli.Validators;
using WitchHutSearch.Generator;
using WitchHutSearch.Searcher;
using WitchHutSearch.Searcher.Parameters;
using WitchHutSearch.Writers;
using ConsoleWriter = WitchHutSearch.Writers.ConsoleWriter;

namespace WitchHutSearch.Cli;

[Command]
public class WitchHutSearchCommand : ICommand
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    [CommandParameter(0, Description = "Number of huts.", Validators = new[] { typeof(HutsParameterValidator) })]
    public int Huts { get; init; }

    [CommandOption("seed", 's', Description = "Seed to search on.", IsRequired = true)]
    public long Seed { get; init; }

    [CommandOption("blocks", 'b', Description = "Number of blocks to search in each direction.")]
    public int Blocks { get; init; } = 128000;

    [CommandOption("threads", 't', Description = "Number of threads to search with.")]
    public int ThreadCount { get; init; } = Environment.ProcessorCount * 2;

    [CommandOption("out", 'o', Description = "Output file for writing locations to.")]
    public string? Output { get; init; }

    [CommandOption("verbose", 'v', Description = "Detailed output")]
    public bool Verbose { get; init; } = false;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        _loggerFactory = LoggerFactory.Create(b =>
            b.AddSimpleConsole()
                .SetMinimumLevel(Verbose ? LogLevel.Trace : LogLevel.Information));
        _logger = _loggerFactory.CreateLogger("Main");
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var requirements = CreateSearchRequirements();
        var generator = new BiomeGenerator(unchecked((ulong)Seed));
        var collation = new HutCollation(_loggerFactory.CreateLogger("Collator"), requirements);
        var worker = new SearchWorker(_loggerFactory.CreateLogger("Worker"), collation, generator);

        _logger.LogInformation("Searching for {Huts} huts on seed: {Seed}",
            requirements.Count, Seed);

        if (!string.IsNullOrWhiteSpace(Output))
            _logger.LogInformation("Output file set to: {File}", Output.Trim());

        var workers = requirements.CreateSearchRanges(ThreadCount)
            .Select(r => new Thread(() => worker.Search(r)))
            .ToArray();

        foreach (var thread in workers)
            thread.Start();

        foreach (var thread in workers)
            thread.Join();

        stopwatch.Stop();

        await WriteAsync(console, collation);

        _logger.LogInformation("Search completed in {ElapsedSeconds} seconds",
            stopwatch.Elapsed.TotalSeconds.ToString("F4"));

        Thread.Sleep(1000); // This is silly but the process ends before the final log message is printed
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

    private async Task WriteAsync(IConsole console, HutCollation huts)
    {
        using var writer = GetAppropriateWriter(console);
        await writer.BeginAsync();

        var count = 0;
        foreach (var hut in huts.Centres.OrderBy(c => c.DistanceFromOrigin))
            if (await writer.WriteAsync(hut))
                count++;

        await writer.CompleteAsync(count);
    }

    private IHutWriter GetAppropriateWriter(IConsole console)
        => string.IsNullOrWhiteSpace(Output)
            ? new ConsoleWriter(_loggerFactory.CreateLogger("Output"), console)
            : new FileWriter(_loggerFactory.CreateLogger("Output"), Output);
}