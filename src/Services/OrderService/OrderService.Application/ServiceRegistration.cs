using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationRegistration(this IServiceCollection services, Type startup)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddAutoMapper(assembly);
        services.AddMediatR(assembly);

        return services;
    }
}