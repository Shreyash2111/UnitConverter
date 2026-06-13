using UnitConversion.Application.DTOs;

namespace UnitConversion.Application.Services;

/// <summary>
/// Orchestrates unit conversions by delegating to the appropriate category-specific converter.
/// </summary>
public interface IConversionService
{
    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    Task<ConversionResponse> ConvertAsync(ConversionRequest request);

    /// <summary>
    /// Returns all supported units grouped by category name.
    /// </summary>
    IReadOnlyDictionary<string, IReadOnlyList<string>> GetSupportedUnits();
}
