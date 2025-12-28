using Egzotech.Domain.Entities;

namespace Egzotech.Application.Interfaces;

public interface IRobotRepository
{
    Task<Robot?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}