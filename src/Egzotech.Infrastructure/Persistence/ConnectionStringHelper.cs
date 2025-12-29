using DotNetEnv;

namespace Egzotech.Infrastructure.Persistence;

public static class ConnectionStringHelper
{
    public static string Build(string? currentConnectionString = null)
    {
        if (!string.IsNullOrWhiteSpace(currentConnectionString))
        {
            return currentConnectionString;
        }

        var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
        if (!File.Exists(envPath))
        {
            envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        }

        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }

        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "EgzotechDb";
        var dbPass = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
        var dbUser = Environment.GetEnvironmentVariable("DB_SA_LOGIN") ?? "sa";

        if (dbHost == "sql-server")
        {
            dbHost = "localhost";
        }

        if (string.IsNullOrEmpty(dbPass))
        {
            throw new InvalidOperationException("Cannot build ConnectionString: DB_SA_PASSWORD is missing in .env or environment variables.");
        }

        return $"Server={dbHost},1433;Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";
    }
}