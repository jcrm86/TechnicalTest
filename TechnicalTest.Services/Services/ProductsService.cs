using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalTest.Services.Models;
using TechnicalTest.Services.Services.Interfaces;

namespace TechnicalTest.Services.Services
{
    public class ProductsService : IProductService
    {
        private static List<ProductDto> _products = new List<ProductDto>();
        public List<ProductDto> getProductsByColour(string colour)
        {
            if(colour.Equals("undefined"))
                return _products;

            return _products.Where(p => p.Colour.Equals(colour, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public ProductDto create(ProductDto product)
        { 
            _products.Add(product);
            return product;
        }
    }
}
