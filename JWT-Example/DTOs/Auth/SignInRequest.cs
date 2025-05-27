using System.ComponentModel.DataAnnotations;

namespace JWT_Example.DTOs.Auth;

public record SignInRequest(
    [Required, Length(5, 30)] string Username, 
    [Required, RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Minimum eight characters, at least one letter and one number")] string Password
);