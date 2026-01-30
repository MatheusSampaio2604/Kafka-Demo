namespace Shared.Contracts.Consumer.MaterialLocation;

/// <summary>
/// Content Message.
/// </summary>
public class MessageData
{
    /// <summary>
    /// List of Locations.
    /// </summary>
    public IEnumerable<Location>? Locations { get; set; }
}
