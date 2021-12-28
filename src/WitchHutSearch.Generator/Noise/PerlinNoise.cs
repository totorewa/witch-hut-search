namespace WitchHutSearch.Generator.Noise;

public struct PerlinNoise
{
    public byte[] D { get; } = new byte[512];
    public double A { get; set; } = 0;
    public double B { get; set; } = 0;
    public double C { get; set; } = 0;
    public double Amplitude { get; set; } = 0;
    public double Lacunarity { get; set; } = 0;
}