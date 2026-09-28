using System.Security.Claims;
using IPL_Franchises.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPL_Franchises.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(
        IOrderService orderService)
    {
        _orderService = orderService;
    }


    private string? GetUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }


    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var order =
            await _orderService
                .CheckoutAsync(userId);

        return Ok(order);
    }


    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var orders =
            await _orderService
                .GetOrdersAsync(userId);

        return Ok(orders);
    }


    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrder(
        int orderId)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var order =
            await _orderService
                .GetOrderByIdAsync(
                    userId,
                    orderId);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }
}
