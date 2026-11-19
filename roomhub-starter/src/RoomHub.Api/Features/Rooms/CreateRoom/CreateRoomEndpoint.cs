using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Api;
using RoomHub.Api.Application.Abstractions;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public sealed record Request(string? Name, int Capacity);
    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void MapCreateRoomEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms", async Task<Results<Created<Response>, ProblemHttpResult>> (
            Request request,
            ICommandHandler<Handler.Command, Response> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(
                new Handler.Command(request.Name, request.Capacity),
                cancellationToken);

            if (result.Error is { } error)
                return error.ToProblem();

            var response = result.Value;
            return TypedResults.Created($"/api/rooms/{response.Id}", response);
        }).WithTags("Rooms");
    }
}
