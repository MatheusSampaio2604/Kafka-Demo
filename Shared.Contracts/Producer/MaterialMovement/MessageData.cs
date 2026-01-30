namespace Shared.Contracts.Producer.MaterialMovement;

/// <summary>
/// Message Content.
/// </summary>
public class MessageData
{
    /// <summary>
    /// Actual Movement Time (date/time + timezone).
    /// Date of Machine Movement.
    /// </summary>
    public required DateTime MovementTime { get; set; }

    /// <summary>
    /// Identification of the yard/warehouse where the operation was performed.
    /// Yard Location | Example: "EXT-BL1".
    /// </summary>
    public required string Yard { get; set; }

    /// <summary>
    /// Name of the location/equipment/vehicle where the operation was performed.
    /// Zone Location | Example: "A01".
    /// </summary>
    public required string Location { get; set; }

    /// <summary>
    /// X Coordinate of the location in relation to the yard/warehouse area.
    /// Example: 10000.
    /// </summary>
    public required int X { get; set; }

    /// <summary>
    /// Y Coordinate of the location in relation to the yard/warehouse area.
    /// Example: 10000.
    /// </summary>
    public required int Y { get; set; }

    /// <summary>
    /// Identification of the equipment (crane/forklift) that performed the movement.
    /// Address of the Equipment | Example: "Empilhadeira01".
    /// </summary>
    public required string Equipment { get; set; }

    /// <summary>
    /// Login of the operator responsible for the movement.
    /// Example: "Pedro".
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// List of Materials moved.
    /// </summary>
    public required IEnumerable<Material> Materials { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of warning messages associated with the current materials movement.
    /// </summary>
    /// <remarks>The collection may be empty if no warnings are present. Each warning provides
    /// additional context or non-critical issues that occurred during processing.</remarks>
    public IEnumerable<string> Warnings { get; set; } = [];


}
