using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using IPL_Franchises.API.Middleware;
using IPL_Franchises.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IPL_Franchises.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenBusinessExceptionOccurs_ShouldReturnBadRequest()
    {
        // Arrange

        const string expectedMessage =
            "Quantity must be greater than zero.";


        RequestDelegate next =
            _ =>
                Task.FromException(
                    new BusinessException(
                        expectedMessage));


        var loggerMock =
            new Mock<
                ILogger<ExceptionHandlingMiddleware>>();


        var middleware =
            new ExceptionHandlingMiddleware(
                next,
                loggerMock.Object);


        var context =
            CreateHttpContext();


        // Act

        await middleware.InvokeAsync(
            context);


        // Assert

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            context.Response.StatusCode);


        Assert.Equal(
            "application/json",
            context.Response.ContentType);


        var body =
            await ReadResponseBodyAsync(
                context);


        using var json =
            JsonDocument.Parse(
                body);


        Assert.Equal(
            400,
            json.RootElement
                .GetProperty("status")
                .GetInt32());


        Assert.Equal(
            expectedMessage,
            json.RootElement
                .GetProperty("message")
                .GetString());
    }


    [Fact]
    public async Task InvokeAsync_WhenUnexpectedExceptionOccurs_ShouldReturnInternalServerError()
    {
        // Arrange

        RequestDelegate next =
            _ =>
                Task.FromException(
                    new InvalidOperationException(
                        "Sensitive internal error"));


        var loggerMock =
            new Mock<
                ILogger<ExceptionHandlingMiddleware>>();


        var middleware =
            new ExceptionHandlingMiddleware(
                next,
                loggerMock.Object);


        var context =
            CreateHttpContext();


        // Act

        await middleware.InvokeAsync(
            context);


        // Assert

        Assert.Equal(
            StatusCodes
                .Status500InternalServerError,
            context.Response.StatusCode);


        Assert.Equal(
            "application/json",
            context.Response.ContentType);


        var body =
            await ReadResponseBodyAsync(
                context);


        using var json =
            JsonDocument.Parse(
                body);


        Assert.Equal(
            500,
            json.RootElement
                .GetProperty("status")
                .GetInt32());


        Assert.Equal(
            "An unexpected error occurred.",
            json.RootElement
                .GetProperty("message")
                .GetString());


        /*
         * Important security check:
         *
         * Internal exception details should
         * never leak to the API consumer.
         */
        Assert.DoesNotContain(
            "Sensitive internal error",
            body);
    }


    [Fact]
    public async Task InvokeAsync_WhenNoExceptionOccurs_ShouldAllowRequestToContinue()
    {
        // Arrange

        RequestDelegate next =
            context =>
            {
                context.Response.StatusCode =
                    StatusCodes.Status204NoContent;

                return Task.CompletedTask;
            };


        var loggerMock =
            new Mock<
                ILogger<ExceptionHandlingMiddleware>>();


        var middleware =
            new ExceptionHandlingMiddleware(
                next,
                loggerMock.Object);


        var context =
            CreateHttpContext();


        // Act

        await middleware.InvokeAsync(
            context);


        // Assert

        Assert.Equal(
            StatusCodes.Status204NoContent,
            context.Response.StatusCode);
    }


    private static DefaultHttpContext
        CreateHttpContext()
    {
        var context =
            new DefaultHttpContext();


        /*
         * Default response streams are not
         * suitable for reading test output,
         * so use an in-memory stream.
         */
        context.Response.Body =
            new MemoryStream();


        context.Request.Method =
            HttpMethods.Get;


        context.Request.Path =
            "/api/test";


        return context;
    }


    private static async Task<string>
        ReadResponseBodyAsync(
            HttpContext context)
    {
        context.Response.Body.Seek(
            0,
            SeekOrigin.Begin);


        using var reader =
            new StreamReader(
                context.Response.Body,
                leaveOpen: true);


        return await reader
            .ReadToEndAsync();
    }
}
