using System.IdentityModel.Tokens.Jwt;
using JWT_Example.Data;
using JWT_Example.DTOs.Auth;
using JWT_Example.Entities;
using JWT_Example.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JWT_Example.Services;

public class AuthService(DatabaseContext data, ITokenService tokenService) : IAuthService
{

    public async Task<TokenResult> SignUpAsync(SignUpRequest requestBody)
    {
        var hashedPassword = new PasswordHasher<User>().HashPassword(new User(), requestBody.Password);
        
        var user = new User
        {
            Username = requestBody.Username,
            PasswordHash = hashedPassword,
            RoleId = 1
        };
        
        var accessToken = await tokenService.CreateAccessTokenAsync(user);
        var refreshToken = tokenService.CreateRefreshToken();

        user.RefreshToken = new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(2),
        };
        
        await data.AddAsync(user);
        await data.SaveChangesAsync();
        
        return new TokenResult(accessToken, refreshToken);
    }

    public async Task<TokenResult> SignInAsync(SignInRequest requestBody)
    {
        var user = await data.Users.SingleOrDefaultAsync(e => e.Username == requestBody.Username);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid username or password.");
        }
        
        var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(
                new User(), 
                user.PasswordHash, 
                requestBody.Password
            );

        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
            throw new UnauthorizedException("Invalid username or password.");
        }
        
        var accessToken = await tokenService.CreateAccessTokenAsync(user);
        var refreshToken = tokenService.CreateRefreshToken();

        var userRefreshToken = await data.RefreshTokens.SingleOrDefaultAsync(e => e.UserId == user.Id);

        if (userRefreshToken == null)
        {
            user.RefreshToken = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
            };
        }
        else
        {
            userRefreshToken.Token = refreshToken;
            userRefreshToken.ExpiresAt = DateTime.UtcNow.AddHours(2);
        }
        
        await data.SaveChangesAsync();
        
        return new TokenResult(accessToken, refreshToken);
    }

    public async Task<TokenResult> RefreshSession(string refreshToken)
    {
        var user = await data.Users
            .Include(e => e.RefreshToken)
            .FirstOrDefaultAsync(e => 
                e.RefreshToken != null 
                && e.RefreshToken.Token == refreshToken 
                && e.RefreshToken.ExpiresAt > DateTime.UtcNow
            );
        
        if (user == null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }
        
        var accessToken = await tokenService.CreateAccessTokenAsync(user);
        var newRefreshToken = tokenService.CreateRefreshToken();
        
        user.RefreshToken!.Token = newRefreshToken;
        user.RefreshToken.ExpiresAt = DateTime.UtcNow.AddHours(2);
        
        await data.SaveChangesAsync();
        
        return new TokenResult(accessToken, newRefreshToken);
    }

    public async Task SignOutAsync(int userId)
    {
        await data.RefreshTokens.Where(e => e.UserId == userId).ExecuteDeleteAsync();
    }
}