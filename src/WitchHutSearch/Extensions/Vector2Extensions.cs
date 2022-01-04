using System.Numerics;
using System.Runtime.CompilerServices;

namespace WitchHutSearch.Extensions;

public static class Vector2Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InSpawnDistanceWith(this Vector2 source, Vector2 other)
    {
        // Despawn sphere is 256 blocks
        const int limit = 256 * 256;
        return Vector2.DistanceSquared(source, other) < limit;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InSpawnDistanceFromCentre(this Vector2 centre, Vector2 hut)
    {
        // Centre should be 128 blocks away from hut
        const int limit = 128 * 128;
        return Vector2.DistanceSquared(centre, hut) < limit;
    }
}