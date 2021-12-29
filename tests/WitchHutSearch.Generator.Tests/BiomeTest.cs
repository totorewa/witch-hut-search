using Xunit;

namespace WitchHutSearch.Generator.Tests;

public class BiomeTest
{
    [Fact]
    public void TestBiomeGen()
    {
        var g = new BiomeGenerator(10579846526078875722UL);
        Assert.Equal(12, g.GetBiomeAtPos(new Pos(0, 0)));
        Assert.Equal(10, g.GetBiomeAtPos(new Pos(1337, -4221)));
    }
}