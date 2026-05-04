namespace DBetter.TrainCompositions.Infrastructure.PostgreSQL;

/// <summary>
/// Helpers for persistence DTOs
/// </summary>
public static class WrappingHelpers
{
    /// <summary>
    /// Synchronizes a database and a domain list
    /// </summary>
    /// <param name="persistenceList">List inside the persistence dto</param>
    /// <param name="domainList">Corresponding list in the domain object</param>
    /// <param name="dtoIdSelector">Id property selector for dto</param>
    /// <param name="domainIdSelector">Id property selector for domain</param>
    /// <typeparam name="TPersistence">Persistence dto Wrapper Type</typeparam>
    /// <typeparam name="TId">Type of the id</typeparam>
    /// <typeparam name="TDomain">Domain type</typeparam>
    public static void Synchronize<TPersistence, TId, TDomain>(
        this List<TPersistence> persistenceList,
        IEnumerable<TDomain> domainList,
        Func<TPersistence, TId> dtoIdSelector,
        Func<TDomain, TId> domainIdSelector)
        where TPersistence: IPersistenceDto<TDomain, TPersistence>
        where TId: notnull
    {
        // Remove
        var domainObjects = domainList.ToList();
        foreach (var persistenceObject in persistenceList.ToList())
        {
            if (!domainObjects.Any(incoming =>
                {
                    var incomingId = domainIdSelector(incoming);
                    var existingId = dtoIdSelector(persistenceObject);
                    return incomingId.Equals(existingId);
                }))
            {
                persistenceList.Remove(persistenceObject);
            }
        }

        // Add & Apply
        foreach (var domainObject in domainObjects)
        {
            var existingObject = persistenceList.FirstOrDefault(existing =>
            {
                var incomingId = domainIdSelector(domainObject);
                var existingId = dtoIdSelector(existing);
                return incomingId.Equals(existingId);
            });
            if (existingObject is not null)
            {
                existingObject.Apply(domainObject);
            }
            else
            {
                persistenceList.Add(TPersistence.FromDomain(domainObject));
            }
        }
    }

}