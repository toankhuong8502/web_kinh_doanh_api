using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WebKinhDoanh.Application.Interfaces.IServices;
using WebKinhDoanh.Application.Services;

namespace WebKinhDoanh.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IContactService, ContactService>();
        return services;
    }
}
