using MyAPI.Configurations;
using MyAPI.Contracts;
using MyAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
