using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ProyectChallenge.Api.Application.Middleware;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundAppException : Exception
{
    public NotFoundAppException(string message) : base(message) { }
}

public class ExceptionHandlingMiddleware(
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env,
    IProblemDetailsService problemDetailsService) : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
    private readonly IHostEnvironment _env = env;
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning(exception, "Response has already started; cannot write ProblemDetails.");
            throw exception;
        }

        var (status, title) = exception switch
        {
            NotFoundAppException => (StatusCodes.Status404NotFound, "Resource not found"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            DomainException => (StatusCodes.Status400BadRequest, "Business rule violation"),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Bad request"),
            OperationCanceledException => (499, "Client Closed Request"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
        };

        var level = status >= 500 ? LogLevel.Error : LogLevel.Warning;
        using (_logger.BeginScope(new Dictionary<string, object?>
        {
            ["TraceId"] = context.TraceIdentifier,
            ["Path"] = context.Request.Path.Value
        }))
        {
            _logger.Log(level, exception, "Exception handled. Status: {Status}", status);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Type = $"https://httpstatuses.com/{status}",
            Detail = exception is ValidationException ? "Validations Errors" : _env.IsDevelopment() ? exception.Message : null

        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        // if is Fluent Validation, we put errors with for field
        if (exception is ValidationException fv)
        {
            problem.Status = StatusCodes.Status422UnprocessableEntity;
            problem.Title = "Validation failed";
            problem.Extensions["errors"] = fv.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        context.Response.Clear();
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

        // Format (JSON/XML si tuvieras) and content-type official service
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problem
        });
    }
}
