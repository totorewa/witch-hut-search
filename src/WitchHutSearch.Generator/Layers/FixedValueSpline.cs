namespace WitchHutSearch.Generator.Layers;

public class FixedValueSpline
{
    public float Value { get; set; } = 0.0f;

    public FixedValueSpline()
    {
    }

    public FixedValueSpline(float value)
    {
        Value = value;
    }
}