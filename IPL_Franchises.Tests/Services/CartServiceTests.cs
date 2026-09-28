using IPL_Franchises.Application.DTOs.Cart;
using IPL_Franchises.Application.Exceptions;
using IPL_Franchises.Domain.Entities;
using IPL_Franchises.Domain.Enums;
using IPL_Franchises.Infrastructure.Data;
using IPL_Franchises.Infrastructure.Services;
using IPL_Franchises.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace IPL_Franchises.Tests.Services;

public class CartServiceTests
{
    /*
     * =========================================================
     * TEST 1
     * Empty cart should return a valid empty cart DTO
     * =========================================================
     */

    [Fact]
    public async Task GetCartAsync_WhenUserHasNoCart_ShouldReturnEmptyCart()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();

        var service =
            new CartService(context);

        const string userId =
            "user-1";


        // Act

        var result =
            await service.GetCartAsync(
                userId);


        // Assert

        Assert.NotNull(result);

        Assert.Equal(
            userId,
            result.UserId);

        Assert.Empty(
            result.Items);
    }


    /*
     * =========================================================
     * TEST 2
     * Same jersey + same size should merge quantities
     * =========================================================
     */

    [Fact]
    public async Task AddItemAsync_WhenSameJerseyAndSizeExists_ShouldIncreaseQuantity()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();

        const string userId =
            "user-1";


        var product =
            await SeedProductAsync(
                context,
                ProductType.Jersey,
                stockQuantity: 10);


        var cart =
            new Cart
            {
                UserId = userId
            };


        cart.Items.Add(
            new CartItem
            {
                ProductId =
                    product.Id,

                Product =
                    product,

                Quantity =
                    1,

                SelectedSize =
                    "M"
            });


        context.Carts.Add(
            cart);

        await context.SaveChangesAsync();


        var service =
            new CartService(context);


        var request =
            new AddToCartRequest
            {
                ProductId =
                    product.Id,

                Quantity =
                    2,

                /*
                 * Intentionally lower case.
                 * CartService should normalize
                 * it to M.
                 */
                SelectedSize =
                    "m"
            };


        // Act

        var result =
            await service.AddItemAsync(
                userId,
                request);


        // Assert

        Assert.Single(
            result.Items);


        var item =
            result.Items.Single();


        Assert.Equal(
            product.Id,
            item.ProductId);

        Assert.Equal(
            "M",
            item.SelectedSize);

        Assert.Equal(
            3,
            item.Quantity);


        var persistedItems =
            await context.CartItems
                .ToListAsync();


        Assert.Single(
            persistedItems);

        Assert.Equal(
            3,
            persistedItems[0].Quantity);
    }


    /*
     * =========================================================
     * TEST 3
     * Same jersey + different size should create separate lines
     * =========================================================
     */

    [Fact]
    public async Task AddItemAsync_WhenSameJerseyHasDifferentSize_ShouldCreateSeparateCartItem()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();

        const string userId =
            "user-1";


        var product =
            await SeedProductAsync(
                context,
                ProductType.Jersey,
                stockQuantity: 10);


        var cart =
            new Cart
            {
                UserId = userId
            };


        cart.Items.Add(
            new CartItem
            {
                ProductId =
                    product.Id,

                Product =
                    product,

                Quantity =
                    1,

                SelectedSize =
                    "M"
            });


        context.Carts.Add(
            cart);

        await context.SaveChangesAsync();


        var service =
            new CartService(context);


        var request =
            new AddToCartRequest
            {
                ProductId =
                    product.Id,

                Quantity =
                    1,

                SelectedSize =
                    "XL"
            };


        // Act

        var result =
            await service.AddItemAsync(
                userId,
                request);


        // Assert

        Assert.Equal(
            2,
            result.Items.Count);


        Assert.Contains(
            result.Items,
            item =>
                item.ProductId ==
                    product.Id &&
                item.SelectedSize ==
                    "M" &&
                item.Quantity ==
                    1);


        Assert.Contains(
            result.Items,
            item =>
                item.ProductId ==
                    product.Id &&
                item.SelectedSize ==
                    "XL" &&
                item.Quantity ==
                    1);


        var persistedItems =
            await context.CartItems
                .OrderBy(x =>
                    x.SelectedSize)
                .ToListAsync();


        Assert.Equal(
            2,
            persistedItems.Count);
    }


    /*
     * =========================================================
     * TEST 4
     * Invalid jersey sizes must be rejected
     * =========================================================
     */

    [Theory]
    [InlineData("")]
    [InlineData("S")]
    [InlineData("XXL")]
    [InlineData("ABC")]
    public async Task AddItemAsync_WhenJerseySizeIsInvalid_ShouldThrowBusinessException(
        string invalidSize)
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();


        var product =
            await SeedProductAsync(
                context,
                ProductType.Jersey,
                stockQuantity: 10);


        var service =
            new CartService(context);


        var request =
            new AddToCartRequest
            {
                ProductId =
                    product.Id,

                Quantity =
                    1,

                SelectedSize =
                    invalidSize
            };


        // Act

        var exception =
            await Assert.ThrowsAsync<BusinessException>(
                () =>
                    service.AddItemAsync(
                        "user-1",
                        request));


        // Assert

        Assert.Equal(
            "Please select a valid jersey size: M, L or XL.",
            exception.Message);


        Assert.Empty(
            await context.CartItems
                .ToListAsync());
    }


    /*
     * =========================================================
     * TEST 5
     * Quantity cannot exceed available inventory
     * =========================================================
     */

    [Fact]
    public async Task AddItemAsync_WhenFinalQuantityExceedsStock_ShouldThrowBusinessException()
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();

        const string userId =
            "user-1";


        var product =
            await SeedProductAsync(
                context,
                ProductType.Jersey,
                stockQuantity: 2);


        /*
         * User already has 1 unit.
         */
        var cart =
            new Cart
            {
                UserId = userId
            };


        cart.Items.Add(
            new CartItem
            {
                ProductId =
                    product.Id,

                Product =
                    product,

                Quantity =
                    1,

                SelectedSize =
                    "M"
            });


        context.Carts.Add(
            cart);

        await context.SaveChangesAsync();


        var service =
            new CartService(context);


        /*
         * Existing = 1
         * Request  = 2
         *
         * Final = 3
         *
         * Stock = 2
         *
         * Therefore request must fail.
         */
        var request =
            new AddToCartRequest
            {
                ProductId =
                    product.Id,

                Quantity =
                    2,

                SelectedSize =
                    "M"
            };


        // Act

        var exception =
            await Assert.ThrowsAsync<BusinessException>(
                () =>
                    service.AddItemAsync(
                        userId,
                        request));


        // Assert

        Assert.Contains(
            "Only 2 units",
            exception.Message);


        /*
         * Existing cart quantity must
         * remain unchanged.
         */
        var existingItem =
            await context.CartItems
                .SingleAsync();


        Assert.Equal(
            1,
            existingItem.Quantity);
    }


    /*
     * =========================================================
     * TEST 6
     * Quantity <= 0 must be rejected
     * =========================================================
     */

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task AddItemAsync_WhenQuantityIsNotPositive_ShouldThrowBusinessException(
        int quantity)
    {
        // Arrange

        await using var context =
            await TestDbContextFactory.CreateAsync();


        var service =
            new CartService(context);


        var request =
            new AddToCartRequest
            {
                ProductId =
                    1,

                Quantity =
                    quantity,

                SelectedSize =
                    null
            };


        // Act

        var exception =
            await Assert.ThrowsAsync<BusinessException>(
                () =>
                    service.AddItemAsync(
                        "user-1",
                        request));


        // Assert

        Assert.Equal(
            "Quantity must be greater than zero.",
            exception.Message);
    }


    /*
     * =========================================================
     * TEST HELPER
     * =========================================================
     */

    private static async Task<Product>
        SeedProductAsync(
            IPLDbContext context,
            ProductType productType,
            int stockQuantity)
    {
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
                    productType ==
                    ProductType.Jersey
                        ? "CSK Official Fan Jersey"
                        : "CSK Merchandise",

                Description =
                    "Test product",

                ProductType =
                    productType,

                Price =
                    1499m,

                StockQuantity =
                    stockQuantity,

                IsActive =
                    true,

                Franchise =
                    franchise
            };


        context.Products.Add(
            product);


        await context.SaveChangesAsync();


        return product;
    }
}
