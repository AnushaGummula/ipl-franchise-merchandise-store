using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IPL_Franchises.API.Controllers;
using IPL_Franchises.Application.DTOs.Orders;
using IPL_Franchises.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IPL_Franchises.Tests.Controllers;

public class OrdersControllerTests
{
    [Fact]
    public async Task Checkout_WhenUserIdClaimIsMissing_ShouldReturnUnauthorized()
    {
        // Arrange

        var serviceMock =
            new Mock<IOrderService>();

        var controller =
            new OrdersController(
                serviceMock.Object);

        SetUser(
            controller,
            userId: null);


        // Act

        var result =
            await controller.Checkout();


        // Assert

        Assert.IsType<UnauthorizedResult>(
            result);

        serviceMock.Verify(
            service =>
                service.CheckoutAsync(
                    It.IsAny<string>()),
            Times.Never);
    }


    [Fact]
    public async Task Checkout_WhenAuthenticated_ShouldReturnOkWithOrder()
    {
        // Arrange

        const string userId =
            "user-123";


        var expectedOrder =
            new OrderDto
            {
                Id =
                    10,

                UserId =
                    userId,

                OrderNumber =
                    "IPL-TEST-001",

                TotalAmount =
                    1499m,

                Status =
                    "Placed"
            };


        var serviceMock =
            new Mock<IOrderService>();


        serviceMock
            .Setup(service =>
                service.CheckoutAsync(
                    userId))
            .ReturnsAsync(
                expectedOrder);


        var controller =
            new OrdersController(
                serviceMock.Object);


        SetUser(
            controller,
            userId);


        // Act

        var result =
            await controller.Checkout();


        // Assert

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);


        Assert.Same(
            expectedOrder,
            okResult.Value);


        serviceMock.Verify(
            service =>
                service.CheckoutAsync(
                    userId),
            Times.Once);
    }


    [Fact]
    public async Task GetOrders_WhenAuthenticated_ShouldReturnOkWithOrders()
    {
        // Arrange

        const string userId =
            "user-123";


        var expectedOrders =
            new List<OrderDto>
            {
                new()
                {
                    Id =
                        1,

                    UserId =
                        userId,

                    OrderNumber =
                        "IPL-001",

                    TotalAmount =
                        1499m,

                    Status =
                        "Placed"
                },

                new()
                {
                    Id =
                        2,

                    UserId =
                        userId,

                    OrderNumber =
                        "IPL-002",

                    TotalAmount =
                        699m,

                    Status =
                        "Placed"
                }
            };


        var serviceMock =
            new Mock<IOrderService>();


        serviceMock
            .Setup(service =>
                service.GetOrdersAsync(
                    userId))
            .ReturnsAsync(
                expectedOrders);


        var controller =
            new OrdersController(
                serviceMock.Object);


        SetUser(
            controller,
            userId);


        // Act

        var result =
            await controller.GetOrders();


        // Assert

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);


        Assert.Same(
            expectedOrders,
            okResult.Value);


        serviceMock.Verify(
            service =>
                service.GetOrdersAsync(
                    userId),
            Times.Once);
    }


    [Fact]
    public async Task GetOrders_WhenUserIdClaimIsMissing_ShouldReturnUnauthorized()
    {
        // Arrange

        var serviceMock =
            new Mock<IOrderService>();


        var controller =
            new OrdersController(
                serviceMock.Object);


        SetUser(
            controller,
            userId: null);


        // Act

        var result =
            await controller.GetOrders();


        // Assert

        Assert.IsType<UnauthorizedResult>(
            result);


        serviceMock.Verify(
            service =>
                service.GetOrdersAsync(
                    It.IsAny<string>()),
            Times.Never);
    }


    [Fact]
    public async Task GetOrder_WhenOrderDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange

        const string userId =
            "user-123";

        const int orderId =
            999;


        var serviceMock =
            new Mock<IOrderService>();


        serviceMock
            .Setup(service =>
                service.GetOrderByIdAsync(
                    userId,
                    orderId))
            .ReturnsAsync(
                (OrderDto?)null);


        var controller =
            new OrdersController(
                serviceMock.Object);


        SetUser(
            controller,
            userId);


        // Act

        var result =
            await controller.GetOrder(
                orderId);


        // Assert

        Assert.IsType<NotFoundResult>(
            result);


        serviceMock.Verify(
            service =>
                service.GetOrderByIdAsync(
                    userId,
                    orderId),
            Times.Once);
    }


    [Fact]
    public async Task GetOrder_WhenOrderExists_ShouldReturnOkWithOrder()
    {
        // Arrange

        const string userId =
            "user-123";

        const int orderId =
            10;


        var expectedOrder =
            new OrderDto
            {
                Id =
                    orderId,

                UserId =
                    userId,

                OrderNumber =
                    "IPL-ORDER-010",

                TotalAmount =
                    2199m,

                Status =
                    "Placed"
            };


        var serviceMock =
            new Mock<IOrderService>();


        serviceMock
            .Setup(service =>
                service.GetOrderByIdAsync(
                    userId,
                    orderId))
            .ReturnsAsync(
                expectedOrder);


        var controller =
            new OrdersController(
                serviceMock.Object);


        SetUser(
            controller,
            userId);


        // Act

        var result =
            await controller.GetOrder(
                orderId);


        // Assert

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);


        var order =
            Assert.IsType<OrderDto>(
                okResult.Value);


        Assert.Equal(
            orderId,
            order.Id);

        Assert.Equal(
            "IPL-ORDER-010",
            order.OrderNumber);


        serviceMock.Verify(
            service =>
                service.GetOrderByIdAsync(
                    userId,
                    orderId),
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
                "TestAuthentication");


        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext =
                    new DefaultHttpContext
                    {
                        User =
                            new ClaimsPrincipal(
                                identity)
                    }
            };
    }
}
