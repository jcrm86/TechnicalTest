using TechnicalTest.Services.Models;

namespace TechnicalTest.Services.Services.Interfaces
{
    public interface IProductService
    {
        List<ProductDto> getProductsByColour(string colour);

        ProductDto create(ProductDto product);
    }
}
