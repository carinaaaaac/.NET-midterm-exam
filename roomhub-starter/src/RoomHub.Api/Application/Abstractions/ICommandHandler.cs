using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Application.Abstractions;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}
