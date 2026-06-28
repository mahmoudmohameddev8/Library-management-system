using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Interfaces.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Infrastructure.Services;
using LibraryManagement.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<LibraryDbContext>(options =>
        {
            options.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 0)),
                mySqlOptions =>
                {
                    mySqlOptions.MigrationsAssembly(typeof(LibraryDbContext).Assembly.FullName);
                    mySqlOptions.CommandTimeout(30);
                });
        });

        services.Configure<JwtSettings>(opts =>
        {
            var section = configuration.GetSection(JwtSettings.SectionName);
            opts.SecretKey = section["SecretKey"] ?? string.Empty;
            opts.Issuer = section["Issuer"] ?? string.Empty;
            opts.Audience = section["Audience"] ?? string.Empty;
            if (int.TryParse(section["ExpirationMinutes"], out var mins))
                opts.ExpirationMinutes = mins;
        });

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBorrowingRepository, BorrowingRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}
