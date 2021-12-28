using System.Security.Cryptography;

namespace WitchHutSearch.Generator;

public struct WorldSeed
{
    public ulong Seed { get; set; }
    public ulong Hash { get; set; }

    public WorldSeed(ulong seed)
    {
        Seed = seed;
        Hash = HashSeed(seed);
    }
    
    public static ulong HashSeed(ulong seed)
    {
        // Whether this works on big endian is untested
        using var sha = SHA256.Create();
        var seedBytes = BitConverter.GetBytes(seed);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(seedBytes);

        var hash = sha.ComputeHash(seedBytes);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(hash);
        return BitConverter.ToUInt64(hash);
    }
}