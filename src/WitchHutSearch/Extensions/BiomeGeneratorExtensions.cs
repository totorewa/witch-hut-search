using System.Numerics;
using System.Runtime.CompilerServices;
using WitchHutSearch.Generator;

namespace WitchHutSearch.Extensions;

public static class BiomeGeneratorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSwamp(this BiomeGenerator g, Vector2 pos)
        => g.GetBiomeAtPos(pos) == 6;
}