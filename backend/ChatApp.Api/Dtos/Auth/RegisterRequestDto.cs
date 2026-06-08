using System.ComponentModel.DataAnnotations;

namespace ChatApp.Api.Dtos.Auth;

public class RegisterRequestDto
{
    [Required] [MaxLength(50)] public string DisplayName { get; init; } = string.Empty;

    [Required] [EmailAddress] public string Email { get; init; } = string.Empty;

    [Required] [MinLength(8)] public string Password { get; init; } = string.Empty;
}