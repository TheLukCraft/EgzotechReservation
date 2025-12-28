# Create migration
dotnet ef migrations add InitialCreate -p src/Egzotech.Infrastructure -s src/Egzotech.Api -o Persistence/Migrations

# Update Database
dotnet ef database update -p src/Egzotech.Infrastructure -s src/Egzotech.Api