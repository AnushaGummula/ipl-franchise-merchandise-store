using IPL_Franchises.Application.DTOs.Products;
using IPL_Franchises.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPL_Franchises.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] ProductSearchRequest request)
    {
        var result =
            await _productService.GetProductsAsync(request);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product =
            await _productService.GetProductByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }
}
