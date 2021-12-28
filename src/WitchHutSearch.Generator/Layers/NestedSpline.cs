namespace WitchHutSearch.Generator.Layers;

public class NestedSpline : Spline
{
    public SplineType Type { get; set; }
    public List<SplineValue> Values { get; } = new(12);

    public void AddValue(float loc, float value, float derivative = 0.0f)
        => AddValue(loc, new ConstantSpline(value), derivative);

    public void AddValue(float loc, Spline value, float derivative = 0.0f)
        => Values.Add(new SplineValue(loc, derivative, value));

    public override float Get(Span<float> values)
    {
        var type = (int)Type;
        var f = values.Length > type ? values[type] : default;

        SplineValue? prevSv = null;
        SplineValue? sv = null;
        for (var i = 0; i < Values.Count; i++)
        {
            prevSv = sv;
            sv = Values[i];
            if (!(sv.Location >= f)) continue;
            if (i != 0 && i != Values.Count - 1) break;

            var v = sv.Value.Get(values);
            return v + sv.Derivative * (f - sv.Location);
        }

        var g = (f - prevSv!.Location) / (sv!.Location - prevSv.Location);
        var h = prevSv.Value.Get(values);
        var k = sv.Value.Get(values);
        var l = prevSv.Derivative * (sv.Location - prevSv.Location) - (k - h);
        var m = -sv.Derivative * (sv.Location - prevSv.Location) + (k - h);

        return (float)McMath.Lerp(g, h, k) + g * (1f - g) * (float)McMath.Lerp(g, l, m);
    }
}

public class SplineValue
{
    public float Location { get; }
    public float Derivative { get; }
    public Spline Value { get; }

    public SplineValue(float location, float derivative, Spline value)
    {
        Location = location;
        Derivative = derivative;
        Value = value;
    }
}