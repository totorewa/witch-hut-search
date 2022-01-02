using CliFx.Extensibility;

namespace WitchHutSearch.Cli.Validators;

public class HutsParameterValidator : BindingValidator<int>
{
    public override BindingValidationError? Validate(int value)
        => value is < 2 or > 4
            ? new BindingValidationError("You must select 2, 3, or 4 huts")
            : null;
}