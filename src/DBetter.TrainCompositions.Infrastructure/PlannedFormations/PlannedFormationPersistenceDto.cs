using System.Security.Cryptography;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.PostgreSQL;

namespace DBetter.TrainCompositions.Infrastructure.PlannedFormations;

public class PlannedFormationPersistenceDto: IPersistenceDto<PlannedFormation, PlannedFormationPersistenceDto>
{
    public required Guid Id { get; init; }
    
    public required List<PlannedCoachPersistenceDto> CoachSequence { get; init; }
    
    public required string CoachSequenceHash { get; init; }
    
    public required string ReverseCoachSequenceHash { get; init; }
    
    public static PlannedFormationPersistenceDto FromDomain(PlannedFormation domain)
    {
        return new PlannedFormationPersistenceDto
        {
            Id = domain.Id.Value,
            CoachSequence = domain.CoachSequence.Select(PlannedCoachPersistenceDto.FromDomain).ToList(),
            CoachSequenceHash = ComputeCoachSequenceHash(domain.CoachSequence.Select(c => c.LayoutId)),
            ReverseCoachSequenceHash = ComputeCoachSequenceHash(domain.CoachSequence.Reverse().Select(c => c.LayoutId)),
        };

    }

    public PlannedFormation ToDomain()
    {
        return new PlannedFormation(
            new PlannedFormationId(Id),
            CoachSequence.Select(coach => coach.ToDomain()).ToList());
    }

    public void Apply(PlannedFormation domain)
    {
    }

    public static string ComputeCoachSequenceHash(IEnumerable<CoachLayoutId> coachLayoutIds)
    {
        var bytes = coachLayoutIds.SelectMany(id => id.Value.ToByteArray()).ToArray();
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hash[..16]);
    }
}