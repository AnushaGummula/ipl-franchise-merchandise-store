using IPL_Franchises.Application.DTOs;
using IPL_Franchises.Application.DTOs.Products;
using IPL_Franchises.Application.Interfaces;
using IPL_Franchises.Domain.Enums;
using IPL_Franchises.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IPL_Franchises.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IPLDbContext _context;

    public ProductService(IPLDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        ProductSearchRequest request)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .AsQueryable();

        // General text search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search =
                request.Search
                    .Trim()
                    .ToLower();

            query = query.Where(p =>
                p.Name
                    .ToLower()
                    .Contains(search) ||

                (
                    p.Description != null &&
                    p.Description
                        .ToLower()
                        .Contains(search)
                ));
        }

        // Franchise filter - CSK, MI, RCB etc.
        if (!string.IsNullOrWhiteSpace(request.Franchise))
        {
            var franchise = request.Franchise
                .Trim()
                .ToUpper();

            query = query.Where(p =>
                p.Franchise.Code == franchise);
        }

        // Product type filter
        if (!string.IsNullOrWhiteSpace(request.Type) &&
            Enum.TryParse<ProductType>(
                request.Type,
                true,
                out var productType))
        {
            query = query.Where(p =>
                p.ProductType == productType);
        }

        var totalCount = await query.CountAsync();

        var page = request.Page < 1 ? 1 : request.Page;

        var pageSize = request.PageSize switch
        {
            < 1 => 12,
            > 50 => 50,
            _ => request.PageSize
        };

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ProductType = p.ProductType.ToString(),
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                FranchiseId = p.FranchiseId,
                FranchiseName = p.Franchise.Name,
                FranchiseCode = p.Franchise.Code
            })
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            Items = products,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ProductType = p.ProductType.ToString(),
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                FranchiseId = p.FranchiseId,
                FranchiseName = p.Franchise.Name,
                FranchiseCode = p.Franchise.Code
            })
            .FirstOrDefaultAsync();
    }
}
