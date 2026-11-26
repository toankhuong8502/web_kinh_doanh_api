using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Infrastructure.Sql;
using WebKinhDoanh.Infrastructure.Sql.Repositories;

namespace WebKinhDoanh.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' chưa được cấu hình.");
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseSslClientAuthenticationOptionsCallback(sslOptions =>
        {
            sslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        });

        var dataSource = dataSourceBuilder.Build();

        services.AddSingleton(dataSource);
        services.AddDbContext<StoreDbContext>(options => options.UseNpgsql(dataSource));
        services.AddScoped<SpRunner>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
        return services;
    }
}
