namespace IPL_Franchises.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public string FranchiseCode { get; set; }
        = string.Empty;

    public string ProductType { get; set; }
        = string.Empty;

    public string? SelectedSize { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}
