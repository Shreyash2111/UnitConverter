using System.Net;
using System.Text.Json;
using UnitConversion.Api.Models;
using UnitConversion.Domain.Exceptions;

namespace UnitConversion.Api.Middleware;

/// <summary>
/// Catches unhandled exceptions and translates them into consistent API error responses.
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>Initializes the middleware.</summary>
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invokes the middleware, catching any unhandled exceptions.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            UnitNotFoundException ex => (
                HttpStatusCode.BadRequest,
                new ApiErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                }),

            UnsupportedConversionException ex => (
                HttpStatusCode.BadRequest,
                new ApiErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                }),

            _ => LogAndCreateInternalError(exception)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private (HttpStatusCode, ApiErrorResponse) LogAndCreateInternalError(Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");
        return (
            HttpStatusCode.InternalServerError,
            new ApiErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred. Please try again later."
            });
    }
}
