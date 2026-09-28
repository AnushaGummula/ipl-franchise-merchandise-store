namespace IPL_Franchises.Application.DTOs.Orders;

public class OrderItemDto
{
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

    public decimal SubTotal =>
        UnitPrice * Quantity;
}
