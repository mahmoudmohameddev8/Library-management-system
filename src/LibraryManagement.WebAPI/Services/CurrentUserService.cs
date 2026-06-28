using System.Security.Claims;
using LibraryManagement.Application.Common.Interfaces;

namespace LibraryManagement.WebAPI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return claim is not null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
        }
    }

    public string Username
        => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";

    public string Role
        => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

    public bool IsAuthenticated
        => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
