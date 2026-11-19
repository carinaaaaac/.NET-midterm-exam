using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed class Handler(RoomHubDbContext dbContext)
{
    public async Task<Result<GetRoomEndpoint.Response>> Handle(
        GetRoomEndpoint.Request request,
        CancellationToken cancellationToken)
    {
        var response = await dbContext.Rooms
            .AsNoTracking()
            .Where(room => room.Id == request.Id)
            .Select(room => new GetRoomEndpoint.Response(room.Id, room.Name, room.Capacity))
            .SingleOrDefaultAsync(cancellationToken);

        if (response is null)
            return new Error("RoomNotFound", "The room does not exist.", ErrorType.NotFound);

        return response;
    }
}
