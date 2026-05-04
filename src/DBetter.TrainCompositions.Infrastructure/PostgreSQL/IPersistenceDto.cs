namespace DBetter.TrainCompositions.Infrastructure.PostgreSQL;

/// <summary>
/// Wrapper method to ensure consistent naming for persistent domain representations
/// </summary>
/// <typeparam name="TDomain">Domain class that shall be persisted</typeparam>
/// <typeparam name="TPersistence">The class itself</typeparam>
public interface IPersistenceDto<TDomain, TPersistence> where TPersistence: IPersistenceDto<TDomain, TPersistence>
{
    /// <summary>
    /// Initialize new persistence dto from domain object
    /// </summary>
    /// <param name="domain">Incoming domain object</param>
    /// <returns>Persistence dto</returns>
    static abstract TPersistence FromDomain(TDomain domain);
    
    /// <summary>
    /// Convert dto to domain object
    /// </summary>
    /// <returns>Domain object</returns>
    TDomain ToDomain();
    
    /// <summary>
    /// Apply changes from domain to dto
    /// </summary>
    /// <param name="domain">incoming domain object</param>
    void Apply(TDomain domain);
}