using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Producer.MaterialLocation
{
    /// <summary>
    /// Content Message.
    /// </summary>
    public class MessageData
    {
        /// <summary>
        /// Identification of the yard/warehouse where the operation was performed.
        /// Yard Location | Example: "EXT-BL1".
        /// </summary>
        public required string Yard { get; set; }

        /// <summary>
        /// Identifying the specific location to be synchronized (optional).
        /// Example: "A01" or null 
        /// </summary>
        public string? Location { get; set; }
    }
}
