using DBetter.TrainCompositions.Domain.Vehicles;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Tests;

public class PassengerCarTests
{
    [Fact]
    public void Create_ShouldReturnError_WhenEvnIsNoPassengerCar()
    {
        var evn = EuropeanVehicleNumber.Parse("91 80 6143 958-7").Value;
        
        var passengerCarResult = PassengerCar.Create(evn);
        
        Assert.True(passengerCarResult.HasFailed);
        Assert.Single(passengerCarResult.Errors);
        Assert.Equal(VehicleErrors.PassengerCar.InvalidEvn, passengerCarResult.Errors.First());
    }
    
    [Theory]
    [InlineData("61 80 8091 156-2", false)]
    [InlineData("50 80 2681 207-5", true)]
    public void Create_ShouldSetCorrectDoubleDeckerFlag(string rawEvn, bool isDoubleDecker)
    {
        var evn = EuropeanVehicleNumber.Parse(rawEvn).Value;
        
        var passengerCar = PassengerCar.Create(evn).Value;
        
        Assert.Equal(isDoubleDecker, passengerCar.IsDoubleDecker);
    }
    
    [Theory]
    [InlineData("50 80 2681 207-5", 160)]
    public void Create_ShouldSetCorrectMaxSpeed(string rawEvn, float maxSpeed)
    {
        var evn = EuropeanVehicleNumber.Parse(rawEvn).Value;
        
        var passengerCar = PassengerCar.Create(evn).Value;
        
        Assert.Equal(maxSpeed, passengerCar.MaxSpeed);
    }
    
    [Theory]
    [InlineData("61 80 8091 156-2")]
    public void Create_ShouldNotSetMaxSpeedOver160KmH(string rawEvn)
    {
        var evn = EuropeanVehicleNumber.Parse(rawEvn).Value;
        
        var passengerCar = PassengerCar.Create(evn).Value;
        
        Assert.Null(passengerCar.MaxSpeed);
    }
}