using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace OrderManagement.Infrastructure.UnitTest
{
    public class ExceptionEndpointTests
    {
        [Fact]
        public async Task ErrorEndpoint_ShouldReturnProblemDetails_WithStatus500_AndExceptionMessage()
        {
            // Arrange
            var exceptionMessage = "Test exception message";

            var exceptionFeature = new ExceptionHandlerFeature
            {
                Error = new System.Exception(exceptionMessage)
            };

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Features.Set<IExceptionHandlerPathFeature>(exceptionFeature);
            mockHttpContext.Response.Body = new MemoryStream();

            var errorEndpoint = new RequestDelegate(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = feature?.Error;

                var problemDetails = new ProblemDetails
                {
                    Status = 500,
                    Detail = exception?.Message,
                    Title = "An error occurred."
                };

                var json = JsonSerializer.Serialize(problemDetails);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(json, Encoding.UTF8);
            });

            // Act
            await errorEndpoint(mockHttpContext);

            // Assert
            mockHttpContext.Response.StatusCode.Should().Be(500);

            mockHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(mockHttpContext.Response.Body);
            var body = await reader.ReadToEndAsync();

            body.Should().Contain(exceptionMessage);
        }
    }

}