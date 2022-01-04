using System.Numerics;
using Microsoft.Extensions.Logging;
using WitchHutSearch.Searcher.Parameters;

namespace WitchHutSearch.Searcher;

public class HutCollation
{
    private readonly ILogger _logger;
    private readonly object _lock = new();
    private readonly ICollection<HutCentre> _foundCentres = new HashSet<HutCentre>();
    public SearchRequirements Requirements { get; }
    public IEnumerable<HutCentre> Centres => _foundCentres;

    public HutCollation(
        ILogger logger,
        SearchRequirements requirements)
    {
        _logger = logger;
        Requirements = requirements;
    }

    public void Submit(int huts, in Vector2 pos)
    {
        if (huts < Requirements.Count)
            return;

        lock (_lock)
            _foundCentres.Add(new HutCentre(huts, pos));
        
        _logger.LogTrace("Added {Huts} huts centred at {X}, {Z} from thread {Thread}",
            huts, pos.X, pos.Y, Environment.CurrentManagedThreadId);
    }
}