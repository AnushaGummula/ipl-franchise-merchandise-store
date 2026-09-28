using IPL_Franchises.Application.DTOs.Products;
using IPL_Franchises.Domain.Entities;
using IPL_Franchises.Domain.Enums;
using IPL_Franchises.Infrastructure.Services;
using IPL_Franchises.Tests.Helpers;

namespace IPL_Franchises.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task GetProductsAsync_WhenNoFiltersProvided_ShouldReturnOnlyActiveProducts()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);

        var request =
            new ProductSearchRequest
            {
                Page = 1,
                PageSize = 50
            };


        // Act
        var result =
            await service.GetProductsAsync(request);


        // Assert
        Assert.Equal(
            5,
            result.TotalCount);

        Assert.Equal(
            5,
            result.Items.Count);

        Assert.DoesNotContain(
            result.Items,
            p => p.Name == "Inactive Test Product");
    }


    [Fact]
    public async Task GetProductsAsync_WhenSearchingByName_ShouldReturnMatchingProduct()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);

        var request =
            new ProductSearchRequest
            {
                Search = "Jersey",
                Page = 1,
                PageSize = 50
            };


        // Act
        var result =
            await service.GetProductsAsync(request);


        // Assert
        Assert.Equal(
            2,
            result.TotalCount);

        Assert.All(
            result.Items,
            product =>
                Assert.Contains(
                    "Jersey",
                    product.Name,
                    StringComparison.OrdinalIgnoreCase));
    }


    [Fact]
    public async Task GetProductsAsync_WhenSearchingDescription_ShouldReturnMatchingProduct()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);

        var request =
            new ProductSearchRequest
            {
                Search = "premium",
                Page = 1,
                PageSize = 50
            };


        // Act
        var result =
            await service.GetProductsAsync(request);


        // Assert
        Assert.Single(
            result.Items);

        Assert.Equal(
            "CSK Official Fan Jersey",
            result.Items[0].Name);
    }


    [Fact]
    public async Task GetProductsAsync_WhenFranchiseIsCSK_ShouldReturnOnlyCSKProducts()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);

        var request =
            new ProductSearchRequest
            {
                Franchise = "csk",
                Page = 1,
                PageSize = 50
            };


        // Act
        var result =
            await service.GetProductsAsync(request);


        // Assert
        Assert.Equal(
            3,
            result.TotalCount);

        Assert.All(
            result.Items,
            product =>
                Assert.Equal(
                    "CSK",
                    product.FranchiseCode));
    }


    [Fact]
    public async Task GetProductsAsync_WhenTypeIsJersey_ShouldReturnOnlyJerseys()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);

        var request =
            new ProductSearchRequest
            {
                Type = "Jersey",
                Page = 1,
                PageSize = 50
            };


        // Act
        var result =
            await service.GetProductsAsync(request);


        // Assert
        Assert.Equal(
            2,
            result.TotalCount);

        Assert.All(
            result.Items,
            product =>
                Assert.Equal(
                    "Jersey",
                    product.ProductType));
    }


    [Fact]
    public async Task GetProductsAsync_WhenFranchiseAndTypeSpecified_ShouldApplyBothFilters()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);

        var request =
            new ProductSearchRequest
            {
                Franchise = "RR",
                Type = "Flag",
                Page = 1,
                PageSize = 50
            };


        // Act
        var result =
            await service.GetProductsAsync(request);


        // Assert
        Assert.Single(
            result.Items);

        var product =
            result.Items.Single();

        Assert.Equal(
            "RR",
            product.FranchiseCode);

        Assert.Equal(
            "Flag",
            product.ProductType);

        Assert.Equal(
            "RR Team Flag",
            product.Name);
    }


    [Fact]
    public async Task GetProductsAsync_WhenFiltersDoNotMatch_ShouldReturnEmptyResult()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);

        var request =
            new ProductSearchRequest
            {
                Franchise = "RR",
                Search = "CSK",
                Page = 1,
                PageSize = 50
            };


        // Act
        var result =
            await service.GetProductsAsync(request);


        // Assert
        Assert.Equal(
            0,
            result.TotalCount);

        Assert.Empty(
            result.Items);
    }


    [Fact]
    public async Task GetProductsAsync_WhenPageSizeExceedsFifty_ShouldClampPageSizeToFifty()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);

        var request =
            new ProductSearchRequest
            {
                Page = 1,
                PageSize = 100
            };


        // Act
        var result =
            await service.GetProductsAsync(request);


        // Assert
        Assert.Equal(
            50,
            result.PageSize);
    }


    [Fact]
    public async Task GetProductsAsync_WhenPageIsLessThanOne_ShouldDefaultPageToOne()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);

        var request =
            new ProductSearchRequest
            {
                Page = 0,
                PageSize = 12
            };


        // Act
        var result =
            await service.GetProductsAsync(request);


        // Assert
        Assert.Equal(
            1,
            result.Page);
    }


    [Fact]
    public async Task GetProductByIdAsync_WhenProductExistsAndActive_ShouldReturnProduct()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        var ids =
            await SeedCatalogAsync(context);

        var service =
            new ProductService(context);


        // Act
        var result =
            await service.GetProductByIdAsync(
                ids.CskJerseyId);


        // Assert
        Assert.NotNull(
            result);

        Assert.Equal(
            "CSK Official Fan Jersey",
            result.Name);

        Assert.Equal(
            "CSK",
            result.FranchiseCode);

        Assert.Equal(
            "Jersey",
            result.ProductType);

        Assert.Equal(
            1499m,
            result.Price);
    }


    [Fact]
    public async Task GetProductByIdAsync_WhenProductIsInactive_ShouldReturnNull()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        var ids =
            await SeedCatalogAsync(context);

        var service =
            new ProductService(context);


        // Act
        var result =
            await service.GetProductByIdAsync(
                ids.InactiveProductId);


        // Assert
        Assert.Null(
            result);
    }


    [Fact]
    public async Task GetProductByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context =
            await TestDbContextFactory.CreateAsync();

        await SeedCatalogAsync(context);

        var service =
            new ProductService(context);


        // Act
        var result =
            await service.GetProductByIdAsync(
                999999);


        // Assert
        Assert.Null(
            result);
    }


    private static async Task<TestProductIds>
        SeedCatalogAsync(
            IPL_Franchises.Infrastructure.Data.IPLDbContext context)
    {
        var csk =
            new Franchise
            {
                Name =
                    "Chennai Super Kings",

                Code =
                    "CSK"
            };


        var rr =
            new Franchise
            {
                Name =
                    "Rajasthan Royals",

                Code =
                    "RR"
            };


        var cskJersey =
            new Product
            {
                Name =
                    "CSK Official Fan Jersey",

                Description =
                    "Premium CSK supporter jersey",

                ProductType =
                    ProductType.Jersey,

                Price =
                    1499m,

                StockQuantity =
                    10,

                IsActive =
                    true,

                Franchise =
                    csk
            };


        var cskCap =
            new Product
            {
                Name =
                    "CSK Team Cap",

                Description =
                    "CSK supporter cap",

                ProductType =
                    ProductType.Cap,

                Price =
                    699m,

                StockQuantity =
                    15,

                IsActive =
                    true,

                Franchise =
                    csk
            };


        var cskFlag =
            new Product
            {
                Name =
                    "CSK Team Flag",

                Description =
                    "CSK supporter flag",

                ProductType =
                    ProductType.Flag,

                Price =
                    449m,

                StockQuantity =
                    20,

                IsActive =
                    true,

                Franchise =
                    csk
            };


        var rrJersey =
            new Product
            {
                Name =
                    "RR Official Fan Jersey",

                Description =
                    "RR supporter jersey",

                ProductType =
                    ProductType.Jersey,

                Price =
                    1499m,

                StockQuantity =
                    8,

                IsActive =
                    true,

                Franchise =
                    rr
            };


        var rrFlag =
            new Product
            {
                Name =
                    "RR Team Flag",

                Description =
                    "RR supporter flag",

                ProductType =
                    ProductType.Flag,

                Price =
                    449m,

                StockQuantity =
                    12,

                IsActive =
                    true,

                Franchise =
                    rr
            };


        var inactiveProduct =
            new Product
            {
                Name =
                    "Inactive Test Product",

                ProductType =
                    ProductType.Cap,

                Price =
                    999m,

                StockQuantity =
                    10,

                IsActive =
                    false,

                Franchise =
                    rr
            };


        context.Products.AddRange(
            cskJersey,
            cskCap,
            cskFlag,
            rrJersey,
            rrFlag,
            inactiveProduct);


        await context.SaveChangesAsync();


        return new TestProductIds
        {
            CskJerseyId =
                cskJersey.Id,

            InactiveProductId =
                inactiveProduct.Id
        };
    }


    private sealed class TestProductIds
    {
        public int CskJerseyId { get; init; }

        public int InactiveProductId { get; init; }
    }
}
