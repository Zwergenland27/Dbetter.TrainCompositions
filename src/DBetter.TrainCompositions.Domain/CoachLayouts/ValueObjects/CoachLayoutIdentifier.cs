namespace DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

/// <summary>
/// Layout identifier used on bahn.de
/// </summary>
/// <remarks>
/// Consists of I and parts of the evn number
/// </remarks>
/// <example>I4010</example>
public record CoachLayoutIdentifier(string Value);