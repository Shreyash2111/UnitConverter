namespace UnitConversion.Application.DTOs;

/// <summary>
/// Represents a request to convert a value from one unit to another.
/// </summary>
public sealed class ConversionRequest
{
    /// <summary>The numerical value to convert.</summary>
    public double Value { get; set; }

    /// <summary>The source unit name (case-insensitive).</summary>
    public string? FromUnit { get; set; }

    /// <summary>The target unit name (case-insensitive).</summary>
    public string? ToUnit { get; set; }
}
