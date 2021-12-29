using WitchHutSearch.Generator;
using WitchHutSearch.Generator.Features;

namespace WitchHutSearch;

public class SearchWorker
{
    private readonly BiomeGenerator _generator;
    private readonly WitchHutCount _count;

    public SearchWorker(BiomeGenerator generator, WitchHutCount count)
    {
        _generator = generator;
        _count = count;
    }

    public void Search(SearchRange range)
    {
        var locator = FeatureLocator.WitchHut;
        var seed = _generator.Seed.Seed;
        var count = (int)_count;
        bool preloaded;
        // Current region is the north-west region, as the neighbour search is east, south-east, and south.
        RegionHutPos nw = new(), ne = new(), sw = new(), se = new();

        for (nw.region.Z = range.MinZ + 1; nw.region.Z <= range.MaxZ; nw.region.Z++)
        {
            preloaded = false;
            ne.region.Z = nw.region.Z;
            se.region.Z = nw.region.Z + 1;
            sw.region.Z = nw.region.Z + 1;
            for (nw.region.X = range.MinX + 1; nw.region.X <= range.MaxX; nw.region.X++)
            {
                ne.region.X = nw.region.X + 1;
                se.region.X = nw.region.X + 1;
                sw.region.X = nw.region.X;
                if (preloaded)
                {
                    nw.hut.Copy(ne.hut);
                    sw.hut.Copy(se.hut);
                }
                else
                {
                    locator.GetPosition(seed, nw.region, ref nw.hut);
                    locator.GetPosition(seed, sw.region, ref sw.hut);
                    preloaded = true;
                }
                
                // Check distance (stop if count met and in range)

                locator.GetPosition(seed, ne.region, ref ne.hut);
                // Check distance (stop if count met and in range)
                
                locator.GetPosition(seed, se.region, ref se.hut);
                // Check distance
            }
        }
    }
}

public ref struct RegionHutPos
{
    public Pos region = new();
    public Pos hut = new();
}