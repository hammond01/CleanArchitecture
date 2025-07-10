using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Infrastructure.Configuration;
using ProductManager.Shared.DateTimes;

namespace ProductManager.Persistence.Extensions;

public class IdentityExtension
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly SecuritySettings _securitySettings;
    private readonly IRepository<RefreshToken, string> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IdentityExtension(IDateTimeProvider dateTimeProvider, IOptions<SecuritySettings> securitySettings,
        IRepository<RefreshToken, string> refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _dateTimeProvider = dateTimeProvider;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _securitySettings = securitySettings.Value;
    }
    public RefreshToken GenerateRefreshToken() => new RefreshToken
    {
        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        Expires = _dateTimeProvider.OffsetUtcNow.DateTime.Add(_securitySettings.RefreshTokenExpiration)
    };

    public async Task SaveRefreshTokenAsync(Guid userId, RefreshToken refreshToken)
    {
        refreshToken.Id = UlidExtension.Generate();
        refreshToken.UserId = userId;
        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _securitySettings.JwtIssuer,
            ValidAudience = _securitySettings.JwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySettings.JwtSecret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
    public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetQueryableSet()
            .Where(t => t.UserId == userId && t.Token == refreshToken)
            .FirstOrDefaultAsync();
        return storedToken is { IsExpired: false, Revoked: null };
    }

    public JwtSecurityToken GenerateJwtToken(List<Claim> claims)
    {
        var secretKey = _securitySettings.JwtSecret;
        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        return new JwtSecurityToken(
        _securitySettings.JwtIssuer,
        _securitySettings.JwtAudience,
        expires: _dateTimeProvider.OffsetUtcNow.DateTime.Add(_securitySettings.BearerTokenExpiration),
        claims: claims,
        signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256));
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetQueryableSet()
            .Where(t => t.Token == refreshToken)
            .FirstOrDefaultAsync();
        if (storedToken != null)
        {
            storedToken.Revoked = _dateTimeProvider.OffsetUtcNow.DateTime;
        }
        await _unitOfWork.SaveChangesAsync();
    }
}
