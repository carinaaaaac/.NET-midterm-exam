using RoomHub.Api.Application.Abstractions;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Repositories;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Domain.UnitOfWork;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed class Handler(IRoomRepository rooms, IUnitOfWork unitOfWork)
    : ICommandHandler<Handler.Command, CreateRoomEndpoint.Response>
{
    public sealed record Command(string? Name, int Capacity)
        : ICommand<CreateRoomEndpoint.Response>;

    public async Task<Result<CreateRoomEndpoint.Response>> Handle(
        Command command,
        CancellationToken cancellationToken)
    {
        var roomResult = Room.Create(command.Name, command.Capacity);
        if (roomResult.Error is { } error)
            return error;

        var room = roomResult.Value;
        rooms.Add(room);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateRoomEndpoint.Response(room.Id, room.Name, room.Capacity);
    }
}
