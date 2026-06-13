using UnitConversion.Domain.Enums;

namespace UnitConversion.Domain.Interfaces;

/// <summary>
/// Defines the contract for a unit converter that handles conversions within a specific category.
/// </summary>
public interface IUnitConverter
{
    /// <summary>
    /// The category of units this converter handles.
    /// </summary>
    ConversionCategory Category { get; }

    /// <summary>
    /// Returns true if both units are supported by this converter.
    /// </summary>
    bool CanConvert(string fromUnit, string toUnit);

    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    /// <param name="value">The numerical value to convert.</param>
    /// <param name="fromUnit">The source unit (case-insensitive).</param>
    /// <param name="toUnit">The target unit (case-insensitive).</param>
    /// <returns>The converted value.</returns>
    Task<double> ConvertAsync(double value, string fromUnit, string toUnit);

    /// <summary>
    /// Returns all unit names supported by this converter.
    /// </summary>
    IReadOnlyList<string> GetSupportedUnits();
}
