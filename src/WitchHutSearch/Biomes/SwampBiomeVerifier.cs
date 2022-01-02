using WitchHutSearch.Extensions;
using WitchHutSearch.Generator;

namespace WitchHutSearch.Biomes;

public interface ISwampBiomeVerifier
{
    bool IsInSwampBiome(Pos pos);
}

public class SwampBiomeVerifier : ISwampBiomeVerifier
{
    private readonly BiomeGenerator _generator;

    public SwampBiomeVerifier(BiomeGenerator generator)
    {
        _generator = generator;
    }

    public bool IsInSwampBiome(Pos pos)
        => _generator.IsSwamp(pos);
}