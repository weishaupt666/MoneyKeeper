using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Configuration;
using MoneyKeeper.Data;

namespace MoneyKeeper.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        try
        {
            logger.LogInformation("--> Attempting to apply migrations...");

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
                logger.LogInformation("--> Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("--> No pending migrations found.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "--> An error occurred while applying migrations.");
        }
    }
}
