using WitchHutSearch.Generator.Layers;
using WitchHutSearch.Generator.Noise;
using WitchHutSearch.Generator.Random;

namespace WitchHutSearch.Generator;

public class BiomeGenerator
{
    public ulong Seed { get; private set; }
    public DoublePerlinNoise Shift { get; set; } = new();
    public DoublePerlinNoise Temperature { get; set; } = new();
    public DoublePerlinNoise Humidity { get; set; } = new();
    public DoublePerlinNoise Continentalness { get; set; } = new();
    public DoublePerlinNoise Erosion { get; set; } = new();
    public DoublePerlinNoise Weirdness { get; set; } = new();
    public PerlinNoise[] Oct { get; } = new PerlinNoise[2 * 23];
    public Spline Spline { get; set; }
    public int PreviousIdx { get; set; }

    public BiomeGenerator(ulong seed)
    {
        Spline = CreateBiomeNoise();
        Seed = seed;

        var pxr = new Xoroshiro128(seed);
        var xlo = (ulong)pxr.NextLong();
        var xhi = (ulong)pxr.NextLong();

        pxr.Lo = xlo ^ 0x080518cf6af25384;
        pxr.Hi = xlo ^ 0x3f3dfb40a54febd5;
    }

    private static Spline CreateBiomeNoise()
    {
        var sp = new Spline();
        var sp1 = Spline.CreateLandSpline(-0.15f, 0.00f, 0.0f, 0.1f, 0.00f, -0.03f, false);
        var sp2 = Spline.CreateLandSpline(-0.10f, 0.03f, 0.1f, 0.1f, 0.01f, -0.03f, false);
        var sp3 = Spline.CreateLandSpline(-0.10f, 0.03f, 0.1f, 0.7f, 0.01f, -0.03f, true);
        var sp4 = Spline.CreateLandSpline(-0.05f, 0.03f, 0.1f, 1.0f, 0.01f, 0.01f, true);

        sp.AddValue(-1.10f, 0.044f);
        sp.AddValue(-1.02f, -0.2222f);
        sp.AddValue(-0.51f, -0.2222f);
        sp.AddValue(-0.44f, -0.12f);
        sp.AddValue(-0.18f, -0.12f);
        sp.AddValue(-0.16f, sp1);
        sp.AddValue(-0.15f, sp1);
        sp.AddValue(-0.10f, sp2);
        sp.AddValue(0.25f, sp3);
        sp.AddValue(1.00f, sp4);
        return sp;
    }
}