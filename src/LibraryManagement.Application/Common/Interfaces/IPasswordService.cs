namespace LibraryManagement.Application.Common.Interfaces;

public interface IPasswordService
{
    string HashPassword(string plainTextPassword);
    bool VerifyPassword(string plainTextPassword, string hashedPassword);
}
