namespace UnitConversion.Domain.Exceptions;

/// <summary>
/// Thrown when a conversion between two units is not supported,
/// typically because they belong to different categories.
/// </summary>
public sealed class UnsupportedConversionException : Exception
{
    /// <summary>The source unit that was requested.</summary>
    public string FromUnit { get; }

    /// <summary>The target unit that was requested.</summary>
    public string ToUnit { get; }

    /// <summary>Creates a new instance for the given unit pair.</summary>
    public UnsupportedConversionException(string fromUnit, string toUnit)
        : base($"Conversion from '{fromUnit}' to '{toUnit}' is not supported. Units may belong to different categories.")
    {
        FromUnit = fromUnit;
        ToUnit = toUnit;
    }
}
