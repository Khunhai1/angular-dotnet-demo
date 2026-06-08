using ChatApp.Api.Models;

namespace ChatApp.Api.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user, IList<string> roles);
}