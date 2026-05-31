using DBetter.TrainCompositions.Domain.Vehicles;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Tests;

public class LocomotiveTests
{
    [Fact]
    public void Create_ShouldReturnError_WhenEvnIsNoLocomotive()
    {
        var evn = EuropeanVehicleNumber.Parse("50 80 2681 207-5").Value;
        
        var locomotiveResult = Locomotive.Create(evn);
        
        Assert.True(locomotiveResult.HasFailed);
        Assert.Single(locomotiveResult.Errors);
        Assert.Equal(VehicleErrors.Locomotive.InvalidEvn, locomotiveResult.Errors.First());
    }

    [Theory]
    [InlineData("91 80 6143 958-7", PropulsionType.Electric)]
    [InlineData("92 80 1246 011-1", PropulsionType.Diesel)]
    [InlineData("90 80 2248 045-7", PropulsionType.Hybrid)]
    public void Create_ShouldSetCorrectCompulsionType(string rawEvn, PropulsionType propulsionType)
    {
        var evn = EuropeanVehicleNumber.Parse(rawEvn).Value;
        
        var locomotive = Locomotive.Create(evn).Value;
        
        Assert.Equal(propulsionType, locomotive.PropulsionType);
    }

    [Fact]
    public void Create_ShouldNotSetSpeed()
    {
        var evn = EuropeanVehicleNumber.Parse("90 80 2248 045-7").Value;
        
        var locomotive = Locomotive.Create(evn).Value;
        Assert.Null(locomotive.MaxSpeed);
    }
}