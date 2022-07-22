namespace iVectorOne.CSSuppliers.iVectorConnect.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("PropertySearchRequest")]
    public class PropertySearchRequest
    {
        public LoginDetails LoginDetails { get; set; } = new();

        [XmlArray("PropertyReferenceIDs")]
        [XmlArrayItem("PropertyReferenceID")]
        public List<int> PropertyReferenceIDs { get; set; }

        public string ArrivalDate { get; set; } = string.Empty;

        public int Duration { get; set; }

        [XmlArray("RoomRequests")]
        [XmlArrayItem("RoomRequest")]
        public List<RoomRequest> RoomRequests { get; set; } = new();

        public int MealBasisID { get; set; }

        public string MinStarRating { get; set; } = string.Empty;

        public bool ShouldSerializeMinStarRating() => !string.IsNullOrEmpty(MinStarRating);

        [XmlArray("ProductAttributes")]
        [XmlArrayItem("ProductAttributeID")]
        public List<int> ProductAttributes { get; set; }
    }
}