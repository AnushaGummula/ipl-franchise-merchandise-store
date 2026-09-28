namespace IPL_Franchises.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string ProductType { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? ImageUrl { get; set; }

    public int FranchiseId { get; set; }

    public string FranchiseName { get; set; } = string.Empty;

    public string FranchiseCode { get; set; } = string.Empty;
}
