using FluentAssertions;
using Microsoft.AspNetCore.Http;
using OrderManagement.Infrastructure.Exceptions;
using System.Text;


namespace OrderManagement.Infrastructure.UnitTest
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WhenExceptionThrown_ShouldReturnProblemJsonWithStatus500()
        {
            // Arrange
            var exceptionMessage = "Test exception in pipeline";
            RequestDelegate next = (HttpContext ctx) => throw new Exception(exceptionMessage);

            var middleware = new ExceptionHandlingMiddleware(next);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(500);
            context.Response.ContentType.Should().Be("application/problem+json");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            var responseBody = await reader.ReadToEndAsync();

            responseBody.Should().Contain(exceptionMessage);
            responseBody.Should().Contain("\"status\":500");
        }
    }

}
