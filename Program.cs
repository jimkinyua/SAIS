using Microsoft.EntityFrameworkCore;
using SAIS.Models.Data;

namespace SAIS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure environment-specific settings
            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<SAISDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("SAISConnection")
                )
            );

            var app = builder.Build();

            // Log environment information
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting SAIS application in {Environment} environment", app.Environment.EnvironmentName);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Run migrations automatically on startup
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SAISDbContext>();
                var migrationLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    // Ensure database exists and run migrations
                    db.Database.Migrate();

                    // Log environment-specific information
                    migrationLogger.LogInformation("Database migrations completed successfully for {Environment} environment",
                        app.Environment.EnvironmentName);
                }
                catch (Exception ex)
                {
                    migrationLogger.LogError(ex, "An error occurred while migrating the database for {Environment} environment",
                        app.Environment.EnvironmentName);
                    throw;
                }
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
