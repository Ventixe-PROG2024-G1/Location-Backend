using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;

namespace WebApi.Extensions;

public class ApiKeyInterceptor(IConfiguration config) : Interceptor
{
    private readonly IConfiguration _config = config;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var key = _config["SecretKeys:ApiKey"];

        var meta = context.RequestHeaders;
        var keyHeader = meta.FirstOrDefault(m => m.Key.ToLower() == "location-api-key");

        if (string.IsNullOrEmpty(key) || keyHeader == null || keyHeader.Value != key)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid or missing API KEY"));

        return await continuation(request, context);
    }
}
