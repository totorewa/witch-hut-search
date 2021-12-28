namespace WitchHutSearch.Generator.Layers;

public class ConstantSpline
{
    public virtual float Value { get; } = 0.0f;

    protected ConstantSpline()
    {
    }

    public ConstantSpline(float value)
    {
        Value = value;
    }
}