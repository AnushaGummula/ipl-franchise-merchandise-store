using IPL_Franchises.Application.DTOs.Cart;
using IPL_Franchises.Application.Exceptions;
using IPL_Franchises.Application.Interfaces;
using IPL_Franchises.Domain.Entities;
using IPL_Franchises.Domain.Enums;
using IPL_Franchises.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IPL_Franchises.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly IPLDbContext _context;

    public CartService(
        IPLDbContext context)
    {
        _context = context;
    }


    public async Task<CartDto> GetCartAsync(
        string userId)
    {
        var cart =
            await GetCartEntityAsync(
                userId);

        if (cart == null)
        {
            return new CartDto
            {
                UserId = userId
            };
        }

        return MapCart(cart);
    }


    public async Task<CartDto> AddItemAsync(
        string userId,
        AddToCartRequest request)
    {
        if (request.Quantity <= 0)
        {
            throw new BusinessException(
                "Quantity must be greater than zero.");
        }


        var product =
            await _context.Products
                .Include(p =>
                    p.Franchise)
                .FirstOrDefaultAsync(
                    p =>
                        p.Id ==
                            request.ProductId &&
                        p.IsActive);


        if (product == null)
        {
            throw new BusinessException(
                "The selected product is not available.");
        }


        string? selectedSize = null;


        if (
            product.ProductType ==
            ProductType.Jersey
        )
        {
            selectedSize =
                request.SelectedSize?
                    .Trim()
                    .ToUpperInvariant();


            var validSizes =
                new[]
                {
                    "M",
                    "L",
                    "XL"
                };


            if (
                string.IsNullOrWhiteSpace(
                    selectedSize
                ) ||
                !validSizes.Contains(
                    selectedSize
                )
            )
            {
                throw new BusinessException(
                    "Please select a valid jersey size: M, L or XL.");
            }
        }


        var cart =
            await _context.Carts
                .Include(c =>
                    c.Items)
                .FirstOrDefaultAsync(
                    c =>
                        c.UserId ==
                        userId);


        if (cart == null)
        {
            cart =
                new Cart
                {
                    UserId =
                        userId
                };

            _context.Carts
                .Add(cart);
        }


        var existingItem =
            cart.Items
                .FirstOrDefault(
                    item =>
                        item.ProductId ==
                            request.ProductId &&
                        item.SelectedSize ==
                            selectedSize
                );


        var finalQuantity =
            (existingItem?.Quantity ?? 0)
            +
            request.Quantity;


        if (
            finalQuantity >
            product.StockQuantity
        )
        {
            throw new BusinessException(
                $"Only {product.StockQuantity} units of " +
                $"{product.Name} are currently available.");
        }


        if (existingItem == null)
        {
            cart.Items.Add(
                new CartItem
                {
                    ProductId =
                        product.Id,

                    Quantity =
                        request.Quantity,

                    SelectedSize =
                        selectedSize
                });
        }
        else
        {
            existingItem.Quantity =
                finalQuantity;
        }


        cart.UpdatedAt =
            DateTime.UtcNow;


        await _context
            .SaveChangesAsync();


        var updatedCart =
            await GetCartEntityAsync(
                userId);


        return MapCart(
            updatedCart!);
    }


    public async Task<CartDto> UpdateQuantityAsync(
        string userId,
        int cartItemId,
        int quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessException(
                "Quantity must be greater than zero.");
        }


        var cart =
            await _context.Carts
                .Include(c =>
                    c.Items)
                .FirstOrDefaultAsync(
                    c =>
                        c.UserId ==
                        userId);


        if (cart == null)
        {
            throw new BusinessException(
                "Cart not found.");
        }


        var item =
            cart.Items
                .FirstOrDefault(
                    i =>
                        i.Id ==
                        cartItemId);


        if (item == null)
        {
            throw new BusinessException(
                "Cart item was not found.");
        }


        var product =
            await _context.Products
                .FirstOrDefaultAsync(
                    p =>
                        p.Id ==
                            item.ProductId &&
                        p.IsActive);


        if (product == null)
        {
            throw new BusinessException(
                "Product is no longer available.");
        }


        if (
            quantity >
            product.StockQuantity
        )
        {
            throw new BusinessException(
                $"Only {product.StockQuantity} units of " +
                $"{product.Name} are currently available.");
        }


        item.Quantity =
            quantity;

        cart.UpdatedAt =
            DateTime.UtcNow;


        await _context
            .SaveChangesAsync();


        var updatedCart =
            await GetCartEntityAsync(
                userId);


        return MapCart(
            updatedCart!);
    }


    public async Task RemoveItemAsync(
        string userId,
        int cartItemId)
    {
        var cart =
            await _context.Carts
                .Include(c =>
                    c.Items)
                .FirstOrDefaultAsync(
                    c =>
                        c.UserId ==
                        userId);


        if (cart == null)
        {
            return;
        }


        var item =
            cart.Items
                .FirstOrDefault(
                    i =>
                        i.Id ==
                        cartItemId);


        if (item == null)
        {
            return;
        }


        _context.CartItems
            .Remove(item);


        cart.UpdatedAt =
            DateTime.UtcNow;


        await _context
            .SaveChangesAsync();
    }


    private async Task<Cart?>
        GetCartEntityAsync(
            string userId)
    {
        return await _context.Carts

            .AsNoTracking()

            .Include(c =>
                c.Items)

                .ThenInclude(i =>
                    i.Product)

                    .ThenInclude(p =>
                        p.Franchise)

            .FirstOrDefaultAsync(
                c =>
                    c.UserId ==
                    userId);
    }


    private static CartDto MapCart(
        Cart cart)
    {
        return new CartDto
        {
            Id =
                cart.Id,

            UserId =
                cart.UserId,

            Items =
                cart.Items

                    .Select(
                        item =>
                            new CartItemDto
                            {
                                Id =
                                    item.Id,

                                ProductId =
                                    item.ProductId,

                                ProductName =
                                    item.Product.Name,

                                FranchiseCode =
                                    item.Product
                                        .Franchise
                                        .Code,

                                ProductType =
                                    item.Product
                                        .ProductType
                                        .ToString(),

                                SelectedSize =
                                    item.SelectedSize,

                                UnitPrice =
                                    item.Product.Price,

                                Quantity =
                                    item.Quantity,

                                StockQuantity =
                                    item.Product
                                        .StockQuantity
                            })

                    .ToList()
        };
    }
}
