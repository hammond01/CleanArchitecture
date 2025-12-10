using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Authorization;
using IdentityServer.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace IdentityServer.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize]
public class DashboardAdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ILogger<DashboardAdminController> _logger;

    public DashboardAdminController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IOpenIddictApplicationManager applicationManager,
        ILogger<DashboardAdminController> logger)
    {
        _context = context;
        _userManager = userManager;
        _applicationManager = applicationManager;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Policy = Policies.DashboardView)]
    [ProducesResponseType(typeof(DashboardStatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            // Total users count
            var totalUsers = await _userManager.Users.CountAsync();

            // Active users (logged in within last 30 days)
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var activeUsers = await _userManager.Users
                .Where(u => u.LastLoginAt != null && u.LastLoginAt > thirtyDaysAgo)
                .CountAsync();

            // Active sessions
            var activeSessions = await _context.UserSessions
                .Where(s => s.IsActive)
                .CountAsync();

            // Total OAuth clients
            var totalClients = await _applicationManager.CountAsync();

            // Recent registrations (last 7 days)
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            var recentRegistrations = await _userManager.Users
                .Where(u => u.CreatedAt > sevenDaysAgo)
                .CountAsync();

            // Users by role
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

            var adminCount = adminRole != null
                ? await _context.UserRoles.CountAsync(ur => ur.RoleId == adminRole.Id)
                : 0;

            var regularUserCount = userRole != null
                ? await _context.UserRoles.CountAsync(ur => ur.RoleId == userRole.Id)
                : 0;

            return Ok(new DashboardStatsResponse
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                ActiveSessions = activeSessions,
                TotalClients = (int)totalClients,
                RecentRegistrations = recentRegistrations,
                AdminCount = adminCount,
                RegularUserCount = regularUserCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard stats");
            return StatusCode(500, new { error = "Failed to retrieve statistics" });
        }
    }

    /// <summary>
    /// Get recent activities
    /// </summary>
    [HttpGet("activities")]
    [Authorize(Policy = Policies.DashboardView)]
    [ProducesResponseType(typeof(List<RecentActivityResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int limit = 10)
    {
        try
        {
            var recentSessions = await _context.UserSessions
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .Take(limit)
                .Select(s => new RecentActivityResponse
                {
                    UserName = s.User.UserName ?? "Unknown",
                    Email = s.User.Email ?? "",
                    Activity = "Login",
                    IpAddress = s.IpAddress ?? "Unknown",
                    DeviceInfo = s.DeviceInfo ?? "Unknown Device",
                    Timestamp = s.CreatedAt
                })
                .ToListAsync();

            return Ok(recentSessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent activities");
            return StatusCode(500, new { error = "Failed to retrieve activities" });
        }
    }
}

/// <summary>
/// Dashboard statistics response
/// </summary>
public class DashboardStatsResponse
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int ActiveSessions { get; set; }
    public int TotalClients { get; set; }
    public int RecentRegistrations { get; set; }
    public int AdminCount { get; set; }
    public int RegularUserCount { get; set; }
}

/// <summary>
/// Recent activity response
/// </summary>
public class RecentActivityResponse
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Activity { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string DeviceInfo { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
