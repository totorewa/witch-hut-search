using Xunit;

namespace WitchHutSearch.Generator.Tests;

public class SeedHashingTest
{
    /// <summary>
    /// Ensure <see cref="WorldSeed.HashSeed"/> is behaving the same as Google's
    /// <c>Hashing.sha256().hashLong(long).asLong()</c> in Java by comparing the output
    /// of <see cref="WorldSeed.HashSeed"/> with the Java output of the same input seed. 
    /// </summary>
    /// <remarks>
    /// Hash is meant to be read in little endian order, however, endianness is not tested by this function and
    /// may fail on big endian systems.
    /// </remarks>
    [Fact]
    public void TestSeedHash()
    {
        // TODO: Run test on big endian
        const ulong seed = 10579846526078875722UL;
        const ulong expectedHashOutput = 4987684401955638491UL;
        
        Assert.Equal(WorldSeed.HashSeed(seed), expectedHashOutput);
    }
}