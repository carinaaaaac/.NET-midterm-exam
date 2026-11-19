using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Api;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    public sealed record Request(Guid Id);
    public sealed record Response(
        Guid Id,
        Guid RoomId,
        string BookedBy,
        DateTime StartUtc,
        DateTime EndUtc,
        int Attendees,
        Domain.Bookings.BookingStatus Status);

    public static void MapGetBookingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bookings/{id:guid}",
            async Task<Results<Ok<Response>, ProblemHttpResult>> (
                Guid id,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new Request(id), cancellationToken);
                if (result.Error is { } error)
                    return error.ToProblem();

                return TypedResults.Ok(result.Value);
            }).WithTags("Bookings");
    }
}
