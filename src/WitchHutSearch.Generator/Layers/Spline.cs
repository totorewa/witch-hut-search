namespace WitchHutSearch.Generator.Layers;

public class Spline : FixedValueSpline
{
    public SplineType Type { get; set; }
    public List<SplineValue> Values { get; } = new(12);

    public void AddValue(float loc, float value, float derivative = 0.0f)
        => AddValue(loc, new FixedValueSpline(value), derivative);

    public void AddValue(float loc, FixedValueSpline value, float derivative = 0.0f)
        => Values.Add(new SplineValue { Value = value, Location = loc, Derivative = derivative });

    public static Spline CreateLandSpline(float f, float g, float h, float i, float j, float k, bool bl)
    {
        var sp1 = CreateRidgeSpline((float)McMath.Lerp(i, 0.6f, 1.5f), bl);
        var sp2 = CreateRidgeSpline((float)McMath.Lerp(i, 0.6f, 1.0f), bl);
        var sp3 = CreateRidgeSpline(i, bl);
        var ih = 0.5f * i;
        var sp4 = CreateRidgeSpline(f - 0.15f, ih, ih, ih, i * 0.6f, 0.5f);
        var sp5 = CreateRidgeSpline(f, j * i, g * i, ih, i * 0.6f, 0.5f);
        var sp6 = CreateRidgeSpline(f, j, j, g, h, 0.5f);
        var sp7 = CreateRidgeSpline(f, j, j, g, h, 0.5f);

        var sp8 = new Spline { Type = SplineType.Ridges };
        sp8.AddValue(-1.0f, CreateFixSpline(f));
        sp8.AddValue(-0.4f, sp6);
        sp8.AddValue(0.0f, CreateFixSpline(h + 0.07f));

        var sp9 = CreateRidgeSpline(-0.02f, k, k, g, h, 0.0f);
        var sp = new Spline { Type = SplineType.Erosion };
        sp.AddValue(-0.85f, sp1);
        sp.AddValue(-0.7f, sp2);
        sp.AddValue(-0.4f, sp3);
        sp.AddValue(-0.35f, sp4);
        sp.AddValue(-0.1f, sp5);
        sp.AddValue(-0.2f, sp6);
        if (bl)
        {
            sp.AddValue(0.4f, sp7);
            sp.AddValue(0.45f, sp8);
            sp.AddValue(0.55f, sp8);
            sp.AddValue(0.58f, sp7);
        }

        sp.AddValue(0.7f, sp9);
        return sp;
    }

    public static Spline CreateRidgeSpline(float f, bool bl)
    {
        var sp = new Spline { Type = SplineType.Ridges };

        var i = GetOffsetValue(-1.0f, f);
        var k = GetOffsetValue(1.0f, f);
        var l = 1.0f - (1.0f - f) * 0.5f;
        var u = 0.5f * (1.0f - f);
        l = u / (0.46082947f * l) - 1.17f;

        if (l is > -0.65f and < 1.0f)
        {
            u = GetOffsetValue(-0.65f, f);
            var p = GetOffsetValue(-0.75f, f);
            var q = (p - i) * 4.0f;
            var r = GetOffsetValue(l, f);
            var s = (k - r) / (1.0f - l);

            sp.AddValue(-1.0f, CreateFixSpline(i), q);
            sp.AddValue(-0.75f, CreateFixSpline(p));
            sp.AddValue(-0.65f, CreateFixSpline(u));
            sp.AddValue(l - 0.01f, CreateFixSpline(r));
            sp.AddValue(l, CreateFixSpline(r), s);
            sp.AddValue(1.0f, CreateFixSpline(k), s);
        }
        else
        {
            u = (k - i) * 0.5f;
            if (bl)
            {
                sp.AddValue(-1.0f, CreateFixSpline(i > 0.2 ? i : 0.2f));
                sp.AddValue(0.0f, CreateFixSpline((float)McMath.Lerp(0.5f, i, k)), u);
            }
            else
            {
                sp.AddValue(-1.0f, CreateFixSpline(i), u);
            }

            sp.AddValue(1.0f, CreateFixSpline(k), u);
        }

        return sp;
    }

    public static FixedValueSpline CreateFixSpline(float value)
    {
        var sp = new FixedValueSpline { Value = value };
        return sp;
    }

    public static Spline CreateRidgeSpline(float f, float g, float h, float i, float j, float k)
    {
        var sp = new Spline { Type = SplineType.Ridges };

        var l = 0.5f * (g - f);
        if (l < k) l = k;
        var m = 5.0f * (h - g);

        sp.AddValue(-1.0f, CreateFixSpline(f), l);
        sp.AddValue(-0.4f, CreateFixSpline(g), l < m ? l : m);
        sp.AddValue(0.0f, CreateFixSpline(h), m);
        sp.AddValue(0.4f, CreateFixSpline(i), 2.0f * (i - h));
        sp.AddValue(1.0f, CreateFixSpline(j), 0.7f * (j - i));

        return sp;
    }

    public static float GetOffsetValue(float weirdness, float continentalness)
    {
        var f0 = 1.0f - (1.0f - continentalness) * 0.5f;
        var f1 = 0.5f * (1.0f - continentalness);
        var f2 = (weirdness + 1.17f) * 0.46082947f;
        var off = f2 * f0 - f1;

        if (weirdness < -0.7f)
            return off > -0.2222f ? off : -0.2222f; // clamp

        return off > 0 ? off : 0; // clamp
    }
}

public class SplineValue
{
    public float Location { get; set; }
    public float Derivative { get; set; }
    public FixedValueSpline Value { get; set; }
}