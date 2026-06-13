using FluentValidation;
using UnitConversion.Application.DTOs;

namespace UnitConversion.Application.Validators;

/// <summary>
/// Validates incoming conversion requests.
/// </summary>
public sealed class ConversionRequestValidator : AbstractValidator<ConversionRequest>
{
    /// <summary>Initializes validation rules for conversion requests.</summary>
    public ConversionRequestValidator()
    {
        RuleFor(x => x.FromUnit)
            .NotNull().WithMessage("'FromUnit' must not be null.")
            .NotEmpty().WithMessage("'FromUnit' must not be empty.");

        RuleFor(x => x.ToUnit)
            .NotNull().WithMessage("'ToUnit' must not be null.")
            .NotEmpty().WithMessage("'ToUnit' must not be empty.");

        RuleFor(x => x.Value)
            .Must(v => !double.IsNaN(v) && !double.IsInfinity(v))
            .WithMessage("'Value' must be a finite number.");
    }
}
