using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ProductManager.Infrastructure.Validation;

/// <summary>
/// Global model validation filter
/// </summary>
public class ModelValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            var apiResponse = new ApiResponse(400, "Validation failed", errors);
            context.Result = new BadRequestObjectResult(apiResponse);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Nothing to do here
    }
}

/// <summary>
/// Custom validation attributes
/// </summary>
public class RequiredGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string str)
        {
            return Guid.TryParse(str, out var guid) && guid != Guid.Empty;
        }
        if (value is Guid guid2)
        {
            return guid2 != Guid.Empty;
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The field {name} must be a valid non-empty GUID.";
    }
}

public class PositiveNumberAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is decimal decimalValue)
            return decimalValue > 0;
        if (value is double doubleValue)
            return doubleValue > 0;
        if (value is float floatValue)
            return floatValue > 0;
        if (value is int intValue)
            return intValue > 0;
        if (value is long longValue)
            return longValue > 0;

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The field {name} must be a positive number.";
    }
}

public class NotEmptyCollectionAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is System.Collections.IEnumerable enumerable)
        {
            return enumerable.Cast<object>().Any();
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The field {name} must contain at least one item.";
    }
}

/// <summary>
/// Extensions for validation configuration
/// </summary>
public static class ValidationExtensions
{
    public static IServiceCollection AddValidationConfiguration(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            // Disable automatic model validation response
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }
}
