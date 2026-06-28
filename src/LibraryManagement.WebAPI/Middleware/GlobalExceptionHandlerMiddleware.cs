using System.Net;
using System.Text.Json;
using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Models;

namespace LibraryManagement.WebAPI.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound,
                ApiResponse<object>.Fail(ex.Message, 404)),

            ValidationException ex => (HttpStatusCode.BadRequest,
                ApiResponse<object>.Fail(ex.Message, 400, ex.Errors)),

            ConflictException ex => (HttpStatusCode.Conflict,
                ApiResponse<object>.Fail(ex.Message, 409)),

            UnauthorizedException ex => (HttpStatusCode.Unauthorized,
                ApiResponse<object>.Fail(ex.Message, 401)),

            ForbiddenException ex => (HttpStatusCode.Forbidden,
                ApiResponse<object>.Fail(ex.Message, 403)),

            _ => (HttpStatusCode.InternalServerError,
                ApiResponse<object>.Fail("An unexpected error occurred. Please try again later.", 500))
        };

        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
