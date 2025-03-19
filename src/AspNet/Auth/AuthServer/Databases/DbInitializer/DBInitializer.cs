using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.EntityFramework.Context;
using Shared.EntityFramework.Entities;

namespace AuthServer.Databases.DbInitializer;

public class DbInitializer(
    IWebHostEnvironment env,
    IServiceProvider serviceProvider,
    ILogger<DbInitializer> logger
    ) : BackgroundService
{
    public string ActivitySourceName = "Migrations";

    private AuthContext _context = null!;
    private UserManager<SystemUser> _userManager = null!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<AuthContext>();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<SystemUser>>();

        ActivitySourceName = env.IsDevelopment() ? "Migrations" : "MySqlMigrations";

        await InitializeDatabaseAsync(cancellationToken);
    }

    private async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

         var databaseProvider = _context.Database.ProviderName;
            logger.LogInformation("Using database provider: {DatabaseProvider}", databaseProvider);

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(_context.Database.MigrateAsync, cancellationToken);

        await SeedAsync(cancellationToken);

        logger.LogInformation("System Database initialization completed after {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);
    }

    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        if (!_context.SystemUsers.Any())
        {
            var user = new SystemUser()
            {
                UserName = "admin",
                Email = ""
            };

            var result = await _userManager.CreateAsync(user, "gameAdmin!123");

            if (!result.Succeeded)
            {
                logger.LogError($"Failed to create default user!");
                logger.LogError(result.Errors.ToList()[0].Description);
            }
            else
                logger.LogInformation("Default user created successfully!");
        }


        await _context.SaveChangesAsync(cancellationToken);
    }
}