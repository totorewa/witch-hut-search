namespace WitchHutSearch.Generator;

public record Pos
{
    public int X { get; }
    public int Z { get; }

    public Pos(int x, int z)
    {
        X = x;
        Z = z;
    }

    public Pos ToChunkPos()
        => new Pos(X >> 4, Z >> 4);
}