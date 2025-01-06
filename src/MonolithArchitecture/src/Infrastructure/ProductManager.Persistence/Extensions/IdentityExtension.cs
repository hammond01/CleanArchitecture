namespace ProductManager.Persistence.Extensions;

public class IdentityExtension
{
    private readonly ApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IdentityConfig _identityConfig;
    public IdentityExtension(IDateTimeProvider dateTimeProvider, ApplicationDbContext context,
        IOptions<IdentityConfig> identityConfig)
    {
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _identityConfig = identityConfig.Value;
    }
    public RefreshToken GenerateRefreshToken() => new RefreshToken
    {
        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        Expires = _dateTimeProvider.UtcNow.AddDays(7),
        Created = _dateTimeProvider.UtcNow
    };

    public async Task SaveRefreshTokenAsync(Guid userId, RefreshToken refreshToken)
    {
        refreshToken.UserId = userId;
        // await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
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
    public static async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        // var storedToken = await _context.RefreshTokens
        //     .Where(t => t.UserId == userId && t.Token == refreshToken)
        //     .FirstOrDefaultAsync();
        // return storedToken is { IsExpired: false, Revoked: null };
        await Task.Delay(100);
        return true;
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
