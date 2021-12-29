namespace WitchHutSearch;

public class SearchRange
{
    private int _regionX;
    private int _regionZ;
    private int _width;
    private int _depth;

    public int RegionX
    {
        get => _regionX;
        set
        {
            if (_regionX != value)
            {
                _regionX = value;
                CalculateX();
            }
        }
    }

    public int RegionZ
    {
        get => _regionZ;
        set
        {
            if (_regionZ != value)
            {
                _regionZ = value;
                CalculateZ();
            }
        }
    }

    public int Width
    {
        get => _width;
        set
        {
            if (_width != value)
            {
                _width = value;
                CalculateX();
            }
        }
    }

    public int Depth
    {
        get => _depth;
        set
        {
            if (_depth != value)
            {
                _depth = value;
                CalculateZ();
            }
        }
    }

    public int MaxX { get; private set; }
    public int MinX => RegionX;
    public int MaxZ { get; private set; }
    public int MinZ => RegionZ;

    public SearchRange()
    {
    }

    public SearchRange(int regionX, int regionZ, int width, int depth)
    {
        _regionX = regionX;
        _regionZ = regionZ;
        _width = width;
        _depth = depth;
    }

    // SearchWorker search the east, south-east, and south neighbouring regions,
    // so the south-east corner is moved in by one as those edge regions will be searched in the neighbour search.
    private void CalculateX() => MaxX = _regionX + _width - 1;

    private void CalculateZ() => MaxZ = _regionZ + _depth - 1;
}