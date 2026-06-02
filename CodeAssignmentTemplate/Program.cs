using CodeAssignmentTemplate;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
	.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
	.AddNegotiate();
builder.Services.AddAuthorization(options => { options.FallbackPolicy = options.DefaultPolicy; });

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at
// https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-10.0
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
	options.Title = "Weather Service API";
	options.Description = "API for retrieving weather information.";
});

builder.Services.AddTransient<IWeatherService, WeatherService>();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi();

app.UseHttpsRedirection();

app.MapControllers();

var apiRouteGroup = app.MapGroup("api");
apiRouteGroup.RegisterWeatherApi();

app.UseAuthentication();
app.UseAuthorization();
app.Run();