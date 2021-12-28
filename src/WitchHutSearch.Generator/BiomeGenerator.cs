using WitchHutSearch.Generator.Layers;
using WitchHutSearch.Generator.Noise;
using WitchHutSearch.Generator.Random;

namespace WitchHutSearch.Generator;

public class BiomeGenerator
{
    public WorldSeed Seed { get; }
    public DoublePerlinNoise Shift { get; set; }
    public DoublePerlinNoise Temperature { get; set; }
    public DoublePerlinNoise Humidity { get; set; }
    public DoublePerlinNoise Continentalness { get; set; }
    public DoublePerlinNoise Erosion { get; set; }
    public DoublePerlinNoise Weirdness { get; set; }
    public Spline Spline { get; set; }
    public int PreviousIdx { get; set; }

    public BiomeGenerator(ulong seed)
    {
        Spline = CreateBiomeNoise();
        Seed = new WorldSeed(seed);

        var pxr = new Xoroshiro128(seed);
        var xlo = (ulong)pxr.NextLong();
        var xhi = (ulong)pxr.NextLong();

        Span<double> amp = stackalloc double[]
        {
            1, 1, 1, 0, // Stack
            1.5, 0, 1, 0, 0, 0, // Temperature
            1, 1, 0, 0, 0, 0, // Humidity
            1, 1, 2, 2, 2, 1, 1, 1, 1, // Continentalness
            1, 1, 0, 1, 1, // Erosion
            1, 2, 1, 0, 0, 0, // Weirdness            
        };
        pxr.Lo = xlo ^ 0x080518cf6af25384;
        pxr.Hi = xhi ^ 0x3f3dfb40a54febd5;
        Shift = new DoublePerlinNoise(pxr, amp[..4], -3);

        pxr.Lo = xlo ^ 0x5c7e6b29735f0d7f;
        pxr.Hi = xhi ^ 0xf7d86f1bbc734988;
        Temperature = new DoublePerlinNoise(pxr, amp[4..10], -10);

        pxr.Lo = xlo ^ 0x81bb4d22e8dc168e;
        pxr.Hi = xhi ^ 0xf1c8b4bea16303cd;
        Humidity = new DoublePerlinNoise(pxr, amp[10..16], -8);

        pxr.Lo = xlo ^ 0x83886c9d0ae3a662;
        pxr.Hi = xhi ^ 0xafa638a61b42e8ad;
        Continentalness = new DoublePerlinNoise(pxr, amp[16..25], -9);

        pxr.Lo = xlo ^ 0xd02491e6058f6fd8;
        pxr.Hi = xhi ^ 0x4792512c94c17a80;
        Erosion = new DoublePerlinNoise(pxr, amp[25..30], -9);

        pxr.Lo = xlo ^ 0xefc8ef4d36102b34;
        pxr.Hi = xhi ^ 0x1beeeb324a0f24ea;
        Weirdness = new DoublePerlinNoise(pxr, amp[30..], -7);
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