using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public class UserActivityLog : BaseEntity
{
    public Guid? UserId { get; private set; }
    public string Username { get; private set; } = "Anonymous";
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime Timestamp { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int HttpStatusCode { get; private set; }

    private UserActivityLog() { }

    public static UserActivityLog Create(
        string action,
        string entityType,
        int httpStatusCode,
        string? entityId = null,
        Guid? userId = null,
        string? username = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        bool isSuccess = true,
        string? errorMessage = null)
    {
        return new UserActivityLog
        {
            UserId = userId,
            Username = username ?? "Anonymous",
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.UtcNow,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            HttpStatusCode = httpStatusCode
        };
    }
}
