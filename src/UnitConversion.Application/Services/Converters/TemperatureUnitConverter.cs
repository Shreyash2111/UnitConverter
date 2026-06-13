using UnitConversion.Domain.Enums;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Interfaces;

namespace UnitConversion.Application.Services.Converters;

/// <summary>
/// Handles temperature conversions using dedicated formulas.
/// Temperature cannot use simple multiplication factors because the scales have different zero points.
/// </summary>
public sealed class TemperatureUnitConverter : IUnitConverter
{
    private static readonly HashSet<string> SupportedUnits = new(StringComparer.OrdinalIgnoreCase)
    {
        "celsius", "fahrenheit", "kelvin"
    };

    /// <inheritdoc />
    public ConversionCategory Category => ConversionCategory.Temperature;

    /// <inheritdoc />
    public bool CanConvert(string fromUnit, string toUnit)
    {
        return SupportedUnits.Contains(fromUnit) && SupportedUnits.Contains(toUnit);
    }

    /// <inheritdoc />
    public Task<double> ConvertAsync(double value, string fromUnit, string toUnit)
    {
        if (!SupportedUnits.Contains(fromUnit))
            throw new UnitNotFoundException(fromUnit);
        if (!SupportedUnits.Contains(toUnit))
            throw new UnitNotFoundException(toUnit);

        // Normalize to kelvin, then convert to target
        var kelvin = ToKelvin(value, fromUnit);
        var result = FromKelvin(kelvin, toUnit);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GetSupportedUnits() => SupportedUnits.ToList();

    private static double ToKelvin(double value, string unit) => unit.ToLowerInvariant() switch
    {
        "celsius" => value + 273.15,
        "fahrenheit" => (value - 32.0) * 5.0 / 9.0 + 273.15,
        "kelvin" => value,
        _ => throw new UnitNotFoundException(unit)
    };

    private static double FromKelvin(double kelvin, string unit) => unit.ToLowerInvariant() switch
    {
        "celsius" => kelvin - 273.15,
        "fahrenheit" => (kelvin - 273.15) * 9.0 / 5.0 + 32.0,
        "kelvin" => kelvin,
        _ => throw new UnitNotFoundException(unit)
    };
}
