using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.PlannedFormations;

/// <summary>
/// Snapshot of a planned formation containing only the coach layouts
/// </summary>
/// <param name="Coaches">Ids of the coaches of the planned formation</param>
public record PlannedFormationSnapshot(List<CoachLayoutId> Coaches);