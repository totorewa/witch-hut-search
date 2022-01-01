namespace WitchHutSearch.Generator;

public struct Pos : IEquatable<Pos>
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

    public void Add(Pos pos)
    {
        X += pos.X;
        Z += pos.Z;
    }

    public void Copy(Pos from)
        => (X, Z) = (from.X, from.Z);

    public void Deconstruct(out int x, out int z)
        => (x, z) = (X, Z);

    public Pos ToChunkPos()
        => new(X >> 4, Z >> 4);

    public bool Equals(Pos other)
        => X == other.X && Z == other.Z;

    public override bool Equals(object? obj)
        => obj is Pos other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(X, Z);
}