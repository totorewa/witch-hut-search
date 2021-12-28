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
    public NestedSpline Spline { get; set; }
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

    public int GetBiomeAtPos(Pos pos)
    {
        const int y = 8;
        pos = pos.ToChunkPos();
        var px = Shift.Sample(pos.X, 0, pos.Z) * 4d;
        var pz = Shift.Sample(pos.Z, pos.X, 0) * 4d;

        var c = Continentalness.Sample(px, 0, pz);
        var e = Erosion.Sample(px, 0, pz);
        var w = Weirdness.Sample(px, 0, pz);

        Span<float> npParam = stackalloc float[]
        {
            (float)c, (float)e, -3.0f * (Math.Abs(Math.Abs((float)w) - 0.6666667f) - 0.33333334f), (float)w
        };
        var off = Spline.Get(npParam) + 0.015f;
        var d = 1.0 - (y << 2) / 128d - 83d / 160d + off;

        var t = Temperature.Sample(px, 0, pz);
        var h = Temperature.Sample(px, 0, pz);

        Span<long> np = stackalloc long[]
        {
            (long)(t * 10000f),
            (long)(h * 10000f),
            (long)(c * 10000f),
            (long)(e * 10000f),
            (long)(d * 10000f),
            (long)(w * 10000f),
        };

        // TODO rest
        return 0;
    }

    private static NestedSpline CreateBiomeNoise()
    {
        var sp = new NestedSpline();
        var sp1 = Layers.Spline.CreateLandSpline(-0.15f, 0.00f, 0.0f, 0.1f, 0.00f, -0.03f, false);
        var sp2 = Layers.Spline.CreateLandSpline(-0.10f, 0.03f, 0.1f, 0.1f, 0.01f, -0.03f, false);
        var sp3 = Layers.Spline.CreateLandSpline(-0.10f, 0.03f, 0.1f, 0.7f, 0.01f, -0.03f, true);
        var sp4 = Layers.Spline.CreateLandSpline(-0.05f, 0.03f, 0.1f, 1.0f, 0.01f, 0.01f, true);

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