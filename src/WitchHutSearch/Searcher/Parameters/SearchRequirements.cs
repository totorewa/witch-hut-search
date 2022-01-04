namespace WitchHutSearch.Searcher.Parameters;

public class SearchRequirements
{
    private readonly int _count = (int)WitchHutCount.Quad;
    public int Range { get; init; } = 500;

    public int Count
    {
        get => _count;
        init => _count = Math.Clamp(value, (int)WitchHutCount.Double, (int)WitchHutCount.Quad);
    }

    public IEnumerable<SearchRange> CreateSearchRanges(int count)
    {
        var rangeSector = (int)Math.Ceiling((double)Range / count);
        var sectorSize = rangeSector * 2;
        var minZ = -(rangeSector * count);
        var minX = -Range;
        var width = Range * 2;
        for (var i = 0; i < count; i++)
            yield return new SearchRange(minX, minZ + sectorSize * i, width, sectorSize - (i == count - 1 ? 0 : 1));
    }
}