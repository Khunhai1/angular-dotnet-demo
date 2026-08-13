using ChatApp.Api.Controllers;
using ChatApp.Api.Dtos.Auth;
using ChatApp.Api.Models;
using ChatApp.Api.Services;
using ChatApp.Api.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatApp.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock = MockUserManagerFactory.GetUserManager();
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    private AuthController CreateController() => new(_userManagerMock.Object, _tokenServiceMock.Object);

    private static readonly (string Token, DateTime ExpiresAt) FakeToken =
        ("fake-jwt-token", DateTime.UtcNow.AddHours(1));

    [Fact]
    public async Task Register_ValidInput_ReturnsOkWithAuthResponse()
    {
        var dto = new RegisterRequestDto { DisplayName = "Alice", Email = "alice@example.com", Password = "Sup3r$ecret" };

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock
            .Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>());
        _tokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()))
            .Returns(FakeToken);

        var controller = CreateController();

        var result = await controller.Register(dto);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        response.Token.Should().Be(FakeToken.Token);
        response.ExpiresAt.Should().Be(FakeToken.ExpiresAt);
        response.Email.Should().Be(dto.Email);
        response.DisplayName.Should().Be(dto.DisplayName);

        _tokenServiceMock.Verify(t => t.GenerateToken(
            It.Is<User>(u => u.Email == dto.Email && u.DisplayName == dto.DisplayName),
            It.Is<IList<string>>(roles => roles.Count == 0)), Times.Once);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequestWithIdentityErrors()
    {
        var dto = new RegisterRequestDto { DisplayName = "Alice", Email = "alice@example.com", Password = "Sup3r$ecret" };
        var identityError = new IdentityError { Code = "DuplicateEmail", Description = "Email 'alice@example.com' is already taken." };

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        var controller = CreateController();

        var result = await controller.Register(dto);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errors = badRequest.Value.Should().BeAssignableTo<IEnumerable<IdentityError>>().Subject;
        errors.Should().ContainSingle(e => e.Code == "DuplicateEmail");

        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()), Times.Never);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithAuthResponse()
    {
        var dto = new LoginRequestDto { Email = "alice@example.com", Password = "Sup3r$ecret" };
        var user = new User { Id = Guid.NewGuid(), Email = dto.Email, DisplayName = "Alice" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());
        _tokenServiceMock
            .Setup(t => t.GenerateToken(user, It.IsAny<IList<string>>()))
            .Returns(FakeToken);

        var controller = CreateController();

        var result = await controller.Login(dto);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        response.Token.Should().Be(FakeToken.Token);
        response.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsUnauthorizedWithoutCheckingPassword()
    {
        var dto = new LoginRequestDto { Email = "ghost@example.com", Password = "whatever" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

        var controller = CreateController();

        var result = await controller.Login(dto);

        var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorized.Value.Should().Be("Invalid credentials.");

        _userManagerMock.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorizedWithSameMessageAsUnknownEmail()
    {
        var dto = new LoginRequestDto { Email = "alice@example.com", Password = "wrong-password" };
        var user = new User { Id = Guid.NewGuid(), Email = dto.Email, DisplayName = "Alice" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

        var controller = CreateController();

        var result = await controller.Login(dto);

        // Same message as the unknown-email case above: proves the API doesn't leak
        // whether the email exists (no user-enumeration).
        var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorized.Value.Should().Be("Invalid credentials.");
    }
}
