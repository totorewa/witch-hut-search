using CliFx.Infrastructure;
using Microsoft.Extensions.Logging;
using WitchHutSearch.Searcher;

namespace WitchHutSearch.Writers;

public sealed class ConsoleWriter : IHutWriter
{
    private readonly ILogger _logger;
    private readonly IConsole _console;

    public ConsoleWriter(ILogger logger, IConsole console)
    {
        _logger = logger;
        _console = console;
    }

    public Task BeginAsync() => Task.CompletedTask;

    public Task<bool> WriteAsync(HutCentre centre)
    {
        _console.Output.WriteLine($"{centre.Huts} huts at {centre.X}, {centre.Z}");
        return Task.FromResult(true);
    }

    public Task CompleteAsync(int count)
    {
        _logger.LogInformation("Printed {Locations} locations", count);
        return Task.CompletedTask;
    }

    void IDisposable.Dispose()
    {
    }
}