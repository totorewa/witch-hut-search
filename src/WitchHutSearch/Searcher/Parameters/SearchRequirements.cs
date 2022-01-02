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
        var sectorSize = (int)Math.Ceiling((double)Range / count);
        var adjustedRange = sectorSize * count;
        var adjustedMin = -adjustedRange;
        var reqMin = -Range;
        var rangeZ = Range * 2;
        for (var i = 0; i < count; i++)
            yield return new SearchRange(adjustedMin + sectorSize * i, reqMin, sectorSize - 1, rangeZ);
    }
}