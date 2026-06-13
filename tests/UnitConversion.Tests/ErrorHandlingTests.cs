using FluentAssertions;
using FluentValidation;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Services;
using UnitConversion.Application.Services.Converters;
using UnitConversion.Application.Validators;
using UnitConversion.Domain.Enums;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Interfaces;

namespace UnitConversion.Tests;

public class ErrorHandlingTests
{
    private readonly ConversionService _service;
    private readonly ConversionRequestValidator _validator;

    public ErrorHandlingTests()
    {
        var converters = new IUnitConverter[]
        {
            CreateLengthConverter(),
            CreateWeightConverter(),
            new TemperatureUnitConverter()
        };
        _service = new ConversionService(converters);
        _validator = new ConversionRequestValidator();
    }

    // --- Unknown / unsupported units ---

    [Fact]
    public async Task UnknownFromUnit_ShouldThrowUnitNotFoundException()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "lightyear", ToUnit = "meter" };

        var act = () => _service.ConvertAsync(request);

        await act.Should().ThrowAsync<UnitNotFoundException>()
            .Where(ex => ex.UnitName == "lightyear");
    }

    [Fact]
    public async Task UnknownToUnit_ShouldThrowUnitNotFoundException()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "parsec" };

        var act = () => _service.ConvertAsync(request);

        await act.Should().ThrowAsync<UnitNotFoundException>()
            .Where(ex => ex.UnitName == "parsec");
    }

    [Fact]
    public async Task CrossCategory_ShouldThrowUnsupportedConversionException()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "kilogram" };

        var act = () => _service.ConvertAsync(request);

        await act.Should().ThrowAsync<UnsupportedConversionException>();
    }

    [Fact]
    public async Task LengthToTemperature_ShouldThrowUnsupportedConversionException()
    {
        var request = new ConversionRequest { Value = 100, FromUnit = "meter", ToUnit = "celsius" };

        var act = () => _service.ConvertAsync(request);

        await act.Should().ThrowAsync<UnsupportedConversionException>();
    }

    // --- Validation failures ---

    [Fact]
    public async Task NullFromUnit_ShouldFailValidation()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = null, ToUnit = "meter" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FromUnit");
    }

    [Fact]
    public async Task NullToUnit_ShouldFailValidation()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = null };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ToUnit");
    }

    [Fact]
    public async Task EmptyFromUnit_ShouldFailValidation()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "", ToUnit = "meter" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task EmptyToUnit_ShouldFailValidation()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task NaNValue_ShouldFailValidation()
    {
        var request = new ConversionRequest { Value = double.NaN, FromUnit = "meter", ToUnit = "foot" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task InfinityValue_ShouldFailValidation()
    {
        var request = new ConversionRequest { Value = double.PositiveInfinity, FromUnit = "meter", ToUnit = "foot" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidRequest_ShouldPassValidation()
    {
        var request = new ConversionRequest { Value = 100, FromUnit = "meter", ToUnit = "foot" };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    // --- Response shape ---

    [Fact]
    public async Task Response_ShouldContainAllFields()
    {
        var request = new ConversionRequest { Value = 5, FromUnit = "kilogram", ToUnit = "pound" };

        var result = await _service.ConvertAsync(request);

        result.OriginalValue.Should().Be(5);
        result.FromUnit.Should().Be("kilogram");
        result.ToUnit.Should().Be("pound");
        result.Category.Should().Be("Weight");
        result.ConvertedValue.Should().BeGreaterThan(0);
    }

    // --- GetSupportedUnits ---

    [Fact]
    public void GetSupportedUnits_ShouldReturnAllCategories()
    {
        var units = _service.GetSupportedUnits();

        units.Should().ContainKey("Length");
        units.Should().ContainKey("Weight");
        units.Should().ContainKey("Temperature");
        units.Should().HaveCount(3);
    }

    [Fact]
    public void GetSupportedUnits_LengthShouldHave8Units()
    {
        var units = _service.GetSupportedUnits();

        units["Length"].Should().HaveCount(8);
        units["Length"].Should().Contain("meter");
        units["Length"].Should().Contain("mile");
    }

    [Fact]
    public void GetSupportedUnits_WeightShouldHave5Units()
    {
        var units = _service.GetSupportedUnits();

        units["Weight"].Should().HaveCount(5);
        units["Weight"].Should().Contain("kilogram");
        units["Weight"].Should().Contain("pound");
    }

    [Fact]
    public void GetSupportedUnits_TemperatureShouldHave3Units()
    {
        var units = _service.GetSupportedUnits();

        units["Temperature"].Should().HaveCount(3);
        units["Temperature"].Should().Contain("celsius");
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
