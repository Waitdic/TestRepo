namespace iVectorOne.SDK.V2.PropertySearch
{
    using System.Collections.Generic;

    /// <summary>
    /// The room request
    /// </summary>
    public class RoomRequest
    {
        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets the child ages.
        /// </summary>
        public List<int> ChildAges { get; set; } = new List<int>();
    }
}
