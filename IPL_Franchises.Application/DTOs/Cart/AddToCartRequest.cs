using System.ComponentModel.DataAnnotations;

namespace IPL_Franchises.Application.DTOs.Cart;

public class AddToCartRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 50)]
    public int Quantity { get; set; }

    public string? SelectedSize { get; set; }
}
