using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Egzotech.Infrastructure.Persistence;

public class EgzotechDbContextFactory : IDesignTimeDbContextFactory<EgzotechDbContext>
{
    public EgzotechDbContext CreateDbContext(string[] args)
    {
        // Looking for .env two levels up src/Egzotech.Infrastructure
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");

        if (!File.Exists(envPath))
        {
            envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        }

        Env.Load(envPath);
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "EgzotechDb";
        var dbPass = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
        var dbUser = Environment.GetEnvironmentVariable("DB_SA_LOGIN") ?? "sa";

        if (dbHost == "sql-server")
        {
            dbHost = "localhost";
        }

        var connectionString = $"Server={dbHost},1433;Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<EgzotechDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new EgzotechDbContext(optionsBuilder.Options);
    }
}