namespace IPL_Franchises.Application.DTOs.Products;

public class ProductSearchRequest
{
    public string? Search { get; set; }

    public string? Franchise { get; set; }

    public string? Type { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 12;
}
