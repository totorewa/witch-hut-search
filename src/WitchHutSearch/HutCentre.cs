using WitchHutSearch.Generator;

namespace WitchHutSearch;

public struct HutCentre : IEquatable<HutCentre>
{
    public int X { get; }
    public int Z { get; }
    public int Huts { get; }
    public int DistanceFromOrigin { get; }

    public HutCentre(int huts, Pos pos)
        : this(huts, pos.X, pos.Z)
    {
    }

    public HutCentre(int huts, int x, int z)
    {
        Huts = huts;
        X = x;
        Z = z;
        DistanceFromOrigin = (int)Math.Sqrt((long)x * x + (long)z * z);
    }

    public bool Equals(HutCentre other)
        => X == other.X && Z == other.Z && Huts == other.Huts;

    public override bool Equals(object? obj)
        => obj is HutCentre other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(X, Z, Huts);
}