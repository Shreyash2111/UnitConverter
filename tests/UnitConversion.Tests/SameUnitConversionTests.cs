using FluentAssertions;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Services;
using UnitConversion.Application.Services.Converters;
using UnitConversion.Domain.Enums;
using UnitConversion.Domain.Interfaces;

namespace UnitConversion.Tests;

public class SameUnitConversionTests
{
    private readonly ConversionService _service;

    public SameUnitConversionTests()
    {
        var converters = new IUnitConverter[]
        {
            CreateLengthConverter(),
            CreateWeightConverter(),
            new TemperatureUnitConverter()
        };
        _service = new ConversionService(converters);
    }

    [Fact]
    public async Task MeterToMeter_ShouldReturnSameValue()
    {
        var request = new ConversionRequest { Value = 42, FromUnit = "meter", ToUnit = "meter" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().Be(42);
    }

    [Fact]
    public async Task KilogramToKilogram_ShouldReturnSameValue()
    {
        var request = new ConversionRequest { Value = 99.5, FromUnit = "kilogram", ToUnit = "kilogram" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().Be(99.5);
    }

    [Fact]
    public async Task CelsiusToCelsius_ShouldReturnSameValue()
    {
        var request = new ConversionRequest { Value = 37, FromUnit = "celsius", ToUnit = "celsius" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(37.0, 0.001);
    }

    [Fact]
    public async Task FahrenheitToFahrenheit_ShouldReturnSameValue()
    {
        var request = new ConversionRequest { Value = 98.6, FromUnit = "fahrenheit", ToUnit = "fahrenheit" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(98.6, 0.001);
    }

    private static LinearUnitConverter CreateLengthConverter() =>
        new(ConversionCategory.Length, new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["meter"] = 1.0,
            ["kilometer"] = 1000.0,
            ["centimeter"] = 0.01,
            ["millimeter"] = 0.001,
            ["foot"] = 0.3048,
            ["inch"] = 0.0254,
            ["yard"] = 0.9144,
            ["mile"] = 1609.344,
        });

    private static LinearUnitConverter CreateWeightConverter() =>
        new(ConversionCategory.Weight, new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["kilogram"] = 1.0,
            ["gram"] = 0.001,
            ["milligram"] = 0.000001,
            ["pound"] = 0.45359237,
            ["ounce"] = 0.028349523125,
        });
}
