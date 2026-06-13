# Unit Conversion API

A production-quality ASP.NET Core Web API for converting numerical values between units of measurement across length, weight/mass, and temperature categories.

## Project Overview

This API provides a single `POST /api/conversions` endpoint that accepts a value with source and target units, and returns the converted result. A supplementary `GET /api/conversions/units` endpoint lists all supported units grouped by category.

Supported categories:
- **Length** - meter, kilometer, centimeter, millimeter, foot, inch, yard, mile
- **Weight/Mass** - kilogram, gram, milligram, pound, ounce
- **Temperature** - celsius, fahrenheit, kelvin

## Architecture

The solution follows **Clean Architecture** with four projects:

```
UnitConversion.sln
├── src/
│   ├── UnitConversion.Domain        # Core domain: models, enums, exceptions, interfaces
│   ├── UnitConversion.Application   # Business logic: services, DTOs, validators, converters
│   └── UnitConversion.Api           # HTTP layer: controllers, middleware, Swagger config
└── tests/
    └── UnitConversion.Tests         # Unit tests (xUnit + FluentAssertions)
```

**Dependency flow**: `Api -> Application -> Domain`

- **Domain** has zero external dependencies and defines the contracts (`IUnitConverter`) and domain models.
- **Application** implements business logic and references only Domain.
- **Api** is the composition root that wires everything together.
- **Tests** reference Application and Domain to test business logic directly.

## Design Decisions

### Base-unit conversion pattern
Linear categories (length, weight) use a **factor-to-base-unit** approach. Each unit stores a single multiplication factor relative to the base unit (meter for length, kilogram for weight). Converting A to B is: `result = value * (factorA / factorB)`. Adding a new unit requires a single dictionary entry.

### Temperature as a special case
Temperature conversions involve offsets (not just scaling), so a dedicated `TemperatureUnitConverter` uses formula-based conversion through Kelvin as an intermediate.

### Registry-based extensibility
All converters implement `IUnitConverter` and are registered via DI. Adding a new category (e.g., volume, speed) means creating one new converter instance and registering it -- no existing code changes required.

### Global exception middleware
Domain exceptions (`UnitNotFoundException`, `UnsupportedConversionException`) are caught by middleware and translated into consistent `ApiErrorResponse` JSON. This keeps controllers thin and error handling centralized.

### FluentValidation
Request validation is handled by `ConversionRequestValidator`, keeping validation rules separate from controller logic and easily testable.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or later)
- A terminal / command prompt

## Getting Started

### Restore dependencies

```bash
dotnet restore
```

### Build

```bash
dotnet build
```

### Run tests

```bash
dotnet test
```

### Run the API

```bash
cd src/UnitConversion.Api
dotnet run
```

The API starts on `http://localhost:5000` (HTTP) and `https://localhost:5001` (HTTPS) by default.

### Swagger UI

After starting the API, open your browser to:

```
http://localhost:5000
```

Swagger UI is configured at the root URL so you can explore and test endpoints immediately.

## API Endpoints

### POST /api/conversions

Convert a value from one unit to another.

**Request:**
```json
{
  "value": 100,
  "fromUnit": "meter",
  "toUnit": "foot"
}
```

**Response (200 OK):**
```json
{
  "originalValue": 100,
  "fromUnit": "meter",
  "toUnit": "foot",
  "convertedValue": 328.084,
  "category": "Length"
}
```

**Error Response (400 Bad Request):**
```json
{
  "statusCode": 400,
  "message": "Unit 'lightyear' is not recognized. Use the supported units endpoint to see available units.",
  "errors": null
}
```

### GET /api/conversions/units

Returns all supported units grouped by category.

**Response (200 OK):**
```json
{
  "Length": ["meter", "kilometer", "centimeter", "millimeter", "foot", "inch", "yard", "mile"],
  "Weight": ["kilogram", "gram", "milligram", "pound", "ounce"],
  "Temperature": ["celsius", "fahrenheit", "kelvin"]
}
```

## Future Enhancements

- **Database-backed unit registry** - Persist units and factors in a database so new units can be added at runtime without redeployment.
- **Batch conversions** - Accept an array of conversion requests in a single API call.
- **Conversion history** - Store past conversions for analytics.
- **Additional categories** - Volume, speed, area, data storage, time.
- **Unit aliases** - Support shorthand names (e.g., "m" for "meter", "kg" for "kilogram").
- **Localization** - Return unit names in different languages.
- **Rate limiting** - Protect the API from excessive usage.
- **Health checks** - Add ASP.NET Core health check endpoints for monitoring.
