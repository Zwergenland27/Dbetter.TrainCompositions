using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

public record EuropeanVehicleNumber
{
    public string Digits { get; private init; }

    public int VehicleTypeCode => int.Parse(Digits[..2]);

    public int CountryCode => int.Parse(Digits[2..4]);
    
    public int SeriesNumber => int.Parse(Digits[4..8]);
    
    public int SerialNumber => int.Parse(Digits[8..11]);
    
    public int CheckDigit => int.Parse(Digits[11..]);

    public bool IsLocomotive => EvnVehicleTypeCodes.Locomotives.Contains(VehicleTypeCode);

    public bool IsMultipleUnit => EvnVehicleTypeCodes.MultipleUnits.Contains(VehicleTypeCode);

    public bool IsPassengerCar => EvnVehicleTypeCodes.PassengerCars.Contains(VehicleTypeCode);

    internal EuropeanVehicleNumber(string digits)
    {
        Digits = digits;
    }

    /// <summary>
    /// Try to parse the <paramref name="value"/> as an evn
    /// </summary>
    /// <exception cref="VehicleErrors.Evn.Empty">The evn is an empty string</exception>
    /// <exception cref="VehicleErrors.Evn.InvalidLength">The length of the digit only evn does not match the specification of 12</exception>
    /// <exception cref="VehicleErrors.Evn.InvalidCheckDigit">The check digit of the provided evn does not match the calculated one</exception>
    public static CanFail<EuropeanVehicleNumber> Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return VehicleErrors.Evn.Empty;
        }
        
        var digitsOnly = new string(value.Where(char.IsDigit).ToArray());
        if (digitsOnly.Length != 12)
        {
            return VehicleErrors.Evn.InvalidLength;
        }
        
        var computed = ComputeCheckDigit(digitsOnly[..11]);
        var provided  = int.Parse(digitsOnly[11..]);

        if (computed != provided)
        {
            return VehicleErrors.Evn.InvalidCheckDigit;
        }

        return new EuropeanVehicleNumber(digitsOnly);
    }

    private static int ComputeCheckDigit(string elevenDigits)
    {
        var sum = 0;

        for (var i = 0; i < elevenDigits.Length; i++)
        {
            var digit   = elevenDigits[i] - '0';
            var product = digit * (i % 2 == 0 ? 2 : 1);
            sum += product / 10 + product % 10;
        }

        return (10 - sum % 10) % 10;
    }
    
    public string ToUicFormat()
        => $"{Digits[..2]} {Digits[2..4]} {Digits[4..11]}-{Digits[11]}";

    public override string ToString() => ToUicFormat();
}