using WitchHutSearch.Generator.Features;
using Xunit;

namespace WitchHutSearch.Generator.Tests;

public class WitchHutTest
{
    [Fact]
    public void TestWitchHutLocator()
    {
        const ulong seed = 10579846526078875722;

        Assert.Equal(new Pos(352, 176), FeatureLocator.WitchHut.GetPosition(seed, new Pos(0, 0)));
        Assert.Equal(new Pos(-17392, 26944), FeatureLocator.WitchHut.GetPosition(seed, new Pos(-34, 52)));
    }
}