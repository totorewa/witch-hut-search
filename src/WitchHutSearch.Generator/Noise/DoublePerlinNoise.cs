using WitchHutSearch.Generator.Random;

namespace WitchHutSearch.Generator.Noise;

public class DoublePerlinNoise
{
    public double Amplitude { get; set; }
    public OctaveNoise OctA { get; set; }
    public OctaveNoise OctB { get; set; }

    public DoublePerlinNoise(Xoroshiro128 xr, Span<double> amplitudes, int omin)
    {
        OctA = new OctaveNoise(xr, amplitudes, omin);
        OctB = new OctaveNoise(xr, amplitudes, omin);

        var len = amplitudes.Length;
        for (var i = len - 1; i >= 0 && amplitudes[i] == 0.0; i--)
            len--;
        for (var i = 0; amplitudes[i] == 0.0; i++)
            len--;
        Amplitude = 10d / 6d * len / (len + 1);
    }

    public double Sample(double x, double y, double z)
    {
        const double f = 337.0 / 331.0;
        var v = 0d;

        v += OctA.Sample(x, y, z);
        v += OctB.Sample(x * f, y * f, z * f);

        return v * Amplitude;
    }
}