using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using TechnicalTest.Controllers;

namespace TechnicalTest.Tests;

public class AuthControllerTests
{
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]              = "unit-test-secret-key-at-least-32-chars!!",
                ["Jwt:Issuer"]           = "TechnicalTest",
                ["Jwt:Audience"]         = "TechnicalTestUsers",
                ["Jwt:ExpiresInMinutes"] = "60"
            })
            .Build();

        _sut = new AuthController(config);
    }

    [Fact]
    public void Login_ValidCredentials_ReturnsOk()
    {
        var result = _sut.Login(new LoginRequest("admin", "password"));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Login_ValidCredentials_ResponseContainsToken()
    {
        var result = (OkObjectResult)_sut.Login(new LoginRequest("admin", "password"));

        var json = JsonSerializer.Serialize(result.Value);
        var doc  = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("token", out var tokenProp));
        Assert.False(string.IsNullOrEmpty(tokenProp.GetString()));
    }

    [Fact]
    public void Login_WrongUsername_ReturnsUnauthorized()
    {
        var result = _sut.Login(new LoginRequest("hacker", "password"));

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void Login_WrongPassword_ReturnsUnauthorized()
    {
        var result = _sut.Login(new LoginRequest("admin", "wrongpassword"));

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void Login_BothWrong_ReturnsUnauthorized()
    {
        var result = _sut.Login(new LoginRequest("unknown", "unknown"));

        Assert.IsType<UnauthorizedResult>(result);
    }
}
