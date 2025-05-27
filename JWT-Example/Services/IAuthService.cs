using JWT_Example.DTOs;
using JWT_Example.DTOs.Auth;

namespace JWT_Example.Services;

public interface IAuthService
{
    public Task<TokenResult> SignUpAsync(SignUpRequest requestBody);
    public Task<TokenResult> SignInAsync(SignInRequest requestBody);
    public Task<TokenResult> RefreshSession(string refreshToken);
    public Task SignOutAsync(int userId);
}