using System.ComponentModel.DataAnnotations;

namespace ChatApp.Api.Dtos.Auth;

public class LoginRequestDto
{
    [Required] [EmailAddress] public string Email { get; init; } = string.Empty;

    [Required] public string Password { get; init; } = string.Empty;
}