using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Web.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public IndexModel(
        ILogger<IndexModel> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public List<UserDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    public async Task OnGetAsync(int page = 1)
    {
        _logger.LogInformation("Users management page accessed by '{UserName}'", User.Identity?.Name);

        CurrentPage = page;

        try
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
            var client = _httpClientFactory.CreateClient();

            // Forward authentication cookie
            var authCookie = Request.Cookies[".AspNetCore.Identity.Application"];
            if (!string.IsNullOrEmpty(authCookie))
            {
                client.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Identity.Application={authCookie}");
            }

            // Build query string
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={PageSize}"
            };

            if (!string.IsNullOrEmpty(Search))
            {
                queryParams.Add($"search={Uri.EscapeDataString(Search)}");
            }

            if (!string.IsNullOrEmpty(Status))
            {
                queryParams.Add($"status={Uri.EscapeDataString(Status)}");
            }

            var queryString = string.Join("&", queryParams);
            var response = await client.GetAsync($"{apiBaseUrl}/api/admin/users?{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UsersResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null)
                {
                    Users = result.Data ?? new();
                    TotalCount = result.TotalCount;
                    TotalPages = result.TotalPages;
                }
            }
            else
            {
                _logger.LogWarning("Failed to load users. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
        }
    }

    public class UsersResponse
    {
        public List<UserDto>? Data { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? FullName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
