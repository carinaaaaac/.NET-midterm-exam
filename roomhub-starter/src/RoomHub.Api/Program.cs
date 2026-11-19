using System.Text.Json.Serialization;
using RoomHub.Api.Application;
using RoomHub.Api.Endpoints;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Middleware;
using RoomHub.Api.Repositories;
using RoomHub.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Starter organized in layers: Endpoints -> Services -> Repositories -> Models.
builder.Services.AddSingleton<RoomRepository>();
builder.Services.AddSingleton<BookingRepository>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddHandlers(typeof(Program).Assembly);
builder.Services.AddScoped<RoomHub.Api.Features.Rooms.GetRoom.Handler>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/", () => "RoomHub API is running.");
app.MapCreateRoomEndpoint();
app.MapGetRoomEndpoint();
app.MapLegacyBookingCreationEndpoint();
app.MapBookingEndpoints();

app.Run();
