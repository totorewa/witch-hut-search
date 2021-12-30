using WitchHutSearch.Extensions;
using WitchHutSearch.Generator;
using Xunit;

namespace WitchHutSearch.Tests;

public class WitchHutTest
{
    [Fact]
    public void TestPosDistance()
    {
        var pos1 = new Pos(255, 0);
        var pos2 = new Pos(0, 0);
        Assert.True(pos1.InSpawnDistanceFromCentre(pos2));

        pos1.X = 128;
        pos1.Z = 64;
        Assert.True(pos1.InSpawnDistanceFromCentre(pos2));

        pos2.X = -128;
        Assert.False(pos1.InSpawnDistanceFromCentre(pos2));
    }
}