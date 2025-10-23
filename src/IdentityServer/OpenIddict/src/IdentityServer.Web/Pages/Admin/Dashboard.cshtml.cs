using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Web.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly ILogger<DashboardModel> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public DashboardModel(
        ILogger<DashboardModel> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public DashboardStats Stats { get; set; } = new();

    public async Task OnGetAsync()
    {
        _logger.LogInformation("Admin dashboard accessed by user '{UserName}'", User.Identity?.Name);

        try
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
            var client = _httpClientFactory.CreateClient();

            // Get the authentication cookie and pass it to the API
            var authCookie = Request.Cookies[".AspNetCore.Identity.Application"];
            if (!string.IsNullOrEmpty(authCookie))
            {
                client.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Identity.Application={authCookie}");
            }

            var response = await client.GetAsync($"{apiBaseUrl}/api/admin/dashboard/stats");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var stats = JsonSerializer.Deserialize<DashboardStats>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (stats != null)
                {
                    Stats = stats;
                }
            }
            else
            {
                _logger.LogWarning("Failed to load dashboard stats. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard stats");
        }
    }

    public class DashboardStats
    {
        public int TotalUsers { get; set; }
        public int ActiveSessions { get; set; }
        public int TotalClients { get; set; }
        public int ActiveTokens { get; set; }
    }
}
