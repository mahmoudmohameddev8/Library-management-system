using System.Diagnostics;
using System.Security.Claims;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Persistence;

namespace LibraryManagement.WebAPI.Middleware;

public class ActivityLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActivityLoggingMiddleware> _logger;

    public ActivityLoggingMiddleware(RequestDelegate next, ILogger<ActivityLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, LibraryDbContext dbContext)
    {
        var sw = Stopwatch.StartNew();

        await _next(context);

        sw.Stop();

        // Only log mutating or sensitive operations — skip GET and static assets
        if (ShouldLog(context))
        {
            try
            {
                var userId = GetUserId(context);
                var username = context.User?.FindFirst(ClaimTypes.Name)?.Value;

                var log = UserActivityLog.Create(
                    action: $"{context.Request.Method} {context.Request.Path}",
                    entityType: ExtractEntityType(context.Request.Path),
                    httpStatusCode: context.Response.StatusCode,
                    entityId: ExtractEntityId(context.Request.Path),
                    userId: userId,
                    username: username,
                    ipAddress: context.Connection.RemoteIpAddress?.ToString(),
                    userAgent: context.Request.Headers.UserAgent.ToString(),
                    isSuccess: context.Response.StatusCode < 400,
                    errorMessage: context.Response.StatusCode >= 400
                        ? $"HTTP {context.Response.StatusCode}"
                        : null);

                await dbContext.UserActivityLogs.AddAsync(log);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Activity logging must never break the request pipeline
                _logger.LogWarning(ex, "Failed to write activity log for {Method} {Path}",
                    context.Request.Method, context.Request.Path);
            }
        }
    }

    private static bool ShouldLog(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/scalar") || path.StartsWith("/openapi") || path == "/favicon.ico")
            return false;

        return context.Request.Method != HttpMethods.Get || context.Response.StatusCode >= 400;
    }

    private static Guid? GetUserId(HttpContext context)
    {
        var claim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
        return claim is not null && Guid.TryParse(claim.Value, out var id) ? id : null;
    }

    private static string ExtractEntityType(PathString path)
    {
        var segments = path.Value?.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray()
                       ?? Array.Empty<string>();
        return segments.Length >= 2 ? segments[1] : "Unknown";
    }

    private static string? ExtractEntityId(PathString path)
    {
        var segments = path.Value?.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray()
                       ?? Array.Empty<string>();
        return segments.Length >= 3 ? segments[2] : null;
    }
}
