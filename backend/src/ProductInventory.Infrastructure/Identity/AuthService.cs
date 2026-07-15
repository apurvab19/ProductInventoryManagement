using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductInventory.Application.Common.Models;
using ProductInventory.Application.DTOs.Auth;
using ProductInventory.Application.Interfaces;
using ProductInventory.Domain.Entities;
using ProductInventory.Infrastructure.Data;

namespace ProductInventory.Infrastructure.Identity;

public sealed class AuthService(
    ApplicationDbContext context,
    IPasswordHasher<AppUser> passwordHasher,
    IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<AuthResponse?> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();

        var exists = await context.Users.AnyAsync(
            user => user.Username == username,
            cancellationToken);

        if (exists)
        {
            return null;
        }

        var user = new AppUser
        {
            Username = username,
            Role = "User"
        };

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            request.Password);

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await CreateTokenResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();

        var user = await context.Users.FirstOrDefaultAsync(
            existingUser => existingUser.Username == username,
            cancellationToken);

        if (user is null)
        {
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return await CreateTokenResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse?> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(request.RefreshToken);

        var storedToken = await context.RefreshTokens
            .Include(token => token.User)
            .FirstOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            return null;
        }

        storedToken.RevokedOnUtc = DateTime.UtcNow;

        return await CreateTokenResponseAsync(
            storedToken.User,
            cancellationToken);
    }

    private async Task<AuthResponse> CreateTokenResponseAsync(
        AppUser user,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var accessTokenExpiry = now.AddMinutes(
            _jwtSettings.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: now,
            expires: accessTokenExpiry,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(jwt);

        var rawRefreshToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        var refreshToken = new RefreshToken
        {
            TokenHash = HashToken(rawRefreshToken),
            CreatedOnUtc = now,
            ExpiresOnUtc = now.AddDays(_jwtSettings.RefreshTokenDays),
            UserId = user.Id
        };

        await context.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            AccessTokenExpiresOnUtc = accessTokenExpiry,
            Username = user.Username,
            Role = user.Role
        };
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}
