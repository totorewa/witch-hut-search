using WitchHutSearch.Generator.Random;

namespace WitchHutSearch.Generator.Noise;

public class OctaveNoise
{
    public List<PerlinNoise> Octaves { get; set; } = new(4);

    public OctaveNoise(Xoroshiro128 xr, Span<double> amplitudes, int omin)
    {
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
                Octaves.Add(new PerlinNoise(pxr) { Amplitude = amp * persist, Lacunarity = lacuna });
            }

            i++;
            lacuna *= 2.0;
            persist *= 0.5;
        }
    }

    public double Sample(double x, double y, double z)
    {
        double sum = 0;
        foreach (var pn in Octaves)
        {
            var lf = pn.Lacunarity;
            var ax = MaintainPrecision(x * lf);
            var ay = MaintainPrecision(y * lf);
            var az = MaintainPrecision(z * lf);
            var pv = pn.Sample(ax, ay, az, 0, 0);
            sum += pn.Amplitude * pv;
        }

        return sum;
    }

    private static double MaintainPrecision(double x)
        => x - Math.Floor(x / 33554432.0 + 0.5) * 33554432.0;

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