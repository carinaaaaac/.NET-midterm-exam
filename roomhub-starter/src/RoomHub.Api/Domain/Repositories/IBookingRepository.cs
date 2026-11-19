using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Domain.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetActiveForParticipantOnUtcDayAsync(
        string bookedBy,
        DateTime utcDay,
        CancellationToken cancellationToken);
    void Add(Booking booking);
}
