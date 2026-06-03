using System.Diagnostics;
using System.Net.Http.Headers;
using CodeAssignmentTemplate.Clients;
using CodeAssignmentTemplate.Configuration;
using CodeAssignmentTemplate.Infrastructure;
using CodeAssignmentTemplate.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<WeatherApiOptions>()
    .BindConfiguration("WeatherApi")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "Weather Service API";
    options.Description = "API for retrieving weather information.";
});

builder.Services.AddHttpClient<ISmhiClient, SmhiClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<WeatherApiOptions>>().Value;

    // Trailing slash required, or HttpClient drops the last path segment of the base address.
    client.BaseAddress = new Uri(options.SmhiBaseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<IWeatherService, WeatherService>();

builder.Services.AddProblemDetails(options =>
{
    // Covers framework-generated problem responses (e.g. 400 model validation) as well.
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

app.UseOpenApi();
app.UseSwaggerUi();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
