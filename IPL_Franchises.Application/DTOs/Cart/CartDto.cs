namespace IPL_Franchises.Application.DTOs.Cart;

public class CartDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public List<CartItemDto> Items { get; set; } = [];

    public decimal TotalAmount =>
        Items.Sum(x => x.SubTotal);

    public int TotalItems =>
        Items.Sum(x => x.Quantity);
}
