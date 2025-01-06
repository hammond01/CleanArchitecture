﻿namespace ProductManager.Persistence.Extensions;

public class IdentityExtension
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IdentityConfig _identityConfig;
    private readonly IRepository<RefreshToken, string> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    public IdentityExtension(IDateTimeProvider dateTimeProvider, IOptions<IdentityConfig> identityConfig,
        IRepository<RefreshToken, string> refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _dateTimeProvider = dateTimeProvider;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _identityConfig = identityConfig.Value;
    }
    public RefreshToken GenerateRefreshToken() => new RefreshToken
    {
        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)), Expires = _dateTimeProvider.UtcNow.AddDays(7)
    };

    public async Task SaveRefreshTokenAsync(Guid userId, RefreshToken refreshToken)
    {
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
            ValidIssuer = _identityConfig.ISSUER,
            ValidAudience = _identityConfig.AUDIENCE,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_identityConfig.SECRET))
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
        var secretKey = _identityConfig.SECRET;
        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        return new JwtSecurityToken(
        _identityConfig.ISSUER,
        _identityConfig.AUDIENCE,
        expires: _dateTimeProvider.UtcNow.AddMinutes(5),
        claims: claims,
        signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256));
    }
}
