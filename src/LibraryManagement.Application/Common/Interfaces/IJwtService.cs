using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    int GetExpirationMinutes();
}
