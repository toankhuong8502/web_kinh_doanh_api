using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebKinhDoanh.Infrastructure.Sql;

public sealed class StoreDbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
{
    public StoreDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Database=webkinhdoanh;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new StoreDbContext(options);
    }
}
