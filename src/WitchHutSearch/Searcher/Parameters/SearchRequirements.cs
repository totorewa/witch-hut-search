namespace WitchHutSearch.Searcher.Parameters;

public class SearchRequirements
{
    private int _count = (int)WitchHutCount.Quad;
    public int Range { get; set; } = 500;

    public int Count
    {
        get => _count;
        set => _count = Math.Clamp(value, (int)WitchHutCount.Double, (int)WitchHutCount.Quad);
    }

    public IEnumerable<SearchRange> CreateSearchRanges(int count)
    {
        var rangeSector = (int)Math.Ceiling((double)Range / count);
        var sectorSize = rangeSector * 2;
        var minX = -(rangeSector * count);
        var minZ = -Range;
        var depthZ = Range * 2;
        for (var i = 0; i < count; i++)
            yield return new SearchRange(minX + sectorSize * i, minZ, sectorSize - (i == count - 1 ? 0 : 1), depthZ);
    }
}