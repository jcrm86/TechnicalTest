using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TechnicalTest.Controllers;

namespace TechnicalTest.Tests;

public class HealthCheckControllerTests
{
    private readonly HealthCheckController _sut = new();

    [Fact]
    public void Get_ReturnsOk()
    {
        var result = _sut.Get();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Get_ResponseContainsHealthyStatus()
    {
        var result = (OkObjectResult)_sut.Get();

        var json = JsonSerializer.Serialize(result.Value);
        var doc  = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("status", out var statusProp));
        Assert.Equal("Healthy", statusProp.GetString());
    }

    [Fact]
    public void Get_ResponseContainsUtcTime()
    {
        var before = DateTime.UtcNow;

        var result = (OkObjectResult)_sut.Get();

        var json = JsonSerializer.Serialize(result.Value);
        var doc  = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("utcTime", out var timeProp));
        var utcTime = timeProp.GetDateTime();
        Assert.True(utcTime >= before && utcTime <= DateTime.UtcNow);
    }
}
