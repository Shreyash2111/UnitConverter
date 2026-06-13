namespace UnitConversion.Domain.Enums;

/// <summary>
/// Represents the supported categories of unit conversion.
/// </summary>
public enum ConversionCategory
{
    /// <summary>Length units (meter, foot, mile, etc.).</summary>
    Length,

    /// <summary>Weight/mass units (kilogram, pound, ounce, etc.).</summary>
    Weight,

    /// <summary>Temperature units (celsius, fahrenheit, kelvin).</summary>
    Temperature,

    Volume
}
