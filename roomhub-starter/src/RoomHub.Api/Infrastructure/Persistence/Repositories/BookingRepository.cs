using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Repositories;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public sealed class BookingRepository(RoomHubDbContext dbContext) : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Bookings.SingleOrDefaultAsync(booking => booking.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetActiveForRoomAsync(
        Guid roomId,
        CancellationToken cancellationToken) =>
        await dbContext.Bookings
            .Where(booking => booking.RoomId == roomId && booking.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetActiveForParticipantOnUtcDayAsync(
        string bookedBy,
        DateTime utcDay,
        CancellationToken cancellationToken)
    {
        var dayStartUtc = utcDay.Date;
        var nextDayStartUtc = dayStartUtc == DateTime.MaxValue.Date
            ? DateTime.MaxValue
            : dayStartUtc.AddDays(1);

        return await dbContext.Bookings
            .Where(booking =>
                booking.Status == BookingStatus.Active &&
                booking.BookedBy == bookedBy &&
                booking.Period.StartUtc >= dayStartUtc &&
                (dayStartUtc == DateTime.MaxValue.Date
                    ? booking.Period.StartUtc.Date == dayStartUtc
                    : booking.Period.StartUtc < nextDayStartUtc))
            .ToListAsync(cancellationToken);
    }

    public void Add(Booking booking) => dbContext.Bookings.Add(booking);
}
