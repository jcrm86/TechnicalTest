using Moq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechnicalTest.Services.Models;

namespace TechnicalTest.IntegrationTests;
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenAsync()
    {
        var response = await _client.PostAsJsonAsync("/auth/login",
            new { username = "admin", password = "password" });
        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result!.Token;
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var response = await _client.PostAsJsonAsync("/auth/login",
            new { username = "admin", password = "password" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.False(string.IsNullOrEmpty(result!.Token));
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/auth/login",
            new { username = "wrong", password = "wrong" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient(); // fresh client with no auth header
        var response = await client.GetAsync("/products/getProducts/red");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithToken_Returns200()
    {
        _factory.ProductServiceMock
            .Setup(s => s.getProductsByColour("red"))
            .Returns([new ProductDto { Name = "Apple", Colour = "red" }]);

        var token = await GetTokenAsync();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/products/getProducts/red");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/products/create",
            new ProductDto { Name = "Tomato", Colour = "red" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithToken_Returns200()
    {
        var product = new ProductDto { Name = "Tomato", Colour = "red" };
        _factory.ProductServiceMock
            .Setup(s => s.create(It.IsAny<ProductDto>()))
            .Returns(product);

        var token = await GetTokenAsync();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/products/create", product);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

record TokenResponse(string Token);
