namespace WitchHutSearch.Generator;

public ref struct Pos
{
    public int X { get; set; }
    public int Z { get; set; }

    public Pos() : this(0, 0)
    {
    }

    public Pos(int x, int z)
    {
        X = x;
        Z = z;
    }

    public void Copy(Pos from)
    {
        X = from.X;
        Z = from.Z;
    }

    public void Deconstruct(out int x, out int z) => (x, z) = (Z, Z);

    public Pos ToChunkPos()
        => new(X >> 4, Z >> 4);
}