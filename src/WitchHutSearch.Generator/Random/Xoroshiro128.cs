using System.Numerics;

namespace WitchHutSearch.Generator.Random;

public class Xoroshiro128
{
    public ulong Lo { get; set; }

    public ulong Hi { get; set; }

    public Xoroshiro128()
    {
    }

    public Xoroshiro128(ulong seed)
    {
        const ulong xl = 0x9e3779b97f4a7c15;
        const ulong xh = 0x6a09e667f3bcc909;
        const ulong a = 0xbf58476d1ce4e5b9;
        const ulong b = 0x94d049bb133111eb;

        var l = seed ^ xh;
        var h = l + xl;
        l = (l ^ (l >> 30)) * a;
        h = (h ^ (h >> 30)) * a;
        l = (l ^ (l >> 27)) * b;
        h = (h ^ (h >> 27)) * b;
        Lo = l ^ (l >> 31);
        Hi = h ^ (h >> 31);
    }

    public long NextLong()
    {
        var l = Lo;
        var h = Hi;
        var n = BitOperations.RotateLeft(l + h, 17) + l;
        h ^= l;
        Lo = BitOperations.RotateLeft(l, 49) ^ h ^ (h << 21);
        Hi = BitOperations.RotateLeft(h, 28);
        return (long)n;
    }

    public int Next(int n)
    {
        var r = unchecked(((ulong)NextLong() & 0xFFFFFFFF) * (ulong)n);
        if ((uint)r < (uint)n)
        {
            while ((uint)r < (~n + 1) % n)
            {
                r = unchecked(((ulong)NextLong() & 0xFFFFFFFF) * (ulong)n);
            }
        }

        return (int)(r >> 32);
    }

    public double NextDouble()
        => unchecked(((ulong)NextLong() >> 11) * 1.1102230246251565E-16);

    public double NextFloat()
        => unchecked(((ulong)NextLong() >> 40) * 5.9604645E-8F);

    public void Skip(int count)
    {
        while (count-- > 0)
            NextLong();
    }
}