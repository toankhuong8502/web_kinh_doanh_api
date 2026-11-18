using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebKinhDoanh.Infrastructure.Sql;

public sealed class StoreDbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
{
    public StoreDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=MMTTStore;Trusted_Connection=True;TrustServerCertificate=True";
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new StoreDbContext(options);
    }
}
