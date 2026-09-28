using IPL_Franchises.Application.DTOs;
using IPL_Franchises.Application.DTOs.Products;

namespace IPL_Franchises.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(
        ProductSearchRequest request);

    Task<ProductDto?> GetProductByIdAsync(int id);
}
