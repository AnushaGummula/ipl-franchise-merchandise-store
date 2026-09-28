using System.Security.Claims;
using IPL_Franchises.Application.DTOs.Cart;
using IPL_Franchises.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPL_Franchises.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(
        ICartService cartService)
    {
        _cartService = cartService;
    }


    private string? GetUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }


    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var cart =
            await _cartService
                .GetCartAsync(userId);

        return Ok(cart);
    }


    [HttpPost("items")]
    public async Task<IActionResult> AddItem(
        AddToCartRequest request)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var cart =
            await _cartService
                .AddItemAsync(
                    userId,
                    request);

        return Ok(cart);
    }



    [HttpPut("items/{cartItemId:int}")]
    public async Task<IActionResult> UpdateQuantity(
    int cartItemId,
    [FromQuery] int quantity)
    {
        var userId =
            GetUserId();

        if (
            string.IsNullOrWhiteSpace(
                userId
            )
        )
        {
            return Unauthorized();
        }

        var cart =
            await _cartService
                .UpdateQuantityAsync(
                    userId,
                    cartItemId,
                    quantity);

        return Ok(cart);
    }


    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(
    int cartItemId)
    {
        var userId =
            GetUserId();

        if (
            string.IsNullOrWhiteSpace(
                userId
            )
        )
        {
            return Unauthorized();
        }

        await _cartService
            .RemoveItemAsync(
                userId,
                cartItemId);

        return NoContent();
    }
}
