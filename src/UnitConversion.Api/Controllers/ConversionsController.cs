using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Models;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Services;

namespace UnitConversion.Api.Controllers;

/// <summary>
/// Handles unit conversion requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ConversionsController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IValidator<ConversionRequest> _validator;

    /// <summary>Initializes the controller with required services.</summary>
    public ConversionsController(
        IConversionService conversionService,
        IValidator<ConversionRequest> validator)
    {
        _conversionService = conversionService;
        _validator = validator;
    }

    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    /// <param name="request">The conversion request containing value, fromUnit, and toUnit.</param>
    /// <returns>The conversion result including the converted value and category.</returns>
    /// <response code="200">Conversion successful.</response>
    /// <response code="400">Validation failed or units are not supported.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConversionResponse>> Convert([FromBody] ConversionRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "One or more validation errors occurred.",
                Errors = errors
            });
        }

        var response = await _conversionService.ConvertAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Returns all supported units grouped by category.
    /// </summary>
    /// <returns>A dictionary of category names to their supported unit names.</returns>
    /// <response code="200">Returns the supported units.</response>
    [HttpGet("units")]
    [ProducesResponseType(typeof(IReadOnlyDictionary<string, IReadOnlyList<string>>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUnits()
    {
        var units = _conversionService.GetSupportedUnits();
        return Ok(units);
    }
}
