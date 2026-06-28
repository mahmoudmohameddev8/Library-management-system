using LibraryManagement.Application.Common.Interfaces;

namespace LibraryManagement.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public string HashPassword(string plainTextPassword)
        => BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: 12);

    public bool VerifyPassword(string plainTextPassword, string hashedPassword)
        => BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
}
