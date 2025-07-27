using BaseApi.Application.Exceptions;
using Microsoft.AspNetCore.Http.Json;
using System.Net;
using System.Text.Json;

namespace BaseApi.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            Message = exception.Message,
            ErrorCode = GetErrorCode(exception),
            Timestamp = DateTime.UtcNow,
            Path = context.Request.Path,
            Method = context.Request.Method
        };

        response.StatusCode = GetStatusCode(exception);

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }

    private static string GetErrorCode(Exception exception)
    {
        return exception switch
        {
            BusinessException businessEx => businessEx.ErrorCode,
            ArgumentException => "INVALID_ARGUMENT",
            UnauthorizedAccessException => "UNAUTHORIZED",
            _ => "INTERNAL_SERVER_ERROR"
        };
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            BusinessException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }
} 