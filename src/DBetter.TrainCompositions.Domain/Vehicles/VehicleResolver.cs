using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Vehicles;

/// <inheritdoc/>
public class VehicleResolver(IVehicleRepository repository): IVehicleResolver
{
    private readonly List<Vehicle> _knownVehicles = [];

    internal VehicleResolver(IVehicleRepository repository, List<Vehicle> knownVehicles) : this(repository)
    {
        _knownVehicles = knownVehicles;
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<Vehicle> AllKnownCoachLayouts => _knownVehicles.AsReadOnly();
    
    /// <inheritdoc/>
    public async Task<List<Vehicle>> ResolveManyAsync(List<EuropeanVehicleNumber> evns)
    {
        evns = evns.Distinct().ToList();
        var fromKnown = _knownVehicles
            .Where(cl => evns.Contains(cl.Evn))
            .ToList();

        var stillMissing = evns
            .Except(fromKnown.Select(c => c.Evn));
        
        var fromRepository = await repository.GetManyAsync(stillMissing);
        _knownVehicles.AddRange(fromRepository);

        var existing = fromKnown.Concat(fromRepository).ToList();
        
        var missingVehicleEvns = evns
            .Where(evn => existing.All(e => e.Evn != evn))
            .ToList();
        
        foreach (var missingVehicleEvn in missingVehicleEvns)
        {
            var createdVehicleResult = Vehicle.Create(missingVehicleEvn);
            if (createdVehicleResult.HasFailed)
            {
                throw new InvalidOperationException($"Could not create vehicle {missingVehicleEvn}");
            }
            var createdVehicle = createdVehicleResult.Value;
            repository.Store(createdVehicle);
            _knownVehicles.Add(createdVehicle);
            existing.Add(createdVehicle);
        }
        
        return existing.ToList();
    }
}