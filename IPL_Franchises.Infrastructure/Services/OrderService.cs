using IPL_Franchises.Application.DTOs.Orders;
using IPL_Franchises.Application.Exceptions;
using IPL_Franchises.Application.Interfaces;
using IPL_Franchises.Domain.Entities;
using IPL_Franchises.Domain.Enums;
using IPL_Franchises.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IPL_Franchises.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IPLDbContext _context;

    public OrderService(IPLDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto> CheckoutAsync(string userId)
    {
        var cart =
    await _context.Carts
        .Include(c => c.Items)
            .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Franchise)
        .FirstOrDefaultAsync(
            c => c.UserId == userId);

        if (cart == null || cart.Items.Count == 0)
        {
            throw new BusinessException(
                "Your cart is empty.");
        }

        // Stock must be checked again at checkout.
        foreach (var item in cart.Items)
        {
            if (!item.Product.IsActive)
            {
                throw new BusinessException(
                    $"{item.Product.Name} is no longer available.");
            }

            if (item.Quantity > item.Product.StockQuantity)
            {
                throw new BusinessException(
                    $"Only {item.Product.StockQuantity} units of " +
                    $"{item.Product.Name} are currently available.");
            }
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                UserId = userId,

                // Human-readable reference without exposing only DB ID.
                OrderNumber =
                    $"IPL-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",

                Status = OrderStatus.Placed,

                CreatedAt = DateTime.UtcNow
            };

            foreach (var cartItem in cart.Items)
            {
                order.Items.Add(
    new OrderItem
    {
        ProductId =
            cartItem.ProductId,

        ProductName =
            cartItem.Product.Name,

        FranchiseCode =
            cartItem.Product.Franchise.Code,

        ProductType =
            cartItem.Product.ProductType.ToString(),

        SelectedSize =
            cartItem.SelectedSize,

        UnitPrice =
            cartItem.Product.Price,

        Quantity =
            cartItem.Quantity
    });
                // Inventory changes only at checkout.
                cartItem.Product.StockQuantity -=
                    cartItem.Quantity;
            }

            order.TotalAmount = order.Items.Sum(
                x => x.UnitPrice * x.Quantity);

            _context.Orders.Add(order);

            // Checkout consumes the current cart.
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return MapOrder(order);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<OrderDto>> GetOrdersAsync(
        string userId)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders
            .Select(MapOrder)
            .ToList();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(
        string userId,
        int orderId)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o =>
                o.Id == orderId &&
                o.UserId == userId);

        return order == null
            ? null
            : MapOrder(order);
    }

    private static OrderDto MapOrder(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,

            Items = order.Items
    .Select(item =>
        new OrderItemDto
        {
            ProductId =
                item.ProductId,

            ProductName =
                item.ProductName,

            FranchiseCode =
                item.FranchiseCode,

            ProductType =
                item.ProductType,

            SelectedSize =
                item.SelectedSize,

            UnitPrice =
                item.UnitPrice,

            Quantity =
                item.Quantity
        })
    .ToList()
        };
    }
}
