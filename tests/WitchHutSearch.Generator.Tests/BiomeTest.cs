using System.Numerics;
using Xunit;

namespace WitchHutSearch.Generator.Tests;

public class BiomeTest
{
    [Fact]
    public void TestBiomeGen()
    {
        var g = new BiomeGenerator(10579846526078875722UL);
        Assert.Equal(12, g.GetBiomeAtPos(new Vector2(0, 0)));
        Assert.Equal(1, g.GetBiomeAtPos(new Vector2(1337, -4221)));
    }
}