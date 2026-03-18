using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Api.Controllers;

/// <summary>
/// Base controller for all API controllers
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult Success()
    {
        return base.Ok(new { success = true });
    }

    protected IActionResult Success<T>(T data)
    {
        return base.Ok(new { success = true, data });
    }

    protected IActionResult SuccessMessage(string message)
    {
        return base.Ok(new { success = true, message });
    }

    protected IActionResult CreatedSuccess<T>(string location, T data)
    {
        return base.Created(location, new { success = true, data });
    }

    protected IActionResult Ok<T>(T data)
    {
        return Success(data);
    }

    protected IActionResult Created<T>(T data)
    {
        return base.StatusCode(StatusCodes.Status201Created, new { success = true, data });
    }

    protected IActionResult BadRequest(string message)
    {
        return base.BadRequest(new { success = false, error = message });
    }

    protected IActionResult NotFound(string message)
    {
        return base.NotFound(new { success = false, error = message });
    }
}
