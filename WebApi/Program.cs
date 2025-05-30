using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using WebApi.Data;
using WebApi.Extensions;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(o =>
{
    o.EnableAnnotations();
    o.ExampleFilters();
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v 1.0",
        Title = "Ventixe Location API Documentation",
        Description = "Standard documentation for Ventixe Location API."
    });
    var apiScheme = new OpenApiSecurityScheme
    {
        Name = "location-api-key",
        Description = "Api-Key Required",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme",
        Reference = new OpenApiReference
        {
            Id = "ApiKey",
            Type = ReferenceType.SecurityScheme
        }
    };
    o.AddSecurityDefinition("ApiKey", apiScheme);
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { apiScheme, new List<string>() }
    });
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddGrpc(o =>
{
    o.Interceptors.Add<ApiKeyInterceptor>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Dev", policy =>
    {
        policy
            .WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<LocationContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddSingleton<ApiKeyInterceptor>();
builder.Services.AddScoped<ILocationCache, LocationCache>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

var app = builder.Build();

app.UseCors("Dev");

app.UseRouting();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Ventixe Booking API");
});

app.MapControllers();
app.MapGrpcService<LocationService>();
app.MapGet("/", () => "gRPC & Rest Server Running!!!");

app.Run();
