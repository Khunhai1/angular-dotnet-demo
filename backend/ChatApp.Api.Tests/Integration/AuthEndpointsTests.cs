using System.Net;
using System.Net.Http.Json;
using ChatApp.Api.Dtos.Auth;
using FluentAssertions;

namespace ChatApp.Api.Tests.Integration;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // Unique email per call so tests never collide against the shared in-memory DB.
    private static RegisterRequestDto ValidRegisterDto() => new()
    {
        DisplayName = "Alice",
        Email = $"{Guid.NewGuid():N}@example.com",
        Password = "Sup3r$ecret1"
    };

    [Fact]
    public async Task Register_ValidInput_ReturnsOk()
    {
        var dto = ValidRegisterDto();

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.Email.Should().Be(dto.Email);
    }

    [Fact]
    public async Task Register_MalformedEmail_ReturnsBadRequest()
    {
        // This is the case a controller-level unit test structurally cannot prove:
        // [EmailAddress] is only enforced by the real model-binding pipeline.
        var dto = new RegisterRequestDto { DisplayName = "Alice", Email = "not-an-email", Password = "Sup3r$ecret1" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_DuplicateEmail_SecondCallReturnsBadRequest()
    {
        var dto = ValidRegisterDto();

        var first = await _client.PostAsJsonAsync("/api/auth/register", dto);
        var second = await _client.PostAsJsonAsync("/api/auth/register", dto);

        first.StatusCode.Should().Be(HttpStatusCode.OK);
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterThenLogin_ValidCredentials_ReturnsOkWithToken()
    {
        var registerDto = ValidRegisterDto();
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginRequestDto { Email = registerDto.Email, Password = registerDto.Password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        body!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_DifferentEmailCasing_StillSucceeds()
    {
        // Identity normalizes email internally, so lookup should be case-insensitive.
        var registerDto = ValidRegisterDto();
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginRequestDto { Email = registerDto.Email.ToUpperInvariant(), Password = registerDto.Password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
