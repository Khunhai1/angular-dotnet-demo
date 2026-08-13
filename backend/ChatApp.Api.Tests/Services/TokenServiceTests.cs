using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ChatApp.Api.Models;
using ChatApp.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ChatApp.Api.Tests.Services;

public class TokenServiceTests
{
    private const string TestKey = "this-is-a-test-signing-key-32chars-min"; // HMAC-SHA256 needs >= 32 chars
    private const string TestIssuer = "TestIssuer";
    private const string TestAudience = "TestAudience";

    private static TokenService CreateService()
    {
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["Jwt:Key"]).Returns(TestKey);
        config.Setup(c => c["Jwt:Issuer"]).Returns(TestIssuer);
        config.Setup(c => c["Jwt:Audience"]).Returns(TestAudience);
        return new TokenService(config.Object);
    }

    private static User CreateUser() => new()
    {
        Id = Guid.NewGuid(),
        Email = "alice@example.com",
        DisplayName = "Alice"
    };

    [Fact]
    public void GenerateToken_ValidUser_ReturnsNonEmptyTokenAndOneHourExpiry()
    {
        var sut = CreateService();
        var user = CreateUser();

        var (token, expiresAt) = sut.GenerateToken(user, new List<string>());

        token.Should().NotBeNullOrWhiteSpace();
        expiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void GenerateToken_ValidUser_IncludesExpectedClaims()
    {
        var sut = CreateService();
        var user = CreateUser();

        var (token, _) = sut.GenerateToken(user, new List<string>());

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Claims.Should().ContainSingle(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
        jwt.Claims.Should().ContainSingle(c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        jwt.Claims.Should().ContainSingle(c => c.Type == ClaimTypes.Name && c.Value == user.DisplayName);
    }

    [Theory]
    [InlineData(new string[] { }, 0)]
    [InlineData(new[] { "User" }, 1)]
    [InlineData(new[] { "User", "Admin" }, 2)]
    public void GenerateToken_RolesGiven_AddsOneRoleClaimPerRole(string[] roles, int expectedCount)
    {
        var sut = CreateService();
        var user = CreateUser();

        var (token, _) = sut.GenerateToken(user, roles);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Claims.Count(c => c.Type == ClaimTypes.Role).Should().Be(expectedCount);
        foreach (var role in roles)
            jwt.Claims.Should().ContainSingle(c => c.Type == ClaimTypes.Role && c.Value == role);
    }

    [Fact]
    public void GenerateToken_MissingSigningKey_Throws()
    {
        var config = new Mock<IConfiguration>(); // no Jwt:Key set up -> returns null
        var sut = new TokenService(config.Object);

        var act = () => sut.GenerateToken(CreateUser(), new List<string>());

        act.Should().Throw<Exception>();
    }
}