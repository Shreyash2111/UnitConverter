namespace UnitConversion.Application.DTOs;

/// <summary>
/// Represents the result of a unit conversion.
/// </summary>
public sealed class ConversionResponse
{
    /// <summary>The original value that was converted.</summary>
    public double OriginalValue { get; set; }

    /// <summary>The source unit.</summary>
    public string FromUnit { get; set; } = string.Empty;

    /// <summary>The target unit.</summary>
    public string ToUnit { get; set; } = string.Empty;

    /// <summary>The converted value.</summary>
    public double ConvertedValue { get; set; }

    /// <summary>The category of the conversion (e.g. "Length", "Weight", "Temperature").</summary>
    public string Category { get; set; } = string.Empty;
}
