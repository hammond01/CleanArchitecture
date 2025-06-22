using ProductManager.Blazor.Services.Impl;
using ProductManager.Shared.DTOs.UserDto;
using Console=System.Console;

namespace ProductManager.Blazor.Services;

public class IdentityServices : IIdentityServices
{
    private readonly HttpClient _httpClient;

    public IdentityServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var apiResponse = await Program.HttpClient.PostAsJsonAsync("api/identity/login", request);
            var responseBody = await apiResponse.Content.ReadAsStringAsync();
            //var LoginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody);
            var response = await _httpClient.PostAsJsonAsync("api/identity/login", request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return new LoginResponse();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}
