using System.Text.Json;
using CodeAssignmentTemplate.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CodeAssignmentTemplate.Infrastructure;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Client disconnected; no one left to respond to.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
            return false;

        var (status, title, detail, logLevel) = exception switch
        {
            CityNotFoundException e =>
                (StatusCodes.Status404NotFound,
                 "Not Found",
                 e.Message,
                 LogLevel.Information),

            ArgumentException e when (e.ParamName == "city") =>
                (StatusCodes.Status400BadRequest,
                 "Bad Request",
                 e.Message,
                 LogLevel.Warning),

            SmhiUnavailableException e =>
                (StatusCodes.Status502BadGateway,
                 "Bad Gateway",
                 e.Message,
                 LogLevel.Warning),

            HttpRequestException =>
                (StatusCodes.Status502BadGateway,
                 "Bad Gateway",
                 "Weather data provider (SMHI) is currently unavailable.",
                 LogLevel.Warning),

            JsonException =>
                (StatusCodes.Status502BadGateway,
                 "Bad Gateway",
                 "Weather data provider (SMHI) returned an unexpected response format.",
                 LogLevel.Warning),

            // Request to SMHI timed out.
            OperationCanceledException =>
                (StatusCodes.Status502BadGateway,
                 "Bad Gateway",
                 "The request to SMHI timed out.",
                 LogLevel.Warning),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 "Internal Server Error",
                 "An unexpected error occurred.",
                 LogLevel.Error)
        };

        _logger.Log(logLevel, exception,
            "Unhandled exception mapped to HTTP {Status}: {Detail}", status, detail);

        httpContext.Response.StatusCode = status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            }
        });
    }
}
