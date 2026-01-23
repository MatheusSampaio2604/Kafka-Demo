using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts
{
    /// <summary>
    /// Content Material.
    /// </summary>
    public class Material
    {
        /// <summary>
        /// Part Identification.
        /// Example: "0000000000-111".
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// The direction in which the pile | Example: "N", "S", "L", "O" | Norte, Sul, Leste, Oeste.
        /// If it doesn't exist, it's null.
        /// </summary>
        public char? SubLocation { get; set; }

        /// <summary>
        /// Sequential position of the part vertically.
        /// If applicable (1,2,3, ...).        
        /// If it doesn't exist = Default: 1.
        /// </summary>
        public int LevelIndex { get; set; } = 1;

        /// <summary>
        /// Sequential position of the part horizontally.
        /// if applicable (1, 2, 3, ...)
        /// If it doesn't exist = Default: 1
        /// </summary>
        public int PositionIndex { get; set; } = 1;
    }
}
