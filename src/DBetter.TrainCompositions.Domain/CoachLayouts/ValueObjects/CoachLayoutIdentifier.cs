using System.Text.RegularExpressions;

namespace DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

/// <summary>
/// Layout identifier used on bahn.de
/// </summary>
/// <remarks>
/// Consists of I and parts of the evn number
/// </remarks>
/// <example>I4010</example>
public record CoachLayoutIdentifier
{
    public string Value { get; private set; }

    internal CoachLayoutIdentifier(string value)
    {
        Value = value;
    }
    public static CoachLayoutIdentifier Create(string value)
    {
        return new CoachLayoutIdentifier(Regex.Replace(value, @"-\d+$", ""));
    }
}