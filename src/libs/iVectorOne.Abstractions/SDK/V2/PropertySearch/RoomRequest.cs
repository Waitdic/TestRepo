namespace iVectorOne.SDK.V2.PropertySearch
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class RoomRequest
    {
        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        /// <value>
        /// The adults.
        /// </value>
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        /// <value>
        /// The infants.
        /// </value>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets the child ages.
        /// </summary>
        /// <value>
        /// The child ages.
        /// </value>
        public List<int> ChildAges { get; set; } = new List<int>();
    }
}
