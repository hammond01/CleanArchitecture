using Auditing.Application.Features.AuditLogEntries.Queries;
using Auditing.Application.DTOs;
using BuildingBlocks.Api.Controllers;
using BuildingBlocks.Application.Dispatcher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auditing.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuditLogsController : BaseController
{
    private readonly IDispatcher _dispatcher;

    public AuditLogsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Get paginated audit log entries
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AuditLogEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuditEntries(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuditEntriesQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _dispatcher.DispatchAsync(query, cancellationToken);
        return Ok(result);
    }
}
