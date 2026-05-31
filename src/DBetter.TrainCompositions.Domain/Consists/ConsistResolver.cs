using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Consists;

/// <inheritdoc/>
public class ConsistResolver(IConsistRepository repository): IPlannedConsistResolver
{
    private readonly List<Consist> _knownConsists = [];

    internal ConsistResolver(IConsistRepository repository, List<Consist> consists)
        : this(repository)
    {
        _knownConsists = consists;
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<Consist> AllKnownConsists => _knownConsists.AsReadOnly();
    
    /// <inheritdoc/>
    public async Task<Consist?> ResolveAsync(List<VehicleId> vehicleIds)
    {
        var existing = _knownConsists.FirstOrDefault(consist => consist.Matches(vehicleIds));
        if (existing is not null)
        {
            return existing;
        }

        var fromRepository = await repository.FindAsync(vehicleIds);
        if (fromRepository is not null)
        {
            _knownConsists.Add(fromRepository);
            return fromRepository;
        }

        var created = Consist.Create(vehicleIds);
        if (created.HasFailed) return null;
        var consist = created.Value;
        repository.Store(consist);
        _knownConsists.Add(consist);
        return consist;
    }
}