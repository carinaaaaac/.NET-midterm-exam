using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoomHub.Api.Domain.Repositories;
using RoomHub.Api.Domain.UnitOfWork;
using RoomHub.Api.Infrastructure.Persistence.Repositories;

namespace RoomHub.Api.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RoomHub")
            ?? throw new InvalidOperationException("Connection string 'RoomHub' is missing.");

        services.AddDbContext<RoomHubDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<RoomHubDbContext>());
        services.AddScoped<DatabaseInitializer>();

        return services;
    }
}
