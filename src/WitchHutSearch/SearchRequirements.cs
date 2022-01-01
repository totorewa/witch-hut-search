namespace WitchHutSearch;

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
        var min = -Range;
        var rangeZ = Range * 2;
        var sectorSize = (int)Math.Ceiling((double)Range / count);
        for (var i = 0; i < count; i++)
            yield return new SearchRange(min + sectorSize * i, min, sectorSize - 1, rangeZ);
    }
}