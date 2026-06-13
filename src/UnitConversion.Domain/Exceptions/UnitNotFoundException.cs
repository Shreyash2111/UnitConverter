namespace UnitConversion.Domain.Exceptions;

/// <summary>
/// Thrown when a requested unit is not found in the registry.
/// </summary>
public sealed class UnitNotFoundException : Exception
{
    /// <summary>The unit name that was not found.</summary>
    public string UnitName { get; }

    /// <summary>Creates a new instance for the given unit name.</summary>
    public UnitNotFoundException(string unitName)
        : base($"Unit '{unitName}' is not recognized. Use the supported units endpoint to see available units.")
    {
        UnitName = unitName;
    }
}
