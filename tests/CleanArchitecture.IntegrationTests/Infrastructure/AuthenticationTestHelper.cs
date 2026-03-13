using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using FluentAssertions;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace CleanArchitecture.IntegrationTests.Infrastructure;

internal static class AuthenticationTestHelper
{
    public static async Task<string> AuthenticateAsync(HttpClient client)
    {
        FakeEmailService.Clear();

        var username = $"auth_{Guid.NewGuid():N}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";
        var registerCommand = new
        {
            UserName = username,
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Auth",
            LastName = "User",
            PhoneNumber = "1234567890"
        };

        var registerResponse = await client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);
        registerResponse.EnsureSuccessStatusCode();

        var (userId, confirmationToken) = ExtractUserIdAndTokenFromLastEmail(email);
        var confirmResponse = await client.PostAsJsonAsync("/api/v1/authentication/confirm-email", new
        {
            UserId = userId,
            Token = confirmationToken
        });
        confirmResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/api/v1/authentication/login", new
        {
            UserName = username,
            Password = password,
            RememberMe = false
        });
        loginResponse.EnsureSuccessStatusCode();

        var content = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractAccessToken(content);
        token.Should().NotBeNullOrEmpty();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return token!;
    }

    private static string? ExtractAccessToken(string jsonContent)
    {
        var doc = JsonDocument.Parse(jsonContent);
        var payload = doc.RootElement.TryGetProperty("data", out var dataElement)
            ? dataElement
            : doc.RootElement;

        return payload.TryGetProperty("token", out var tokenElement)
            ? tokenElement.GetString()
            : null;
    }

    private static (string userId, string token) ExtractUserIdAndTokenFromLastEmail(string recipient)
    {
        var message = FakeEmailService.GetLastMessage(recipient);
        message.Should().NotBeNull("Expected an email to be sent");

        var link = ExtractFirstLink(message!.TextBody);
        var uri = new Uri(link);
        var query = QueryHelpers.ParseQuery(uri.Query);

        var userId = query["userId"].ToString();
        var token = query["token"].ToString();

        userId.Should().NotBeNullOrEmpty();
        token.Should().NotBeNullOrEmpty();

        return (userId, token);
    }

    private static string ExtractFirstLink(string text)
    {
        var match = Regex.Match(text, @"https?://\S+");
        match.Success.Should().BeTrue("Expected an email link in the message body");
        return match.Value.TrimEnd('.', ')', '"');
    }
}
