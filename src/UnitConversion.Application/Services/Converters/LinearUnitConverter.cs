using UnitConversion.Domain.Enums;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Interfaces;

namespace UnitConversion.Application.Services.Converters;

/// <summary>
/// Handles conversions for categories where units relate to a base unit via a simple multiplication factor.
/// Suitable for length, weight, and most other linear unit categories.
/// </summary>
public sealed class LinearUnitConverter : IUnitConverter
{
    private readonly Dictionary<string, double> _factors;

    /// <inheritdoc />
    public ConversionCategory Category { get; }

    /// <summary>
    /// Creates a linear unit converter for the given category.
    /// </summary>
    /// <param name="category">The conversion category.</param>
    /// <param name="factorsToBase">
    /// A dictionary mapping unit names (lowercase) to their conversion factor to the base unit.
    /// The base unit itself should have a factor of 1.0.
    /// </param>
    public LinearUnitConverter(ConversionCategory category, Dictionary<string, double> factorsToBase)
    {
        Category = category;
        _factors = new Dictionary<string, double>(factorsToBase, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public bool CanConvert(string fromUnit, string toUnit)
    {
        return _factors.ContainsKey(fromUnit) && _factors.ContainsKey(toUnit);
    }

    /// <inheritdoc />
    public Task<double> ConvertAsync(double value, string fromUnit, string toUnit)
    {
        if (!_factors.TryGetValue(fromUnit, out var fromFactor))
            throw new UnitNotFoundException(fromUnit);
        if (!_factors.TryGetValue(toUnit, out var toFactor))
            throw new UnitNotFoundException(toUnit);

        var result = value * (fromFactor / toFactor);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GetSupportedUnits() => _factors.Keys.ToList();
}
