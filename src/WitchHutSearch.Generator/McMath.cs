using System.Runtime.CompilerServices;

namespace WitchHutSearch.Generator;

public static class McMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Lerp(double part, double from, double to)
        => from + part * (to - from);
}