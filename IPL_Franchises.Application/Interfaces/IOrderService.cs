using IPL_Franchises.Application.DTOs.Orders;

namespace IPL_Franchises.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CheckoutAsync(string userId);

    Task<List<OrderDto>> GetOrdersAsync(string userId);

    Task<OrderDto?> GetOrderByIdAsync(
        string userId,
        int orderId);
}
