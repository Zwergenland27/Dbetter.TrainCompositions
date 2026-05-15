namespace DBetter.TrainCompositions.Domain.PlannedFormations;

public class PlannedFormationSnapshotComparer : IEqualityComparer<PlannedFormationSnapshot>
{
    public bool Equals(PlannedFormationSnapshot? x, PlannedFormationSnapshot? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Coaches.SequenceEqual(y.Coaches);
    }

    public int GetHashCode(PlannedFormationSnapshot obj)
        => obj.Coaches.Aggregate(
            new HashCode(),
            (hash, id) => { hash.Add(id); return hash; },
            hash => hash.ToHashCode());
}