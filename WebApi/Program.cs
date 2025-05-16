using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Extensions;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(o =>
{
    o.Interceptors.Add<ApiKeyInterceptor>();
});

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<LocationContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddSingleton<ApiKeyInterceptor>();
builder.Services.AddScoped<LocationCache>();
builder.Services.AddScoped<LocationRepository>();

var app = builder.Build();

app.MapGrpcService<LocationService>();

app.MapGet("/", () => "gRPC Server Running");

app.Run();
