using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Api;
using RoomHub.Api.Application.Abstractions;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public sealed record Request(Guid Id);

    public static void MapCancelBookingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{id:guid}/cancel",
            async Task<Results<NoContent, ProblemHttpResult>> (
                Guid id,
                ICommandHandler<Handler.Command, bool> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new Handler.Command(id), cancellationToken);
                if (result.Error is { } error)
                    return error.ToProblem();

                return TypedResults.NoContent();
            }).WithTags("Bookings");
    }
}
