using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Producer.VehicleDetection
{
    /// <summary>
    /// Message Content
    /// </summary>
    public class MessageData
    {
        /// <summary>
        /// Message date in ISO format with offset. (must match the Timestamp in the Header).
        /// </summary>
        public required DateTime EventTime { get; set; }

        /// <summary>
        /// Yard/Area name.
        /// </summary>
        public required string Site { get; set; }

        /// <summary>
        /// Equipment name.
        /// </summary>
        public required string Equipment { get; set; }

        /// <summary>
        /// Device name.
        /// </summary>
        public required string Device { get; set; }

        /// <summary>
        /// License plate code that was detected.
        /// </summary>
        public required string Plate { get; set; }

        /// <summary>
        /// Vehicle type: "Railcar", "Truck", "Trailer" or other...
        /// </summary>
        public required string VehicleType { get; set; }

        /// <summary>
        /// Direction of movement: "Inward" or "Outward".
        /// </summary>
        public required string Orientation { get; set; }

        /// <summary>
        /// Indicates whether the reading was complete/valid ("true" or "false", whether or not it has "*")
        /// Default true
        /// </summary>
        public bool IsValid { get; set; } = true;
    }
}
