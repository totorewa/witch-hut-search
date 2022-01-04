using System.Collections;
using System.Numerics;
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
        var seed = _generator.Seed;
        var target = _collation.Requirements.Count;
        bool preloaded;

        _logger.LogTrace("Searching for {Huts} huts between {MinX}, {MinZ} and {MaxX}, {MaxZ} on thread {Thread}",
            target, range.MinX,
            range.MinZ, range.MaxX, range.MaxZ, Environment.CurrentManagedThreadId);

        var huts = new SearchRegionCollection(_swampBiomeVerifier);
        var validHuts = new Vector2[4];

        for (var z = range.MaxZ; z >= range.MinZ; z--)
        {
            preloaded = false;
            huts.MoveZ(z);
            for (var x = range.MaxX; x >= range.MinX; x--)
            {
                huts.MoveX(x);
                if (preloaded)
                {
                    huts.NorthEast.CopyFrom(huts.NorthWest);
                    huts.SouthEast.CopyFrom(huts.SouthWest);
                }
                else
                {
                    locator.GetPosition(seed, huts.SouthEast.Region, ref huts.SouthEast.Hut);
                    locator.GetPosition(seed, huts.NorthEast.Region, ref huts.NorthEast.Hut);
                    huts.SouthEast.Reset();
                    huts.NorthEast.Reset();
                    preloaded = true;
                }

                locator.GetPosition(seed, huts.SouthWest.Region, ref huts.SouthWest.Hut);
                locator.GetPosition(seed, huts.NorthWest.Region, ref huts.NorthWest.Hut);
                huts.SouthWest.Reset();
                huts.NorthWest.Reset();

                if (!huts.AnyCloseTogether()) continue;

                var targetMet = true;
                var hutCount = 0;
                var i = 0;
                foreach (var r in huts)
                {
                    if (r.IsSwamp ?? false)
                        validHuts[hutCount++] = r.Hut;
                    if (target + hutCount - i++ > 0)
                        continue;
                    targetMet = false;
                    break;
                }

                if (!targetMet) continue;
                for (; hutCount >= target; hutCount--)
                {
                    var centre = validHuts.Take(hutCount).Aggregate(new Vector2(), Vector2.Add);
                    centre.X = (int)((double)centre.X / hutCount);
                    centre.Y = (int)((double)centre.Y / hutCount);
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
    public Vector2 Region;
    public Vector2 Hut;

    public bool? IsSwamp
    {
        get => _isSwamp ??= _swampBiomeVerifier.IsInSwampBiome(Hut);
        set => _isSwamp = value;
    }

    public RegionHutPos(ISwampBiomeVerifier swampBiomeVerifier)
    {
        _swampBiomeVerifier = swampBiomeVerifier;
    }

    public void CopyFrom(RegionHutPos other)
    {
        Hut.X = other.Hut.X;
        Hut.Y = other.Hut.Y;
        _isSwamp = other._isSwamp;
    }

    public void Reset()
    {
        _isSwamp = null;
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
        SouthEast.Region.X = x;
        SouthWest.Region.X = x - 1;
        NorthEast.Region.X = x;
        NorthWest.Region.X = x - 1;
    }

    public void MoveZ(int z)
    {
        SouthEast.Region.Y = z;
        SouthWest.Region.Y = z;
        NorthEast.Region.Y = z - 1;
        NorthWest.Region.Y = z - 1;
    }

    public bool AnyCloseTogether()
    {
        var count = 0;
        return this.Any(a => this.Skip(++count).Any(b => a.Hut.InSpawnDistanceWith(b.Hut)));
    }

    public IEnumerator<RegionHutPos> GetEnumerator()
    {
        for (var i = 0; i < 4; i++)
            yield return this[i]!;
    }

    public RegionHutPos? this[int i]
        => i switch
        {
            0 => SouthEast,
            1 => NorthEast,
            2 => SouthWest,
            3 => NorthWest,
            _ => default
        };

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}