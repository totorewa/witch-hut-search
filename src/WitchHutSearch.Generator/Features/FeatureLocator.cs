using System.Numerics;

namespace WitchHutSearch.Generator.Features;

public abstract class FeatureLocator
{
    public static FeatureLocator WitchHut { get; } = new WitchHutLocator();

    private FeatureLocator()
    {
    }

    public abstract void GetPosition(ulong seed, Vector2 region, ref Vector2 pos);

    private sealed class WitchHutLocator : FeatureLocator
    {
        public override void GetPosition(ulong seed, Vector2 region, ref Vector2 pos)
        {
            const ulong k = 0x5deece66d;
            const ulong mask = (1UL << 48) - 1;
            const ulong b = 0xb;

            seed = seed + (ulong)region.X * 341873128712UL + (ulong)region.Y * 132897987541UL + 14357620;
            seed ^= k;
            seed = (seed * k + b) & mask;

            pos.X = (int)(seed >> 17) % 24;
            pos.X = (int)(((ulong)region.X * 32 + (ulong)pos.X) << 4);
            seed = (seed * k + b) & mask;
            pos.Y = (int)(seed >> 17) % 24;
            pos.Y = (int)(((ulong)region.Y * 32 + (ulong)pos.Y) << 4);
        }
    }
}