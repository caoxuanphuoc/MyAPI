using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using MyAPI.Configurations;
using MyAPI.Contracts;
using MyAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyAPI",
        Version = "v1",
        Description = "API documentation for MyAPI services."
    });
});
builder.Services
    .AddOptions<ExternalApiOptions>()
    .Bind(builder.Configuration.GetSection(ExternalApiOptions.SectionName))
    .Validate(options =>
    {
        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var isSecureScheme = uri.Scheme == Uri.UriSchemeHttps;
        var isLocalHttp = uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase);

        return isSecureScheme || isLocalHttp;
    }, "ExternalApi:BaseUrl must be HTTPS, except localhost.")
    .Validate(options => options.TimeoutSeconds > 0, "ExternalApi:TimeoutSeconds must be greater than 0.")
    .Validate(options => options.RetryCount >= 0, "ExternalApi:RetryCount must be greater than or equal to 0.")
    .ValidateOnStart();

builder.Services
    .AddHttpClient<IExternalApiClient, ExternalApiClient>((serviceProvider, httpClient) =>
    {
        var options = serviceProvider
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<ExternalApiOptions>>()
            .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
        httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    });

builder.Services
    .AddOptions<BinanceFuturesOptions>()
    .Bind(builder.Configuration.GetSection(BinanceFuturesOptions.SectionName))
    .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _), "BinanceFutures:BaseUrl must be a valid absolute URL.")
    .ValidateOnStart();

builder.Services.AddScoped<IBinanceFuturesService, BinanceFuturesService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
