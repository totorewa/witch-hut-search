using WitchHutSearch.Generator.Random;

namespace WitchHutSearch.Generator.Noise;

public struct PerlinNoise
{
    public byte[] D { get; } = new byte[512];
    public double A { get; set; } = 0;
    public double B { get; set; } = 0;
    public double C { get; set; } = 0;
    public double Amplitude { get; set; } = 1d;
    public double Lacunarity { get; set; } = 1d;

    public PerlinNoise(Xoroshiro128 xr)
    {
        unchecked
        {
            A = xr.NextDouble() * 256;
            B = xr.NextDouble() * 256;
            C = xr.NextDouble() * 256;
        }

        for (var i = 0; i < 256; i++)
            D[i] = (byte)i;

        for (var i = 0; i < 256; i++)
        {
            var j = xr.Next(256 - i) + i;
            (D[i], D[j]) = (D[j], D[i]);
            D[i + 256] = D[i];
        }
    }
}