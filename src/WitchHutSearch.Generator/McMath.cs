using System.Runtime.CompilerServices;

namespace WitchHutSearch.Generator;

public static class McMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Lerp(double part, double from, double to)
        => from + part * (to - from);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double IndexedLerp(int idx, double a, double b, double c)
        => (idx & 0xf) switch
        {
            0 => a + b,
            1 => -a + b,
            2 => a - b,
            3 => -a - b,
            4 => a + c,
            5 => -a + c,
            6 => a - c,
            7 => -a - c,
            8 => b + c,
            9 => -b + c,
            10 => b - c,
            11 => -b - c,
            12 => a + b,
            13 => -b + c,
            14 => -a + b,
            15 => -b - c,
            _ => 0
        };
}