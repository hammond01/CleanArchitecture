using Identity.Application.Features.Authentication.Commands;
using Identity.Application.Features.PasswordManagement.Commands;
using Identity.Application.Features.Registration.Commands;
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
    public async Task HandleAsync_UserLogin_ReturnsUserName()
    {
        var command = new UserLoginCommand
        {
            UserName = "alice",
            Password = "password"
        };

        var handler = new UserLoginCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().Be("alice");
        _identityRepositoryMock.Verify(
            repo => repo.LoginAsync("alice", "password", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserRefreshToken_ReturnsAccessToken()
    {
        var command = new UserRefreshTokenCommand
        {
            AccessToken = "access",
            RefreshToken = "refresh"
        };

        var handler = new UserRefreshTokenCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().Be("access");
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
            repo => repo.LogoutAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserCreate_ReturnsUserName()
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

        var handler = new UserCreateCommandHandler(_identityRepositoryMock.Object);

        var result = await handler.HandleAsync(command);

        result.Should().Be("bob");
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
