using Grpc.Core;
using Grpc.Core.Interceptors;

namespace WebApi.Extensions;

// Partialy AI writen using our old friend ChatGPT
public class ApiKeyInterceptor(IConfiguration config) : Interceptor
{
    private readonly IConfiguration _config = config;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            var key = _config["SecretKeys:ApiKey"];

            var meta = context.RequestHeaders;
            var keyHeader = meta.FirstOrDefault(m => string.Equals(m.Key, "location-api-key", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(key) || keyHeader == null || keyHeader.Value != key)
                return CreateErrorResponse<TResponse>("Invalid or missing API-KEY", 401);

            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse<TResponse>(ex.Message, 500);
        }
    }

    private TResponse CreateErrorResponse<TResponse>(string message, int statusCode)
    {
        var response = Activator.CreateInstance<TResponse>();
        var type = typeof(TResponse);

        var succeededProp = type.GetProperty("Succeeded");
        var statusCodeProp = type.GetProperty("StatusCode");
        var messageProp = type.GetProperty("Message");

        succeededProp?.SetValue(response, false);
        statusCodeProp?.SetValue(response, statusCode);
        messageProp?.SetValue(response, message);

        return response;
    }
}
