using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using FluentAssertions;
using CleanArchitecture.IntegrationTests.Infrastructure;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace CleanArchitecture.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for Identity/Authentication API endpoints
/// Tests the complete authentication and registration flow
/// </summary>
public class IdentityIntegrationTests : IClassFixture<SqlServerWebApplicationFactory>
{
    private sealed record AuthPayload(string UserId, string Token, string RefreshToken);

    private readonly HttpClient _client;
    private readonly SqlServerWebApplicationFactory _factory;

    public IdentityIntegrationTests(SqlServerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Helper Methods

    private static AuthPayload ExtractAuthPayload(string jsonContent)
    {
        var doc = JsonDocument.Parse(jsonContent);
        var payload = doc.RootElement.TryGetProperty("data", out var dataElement)
            ? dataElement
            : doc.RootElement;

        var userId = payload.GetProperty("userId").GetString();
        var token = payload.GetProperty("token").GetString();
        var refreshToken = payload.GetProperty("refreshToken").GetString();

        userId.Should().NotBeNullOrEmpty();
        token.Should().NotBeNullOrEmpty();
        refreshToken.Should().NotBeNullOrEmpty();

        return new AuthPayload(userId!, token!, refreshToken!);
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

    #endregion

    #region Registration Tests

    [Fact]
    public async Task Register_WithValidData_ReturnsCreated()
    {
        FakeEmailService.Clear();
        // Arrange
        var command = new
        {
            UserName = $"testuser_{Guid.NewGuid()}",
            Email = $"test_{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange - missing Email and Password
        var command = new
        {
            UserName = "testuser"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"success\":false");
        content.Should().Contain("\"statusCode\":400");
        content.Should().Contain("\"error\"");
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        var command = new
        {
            UserName = $"testuser_{Guid.NewGuid()}",
            Email = $"test_{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!",
            ConfirmPassword = "DifferentPassword123!",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
    {
        FakeEmailService.Clear();
        // Arrange - Register first user
        var username = $"duplicate_{Guid.NewGuid()}";
        var firstCommand = new
        {
            UserName = username,
            Email = $"first_{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "First",
            LastName = "User",
            PhoneNumber = "1111111111"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", firstCommand);

        // Try to register with same username
        var duplicateCommand = new
        {
            UserName = username,
            Email = $"second_{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Second",
            LastName = "User",
            PhoneNumber = "2222222222"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/register", duplicateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        FakeEmailService.Clear();
        // Arrange - Register user first
        var username = $"loginuser_{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";
        var registerCommand = new
        {
            UserName = username,
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Login",
            LastName = "Test",
            PhoneNumber = "3333333333"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);

        var (userId, confirmationToken) = ExtractUserIdAndTokenFromLastEmail(email);
        await _client.PostAsJsonAsync("/api/v1/authentication/confirm-email", new { UserId = userId, Token = confirmationToken });

        var loginCommand = new
        {
            UserName = username,
            Password = password,
            RememberMe = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/login", loginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var authPayload = ExtractAuthPayload(content);
        authPayload.Token.Should().NotBeNullOrEmpty();
        authPayload.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var command = new
        {
            UserName = "nonexistentuser",
            Password = "AnyPassword123!",
            RememberMe = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        FakeEmailService.Clear();
        // Arrange - Register user first
        var username = $"passwordtest_{Guid.NewGuid()}";
        var email = $"{username}@example.com";
        var registerCommand = new
        {
            UserName = username,
            Email = email,
            Password = "CorrectPassword123!",
            ConfirmPassword = "CorrectPassword123!",
            FirstName = "Password",
            LastName = "Test",
            PhoneNumber = "4444444444"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);

        var (userId, confirmationToken) = ExtractUserIdAndTokenFromLastEmail(email);
        await _client.PostAsJsonAsync("/api/v1/authentication/confirm-email", new { UserId = userId, Token = confirmationToken });

        var loginCommand = new
        {
            UserName = username,
            Password = "WrongPassword123!",
            RememberMe = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/login", loginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Token Management Tests

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsOkWithNewToken()
    {
        FakeEmailService.Clear();
        // Arrange - Register and login to get token
        var username = $"refreshtest_{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";
        var registerCommand = new
        {
            UserName = username,
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Refresh",
            LastName = "Test",
            PhoneNumber = "5555555555"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);

        var (userId, confirmationToken) = ExtractUserIdAndTokenFromLastEmail(email);
        await _client.PostAsJsonAsync("/api/v1/authentication/confirm-email", new { UserId = userId, Token = confirmationToken });

        var loginCommand = new
        {
            UserName = username,
            Password = password,
            RememberMe = false
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/authentication/login", loginCommand);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginPayload = ExtractAuthPayload(loginContent);

        var refreshCommand = new
        {
            AccessToken = loginPayload.Token,
            RefreshToken = loginPayload.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/refresh-token", refreshCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshContent = await response.Content.ReadAsStringAsync();
        var refreshedPayload = ExtractAuthPayload(refreshContent);
        refreshedPayload.Token.Should().NotBeNullOrEmpty();
        refreshedPayload.RefreshToken.Should().NotBe(loginPayload.RefreshToken);
    }

    [Fact]
    public async Task Logout_WithValidRequest_ReturnsOk()
    {
        FakeEmailService.Clear();
        // Arrange - Register and login first
        var username = $"logouttest_{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";
        var registerCommand = new
        {
            UserName = username,
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Logout",
            LastName = "Test",
            PhoneNumber = "6666666666"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);

        var (userId, confirmationToken) = ExtractUserIdAndTokenFromLastEmail(email);
        await _client.PostAsJsonAsync("/api/v1/authentication/confirm-email", new { UserId = userId, Token = confirmationToken });

        var loginCommand = new
        {
            UserName = username,
            Password = password,
            RememberMe = false
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/authentication/login", loginCommand);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginPayload = ExtractAuthPayload(loginContent);
        loginPayload.Token.Should().NotBeNullOrEmpty();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload.Token);

        // Act
        var response = await _client.PostAsync("/api/v1/authentication/logout", content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_WithoutAuthentication_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsync("/api/v1/authentication/logout", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Email Confirmation Tests

    [Fact]
    public async Task ConfirmEmail_WithValidToken_ReturnsOk()
    {
        FakeEmailService.Clear();
        var username = $"confirm_{Guid.NewGuid()}";
        var email = $"{username}@example.com";
        var registerCommand = new
        {
            UserName = username,
            Email = email,
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Confirm",
            LastName = "Test",
            PhoneNumber = "9999999999"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);

        var (userId, confirmationToken) = ExtractUserIdAndTokenFromLastEmail(email);
        var command = new { UserId = userId, Token = confirmationToken };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/confirm-email", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResendConfirmation_WithValidUser_ReturnsOk()
    {
        FakeEmailService.Clear();
        var username = $"resend_{Guid.NewGuid()}";
        var registerCommand = new
        {
            UserName = username,
            Email = $"{username}@example.com",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Resend",
            LastName = "Test",
            PhoneNumber = "1212121212"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/authentication/resend-confirmation",
            new { UserNameOrEmail = username });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Password Reset Tests

    [Fact]
    public async Task RequestPasswordReset_WithValidEmail_ReturnsOk()
    {
        FakeEmailService.Clear();
        // Arrange - Register user first
        var username = $"resettest_{Guid.NewGuid()}";
        var email = $"{username}@example.com";
        var registerCommand = new
        {
            UserName = username,
            Email = email,
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Reset",
            LastName = "Test",
            PhoneNumber = "7777777777"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);

        var resetCommand = new
        {
            UserName = username
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/request-password-reset", resetCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RequestPasswordReset_RepeatedRequest_ReturnsBadRequest()
    {
        FakeEmailService.Clear();
        var username = $"reset_throttle_{Guid.NewGuid()}";
        var email = $"{username}@example.com";
        var registerCommand = new
        {
            UserName = username,
            Email = email,
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Reset",
            LastName = "Throttle",
            PhoneNumber = "7777770000"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);

        var first = await _client.PostAsJsonAsync(
            "/api/v1/authentication/request-password-reset",
            new { UserName = username });

        var second = await _client.PostAsJsonAsync(
            "/api/v1/authentication/request-password-reset",
            new { UserName = username });

        first.StatusCode.Should().Be(HttpStatusCode.OK);
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_WithValidToken_ReturnsOk()
    {
        FakeEmailService.Clear();
        var username = $"reset_{Guid.NewGuid()}";
        var email = $"{username}@example.com";
        var registerCommand = new
        {
            UserName = username,
            Email = email,
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Reset",
            LastName = "Flow",
            PhoneNumber = "8888888888"
        };
        await _client.PostAsJsonAsync("/api/v1/authentication/register", registerCommand);

        await _client.PostAsJsonAsync("/api/v1/authentication/request-password-reset", new { UserName = username });
        var (userId, resetToken) = ExtractUserIdAndTokenFromLastEmail(email);

        var command = new
        {
            UserId = userId,
            Token = resetToken,
            Password = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/authentication/reset-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}

