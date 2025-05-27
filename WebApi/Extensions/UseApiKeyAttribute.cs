using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Extensions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class UseApiKeyAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = config["SecretKeys:ApiKey"];

        if (!context.HttpContext.Request.Headers.TryGetValue("location-api-key", out var providedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Invalid or missing api-key.");
            return;
        }

        if (string.IsNullOrEmpty(apiKey) || !string.Equals(providedApiKey, apiKey))
        {
            context.Result = new UnauthorizedObjectResult("Invalid api-key.");
            return;
        }

        await next();
    }
}
