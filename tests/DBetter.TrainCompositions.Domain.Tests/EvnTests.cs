using DBetter.TrainCompositions.Domain.Vehicles;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Tests;

public class EvnTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_ShouldReturnError_WhenEvnEmpty(string rawEvn)
    {
        var evnResult = EuropeanVehicleNumber.Parse(rawEvn);
        Assert.True(evnResult.HasFailed);
        Assert.Single(evnResult.Errors);
        Assert.Contains(VehicleErrors.Evn.Empty, evnResult.Errors);
    }
    
    [Theory]
    [InlineData("91 80 6143 958-72")]
    [InlineData("9180614395872")]
    [InlineData("91806143958")]
    public void Parse_ShouldReturnError_WhenEvnInvalidLength(string rawEvn)
    {
        var evnResult = EuropeanVehicleNumber.Parse(rawEvn);
        Assert.True(evnResult.HasFailed);
        Assert.Single(evnResult.Errors);
        Assert.Contains(VehicleErrors.Evn.InvalidLength, evnResult.Errors);
    }
    
    [Theory]
    [InlineData("91 80 6143 958-8")]
    public void Parse_ShouldReturnError_WhenEvnInvalidCheckDigit(string rawEvn)
    {
        var evnResult = EuropeanVehicleNumber.Parse(rawEvn);
        Assert.True(evnResult.HasFailed);
        Assert.Single(evnResult.Errors);
        Assert.Contains(VehicleErrors.Evn.InvalidCheckDigit, evnResult.Errors);
    }
    
    [Theory]
    [InlineData("91 80 6143 958-7", "918061439587")]
    public void Parse_ShouldReturnObject_WhenEvnValid(string rawEvn, string digitsOnly)
    {
        var evnResult = EuropeanVehicleNumber.Parse(rawEvn);
        Assert.False(evnResult.HasFailed);
        Assert.Equal(digitsOnly, evnResult.Value.Digits);
    }
    
    [Fact]
    public void VehicleTypeCode_ShouldBeExtractedCorrectly()
    {
        var evn = EuropeanVehicleNumber.Parse("91 80 6143 958-7").Value;
        Assert.Equal(91, evn.VehicleTypeCode);
    }
    
    [Fact]
    public void CountryCode_ShouldBeExtractedCorrectly()
    {
        var evn = EuropeanVehicleNumber.Parse("91 80 6143 958-7").Value;
        Assert.Equal(80, evn.CountryCode);
    }
    
    [Fact]
    public void SeriesNumber_ShouldBeExtractedCorrectly()
    {
        var evn = EuropeanVehicleNumber.Parse("91 80 6143 958-7").Value;
        Assert.Equal(6143, evn.SeriesNumber);
    }
    
    [Fact]
    public void SerialNumber_ShouldBeExtractedCorrectly()
    {
        var evn = EuropeanVehicleNumber.Parse("91 80 6143 958-7").Value;
        Assert.Equal(958, evn.SerialNumber);
    }
    
    [Fact]
    public void CheckDigit_ShouldBeExtractedCorrectly()
    {
        var evn = EuropeanVehicleNumber.Parse("91 80 6143 958-7").Value;
        Assert.Equal(7, evn.CheckDigit);
    }

    [Theory]
    [InlineData("91 80 6143 958-7", true)]
    [InlineData("50 80 2681 207-5", false)]
    [InlineData("95 80 0642 582-0", false)]
    public void IsLocomotive_ShouldBeExtractedCorrectly(string rawEvn, bool isLocomotive)
    {
        var evn = EuropeanVehicleNumber.Parse(rawEvn).Value;
        Assert.Equal(isLocomotive, evn.IsLocomotive);
    }
    
    [Theory]
    [InlineData("91 80 6143 958-7", false)]
    [InlineData("50 80 2681 207-5", false)]
    [InlineData("95 80 0642 582-0", true)]
    public void IsMultipleUnit_ShouldBeExtractedCorrectly(string rawEvn, bool isMultipleUnit)
    {
        var evn = EuropeanVehicleNumber.Parse(rawEvn).Value;
        Assert.Equal(isMultipleUnit, evn.IsMultipleUnit);
    }
    
    [Theory]
    [InlineData("91 80 6143 958-7", false)]
    [InlineData("50 80 2681 207-5", true)]
    [InlineData("95 80 0642 582-0", false)]
    public void IsPassengerCar_ShouldBeExtractedCorrectly(string rawEvn, bool isPassengerCar)
    {
        var evn = EuropeanVehicleNumber.Parse(rawEvn).Value;
        Assert.Equal(isPassengerCar, evn.IsPassengerCar);
    }
}