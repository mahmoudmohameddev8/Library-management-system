using FluentValidation;
using LibraryManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBorrowingService, BorrowingService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
