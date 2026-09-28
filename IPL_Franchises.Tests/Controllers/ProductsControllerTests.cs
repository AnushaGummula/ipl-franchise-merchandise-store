using System.Threading.Tasks;
using IPL_Franchises.API.Controllers;
using IPL_Franchises.Application.DTOs;
using IPL_Franchises.Application.DTOs.Products;
using IPL_Franchises.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IPL_Franchises.Tests.Controllers;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetProducts_WhenCalled_ShouldReturnOkWithServiceResult()
    {
        // Arrange

        var request =
            new ProductSearchRequest
            {
                Franchise = "CSK",
                Type = "Jersey",
                Page = 1,
                PageSize = 12
            };


        var expectedResult =
            new PagedResult<ProductDto>
            {
                Items =
                [
                    new ProductDto
                    {
                        Id = 1,
                        Name =
                            "CSK Official Fan Jersey",
                        FranchiseCode =
                            "CSK",
                        ProductType =
                            "Jersey",
                        Price =
                            1499m
                    }
                ],

                TotalCount = 1,
                Page = 1,
                PageSize = 12
            };


        var serviceMock =
            new Mock<IProductService>();


        serviceMock
            .Setup(service =>
                service.GetProductsAsync(
                    request))
            .ReturnsAsync(
                expectedResult);


        var controller =
            new ProductsController(
                serviceMock.Object);


        // Act

        var result =
            await controller.GetProducts(
                request);


        // Assert

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);


        Assert.Same(
            expectedResult,
            okResult.Value);


        serviceMock.Verify(
            service =>
                service.GetProductsAsync(
                    request),
            Times.Once);
    }


    [Fact]
    public async Task GetProduct_WhenProductExists_ShouldReturnOkWithProduct()
    {
        // Arrange

        const int productId =
            10;


        var expectedProduct =
            new ProductDto
            {
                Id =
                    productId,

                Name =
                    "MI Team Cap",

                FranchiseCode =
                    "MI",

                ProductType =
                    "Cap",

                Price =
                    699m,

                StockQuantity =
                    15
            };


        var serviceMock =
            new Mock<IProductService>();


        serviceMock
            .Setup(service =>
                service.GetProductByIdAsync(
                    productId))
            .ReturnsAsync(
                expectedProduct);


        var controller =
            new ProductsController(
                serviceMock.Object);


        // Act

        var result =
            await controller.GetProduct(
                productId);


        // Assert

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);


        var product =
            Assert.IsType<ProductDto>(
                okResult.Value);


        Assert.Equal(
            productId,
            product.Id);

        Assert.Equal(
            "MI Team Cap",
            product.Name);


        serviceMock.Verify(
            service =>
                service.GetProductByIdAsync(
                    productId),
            Times.Once);
    }


    [Fact]
    public async Task GetProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange

        const int productId =
            999;


        var serviceMock =
            new Mock<IProductService>();


        serviceMock
            .Setup(service =>
                service.GetProductByIdAsync(
                    productId))
            .ReturnsAsync(
                (ProductDto?)null);


        var controller =
            new ProductsController(
                serviceMock.Object);


        // Act

        var result =
            await controller.GetProduct(
                productId);


        // Assert

        Assert.IsType<NotFoundResult>(
            result);


        serviceMock.Verify(
            service =>
                service.GetProductByIdAsync(
                    productId),
            Times.Once);
    }
}
