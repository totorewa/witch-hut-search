namespace WitchHutSearch.Generator.Layers;

public class ConstantSpline : Spline
{
    public float Value { get; } = 0.0f;

    protected ConstantSpline()
    {
    }

    public ConstantSpline(float value)
    {
        Value = value;
    }

    public override float Get(Span<float> values)
    {
        return Value;
    }
}