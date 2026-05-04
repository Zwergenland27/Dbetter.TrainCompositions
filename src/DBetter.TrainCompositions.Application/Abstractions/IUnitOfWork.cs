namespace DBetter.TrainCompositions.Application.Abstractions;

/// <summary>
/// Manages repository spreading operations
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Opens a new database transaction
    /// </summary>
    Task BeginTransaction(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commit changes to database
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Abort all changes
    /// </summary>
    Task AbortAsync(CancellationToken cancellationToken = default);
}