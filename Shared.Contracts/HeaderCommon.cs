namespace Shared.Contracts;

/// <summary>
/// Message Header Information.
/// </summary>
public class HeaderCommon
{
    /// <summary>
    /// Message Name. 
    /// "MaterialMovement";
    /// "MatLocationSnapshotReq";
    /// 
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Sender Name. Ex: "Rodeo".
    /// Default setted: Rodeo.
    /// </summary>
    public string Sender { get; set; } = "Rodeo";

    /// <summary>
    /// Message Generation Time (date/time + timezone).
    /// </summary>
    public string Timestamp { get; set; } = DateTime.Now.ToString("o");

    /// <summary>
    /// Production Area Identification. Fixed: "PD-BL".
    /// </summary>
    public required string Tenant { get; set; }

    //Version ?
    //public required string Version { get; set; }
}
