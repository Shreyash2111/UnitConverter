using UnitConversion.Domain.Enums;

namespace UnitConversion.Domain.Models;

/// <summary>
/// Defines a unit of measurement with its relationship to the category's base unit.
/// </summary>
public sealed class UnitDefinition
{
    /// <summary>
    /// The canonical name of the unit (lowercase), e.g. "meter", "kilogram".
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The category this unit belongs to.
    /// </summary>
    public ConversionCategory Category { get; }

    /// <summary>
    /// The factor to multiply by when converting this unit to the base unit.
    /// For example, if the base unit is meter, then kilometer has a factor of 1000.
    /// Not applicable for temperature (set to 0).
    /// </summary>
    public double ToBaseFactor { get; }

    /// <summary>
    /// Creates a new unit definition.
    /// </summary>
    public UnitDefinition(string name, ConversionCategory category, double toBaseFactor)
    {
        Name = name.ToLowerInvariant();
        Category = category;
        ToBaseFactor = toBaseFactor;
    }
}
