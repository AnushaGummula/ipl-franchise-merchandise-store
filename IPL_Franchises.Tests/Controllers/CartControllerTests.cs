using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IPL_Franchises.API.Controllers;
using IPL_Franchises.Application.DTOs.Cart;
using IPL_Franchises.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IPL_Franchises.Tests.Controllers;

public class CartControllerTests
{
    [Fact]
    public async Task GetCart_WhenUserIdClaimIsMissing_ShouldReturnUnauthorized()
    {
        // Arrange

        var serviceMock =
            new Mock<ICartService>();

        var controller =
            new CartController(
                serviceMock.Object);

        SetUser(
            controller,
            userId: null);


        // Act

        var result =
            await controller.GetCart();


        // Assert

        Assert.IsType<UnauthorizedResult>(
            result);

        serviceMock.Verify(
            service =>
                service.GetCartAsync(
                    It.IsAny<string>()),
            Times.Never);
    }


    [Fact]
    public async Task GetCart_WhenAuthenticated_ShouldReturnOkWithCart()
    {
        // Arrange

        const string userId =
            "user-123";


        var expectedCart =
            new CartDto
            {
                UserId =
                    userId
            };


        var serviceMock =
            new Mock<ICartService>();


        serviceMock
            .Setup(service =>
                service.GetCartAsync(
                    userId))
            .ReturnsAsync(
                expectedCart);


        var controller =
            new CartController(
                serviceMock.Object);


        SetUser(
            controller,
            userId);


        // Act

        var result =
            await controller.GetCart();


        // Assert

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);


        Assert.Same(
            expectedCart,
            okResult.Value);


        serviceMock.Verify(
            service =>
                service.GetCartAsync(
                    userId),
            Times.Once);
    }


    [Fact]
    public async Task AddItem_WhenAuthenticated_ShouldCallServiceWithLoggedInUser()
    {
        // Arrange

        const string userId =
            "user-123";


        var request =
            new AddToCartRequest
            {
                ProductId =
                    1,

                Quantity =
                    2,

                SelectedSize =
                    "XL"
            };


        var expectedCart =
            new CartDto
            {
                UserId =
                    userId
            };


        var serviceMock =
            new Mock<ICartService>();


        serviceMock
            .Setup(service =>
                service.AddItemAsync(
                    userId,
                    request))
            .ReturnsAsync(
                expectedCart);


        var controller =
            new CartController(
                serviceMock.Object);


        SetUser(
            controller,
            userId);


        // Act

        var result =
            await controller.AddItem(
                request);


        // Assert

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);


        Assert.Same(
            expectedCart,
            okResult.Value);


        serviceMock.Verify(
            service =>
                service.AddItemAsync(
                    userId,
                    request),
            Times.Once);
    }


    [Fact]
    public async Task AddItem_WhenUserIdClaimIsMissing_ShouldReturnUnauthorized()
    {
        // Arrange

        var request =
            new AddToCartRequest
            {
                ProductId =
                    1,

                Quantity =
                    1,

                SelectedSize =
                    "M"
            };


        var serviceMock =
            new Mock<ICartService>();


        var controller =
            new CartController(
                serviceMock.Object);


        SetUser(
            controller,
            userId: null);


        // Act

        var result =
            await controller.AddItem(
                request);


        // Assert

        Assert.IsType<UnauthorizedResult>(
            result);


        serviceMock.Verify(
            service =>
                service.AddItemAsync(
                    It.IsAny<string>(),
                    It.IsAny<AddToCartRequest>()),
            Times.Never);
    }


    [Fact]
    public async Task UpdateQuantity_WhenAuthenticated_ShouldPassItemIdAndQuantityToService()
    {
        // Arrange

        const string userId =
            "user-123";

        const int cartItemId =
            25;

        const int quantity =
            3;


        var expectedCart =
            new CartDto
            {
                UserId =
                    userId
            };


        var serviceMock =
            new Mock<ICartService>();


        serviceMock
            .Setup(service =>
                service.UpdateQuantityAsync(
                    userId,
                    cartItemId,
                    quantity))
            .ReturnsAsync(
                expectedCart);


        var controller =
            new CartController(
                serviceMock.Object);


        SetUser(
            controller,
            userId);


        // Act

        var result =
            await controller.UpdateQuantity(
                cartItemId,
                quantity);


        // Assert

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);


        Assert.Same(
            expectedCart,
            okResult.Value);


        serviceMock.Verify(
            service =>
                service.UpdateQuantityAsync(
                    userId,
                    cartItemId,
                    quantity),
            Times.Once);
    }


    [Fact]
    public async Task RemoveItem_WhenAuthenticated_ShouldCallServiceAndReturnNoContent()
    {
        // Arrange

        const string userId =
            "user-123";

        const int cartItemId =
            25;


        var serviceMock =
            new Mock<ICartService>();


        serviceMock
            .Setup(service =>
                service.RemoveItemAsync(
                    userId,
                    cartItemId))
            .Returns(
                Task.CompletedTask);


        var controller =
            new CartController(
                serviceMock.Object);


        SetUser(
            controller,
            userId);


        // Act

        var result =
            await controller.RemoveItem(
                cartItemId);


        // Assert

        Assert.IsType<NoContentResult>(
            result);


        serviceMock.Verify(
            service =>
                service.RemoveItemAsync(
                    userId,
                    cartItemId),
            Times.Once);
    }


    private static void SetUser(
        ControllerBase controller,
        string? userId)
    {
        var claims =
            new List<Claim>();


        if (!string.IsNullOrWhiteSpace(
                userId))
        {
            claims.Add(
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId));
        }


        var identity =
            new ClaimsIdentity(
                claims,
                authenticationType:
                    "TestAuthentication");


        var principal =
            new ClaimsPrincipal(
                identity);


        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext =
                    new DefaultHttpContext
                    {
                        User =
                            principal
                    }
            };
    }
}
