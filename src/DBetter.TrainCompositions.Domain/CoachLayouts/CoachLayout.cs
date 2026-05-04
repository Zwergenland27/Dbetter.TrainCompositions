using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.CoachLayouts;

/// <summary>
/// Information about one coach
/// </summary>
public class CoachLayout: AggregateRoot<CoachLayoutId>
{
    /// <summary>
    /// Unique, human-readable identifier of the coach
    /// </summary>
    /// <remarks>
    /// Consists of I and parts of the evn number
    /// </remarks>
    /// <example>I4010</example>
    public CoachLayoutIdentifier Identifier { get; private set; }
    
    /// <summary>
    /// Standardized construction type
    /// </summary>
    /// <example>Bmp</example>
    public ConstructionType? ConstructionType { get; private set; }
    
    /// <summary>
    /// Information about amenities of the coach
    /// </summary>
    public Amenities? Amenities { get; private set; }
    
    internal CoachLayout(
        CoachLayoutId id,
        CoachLayoutIdentifier identifier,
        ConstructionType? constructionType,
        Amenities? amenities) : base(id)
    {
        Identifier = identifier;
        ConstructionType = constructionType;
        Amenities = amenities;
    }

    /// <summary>
    /// Create a new coach layout based on a planned coach sequence
    /// </summary>
    /// <param name="identifier">The unique identifier of the coach</param>
    internal static CoachLayout CreateFromPlanned(
        CoachLayoutIdentifier identifier
    )
    {
        return new CoachLayout(CoachLayoutId.CreateNew(), identifier, null, null);
    }

    /// <summary>
    /// Updates the construction type of the coach
    /// </summary>
    /// <param name="constructionType">The new construction type</param>
    public void UpdateConstructionType(ConstructionType constructionType)
    {
        ConstructionType = constructionType;
    }

    /// <summary>
    /// Updates the amenities of the coach
    /// </summary>
    /// <param name="amenities">The new amenities</param>
    public void UpdateAmenities(Amenities amenities)
    {
        Amenities = amenities;
    }
}