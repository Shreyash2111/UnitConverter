using UnitConversion.Application.DTOs;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Interfaces;

namespace UnitConversion.Application.Services;

/// <inheritdoc />
public sealed class ConversionService : IConversionService
{
    private readonly IReadOnlyList<IUnitConverter> _converters;

    /// <summary>
    /// Creates a new conversion service with the given set of category-specific converters.
    /// </summary>
    public ConversionService(IEnumerable<IUnitConverter> converters)
    {
        _converters = converters.ToList();
    }

    /// <inheritdoc />
    public async Task<ConversionResponse> ConvertAsync(ConversionRequest request)
    {
        var fromUnit = request.FromUnit!.Trim().ToLowerInvariant();
        var toUnit = request.ToUnit!.Trim().ToLowerInvariant();

        var converter = _converters.FirstOrDefault(c => c.CanConvert(fromUnit, toUnit));

        if (converter is null)
        {
            // Determine whether the individual units exist at all
            var fromExists = _converters.Any(c => c.GetSupportedUnits()
                .Any(u => u.Equals(fromUnit, StringComparison.OrdinalIgnoreCase)));
            var toExists = _converters.Any(c => c.GetSupportedUnits()
                .Any(u => u.Equals(toUnit, StringComparison.OrdinalIgnoreCase)));

            if (!fromExists)
                throw new UnitNotFoundException(fromUnit);
            if (!toExists)
                throw new UnitNotFoundException(toUnit);

            throw new UnsupportedConversionException(fromUnit, toUnit);
        }

        var result = await converter.ConvertAsync(request.Value, fromUnit, toUnit);

        return new ConversionResponse
        {
            OriginalValue = request.Value,
            FromUnit = fromUnit,
            ToUnit = toUnit,
            ConvertedValue = Math.Round(result, 6),
            Category = converter.Category.ToString()
        };
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, IReadOnlyList<string>> GetSupportedUnits()
    {
        return _converters.ToDictionary(
            c => c.Category.ToString(),
            c => c.GetSupportedUnits());
    }
}
