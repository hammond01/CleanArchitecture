namespace ProductManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestLogController : ControllerBase
{
    private readonly ILogger _logger;

    public TestLogController(ILogger logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult TestLogging()
    {
        _logger.LogInformation("This is an information log");
        _logger.LogWarning("This is a warning log");
        _logger.LogError("This is an error log");
        _logger.LogDebug("This is a debug log");
        _logger.LogTrace("This is a trace log");
        _logger.LogCritical("This is a critical log");

        return Ok("Logging test completed");
    }

    [HttpGet("exception")]
    public IActionResult TestExceptionLogging()
    {
        try
        {
            throw new Exception("This is a test exception");
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while processing the request", ex);
            return StatusCode(500, "Internal server error");
        }
    }
}
