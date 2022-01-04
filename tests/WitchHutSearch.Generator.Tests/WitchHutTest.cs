using System.Numerics;
using WitchHutSearch.Generator.Features;
using Xunit;

namespace WitchHutSearch.Generator.Tests;

public class WitchHutTest
{
    [Fact]
    public void TestWitchHutLocator()
    {
        const ulong seed = 10579846526078875722;

        var expected = new Vector2(352, 176);
        var pos = new Vector2(0, 0);
        var output = new Vector2();
        FeatureLocator.WitchHut.GetPosition(seed, pos, ref output);
        Assert.Equal((expected.X, expected.Y), (output.X, output.Y));

        expected.X = -17392;
        expected.Y = 26944;
        pos.X = -34;
        pos.Y = 52;
        FeatureLocator.WitchHut.GetPosition(seed, pos, ref output);
        Assert.Equal((expected.X, expected.Y), (output.X, output.Y));
    }

    [Fact]
    public void TestWitchHutWithBiome()
    {
        const ulong seed = 10579846526078875722;
        var pos = new Vector2();
        var g = new BiomeGenerator(seed);
        FeatureLocator.WitchHut.GetPosition(seed, new Vector2(-15, -34), ref pos);
        var biomeId = g.GetBiomeAtPos(pos);
        Assert.NotEqual(6, biomeId);
    }
}