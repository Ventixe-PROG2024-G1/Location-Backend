using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(o =>
{
    o.Interceptors.Add<ApiKeyInterceptor>();
});

builder.Services.AddSingleton<ApiKeyInterceptor>();

var app = builder.Build();

app.Run();
