using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Repositories;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Room room);
}
