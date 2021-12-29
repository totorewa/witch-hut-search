using WitchHutSearch.Generator.Random;

namespace WitchHutSearch.Generator.Noise;

public struct PerlinNoise
{
    public byte[] D { get; } = new byte[512];
    public double A { get; set; } = 0;
    public double B { get; set; } = 0;
    public double C { get; set; } = 0;
    public double Amplitude { get; set; } = 1d;  // Might be unneeded for what we're doing
    public double Lacunarity { get; set; } = 1d; // Might be unneeded for what we're doing

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

    public double Sample(double d1, double d2, double d3, double yamp, double ymin)
    {
        d1 += A;
        d2 += B;
        d3 += C;
        var i1 = (int)d1 - (d1 < 0 ? 1 : 0);
        var i2 = (int)d2 - (d2 < 0 ? 1 : 0);
        var i3 = (int)d3 - (d3 < 0 ? 1 : 0);
        d1 -= i1;
        d2 -= i2;
        d3 -= i3;
        var t1 = d1 * d1 * d1 * (d1 * (d1 * 6d - 15d) + 10d);
        var t2 = d2 * d2 * d2 * (d2 * (d2 * 6d - 15d) + 10d);
        var t3 = d3 * d3 * d3 * (d3 * (d3 * 6d - 15d) + 10d);

        if (yamp != 0)
        {
            var yclamp = ymin < d2 ? ymin : d2;
            d2 -= Math.Floor(yclamp / yamp) * yamp;
        }

        i1 &= 0xff;
        i2 &= 0xff;
        i3 &= 0xff;

        var a1 = D[i1] + i2;
        var a2 = D[a1] + i3;
        var a3 = D[a1 + 1] + i3;
        var b1 = D[i1 + 1] + i2;
        var b2 = D[b1] + i3;
        var b3 = D[b1 + 1] + i3;

        var l1 = McMath.IndexedLerp(D[a2], d1, d2, d3);
        var l2 = McMath.IndexedLerp(D[b2], d1 - 1, d2, d3);
        var l3 = McMath.IndexedLerp(D[a3], d1, d2 - 1, d3);
        var l4 = McMath.IndexedLerp(D[b3], d1 - 1, d2 - 1, d3);
        var l5 = McMath.IndexedLerp(D[a2 + 1], d1, d2, d3 - 1);
        var l6 = McMath.IndexedLerp(D[b2 + 1], d1 - 1, d2, d3 - 1);
        var l7 = McMath.IndexedLerp(D[a3 + 1], d1, d2 - 1, d3 - 1);
        var l8 = McMath.IndexedLerp(D[b3 + 1], d1 - 1, d2 - 1, d3 - 1);

        l1 = McMath.Lerp(t1, l1, l2);
        l3 = McMath.Lerp(t1, l3, l4);
        l5 = McMath.Lerp(t1, l5, l6);
        l7 = McMath.Lerp(t1, l7, l8);

        l1 = McMath.Lerp(t2, l1, l3);
        l5 = McMath.Lerp(t2, l5, l7);

        return McMath.Lerp(t3, l1, l5);
    }
}