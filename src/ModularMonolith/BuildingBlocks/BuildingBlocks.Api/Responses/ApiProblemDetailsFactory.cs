using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Api.Responses;

public static class ApiProblemDetailsFactory
{
    public static ProblemDetails Create(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        IEnumerable<string>? errors = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        // Compatibility extensions for clients currently reading legacy fields.
        problemDetails.Extensions["success"] = false;
        problemDetails.Extensions["statusCode"] = statusCode;
        problemDetails.Extensions["error"] = detail;
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        if (errors != null)
        {
            var errorArray = errors.Where(static x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (errorArray.Length > 0)
            {
                problemDetails.Extensions["errors"] = errorArray;
            }
        }

        return problemDetails;
    }
}
