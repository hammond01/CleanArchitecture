using Identity.Application.Features.Authentication.Commands;
using Identity.Application.Features.PasswordManagement.Commands;
using Identity.Application.Features.Registration.Commands;
using Identity.Domain.DTOs;
using Identity.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace CleanArchitecture.UnitTests.Identity;

public class IdentityCommandHandlerTests
{
    private readonly Mock<IIdentityRepository> _identityRepositoryMock;

    public IdentityCommandHandlerTests()
    {
        _identityRepositoryMock = new Mock<IIdentityRepository>();
    }

    [Fact]
    public async Task HandleAsync_UserLogin_ReturnsLoginResponse()
    {
        var command = new UserLoginCommand
        {
            UserName = "alice",
            Password = "password",
            RememberMe = true
        };
        var loginResponse = new LoginResponseDto
        {
            UserId = Guid.NewGuid().ToString(),
            Token = "access-token",
            RefreshToken = "refresh-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        _identityRepositoryMock
            .Setup(repo => repo.LoginAsync("alice", "password", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loginResponse);

        var handler = new UserLoginCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().BeEquivalentTo(loginResponse);
        _identityRepositoryMock.Verify(
            repo => repo.LoginAsync("alice", "password", true, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserRefreshToken_ReturnsLoginResponse()
    {
        var command = new UserRefreshTokenCommand
        {
            AccessToken = "access",
            RefreshToken = "refresh"
        };
        var loginResponse = new LoginResponseDto
        {
            UserId = Guid.NewGuid().ToString(),
            Token = "new-access",
            RefreshToken = "new-refresh",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        _identityRepositoryMock
            .Setup(repo => repo.RefreshTokenAsync("access", "refresh", It.IsAny<CancellationToken>()))
            .ReturnsAsync(loginResponse);

        var handler = new UserRefreshTokenCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().BeEquivalentTo(loginResponse);
        _identityRepositoryMock.Verify(
            repo => repo.RefreshTokenAsync("access", "refresh", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserLogout_ReturnsTrue()
    {
        var command = new UserLogoutCommand { UserId = "user-1" };
        var handler = new UserLogoutCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().BeTrue();
        _identityRepositoryMock.Verify(
            repo => repo.LogoutAsync("user-1", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserCreate_ReturnsUserId()
    {
        var command = new UserCreateCommand
        {
            UserName = "bob",
            Email = "bob@example.com",
            Password = "password",
            ConfirmPassword = "password",
            FirstName = "Bob",
            LastName = "Builder",
            PhoneNumber = "123"
        };
        var userId = Guid.NewGuid();

        _identityRepositoryMock
            .Setup(repo => repo.RegisterAsync(
                "bob",
                "bob@example.com",
                "password",
                "Bob",
                "Builder",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var handler = new UserCreateCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().Be(userId.ToString());
        _identityRepositoryMock.Verify(
            repo => repo.RegisterAsync(
                "bob",
                "bob@example.com",
                "password",
                "Bob",
                "Builder",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserConfirmEmail_ReturnsTrue()
    {
        var command = new UserConfirmEmailCommand
        {
            UserId = "user-2",
            Token = "token"
        };

        var handler = new UserConfirmEmailCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().BeTrue();
        _identityRepositoryMock.Verify(
            repo => repo.ConfirmEmailAsync("user-2", "token", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_RequestPasswordReset_ReturnsTrue()
    {
        var command = new RequestPasswordResetCommand { UserName = "carol" };
        var handler = new RequestPasswordResetCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().BeTrue();
        _identityRepositoryMock.Verify(
            repo => repo.RequestPasswordResetAsync("carol", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ResetPassword_ReturnsTrue()
    {
        var command = new ResetPasswordCommand
        {
            UserId = "user-3",
            Token = "token",
            Password = "newpass",
            ConfirmPassword = "newpass"
        };

        var handler = new ResetPasswordCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().BeTrue();
        _identityRepositoryMock.Verify(
            repo => repo.ResetPasswordAsync("user-3", "token", "newpass", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
