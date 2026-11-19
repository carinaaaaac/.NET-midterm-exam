using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed record BookingPeriod
{
    private BookingPeriod()
    {
    }

    private BookingPeriod(DateTime startUtc, DateTime endUtc)
    {
        StartUtc = startUtc;
        EndUtc = endUtc;
    }

    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }

    public static Result<BookingPeriod> Create(DateTime startUtc, DateTime endUtc)
    {
        if (endUtc <= startUtc)
            return new Error("InvalidRange", "The end must be after the start.", ErrorType.Validation);

        if (endUtc - startUtc > TimeSpan.FromHours(4))
            return new Error("BookingTooLong", "A booking cannot exceed four hours.", ErrorType.Validation);

        return new BookingPeriod(startUtc, endUtc);
    }

    public bool Overlaps(BookingPeriod other) =>
        StartUtc < other.EndUtc && other.StartUtc < EndUtc;
}
