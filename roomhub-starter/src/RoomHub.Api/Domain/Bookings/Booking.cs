using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public enum BookingStatus
{
    Active,
    Cancelled
}

public sealed class Booking
{
    private Booking()
    {
        BookedBy = string.Empty;
        Period = null!;
    }

    private Booking(
        Guid id,
        Guid roomId,
        string bookedBy,
        BookingPeriod period,
        int attendees,
        BookingStatus status)
    {
        Id = id;
        RoomId = roomId;
        BookedBy = bookedBy;
        Period = period;
        Attendees = attendees;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; }
    public BookingPeriod Period { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public DateTime StartUtc => Period.StartUtc;
    public DateTime EndUtc => Period.EndUtc;

    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        DateTime nowUtc,
        IEnumerable<Booking> roomBookings,
        IEnumerable<Booking> participantBookings)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
            return new Error("InvalidBookedBy", "BookedBy is required.", ErrorType.Validation);

        if (attendees <= 0)
            return new Error("InvalidAttendees", "Attendees must be positive.", ErrorType.Validation);

        if (attendees > room.Capacity)
            return new Error("AttendeesExceedCapacity", "The room does not have enough seats.", ErrorType.Validation);

        if (startUtc < nowUtc)
            return new Error("BookingInPast", "A booking cannot start in the past.", ErrorType.Validation);

        var periodResult = BookingPeriod.Create(startUtc, endUtc);
        if (periodResult.Error is { } periodError)
            return periodError;

        var period = periodResult.Value;
        if (roomBookings.Any(existing =>
                existing.Status == BookingStatus.Active &&
                period.Overlaps(existing.Period)))
        {
            return new Error("RoomOccupied", "The room is occupied in this interval.", ErrorType.Conflict);
        }

        var normalizedBookedBy = bookedBy.Trim();
        var utcDay = startUtc.Date;
        if (participantBookings.Count(existing =>
                existing.Status == BookingStatus.Active &&
                string.Equals(existing.BookedBy, normalizedBookedBy, StringComparison.Ordinal) &&
                existing.StartUtc.Date == utcDay) >= 2)
        {
            return new Error(
                "DailyBookingLimitExceeded",
                "A participant cannot have more than two active bookings per day.",
                ErrorType.Conflict);
        }

        return new Booking(
            Guid.NewGuid(),
            room.Id,
            normalizedBookedBy,
            period,
            attendees,
            BookingStatus.Active);
    }

    public Result<Booking> Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return new Error("BookingAlreadyCancelled", "The booking is already cancelled.", ErrorType.Conflict);

        Status = BookingStatus.Cancelled;
        return this;
    }
}
