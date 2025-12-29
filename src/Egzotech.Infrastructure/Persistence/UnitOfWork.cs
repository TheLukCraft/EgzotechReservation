using System.Data;
using Egzotech.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Egzotech.Infrastructure.Persistence;

internal class UnitOfWork(EgzotechDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(isolationLevel);
        return transaction.GetDbTransaction();
    }
}