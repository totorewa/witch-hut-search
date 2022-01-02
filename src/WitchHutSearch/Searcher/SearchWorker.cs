using System.Collections;
using Microsoft.Extensions.Logging;
using WitchHutSearch.Biomes;
using WitchHutSearch.Extensions;
using WitchHutSearch.Generator;
using WitchHutSearch.Generator.Features;
using WitchHutSearch.Searcher.Parameters;

namespace WitchHutSearch.Searcher;

public class SearchWorker
{
    private readonly ILogger _logger;
    private readonly HutCollation _collation;
    private readonly BiomeGenerator _generator;
    private readonly ISwampBiomeVerifier _swampBiomeVerifier;

    public SearchWorker(
        ILogger logger,
        HutCollation collation,
        BiomeGenerator generator)
    {
        _logger = logger;
        _collation = collation;
        _generator = generator;
        _swampBiomeVerifier = new SwampBiomeVerifier(_generator);
    }

    // This method's a bit big... I might clean it up someday... eventually... never..
    public void Search(SearchRange range)
    {
        var locator = FeatureLocator.WitchHut;
        var seed = _generator.Seed.Seed;
        var target = _collation.Requirements.Count;
        bool preloaded;

        _logger.LogTrace("Searching for {Huts} huts between {MinX}, {MinZ} and {MaxX}, {MaxZ} on thread {Thread}",
            target, range.MinX,
            range.MinZ, range.MaxX, range.MaxZ, Environment.CurrentManagedThreadId);

        var huts = new SearchRegionCollection(_swampBiomeVerifier);
        var validHuts = new Pos[4];

        for (var z = range.MaxZ; z >= range.MinZ; z--)
        {
            preloaded = false;
            huts.MoveZ(z);
            for (var x = range.MaxX; x >= range.MinX; x--)
            {
                huts.MoveX(x);
                if (preloaded)
                {
                    huts.NorthEast.hut.Copy(huts.NorthWest.hut);
                    huts.SouthEast.hut.Copy(huts.SouthWest.hut);
                    huts.NorthEast.IsSwamp = huts.NorthWest.IsSwamp;
                    huts.SouthEast.IsSwamp = huts.SouthWest.IsSwamp;
                }
                else
                {
                    locator.GetPosition(seed, huts.SouthEast.region, ref huts.SouthEast.hut);
                    locator.GetPosition(seed, huts.NorthEast.region, ref huts.NorthEast.hut);
                    huts.SouthEast.IsSwamp = null;
                    huts.NorthEast.IsSwamp = null;
                    preloaded = true;
                }

                locator.GetPosition(seed, huts.SouthWest.region, ref huts.SouthWest.hut);
                locator.GetPosition(seed, huts.NorthWest.region, ref huts.NorthWest.hut);
                huts.SouthWest.IsSwamp = null;
                huts.NorthWest.IsSwamp = null;

                if (!huts.AnyCloseTogether()) continue;

                var targetMet = true;
                var hutCount = 0;
                var i = 0;
                foreach (var r in huts)
                {
                    if (r.IsSwamp ?? false)
                        validHuts[hutCount++] = r.hut;
                    if (target + hutCount - i++ > 0)
                        continue;
                    targetMet = false;
                    break;
                }

                if (!targetMet) continue;
                for (; hutCount >= target; hutCount--)
                {
                    var centre = new Pos();
                    foreach (var h in validHuts.Take(hutCount))
                        centre.Add(h);
                    centre.X = (int)((double)centre.X / hutCount);
                    centre.Z = (int)((double)centre.Z / hutCount);
                    if (validHuts.Take(hutCount).All(h => centre.InSpawnDistanceFromCentre(h)))
                        _collation.Submit(hutCount, centre);
                }
            }
        }
    }
}

public class RegionHutPos
{
    private bool? _isSwamp;
    private readonly ISwampBiomeVerifier _swampBiomeVerifier;
    public Pos region = new();
    public Pos hut = new();

    public bool? IsSwamp
    {
        get => _isSwamp ??= _swampBiomeVerifier.IsInSwampBiome(hut);
        set => _isSwamp = value;
    }

    public RegionHutPos(ISwampBiomeVerifier swampBiomeVerifier)
    {
        _swampBiomeVerifier = swampBiomeVerifier;
        IsSwamp = null;
    }
}

public class SearchRegionCollection : IEnumerable<RegionHutPos>
{
    /// <summary>
    /// Current region
    /// </summary>
    public RegionHutPos SouthEast { get; }

    /// <summary>
    /// Neighbouring region
    /// </summary>
    public RegionHutPos NorthEast { get; }

    /// <summary>
    /// Neighbouring region
    /// </summary>
    public RegionHutPos SouthWest { get; }

    /// <summary>
    /// Neighbouring region
    /// </summary>
    public RegionHutPos NorthWest { get; }

    public SearchRegionCollection(ISwampBiomeVerifier swampBiomeVerifier)
    {
        SouthEast = new RegionHutPos(swampBiomeVerifier);
        SouthWest = new RegionHutPos(swampBiomeVerifier);
        NorthWest = new RegionHutPos(swampBiomeVerifier);
        NorthEast = new RegionHutPos(swampBiomeVerifier);
    }

    public void MoveX(int x)
    {
        SouthEast.region.X = x;
        SouthWest.region.X = x - 1;
        NorthEast.region.X = x;
        NorthWest.region.X = x - 1;
    }

    public void MoveZ(int z)
    {
        SouthEast.region.Z = z;
        SouthWest.region.Z = z;
        NorthEast.region.Z = z - 1;
        NorthWest.region.Z = z - 1;
    }

    public bool AnyCloseTogether()
    {
        var count = 0;
        return this.Any(a => this.Skip(++count).Any(b => a.hut.InSpawnDistanceWith(b.hut)));
    }

    public IEnumerator<RegionHutPos> GetEnumerator()
    {
        yield return SouthEast;
        yield return NorthEast;
        yield return SouthWest;
        yield return NorthWest;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}