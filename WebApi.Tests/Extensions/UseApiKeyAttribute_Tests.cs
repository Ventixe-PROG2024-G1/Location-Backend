using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebApi.Extensions;

namespace WebApi.Tests.Extensions;

public class UseApiKeyAttribute_Tests
{
    private readonly string _expectedApiKey = "secret-key";

    private ActionExecutingContext CreateContextWithHeaders(Dictionary<string, string> headers)
    {
        var httpContext = new DefaultHttpContext();
        foreach (var header in headers)
        {
            httpContext.Request.Headers[header.Key] = header.Value;
        }

        var services = new ServiceCollection();
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["SecretKeys:ApiKey"]).Returns(_expectedApiKey);
        services.AddSingleton(configMock.Object);
        httpContext.RequestServices = services.BuildServiceProvider();

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
        };

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            controller: null);
    }

    [Fact]
    public async Task Should_Return_Unauthorized_When_Header_Is_Missing()
    {
        // Arrange
        var attribute = new UseApiKeyAttribute();
        var context = CreateContextWithHeaders(new Dictionary<string, string>());
        var executed = false;

        // Act
        await attribute.OnActionExecutionAsync(context, () =>
        {
            executed = true;
            return Task.FromResult<ActionExecutedContext>(null!);
        });

        // Assert
        executed.Should().BeFalse();
        context.Result.Should().BeOfType<UnauthorizedObjectResult>()
            .Which.Value.Should().Be("Invalid or missing api-key.");
    }

    [Fact]
    public async Task Should_Return_Unauthorized_When_ApiKey_Is_Invalid()
    {
        // Arrange
        var attribute = new UseApiKeyAttribute();
        var context = CreateContextWithHeaders(new Dictionary<string, string>
        {
            { "location-api-key", "wrong-key" }
        });
        var executed = false;

        // Act
        await attribute.OnActionExecutionAsync(context, () =>
        {
            executed = true;
            return Task.FromResult<ActionExecutedContext>(null!);
        });

        // Assert
        executed.Should().BeFalse();
        context.Result.Should().BeOfType<UnauthorizedObjectResult>()
            .Which.Value.Should().Be("Invalid api-key.");
    }

    [Fact]
    public async Task Should_Proceed_When_ApiKey_Is_Valid()
    {
        // Arrange
        var attribute = new UseApiKeyAttribute();
        var context = CreateContextWithHeaders(new Dictionary<string, string>
        {
            { "location-api-key", _expectedApiKey }
        });

        var executed = false;

        // Act
        await attribute.OnActionExecutionAsync(context, () =>
        {
            executed = true;
            return Task.FromResult<ActionExecutedContext>(null!);
        });

        // Assert
        executed.Should().BeTrue();
        context.Result.Should().BeNull();
    }
}
