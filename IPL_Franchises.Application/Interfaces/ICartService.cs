using IPL_Franchises.Application.DTOs.Cart;

namespace IPL_Franchises.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(
        string userId);

    Task<CartDto> AddItemAsync(
        string userId,
        AddToCartRequest request);

    Task<CartDto> UpdateQuantityAsync(
        string userId,
        int cartItemId,
        int quantity);

    Task RemoveItemAsync(
        string userId,
        int cartItemId);
}
