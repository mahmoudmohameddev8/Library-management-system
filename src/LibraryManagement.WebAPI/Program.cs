using LibraryManagement.Application;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Infrastructure;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Infrastructure.Seed;
using LibraryManagement.WebAPI.Extensions;
using LibraryManagement.WebAPI.Middleware;
using LibraryManagement.WebAPI.Services;
using Scalar.AspNetCore;
using Serilog;

// Bootstrap logger so startup errors are captured before configuration loads.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Library Management API");

    var builder = WebApplication.CreateBuilder(args);

    // ─── Serilog ──────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext());

    // ─── Layer registrations ──────────────────────────────────────────────────
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ─── HTTP context + current-user ──────────────────────────────────────────
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    // ─── JWT Authentication + Authorization policies ───────────────────────────
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // ─── Controllers + OpenAPI ────────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // ─── CORS ─────────────────────────────────────────────────────────────────
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                         ?? Array.Empty<string>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("LibraryPolicy", policy =>
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod());
    });

    // ─── Build ────────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ─── Database migration & seeding ─────────────────────────────────────────
    await InitialiseDatabaseAsync(app);

    // ─── HTTP pipeline ────────────────────────────────────────────────────────

    // Global exception handler catches everything — must be first.
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Library Management API";
            options.Theme = ScalarTheme.DeepSpace;
            options.AddPreferredSecuritySchemes("Bearer");
        });
    }

    app.UseHttpsRedirection();
    app.UseCors("LibraryPolicy");

    app.UseAuthentication();
    app.UseAuthorization();

    // Activity logger runs after auth so the user identity is available.
    app.UseMiddleware<ActivityLoggingMiddleware>();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// ─── DB init helper ───────────────────────────────────────────────────────────
static async Task InitialiseDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Recreating database schema...");
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        logger.LogInformation("Schema ready. Running database seeder...");
        await DatabaseSeeder.SeedAsync(context, logger);
        logger.LogInformation("Database initialisation complete.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "A fatal error occurred during database initialisation.");
        throw;
    }
}
