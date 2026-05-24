using Microsoft.AspNetCore.Mvc;
using Moq;
using TechnicalTest.Controllers;
using TechnicalTest.Services.Models;
using TechnicalTest.Services.Services.Interfaces;

namespace TechnicalTest.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _sut;

    public ProductsControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _sut = new ProductsController(_serviceMock.Object);
    }

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_ReturnsOk()
    {
        var product = new ProductDto { Name = "Apple", Colour = "Red" };
        _serviceMock.Setup(s => s.create(product)).Returns(product);

        var result = _sut.Create(product);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Create_ReturnsProductFromService()
    {
        var product  = new ProductDto { Name = "Apple", Colour = "Red" };
        var returned = new ProductDto { Name = "Apple", Colour = "Red" };
        _serviceMock.Setup(s => s.create(product)).Returns(returned);

        var result = (OkObjectResult)_sut.Create(product);

        Assert.Same(returned, result.Value);
    }

    [Fact]
    public void Create_CallsServiceOnce()
    {
        var product = new ProductDto { Name = "Apple", Colour = "Red" };
        _serviceMock.Setup(s => s.create(product)).Returns(product);

        _sut.Create(product);

        _serviceMock.Verify(s => s.create(product), Times.Once);
    }

    // ── GetProducts ───────────────────────────────────────────────────────────

    [Fact]
    public void GetProducts_ReturnsOk()
    {
        _serviceMock.Setup(s => s.getProductsByColour("Red"))
                    .Returns([new ProductDto { Name = "Apple", Colour = "Red" }]);

        var result = _sut.GetProducts("Red");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetProducts_ReturnsListFromService()
    {
        var expected = new List<ProductDto>
        {
            new() { Name = "Apple",  Colour = "Red" },
            new() { Name = "Tomato", Colour = "Red" }
        };
        _serviceMock.Setup(s => s.getProductsByColour("Red")).Returns(expected);

        var result = (OkObjectResult)_sut.GetProducts("Red");

        Assert.Same(expected, result.Value);
    }

    [Fact]
    public void GetProducts_CallsServiceOnceWithCorrectColour()
    {
        _serviceMock.Setup(s => s.getProductsByColour("Blue")).Returns([]);

        _sut.GetProducts("Blue");

        _serviceMock.Verify(s => s.getProductsByColour("Blue"), Times.Once);
    }

    [Fact]
    public void GetProducts_NoMatch_ReturnsOkWithEmptyList()
    {
        _serviceMock.Setup(s => s.getProductsByColour("Purple")).Returns([]);

        var result = (OkObjectResult)_sut.GetProducts("Purple");

        var list = Assert.IsType<List<ProductDto>>(result.Value);
        Assert.Empty(list);
    }
}
