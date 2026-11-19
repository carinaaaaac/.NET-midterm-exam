using RoomHub.Api.Application.Abstractions;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Repositories;
using RoomHub.Api.Domain.UnitOfWork;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed class Handler(IBookingRepository bookings, IUnitOfWork unitOfWork)
    : ICommandHandler<Handler.Command, bool>
{
    public sealed record Command(Guid Id) : ICommand<bool>;

    public async Task<Result<bool>> Handle(Command command, CancellationToken cancellationToken)
    {
        var booking = await bookings.GetByIdAsync(command.Id, cancellationToken);
        if (booking is null)
            return new Error("BookingNotFound", "The booking does not exist.", ErrorType.NotFound);

        var cancelResult = booking.Cancel();
        if (cancelResult.Error is { } error)
            return error;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
