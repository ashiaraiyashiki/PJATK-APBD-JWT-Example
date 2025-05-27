using JWT_Example.Entities;

namespace JWT_Example.Services;

public interface ITokenService
{
    public Task<string> CreateAccessTokenAsync(User user);
    public string CreateRefreshToken();
}