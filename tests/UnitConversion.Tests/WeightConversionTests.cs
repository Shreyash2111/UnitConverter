using FluentAssertions;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Services;
using UnitConversion.Application.Services.Converters;
using UnitConversion.Domain.Enums;
using UnitConversion.Domain.Interfaces;

namespace UnitConversion.Tests;

public class WeightConversionTests
{
    private readonly ConversionService _service;

    public WeightConversionTests()
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
    public async Task KilogramsToPounds_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "kilogram", ToUnit = "pound" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(2.20462, 0.001);
        result.Category.Should().Be("Weight");
    }

    [Fact]
    public async Task PoundsToOunces_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "pound", ToUnit = "ounce" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(16.0, 0.01);
    }

    [Fact]
    public async Task GramsToMilligrams_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "gram", ToUnit = "milligram" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(1000.0, 0.001);
    }

    [Fact]
    public async Task OuncesToGrams_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "ounce", ToUnit = "gram" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(28.349523, 0.001);
    }

    [Fact]
    public async Task KilogramsToGrams_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "kilogram", ToUnit = "gram" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(1000.0, 0.001);
    }

    [Fact]
    public async Task PoundsToKilograms_ShouldReturnCorrectValue()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "pound", ToUnit = "kilogram" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(0.453592, 0.001);
    }

    [Fact]
    public async Task ZeroKilograms_ShouldReturnZeroPounds()
    {
        var request = new ConversionRequest { Value = 0, FromUnit = "kilogram", ToUnit = "pound" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().Be(0);
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
