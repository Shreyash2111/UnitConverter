using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UnitConversion.Application.Services;
using UnitConversion.Application.Services.Converters;
using UnitConversion.Domain.Enums;
using UnitConversion.Domain.Interfaces;

namespace UnitConversion.Application;

/// <summary>
/// Registers all application-layer services into the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services including converters, the conversion service, and validators.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register unit converters
        services.AddSingleton<IUnitConverter>(CreateLengthConverter());
        services.AddSingleton<IUnitConverter>(CreateWeightConverter());
        services.AddSingleton<IUnitConverter>(new TemperatureUnitConverter());
        services.AddSingleton<IUnitConverter>(CreateVolumeConverter());
        // Register services
        services.AddSingleton<IConversionService, ConversionService>();

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }

    private static LinearUnitConverter CreateLengthConverter()
    {
        // Each factor represents how many meters one unit equals.
        var factors = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["meter"] = 1.0,
            ["kilometer"] = 1000.0,
            ["centimeter"] = 0.01,
            ["millimeter"] = 0.001,
            ["foot"] = 0.3048,
            ["inch"] = 0.0254,
            ["yard"] = 0.9144,
            ["mile"] = 1609.344,
        };

        return new LinearUnitConverter(ConversionCategory.Length, factors);
    }

    private static LinearUnitConverter CreateWeightConverter()
    {
        // Each factor represents how many kilograms one unit equals.
        var factors = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["kilogram"] = 1.0,
            ["gram"] = 0.001,
            ["milligram"] = 0.000001,
            ["pound"] = 0.45359237,
            ["ounce"] = 0.028349523125,
        };

        return new LinearUnitConverter(ConversionCategory.Weight, factors);
    }

    private static LinearUnitConverter CreateVolumeConverter()
    {
        // Each factor represents how many litres one unit equals.
        var factors = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["litre"] = 1.0,
            ["millilitre"] = 0.001,
            ["gallon"] = 3.785411784,
            ["cup"] = 0.236588,
            ["fluidounce"] = 0.0295735,
        };
        return new LinearUnitConverter(ConversionCategory.Volume, factors);
    }
}
