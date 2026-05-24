using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnicalTest.Services.Models;
using TechnicalTest.Services.Services.Interfaces;

namespace TechnicalTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        public readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        // POST: /Products/Create
        [HttpPost("Create")]
        public IActionResult Create([FromBody] ProductDto product)
        {
            // For demonstration, just return the product back.
            var response = _productService.create(product);
            return Ok(response);
        }

        // GET: /Products/getProducts/{colour}
        [HttpGet("getProducts/{colour}")]
        public IActionResult GetProducts(string colour)
        {
            var response = _productService.getProductsByColour(colour);

            return Ok(response);
        }
    }
}
