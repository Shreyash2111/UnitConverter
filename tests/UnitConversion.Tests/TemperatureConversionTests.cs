using FluentAssertions;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Services;
using UnitConversion.Application.Services.Converters;
using UnitConversion.Domain.Enums;
using UnitConversion.Domain.Interfaces;

namespace UnitConversion.Tests;

public class TemperatureConversionTests
{
    private readonly ConversionService _service;

    public TemperatureConversionTests()
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
    public async Task CelsiusToFahrenheit_BoilingPoint_ShouldReturn212()
    {
        var request = new ConversionRequest { Value = 100, FromUnit = "celsius", ToUnit = "fahrenheit" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(212.0, 0.01);
        result.Category.Should().Be("Temperature");
    }

    [Fact]
    public async Task FahrenheitToCelsius_FreezingPoint_ShouldReturn0()
    {
        var request = new ConversionRequest { Value = 32, FromUnit = "fahrenheit", ToUnit = "celsius" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(0.0, 0.01);
    }

    [Fact]
    public async Task CelsiusToKelvin_Zero_ShouldReturn273Point15()
    {
        var request = new ConversionRequest { Value = 0, FromUnit = "celsius", ToUnit = "kelvin" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(273.15, 0.01);
    }

    [Fact]
    public async Task KelvinToCelsius_AbsoluteZero_ShouldReturnMinus273Point15()
    {
        var request = new ConversionRequest { Value = 0, FromUnit = "kelvin", ToUnit = "celsius" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(-273.15, 0.01);
    }

    [Fact]
    public async Task KelvinToFahrenheit_AbsoluteZero_ShouldReturnMinus459Point67()
    {
        var request = new ConversionRequest { Value = 0, FromUnit = "kelvin", ToUnit = "fahrenheit" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(-459.67, 0.01);
    }

    [Fact]
    public async Task FahrenheitToKelvin_BoilingPoint_ShouldReturn373Point15()
    {
        var request = new ConversionRequest { Value = 212, FromUnit = "fahrenheit", ToUnit = "kelvin" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(373.15, 0.01);
    }

    [Fact]
    public async Task NegativeForty_CelsiusEqualsFahrenheit()
    {
        var request = new ConversionRequest { Value = -40, FromUnit = "celsius", ToUnit = "fahrenheit" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(-40.0, 0.01);
    }

    [Fact]
    public async Task BodyTemperature_FahrenheitToCelsius()
    {
        var request = new ConversionRequest { Value = 98.6, FromUnit = "fahrenheit", ToUnit = "celsius" };

        var result = await _service.ConvertAsync(request);

        result.ConvertedValue.Should().BeApproximately(37.0, 0.01);
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
