using IPL_Franchises.Application.Exceptions;
using IPL_Franchises.Domain.Entities;
using IPL_Franchises.Domain.Enums;
using IPL_Franchises.Infrastructure.Data;
using IPL_Franchises.Infrastructure.Services;
using IPL_Franchises.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace IPL_Franchises.Tests.Services;

public class OrderServiceTests
{
    /*
     * =========================================================
     * TEST 1
     * Successful checkout should:
     *
     * - create order
     * - snapshot order item data
     * - calculate total
     * - reduce stock
     * - clear cart
     * =========================================================
     */

    [Fact]
    public async Task CheckoutAsync_WhenCartIsValid_ShouldCreateOrderReduceStockAndClearCart()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();

        const string userId =
            "user-1";


        var franchise =
            new Franchise
            {
                Name =
                    "Chennai Super Kings",

                Code =
                    "CSK"
            };


        var product =
            new Product
            {
                Name =
                    "CSK Official Fan Jersey",

                Description =
                    "Official fan jersey",

                ProductType =
                    ProductType.Jersey,

                Price =
                    1499m,

                StockQuantity =
                    10,

                IsActive =
                    true,

                Franchise =
                    franchise
            };


        context.Products.Add(
            product);

        await context.SaveChangesAsync();


        var cart =
            new Cart
            {
                UserId =
                    userId
            };


        cart.Items.Add(
            new CartItem
            {
                ProductId =
                    product.Id,

                Product =
                    product,

                Quantity =
                    2,

                SelectedSize =
                    "XL"
            });


        context.Carts.Add(
            cart);

        await context.SaveChangesAsync();


        var service =
            new OrderService(
                context);


        // Act

        var result =
            await service.CheckoutAsync(
                userId);


        // Assert

        Assert.NotNull(
            result);

        Assert.Equal(
            userId,
            result.UserId);

        Assert.Equal(
            2998m,
            result.TotalAmount);

        Assert.Equal(
            "Placed",
            result.Status);

        Assert.StartsWith(
            "IPL-",
            result.OrderNumber);


        Assert.Single(
            result.Items);


        var orderItem =
            result.Items.Single();


        Assert.Equal(
            product.Id,
            orderItem.ProductId);

        Assert.Equal(
            "CSK Official Fan Jersey",
            orderItem.ProductName);

        Assert.Equal(
            "CSK",
            orderItem.FranchiseCode);

        Assert.Equal(
            "Jersey",
            orderItem.ProductType);

        Assert.Equal(
            "XL",
            orderItem.SelectedSize);

        Assert.Equal(
            1499m,
            orderItem.UnitPrice);

        Assert.Equal(
            2,
            orderItem.Quantity);


        /*
         * Verify order persisted.
         */

        var persistedOrder =
            await context.Orders
                .Include(x =>
                    x.Items)
                .SingleAsync();


        Assert.Equal(
            2998m,
            persistedOrder.TotalAmount);

        Assert.Single(
            persistedOrder.Items);


        /*
         * Verify stock reduced:
         *
         * 10 - 2 = 8
         */

        var persistedProduct =
            await context.Products
                .SingleAsync(
                    x =>
                        x.Id ==
                        product.Id);


        Assert.Equal(
            8,
            persistedProduct.StockQuantity);


        /*
         * Checkout should consume cart items.
         */

        var remainingCartItems =
            await context.CartItems
                .ToListAsync();


        Assert.Empty(
            remainingCartItems);
    }


    /*
     * =========================================================
     * TEST 2
     * Checkout with no cart should fail
     * =========================================================
     */

    [Fact]
    public async Task CheckoutAsync_WhenUserHasNoCart_ShouldThrowBusinessException()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();


        var service =
            new OrderService(
                context);


        // Act

        var exception =
            await Assert.ThrowsAsync<BusinessException>(
                () =>
                    service.CheckoutAsync(
                        "user-1"));


        // Assert

        Assert.Equal(
            "Your cart is empty.",
            exception.Message);


        Assert.Empty(
            await context.Orders
                .ToListAsync());
    }


    /*
     * =========================================================
     * TEST 3
     * Empty existing cart should also fail
     * =========================================================
     */

    [Fact]
    public async Task CheckoutAsync_WhenCartHasNoItems_ShouldThrowBusinessException()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();


        context.Carts.Add(
            new Cart
            {
                UserId =
                    "user-1"
            });


        await context.SaveChangesAsync();


        var service =
            new OrderService(
                context);


        // Act

        var exception =
            await Assert.ThrowsAsync<BusinessException>(
                () =>
                    service.CheckoutAsync(
                        "user-1"));


        // Assert

        Assert.Equal(
            "Your cart is empty.",
            exception.Message);


        Assert.Empty(
            await context.Orders
                .ToListAsync());
    }


    /*
     * =========================================================
     * TEST 4
     * Insufficient inventory should prevent checkout
     * =========================================================
     */

    [Fact]
    public async Task CheckoutAsync_WhenStockIsInsufficient_ShouldNotCreateOrderOrClearCart()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();

        const string userId =
            "user-1";


        var franchise =
            new Franchise
            {
                Name =
                    "Mumbai Indians",

                Code =
                    "MI"
            };


        var product =
            new Product
            {
                Name =
                    "MI Team Cap",

                ProductType =
                    ProductType.Cap,

                Price =
                    699m,

                StockQuantity =
                    1,

                IsActive =
                    true,

                Franchise =
                    franchise
            };


        context.Products.Add(
            product);

        await context.SaveChangesAsync();


        var cart =
            new Cart
            {
                UserId =
                    userId
            };


        cart.Items.Add(
            new CartItem
            {
                ProductId =
                    product.Id,

                Product =
                    product,

                Quantity =
                    3,

                SelectedSize =
                    null
            });


        context.Carts.Add(
            cart);

        await context.SaveChangesAsync();


        var service =
            new OrderService(
                context);


        // Act

        var exception =
            await Assert.ThrowsAsync<BusinessException>(
                () =>
                    service.CheckoutAsync(
                        userId));


        // Assert

        Assert.Contains(
            "Only 1 units",
            exception.Message);


        /*
         * No order should have been created.
         */

        Assert.Empty(
            await context.Orders
                .ToListAsync());


        /*
         * Inventory must remain unchanged.
         */

        var persistedProduct =
            await context.Products
                .SingleAsync(
                    x =>
                        x.Id ==
                        product.Id);


        Assert.Equal(
            1,
            persistedProduct.StockQuantity);


        /*
         * Cart must remain intact.
         */

        var persistedCartItem =
            await context.CartItems
                .SingleAsync();


        Assert.Equal(
            3,
            persistedCartItem.Quantity);
    }


    /*
     * =========================================================
     * TEST 5
     * Inactive product should prevent checkout
     * =========================================================
     */

    [Fact]
    public async Task CheckoutAsync_WhenProductIsInactive_ShouldThrowBusinessException()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();

        const string userId =
            "user-1";


        var franchise =
            new Franchise
            {
                Name =
                    "Royal Challengers Bengaluru",

                Code =
                    "RCB"
            };


        var product =
            new Product
            {
                Name =
                    "RCB Team Flag",

                ProductType =
                    ProductType.Flag,

                Price =
                    449m,

                StockQuantity =
                    10,

                IsActive =
                    false,

                Franchise =
                    franchise
            };


        context.Products.Add(
            product);

        await context.SaveChangesAsync();


        var cart =
            new Cart
            {
                UserId =
                    userId
            };


        cart.Items.Add(
            new CartItem
            {
                ProductId =
                    product.Id,

                Product =
                    product,

                Quantity =
                    1
            });


        context.Carts.Add(
            cart);

        await context.SaveChangesAsync();


        var service =
            new OrderService(
                context);


        // Act

        var exception =
            await Assert.ThrowsAsync<BusinessException>(
                () =>
                    service.CheckoutAsync(
                        userId));


        // Assert

        Assert.Contains(
            "no longer available",
            exception.Message);


        Assert.Empty(
            await context.Orders
                .ToListAsync());


        Assert.Single(
            await context.CartItems
                .ToListAsync());
    }


    /*
     * =========================================================
     * TEST 6
     * Order history must be isolated by user
     * =========================================================
     */

    [Fact]
    public async Task GetOrdersAsync_ShouldReturnOnlyOrdersForRequestedUser()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();


        context.Orders.AddRange(
            new Order
            {
                UserId =
                    "user-a",

                OrderNumber =
                    "IPL-USER-A-001",

                TotalAmount =
                    1499m,

                Status =
                    OrderStatus.Placed,

                CreatedAt =
                    DateTime.UtcNow
                        .AddMinutes(-5)
            },

            new Order
            {
                UserId =
                    "user-b",

                OrderNumber =
                    "IPL-USER-B-001",

                TotalAmount =
                    699m,

                Status =
                    OrderStatus.Placed,

                CreatedAt =
                    DateTime.UtcNow
            });


        await context.SaveChangesAsync();


        var service =
            new OrderService(
                context);


        // Act

        var result =
            await service.GetOrdersAsync(
                "user-a");


        // Assert

        Assert.Single(
            result);

        Assert.Equal(
            "IPL-USER-A-001",
            result[0].OrderNumber);

        Assert.Equal(
            "user-a",
            result[0].UserId);
    }


    /*
     * =========================================================
     * TEST 7
     * A user must not access another user's order
     * =========================================================
     */

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderBelongsToAnotherUser_ShouldReturnNull()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();


        var order =
            new Order
            {
                UserId =
                    "user-a",

                OrderNumber =
                    "IPL-PRIVATE-001",

                TotalAmount =
                    1499m,

                Status =
                    OrderStatus.Placed
            };


        context.Orders.Add(
            order);

        await context.SaveChangesAsync();


        var service =
            new OrderService(
                context);


        // Act

        var result =
            await service.GetOrderByIdAsync(
                "user-b",
                order.Id);


        // Assert

        Assert.Null(
            result);
    }


    /*
     * =========================================================
     * TEST 8
     * Owner should be able to retrieve their order
     * =========================================================
     */

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderBelongsToUser_ShouldReturnOrder()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();


        var order =
            new Order
            {
                UserId =
                    "user-a",

                OrderNumber =
                    "IPL-ORDER-001",

                TotalAmount =
                    1999m,

                Status =
                    OrderStatus.Placed
            };


        context.Orders.Add(
            order);

        await context.SaveChangesAsync();


        var service =
            new OrderService(
                context);


        // Act

        var result =
            await service.GetOrderByIdAsync(
                "user-a",
                order.Id);


        // Assert

        Assert.NotNull(
            result);

        Assert.Equal(
            order.Id,
            result.Id);

        Assert.Equal(
            "IPL-ORDER-001",
            result.OrderNumber);

        Assert.Equal(
            "user-a",
            result.UserId);
    }
}
