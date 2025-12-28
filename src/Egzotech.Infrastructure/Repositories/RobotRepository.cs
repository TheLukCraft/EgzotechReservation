using Egzotech.Application.Interfaces;
using Egzotech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Egzotech.Infrastructure.Persistence.Repositories;

internal class RobotRepository(EgzotechDbContext dbContext) : IRobotRepository
{
    public async Task<Robot?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Robots
            .FirstOrDefaultAsync(robot => robot.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Robots
            .AnyAsync(robot => robot.Id == id, cancellationToken);
    }
}