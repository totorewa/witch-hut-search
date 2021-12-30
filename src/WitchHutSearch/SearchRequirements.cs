namespace WitchHutSearch;

public class SearchRequirements
{
    private int _count = (int)WitchHutCount.Quad;
    public int Range { get; set; }

    public int Count
    {
        get => _count;
        set => _count = Math.Clamp(value, (int)WitchHutCount.Double, (int)WitchHutCount.Quad);
    }
}