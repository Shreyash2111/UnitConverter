namespace UnitConversion.Api.Models;

/// <summary>
/// Standardized error response returned by the API for all error scenarios.
/// </summary>
public sealed class ApiErrorResponse
{
    /// <summary>The HTTP status code.</summary>
    public int StatusCode { get; set; }

    /// <summary>A human-readable error message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Optional list of validation error details.</summary>
    public IReadOnlyList<string>? Errors { get; set; }
}
