using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BioAlga.Backend.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Lee ConnectionStrings__Default de variables de entorno (o usa fallback dev)
            var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                     ?? "server=localhost;port=3306;database=bioalga;user=root;password=mt88xfire;";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseMySql(
                cs,
                ServerVersion.AutoDetect(cs),
                mySql =>
                {
                    // Mismo assembly de migraciones (por si usas proyectos separados)
                    mySql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    mySql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
                });

            // Logging útil cuando corres "dotnet ef ..."
            optionsBuilder.EnableSensitiveDataLogging(); // quitar en prod
            optionsBuilder.EnableDetailedErrors();

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
