using System.Reflection;
using TechnicalTest.Services.Models;
using TechnicalTest.Services.Services;

namespace TechnicalTest.Services.Tests;

/// <summary>
/// Resets the static _products list before and after every test
/// so tests are fully isolated from each other.
/// </summary>
public class ProductsServiceTests : IDisposable
{
    private readonly ProductsService _sut;

    public ProductsServiceTests()
    {
        ResetStaticProducts();
        _sut = new ProductsService();
    }

    public void Dispose() => ResetStaticProducts();

    private static void ResetStaticProducts()
    {
        var field = typeof(ProductsService)
            .GetField("_products", BindingFlags.NonPublic | BindingFlags.Static)!;
        field.SetValue(null, new List<ProductDto>());
    }

    // ── create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_ReturnsTheSameProduct()
    {
        var product = new ProductDto { Name = "Apple", Colour = "Red" };

        var result = _sut.create(product);

        Assert.Same(product, result);
    }

    [Fact]
    public void Create_AddsProductToInternalList()
    {
        var product = new ProductDto { Name = "Apple", Colour = "Red" };

        _sut.create(product);
        var stored = _sut.getProductsByColour("Red");

        Assert.Single(stored);
        Assert.Same(product, stored[0]);
    }

    [Fact]
    public void Create_MultipleProducts_AllAreStored()
    {
        var p1 = new ProductDto { Name = "Apple", Colour = "Red" };
        var p2 = new ProductDto { Name = "Banana", Colour = "Yellow" };

        _sut.create(p1);
        _sut.create(p2);

        Assert.Single(_sut.getProductsByColour("Red"));
        Assert.Single(_sut.getProductsByColour("Yellow"));
    }

    // ── getProductsByColour ───────────────────────────────────────────────────

    [Fact]
    public void GetProductsByColour_ExactMatch_ReturnsMatchingProducts()
    {
        _sut.create(new ProductDto { Name = "Apple", Colour = "Red" });
        _sut.create(new ProductDto { Name = "Tomato", Colour = "Red" });
        _sut.create(new ProductDto { Name = "Banana", Colour = "Yellow" });

        var result = _sut.getProductsByColour("Red");

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Equal("Red", p.Colour));
    }

    [Fact]
    public void GetProductsByColour_CaseInsensitive_ReturnsMatchingProducts()
    {
        _sut.create(new ProductDto { Name = "Apple", Colour = "Red" });

        var resultLower = _sut.getProductsByColour("red");
        var resultUpper = _sut.getProductsByColour("RED");
        var resultMixed = _sut.getProductsByColour("rEd");

        Assert.Single(resultLower);
        Assert.Single(resultUpper);
        Assert.Single(resultMixed);
    }

    [Fact]
    public void GetProductsByColour_NoMatch_ReturnsEmptyList()
    {
        _sut.create(new ProductDto { Name = "Apple", Colour = "Red" });

        var result = _sut.getProductsByColour("Blue");

        Assert.Empty(result);
    }

    [Fact]
    public void GetProductsByColour_EmptyStore_ReturnsEmptyList()
    {
        var result = _sut.getProductsByColour("Red");

        Assert.Empty(result);
    }
}
