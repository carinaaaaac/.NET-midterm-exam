using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RoomHub.Api.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false });

        foreach (var handlerType in handlerTypes)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(type =>
                    type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(Abstractions.ICommandHandler<,>));

            foreach (var handlerInterface in handlerInterfaces)
                services.AddScoped(handlerInterface, handlerType);
        }

        return services;
    }
}
