using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Api;
using RoomHub.Api.Application.Abstractions;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public sealed record Request(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);
    public sealed record Response(Guid Id, Guid RoomId, BookingStatus Status);

    public static void MapCreateBookingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms/{roomId:guid}/bookings",
            async Task<Results<Created<Response>, ProblemHttpResult>> (
                Guid roomId,
                Request request,
                ICommandHandler<Handler.Command, Response> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(
                    new Handler.Command(
                        roomId,
                        request.BookedBy,
                        request.StartUtc,
                        request.EndUtc,
                        request.Attendees),
                    cancellationToken);

                if (result.Error is { } error)
                    return error.ToProblem();

                var response = result.Value;
                return TypedResults.Created($"/api/bookings/{response.Id}", response);
            }).WithTags("Bookings");
    }
}
