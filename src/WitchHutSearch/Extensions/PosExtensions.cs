using System.Runtime.CompilerServices;
using WitchHutSearch.Generator;

namespace WitchHutSearch.Extensions;

public static class PosExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InSpawnDistanceWith(this Pos source, Pos other)
    {
        // Despawn sphere is 256 blocks
        const int limit = 256 * 256;
        return IsCloseEnough(source, other, limit);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InSpawnDistanceFromCentre(this Pos centre, Pos hut)
    {
        // Centre should be 128 blocks away from hut
        const int limit = 128 * 128;
        return IsCloseEnough(centre, hut, limit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsCloseEnough(Pos left, Pos right, int limit)
    {
        double x = left.X - right.X;
        double z = left.Z - right.Z;
        return x * x + z * z < limit;
    }
}