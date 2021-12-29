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

        var match = false;
        SplineValue? prevSv = null;
        SplineValue? sv = null;
        for (var i = 0; i < Values.Count; i++)
        {
            prevSv = sv;
            sv = Values[i];
            if (sv.Location >= f)
            {
                match = i != 0;
                break;
            }
        }

        if (!match)
        {
            var v = sv!.Value.Get(values);
            return v + sv.Derivative * (f - sv.Location);
        }

        var k = (f - prevSv!.Location) / (sv!.Location - prevSv.Location);
        var n = prevSv.Value.Get(values);
        var o = sv.Value.Get(values);
        var p = prevSv.Derivative * (sv.Location - prevSv.Location) - (o - n);
        var q = -sv.Derivative * (sv.Location - prevSv.Location) + (o - n);

        var r = (float)(McMath.Lerp(k, n, o) + k * (1f - k) * McMath.Lerp(k, p, q));
        return r;
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