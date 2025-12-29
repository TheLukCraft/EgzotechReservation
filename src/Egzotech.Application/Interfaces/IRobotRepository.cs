using Egzotech.Domain.Entities;

namespace Egzotech.Application.Interfaces;

public interface IRobotRepository
{
    Task<Robot?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Robot>> GetAllActiveAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Reservation>> GetForRobotAndDateAsync(Guid robotId, DateOnly date, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}