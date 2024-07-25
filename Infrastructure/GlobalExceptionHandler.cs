using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SuggestioApi.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        _logger.LogError(exception, "TraceId: {TraceId} - {Message}", traceId, exception.Message);

        var details = new ProblemDetails
        {
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1",
            Title = "An unexpected error occurred!",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        httpContext.Response.StatusCode = details.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);

        return true;
    }
}