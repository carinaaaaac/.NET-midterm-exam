using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed class Handler(RoomHubDbContext dbContext)
{
    public async Task<Result<GetBookingEndpoint.Response>> Handle(
        GetBookingEndpoint.Request request,
        CancellationToken cancellationToken)
    {
        var response = await dbContext.Bookings
            .AsNoTracking()
            .Where(booking => booking.Id == request.Id)
            .Select(booking => new GetBookingEndpoint.Response(
                booking.Id,
                booking.RoomId,
                booking.BookedBy,
                booking.Period.StartUtc,
                booking.Period.EndUtc,
                booking.Attendees,
                booking.Status))
            .SingleOrDefaultAsync(cancellationToken);

        if (response is null)
            return new Error("BookingNotFound", "The booking does not exist.", ErrorType.NotFound);

        return response;
    }
}
