using RoomHub.Api.Application.Abstractions;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Repositories;
using RoomHub.Api.Domain.UnitOfWork;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed class Handler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<Handler.Command, CreateBookingEndpoint.Response>
{
    public sealed record Command(
        Guid RoomId,
        string? BookedBy,
        DateTime StartUtc,
        DateTime EndUtc,
        int Attendees) : ICommand<CreateBookingEndpoint.Response>;

    public async Task<Result<CreateBookingEndpoint.Response>> Handle(
        Command command,
        CancellationToken cancellationToken)
    {
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return new Error("RoomNotFound", "The room does not exist.", ErrorType.NotFound);

        var roomBookings = await bookings.GetActiveForRoomAsync(command.RoomId, cancellationToken);
        IReadOnlyList<Booking> participantBookings = Array.Empty<Booking>();
        if (!string.IsNullOrWhiteSpace(command.BookedBy))
        {
            participantBookings = await bookings.GetActiveForParticipantOnUtcDayAsync(
                command.BookedBy.Trim(),
                command.StartUtc,
                cancellationToken);
        }

        var bookingResult = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            timeProvider.GetUtcNow().UtcDateTime,
            roomBookings,
            participantBookings);

        if (bookingResult.Error is { } error)
            return error;

        var booking = bookingResult.Value;
        bookings.Add(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBookingEndpoint.Response(booking.Id, booking.RoomId, booking.Status);
    }
}
