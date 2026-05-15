using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Tests;

public class CoachLayoutIdentifierTests
{
    [Theory]
    [InlineData("I8080-402-12", "I8080-402")]
    [InlineData("I7412-412.(7-TLG.)-28", "I7412-412.(7-TLG.)")]
    public void Create_ShouldReplaceTrailingCoachNumber(string rawValue, string expectedValue)
    {
        var result = CoachLayoutIdentifier.Create(rawValue);
        Assert.Equal(expectedValue, result.Value);
    }
}