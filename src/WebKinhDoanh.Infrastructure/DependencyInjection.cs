using System.Text.RegularExpressions;
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

        var normalized = Regex.Replace(
            connectionString,
            @"(sslmode\s*=\s*)(verify-full|verify-ca)",
            "$1require",
            RegexOptions.IgnoreCase);

        normalized = Regex.Replace(
            normalized,
            @"(ssl\s*mode\s*=\s*)(verifyfull|verifyca|verify-full|verify-ca)",
            "$1Require",
            RegexOptions.IgnoreCase);

        try
        {
            var csb = new NpgsqlConnectionStringBuilder(normalized);
            if (csb.SslMode is SslMode.VerifyFull or SslMode.VerifyCA)
            {
                csb.SslMode = SslMode.Require;
            }
            normalized = csb.ConnectionString;
        }
        catch
        {
            // Fallback nếu chuỗi có định dạng tùy biến
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(normalized);
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
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
