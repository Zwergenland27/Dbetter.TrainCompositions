namespace DBetter.TrainCompositions.Domain.Abstractions;

/// <summary>
/// Common repository logic
/// </summary>
public interface IRepository<TAggregate, in TId> where TAggregate: AggregateRoot<TId> where TId : notnull
{
    /// <summary>
    /// Stores the aggregate in the database
    /// </summary>
    /// <param name="aggregate">domain object</param>
    /// <remarks>
    /// This method is an upsert operation. A new database row will be created, when the aggregate was not loaded into the change tracker.
    /// </remarks>
    void Store(TAggregate aggregate);
    
    /// <summary>
    /// Returns the aggregate based on its id
    /// </summary>
    /// <param name="id">Id of the aggregate</param>
    /// <returns>Found aggregate if found</returns>
    Task<TAggregate?> GetAsync(TId id);
}