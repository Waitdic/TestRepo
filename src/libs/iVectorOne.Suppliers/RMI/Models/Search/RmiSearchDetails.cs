namespace iVectorOne.Suppliers.RMI.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    public class RmiSearchDetails
    {
        public string ArrivalDate { get; set; } = string.Empty;
        public int Duration { get; set; }

        [XmlElement("PropertyID")]
        public string PropertyID { get; set; } = string.Empty;
        public bool ShouldSerializePropertyID() => !string.IsNullOrEmpty(PropertyID);

        [XmlArray("Properties")]
        [XmlArrayItem("PropertyID")]
        public List<string> Properties { get; set; } = new();
        public bool ShouldSerializeProperties() => Properties?.Any() ?? false;

        [XmlElement("MealBasisID")]
        public string MealBasisId { get; set; } = string.Empty;

        public int MinStarRating { get; set; }
        public int MinimumPrice { get; set; }
        public int MaximumPrice { get; set; }

        [XmlArray("RoomRequests")]
        [XmlArrayItem("RoomRequest")]
        public List<RoomRequest> RoomRequests { get; set; } = new();
    }
}
