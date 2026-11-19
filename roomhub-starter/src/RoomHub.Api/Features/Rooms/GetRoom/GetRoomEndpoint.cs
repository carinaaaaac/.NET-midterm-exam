using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Api;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public sealed record Request(Guid Id);
    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void MapGetRoomEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/rooms/{id:guid}", async Task<Results<Ok<Response>, ProblemHttpResult>> (
            Guid id,
            Handler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new Request(id), cancellationToken);
            if (result.Error is { } error)
                return error.ToProblem();

            return TypedResults.Ok(result.Value);
        }).WithTags("Rooms");
    }
}
