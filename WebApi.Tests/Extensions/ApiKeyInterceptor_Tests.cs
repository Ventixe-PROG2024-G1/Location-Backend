using FluentAssertions;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Extensions;
using Grpc.Core.Testing;

namespace WebApi.Tests.Extensions;

public class ApiKeyInterceptor_Tests
{
    private const string ValidApiKey = "correct-key";

    private static IConfiguration CreateConfiguration(string? apiKey)
    {
        var config = new Dictionary<string, string?>
        {
            ["SecretKeys:ApiKey"] = apiKey
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(config!)
            .Build();
    }

    private static ServerCallContext CreateContextWithHeader(string? keyValue)
    {
        var headers = new Metadata();
        if (keyValue != null)
            headers.Add("location-api-key", keyValue);

        return TestServerCallContext.Create(
            method: "TestMethod",
            host: "localhost",
            deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: headers,
            cancellationToken: CancellationToken.None,
            peer: "127.0.0.1",
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: metadata => Task.CompletedTask,
            writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: options => { }
        );
    }

    private class TestResponse
    {
        public bool Succeeded { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    private static Task<TestResponse> DummyContinuation(object _, ServerCallContext __) =>
        Task.FromResult(new TestResponse
        {
            Succeeded = true,
            StatusCode = 200,
            Message = "Success"
        });

    [Fact]
    public async Task Returns401_WhenApiKeyMissing()
    {
        var interceptor = new ApiKeyInterceptor(CreateConfiguration(ValidApiKey));
        var response = await interceptor.UnaryServerHandler<object, TestResponse>(
            new(), CreateContextWithHeader(null), DummyContinuation);

        response.Succeeded.Should().BeFalse();
        response.StatusCode.Should().Be(401);
        response.Message.Should().Be("Invalid or missing API-KEY");
    }

    [Fact]
    public async Task Returns401_WhenApiKeyInvalid()
    {
        var interceptor = new ApiKeyInterceptor(CreateConfiguration(ValidApiKey));
        var response = await interceptor.UnaryServerHandler<object, TestResponse>(
            new(), CreateContextWithHeader("wrong-key"), DummyContinuation);

        response.Succeeded.Should().BeFalse();
        response.StatusCode.Should().Be(401);
        response.Message.Should().Be("Invalid or missing API-KEY");
    }

    [Fact]
    public async Task Proceeds_WhenApiKeyValid()
    {
        var interceptor = new ApiKeyInterceptor(CreateConfiguration(ValidApiKey));
        var response = await interceptor.UnaryServerHandler<object, TestResponse>(
            new(), CreateContextWithHeader(ValidApiKey), DummyContinuation);

        response.Succeeded.Should().BeTrue();
        response.StatusCode.Should().Be(200);
        response.Message.Should().Be("Success");
    }

    [Fact]
    public async Task Returns500_OnException()
    {
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["SecretKeys:ApiKey"]).Throws(new Exception("Config error"));

        var interceptor = new ApiKeyInterceptor(config.Object);

        var response = await interceptor.UnaryServerHandler<object, TestResponse>(
            new(), CreateContextWithHeader(ValidApiKey), DummyContinuation);

        response.Succeeded.Should().BeFalse();
        response.StatusCode.Should().Be(500);
        response.Message.Should().Be("Config error");
    }
}
