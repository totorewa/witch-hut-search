namespace WitchHutSearch.Generator;

public record struct Pos
{
    public int X { get; }
    public int Z { get; }

    public Pos(int x, int z)
    {
        X = x;
        Z = z;
    }

    public Pos ToChunkPos() 
        => new(X >> 4, Z >> 4);
}

public ref struct DeferredPos
{
    public int X { get; set; }
    public int Z { get; set; }

    public static implicit operator Pos(DeferredPos pos)
        => new(pos.X, pos.Z);
}