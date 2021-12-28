using WitchHutSearch.Generator.Random;

namespace WitchHutSearch.Generator.Noise;

public struct DoublePerlinNoise
{
    public double Amplitude { get; set; } = 0;
    public OctaveNoise OctA { get; set; } = new();
    public OctaveNoise OctB { get; set; } = new();

    public DoublePerlinNoise(Xoroshiro128 xr, PerlinNoise octaves, Span<double> amplitudes, int omin)
    {
        var n = 0;
        
        
        var len = amplitudes.Length;
        for (var i = len - 1; i >= 0 && amplitudes[i] == 0.0; i--)
            len--;
        for (var i = 0; amplitudes[i] == 0.0; i++)
            len--;
        Amplitude = 10d / 6d * len / (len + 1);
    }
}