using FluentAssertions;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Services;
using UnitConversion.Application.Services.Converters;
using UnitConversion.Domain.Enums;
using UnitConversion.Domain.Interfaces;

namespace UnitConversion.Tests;

public class LengthConversionTests
{
    private readonly ConversionService _service;

    public LengthConversionTests()
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
    public async Task MetersToFeet_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 100, FromUnit = "meter", ToUnit = "foot" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(328.0839895, 0.001);
        result.Category.Should().Be("Length");
    }

    [Fact]
    public async Task KilometersToMiles_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "kilometer", ToUnit = "mile" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(0.621371, 0.001);
    }

    [Fact]
    public async Task InchesToCentimeters_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "inch", ToUnit = "centimeter" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(2.54, 0.001);
    }

    [Fact]
    public async Task YardsToMeters_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "yard", ToUnit = "meter" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(0.9144, 0.001);
    }

    [Fact]
    public async Task MillimetersToInches_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 25.4, FromUnit = "millimeter", ToUnit = "inch" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(1.0, 0.001);
    }

    [Fact]
    public async Task MilesToKilometers_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "mile", ToUnit = "kilometer" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(1.609344, 0.001);
    }

    [Fact]
    public async Task FeetToInches_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "foot", ToUnit = "inch" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(12.0, 0.001);
    }

    [Fact]
    public async Task ZeroMeters_ShouldReturnZeroFeet()
    {
        var request = new ConversionRequest { Value = 0, FromUnit = "meter", ToUnit = "foot" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().Be(0);
    }

    [Fact]
    public async Task NegativeValue_ShouldConvertCorrectly()
    {
        var request = new ConversionRequest { Value = -5, FromUnit = "meter", ToUnit = "foot" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(-16.4042, 0.001);
    }

    [Fact]
    public async Task CaseInsensitive_ShouldWork()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "METER", ToUnit = "Foot" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(3.28084, 0.001);
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
