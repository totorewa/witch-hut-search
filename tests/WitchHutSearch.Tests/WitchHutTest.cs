using System.Numerics;
using WitchHutSearch.Extensions;
using Xunit;

namespace WitchHutSearch.Tests;

public class WitchHutTest
{
    [Fact]
    public void TestPosDistance()
    {
        var pos1 = new Vector2(255, 0);
        var pos2 = new Vector2(0, 0);
        Assert.True(pos1.InSpawnDistanceWith(pos2));

        pos1.X = 128;
        pos1.Y = 64;
        Assert.True(pos1.InSpawnDistanceWith(pos2));

        pos2.X = -128;
        Assert.False(pos1.InSpawnDistanceWith(pos2));
    }
}