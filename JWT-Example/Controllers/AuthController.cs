using System.Security.Claims;
using JWT_Example.DTOs.Auth;
using JWT_Example.Exceptions;
using JWT_Example.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Example.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest requestBody)
    {
        var signUpResult = await authService.SignUpAsync(requestBody);
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(2),
            Secure = true
        };
        
        Response.Cookies.Append("refreshToken", signUpResult.RefreshToken, cookieOptions);
        
        return Ok(new SignUpResponse(signUpResult.AccessToken));
    }
    
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest requestBody)
    {
        var signInResult = await authService.SignInAsync(requestBody);
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(2),
            Secure = true
        };
        
        Response.Cookies.Append("refreshToken", signInResult.RefreshToken, cookieOptions);
        
        return Ok(new SignInResponse(signInResult.AccessToken));
    }
    
    [Authorize, HttpPost("sign-out")]
    public new async Task<IActionResult> SignOut()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            await authService.SignOutAsync(Convert.ToInt32(userId));
        }
        
        Response.Cookies.Delete("refreshToken");

        return NoContent();
    }

    [HttpPost("refresh-session")]
    public async Task<IActionResult> RefreshSession()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized();
        }
        
        var refreshSessionResult = await authService.RefreshSession(refreshToken);
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(2),
            Secure = true
        };
        
        Response.Cookies.Append("refreshToken", refreshSessionResult.RefreshToken, cookieOptions);
        
        return Ok(new RefreshSessionResponse(refreshSessionResult.AccessToken));
    }
}