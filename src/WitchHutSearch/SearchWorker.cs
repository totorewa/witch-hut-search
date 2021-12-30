using System.Buffers;
using WitchHutSearch.Extensions;
using WitchHutSearch.Generator;
using WitchHutSearch.Generator.Features;

namespace WitchHutSearch;

public class SearchWorker
{
    private readonly BiomeGenerator _generator;
    private readonly SearchRequirements _requirements;

    public SearchWorker(BiomeGenerator generator, SearchRequirements requirements)
    {
        _generator = generator;
        _requirements = requirements;
    }

    // This method's a bit big... I might clean it up someday... eventually... never..
    public void Search(SearchRange range)
    {
        var locator = FeatureLocator.WitchHut;
        var seed = _generator.Seed.Seed;
        var target = _requirements.Count;
        bool preloaded;

        var huts = new HutCollection();
        var neighbors = new RegionHutPos[4];

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

                if (!(huts.SouthEast.IsSwamp ??= _generator.IsSwamp(huts.SouthEast.hut)))
                    continue;

                huts.SouthWest.IsSwamp = null;
                huts.NorthWest.IsSwamp = null;

                var count = 0;
                foreach (var neighbor in huts.Neighbors)
                    neighbors[count++] = neighbor;

                if (count + 1 < target) continue;
                for (var i = 0; i < count && count + 1 - i >= target; i++)
                {
                    var centre = new Pos();
                    centre.Copy(huts.SouthEast.hut);
                    for (var j = 0; j < count && j < target - 1; j++)
                    {
                        var n = neighbors[i + j];
                        centre.X += n.hut.X;
                        centre.Z += n.hut.Z;
                    }

                    centre.X /= count + 1;
                    centre.Z /= count + 1;
                    var isValid = centre.InSpawnDistanceFromCentre(huts.SouthEast.hut);
                    if (!isValid) continue;
                    for (var j = 0; j < count && j < target - 1; j++)
                    {
                        var n = neighbors[i + j];
                        if (!centre.InSpawnDistanceFromCentre(n.hut) || !(n.IsSwamp ??= _generator.IsSwamp(n.hut)))
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        // Submit centre pos
                    }
                }
            }
        }
    }
}

public class RegionHutPos
{
    public Pos region = new();
    public Pos hut = new();
    public bool? IsSwamp { get; set; }

    public RegionHutPos()
    {
        IsSwamp = null;
    }
}

public class HutCollection
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

    public IEnumerable<RegionHutPos> Neighbors
    {
        get
        {
            if (SouthEast.hut.InSpawnDistanceWith(SouthWest.hut))
                yield return SouthWest;
            if (SouthEast.hut.InSpawnDistanceWith(NorthWest.hut))
                yield return NorthWest;
            if (SouthEast.hut.InSpawnDistanceWith(NorthEast.hut))
                yield return NorthEast;
        }
    }

    public HutCollection()
    {
        SouthEast = new RegionHutPos();
        SouthWest = new RegionHutPos();
        NorthWest = new RegionHutPos();
        NorthEast = new RegionHutPos();
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
}