using DBetter.TrainCompositions.Application.Abstractions;
using DBetter.TrainCompositions.Infrastructure.OutboxPattern;
using Microsoft.EntityFrameworkCore.Storage;

namespace DBetter.TrainCompositions.Infrastructure.PostgreSQL;

public class UnitOfWork(DBetterContext context) : IUnitOfWork, IAsyncDisposable
{
    private IDbContextTransaction? _transaction;
    
    public async Task BeginTransaction(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            return;
        }
        
        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("Cannot commit a transaction that has not been started.");
        }
        
        await context.SaveChangesAsync(cancellationToken);
        
        await _transaction.CommitAsync(cancellationToken);
        
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task AbortAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("Cannot abort a transaction that has not been started.");
        }
        
        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
        }
        await context.DisposeAsync();
    }
}