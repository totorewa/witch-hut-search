using WitchHutSearch.Generator.Random;

namespace WitchHutSearch.Generator.Noise;

public struct OctaveNoise
{
    public int OctCnt { get; set; } = 0;
    public PerlinNoise Octaves { get; set; } = new();

    public OctaveNoise(Xoroshiro128 xr, PerlinNoise octaves, Span<double> amplitudes, int omin)
    {
        var n = 0;
        var lacuna = Math.Pow(2, omin);
        var persist = Math.Pow(2, amplitudes.Length - 1) / ((1L << amplitudes.Length) - 1d);
        var xlo = (ulong)xr.NextLong();
        var xhi = (ulong)xr.NextLong();

        var pxr = new Xoroshiro128();
        var i = 0;
        foreach (var amp in amplitudes)
        {
            if (amp != 0)
            {
                pxr.Lo = xlo ^ Md5OctaveN[12 + omin + i, 0];
                pxr.Hi = xhi ^ Md5OctaveN[12 + omin + i, 1];
                n++;
            }

            i++;
            lacuna *= 2.0;
            persist *= 0.5;
            
        }
    }

    private static readonly ulong[,] Md5OctaveN =
    {
        { 0xb198de63a8012672, 0x7b84cad43ef7b5a8 }, // md5 "octave_-12"
        { 0x0fd787bfbc403ec3, 0x74a4a31ca21b48b8 }, // md5 "octave_-11"
        { 0x36d326eed40efeb2, 0x5be9ce18223c636a }, // md5 "octave_-10"
        { 0x082fe255f8be6631, 0x4e96119e22dedc81 }, // md5 "octave_-9"
        { 0x0ef68ec68504005e, 0x48b6bf93a2789640 }, // md5 "octave_-8"
        { 0xf11268128982754f, 0x257a1d670430b0aa }, // md5 "octave_-7"
        { 0xe51c98ce7d1de664, 0x5f9478a733040c45 }, // md5 "octave_-6"
        { 0x6d7b49e7e429850a, 0x2e3063c622a24777 }, // md5 "octave_-5"
        { 0xbd90d5377ba1b762, 0xc07317d419a7548d }, // md5 "octave_-4"
        { 0x53d39c6752dac858, 0xbcd1c5a80ab65b3e }, // md5 "octave_-3"
        { 0xb4a24d7a84e7677b, 0x023ff9668e89b5c4 }, // md5 "octave_-2"
        { 0xdffa22b534c5f608, 0xb9b67517d3665ca9 }, // md5 "octave_-1"
        { 0xd50708086cef4d7c, 0x6e1651ecc7f43309 }, // md5 "octave_0"
    };
}