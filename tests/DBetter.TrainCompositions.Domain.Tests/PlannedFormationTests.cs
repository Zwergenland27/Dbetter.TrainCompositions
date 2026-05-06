using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Tests;

public class PlannedFormationTests
{
    [Fact]
    public void Matches_ShouldReturnTrue_WhenCoachLayoutIdsAreIdenticalAndInCorrectOrder()
    {
        var layoutId1 = CoachLayoutId.CreateNew();
        var layoutId2 = CoachLayoutId.CreateNew();

        var formation = new PlannedFormation(
            PlannedFormationId.CreateNew(),
        [
            new PlannedCoach(new PlannedCoachPosition(1), layoutId1),
            new PlannedCoach(new PlannedCoachPosition(2), layoutId2),
        ]);

        var snapshot = new PlannedFormationSnapshot([layoutId1, layoutId2]);

        var match = formation.Matches(snapshot);
        
        Assert.True(match);
    }

    [Fact]
    public void Matches_ShouldReturnFalse_WhenCoachLayoutIdsAreIdenticalAndInWrongOrder()
    {
        var layoutId1 = CoachLayoutId.CreateNew();
        var layoutId2 = CoachLayoutId.CreateNew();

        var formation = new PlannedFormation(
            PlannedFormationId.CreateNew(),
        [
            new PlannedCoach(new PlannedCoachPosition(1), layoutId1),
            new PlannedCoach(new PlannedCoachPosition(2), layoutId2),
        ]);

        var snapshot = new PlannedFormationSnapshot([layoutId2, layoutId1]);

        var match = formation.Matches(snapshot);
        
        Assert.False(match);
    }

    [Fact]
    public void Matches_ShouldReturnFalse_WhenCoachLayoutIdsDiffer()
    {
        var layoutId1 = CoachLayoutId.CreateNew();
        var layoutId2 = CoachLayoutId.CreateNew();
        var layoutId3 = CoachLayoutId.CreateNew();

        var formation = new PlannedFormation(
            PlannedFormationId.CreateNew(),
        [
            new PlannedCoach(new PlannedCoachPosition(1), layoutId1),
            new PlannedCoach(new PlannedCoachPosition(2), layoutId2),
        ]);

        var snapshot = new PlannedFormationSnapshot([layoutId1, layoutId3]);

        var match = formation.Matches(snapshot);
        
        Assert.False(match);
    }
}