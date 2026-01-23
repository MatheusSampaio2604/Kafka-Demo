using System.Xml.Linq;

namespace Shared.Contracts.Consumer.MaterialLocation
{
    /// <summary>
    /// Content Location.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Identification of the yard/warehouse where the materials are located.
        /// </summary>
        public required string Yard { get; set; }

        /// <summary>
        /// Name of the location where the materials are located.
        /// </summary>
        public required string LocationName { get; set; }
        
        /// <summary>
        /// Type of location.Ex: "Stock", "Truck", "Trailer", "Railcar".
        /// </summary>
        public required string LocationType { get; set; }

        /// <summary>
        /// X coordinate of the location in relation to the yard/warehouse area.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y coordinate of the location in relation to the yard/warehouse area.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// List of materials located at the location.
        /// </summary>
        public IEnumerable<Material> Materials { get; set; }
    }
}
