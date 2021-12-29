using System.Runtime.CompilerServices;

namespace WitchHutSearch.Generator.Biomes;

public unsafe ref struct NoiseParameters
{
    public const int ParameterLength = 6;
    private fixed long parameters[ParameterLength];

    public Span<long> Parameters
        => new(Unsafe.AsPointer(ref parameters[0]), ParameterLength);

    public NoiseParameters(
        double temperature, double humidity,
        double continentalness, double erosion,
        double depth /* ?? */, double weirdness)
    {
        parameters[0] = ProcessParameter(temperature);
        parameters[1] = ProcessParameter(humidity);
        parameters[2] = ProcessParameter(continentalness);
        parameters[3] = ProcessParameter(erosion);
        parameters[4] = ProcessParameter(depth);
        parameters[5] = ProcessParameter(weirdness);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ProcessParameter(double input)
        => (long)(input * 10000f);
}

// Using extension methods to escape the unsafe context of the struct
public static class NoiseParametersExtensions
{
    public static int P2Overworld(this NoiseParameters np)
    {
        const ulong ds = unchecked((ulong)-1L);
        var idx = np.GetResultingNode(0, 0, ds, 0);
        var node = BiomeTree.Tree[idx];
        return (int)((node >> 48) & 0xff);
    }

    public static int GetResultingNode(this NoiseParameters np, int idx, int alt, ulong ds, int depth)
    {
        if (depth >= 4)
            return idx;
        uint[] steps = { 1111, 111, 11, 1 };
        uint step;

        do
        {
            step = steps[depth];
            depth++;
            if (depth > 4)
            {
                Console.WriteLine("Fatal error");
                Environment.Exit(1);
            }
        } while (idx + step >= BiomeTree.Tree.Length);

        var node = BiomeTree.Tree[idx];
        var inner = (ushort)(node >> 48);

        var leaf = alt;

        for (var i = 0; i < 10; i++)
        {
            var dsInner = np.GetNpDist(inner);
            if (dsInner < ds)
            {
                var leaf2 = np.GetResultingNode(inner, leaf, ds, depth);
                var dsLeaf2 = inner == leaf2 ? dsInner : np.GetNpDist(leaf2);
                if (dsLeaf2 < ds)
                {
                    ds = dsLeaf2;
                    leaf = leaf2;
                }
            }

            inner += (ushort)step;
            if (inner >= BiomeTree.Tree.Length)
                break;
        }

        return leaf;
    }

    public static ulong GetNpDist(this NoiseParameters np, int idx)
    {
        ulong ds = 0, node = BiomeTree.Tree[idx];
        var parameters = np.Parameters;

        for (var i = 0; i < parameters.Length; i++)
        {
            var idx2 = (byte)((node >> (8 * i)) & 0xff);
            var a = +parameters[i] - BiomeTree.BiomeParams[idx2, 1];
            var b = -parameters[i] + BiomeTree.BiomeParams[idx2, 0];
            var d = a > 0 ? a : b > 0 ? b : 0;
            ds += (ulong)(d * d);
        }

        return ds;
    }
}