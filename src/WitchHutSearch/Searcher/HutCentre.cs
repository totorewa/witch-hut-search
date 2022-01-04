using System.Numerics;

namespace WitchHutSearch.Searcher;

public struct HutCentre : IEquatable<HutCentre>
{
    public Vector2 Position { get; }
    public int X => (int)Position.X;
    public int Y => (int)Position.Y;
    public int Huts { get; }
    public int DistanceFromOrigin { get; }

    public HutCentre(int huts, Vector2 pos)
    {
        Huts = huts;
        Position = pos;
        DistanceFromOrigin = (int)Vector2.Distance(Vector2.Zero, pos);
    }

    public HutCentre(int huts, int x, int y)
        : this(huts, new Vector2(x, y))
    {
    }

    public bool Equals(HutCentre other) 
        => Position.Equals(other.Position) && Huts == other.Huts && DistanceFromOrigin == other.DistanceFromOrigin;

    public override bool Equals(object? obj) 
        => obj is HutCentre other && Equals(other);

    public override int GetHashCode() 
        => HashCode.Combine(Position, Huts, DistanceFromOrigin);
}