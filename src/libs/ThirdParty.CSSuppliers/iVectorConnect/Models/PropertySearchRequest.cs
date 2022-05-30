namespace ThirdParty.CSSuppliers.iVectorConnect.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("PropertySearchRequest")]
    public class PropertySearchRequest
    {
        public PropertySearchRequest(){}

        public PropertySearchRequest(LoginDetails loginDetails)
        {
            this.LoginDetails = loginDetails;
        }

        public LoginDetails LoginDetails { get; set; } = new();

        public int PropertyReferenceID { get; set; }

        [XmlArray("PropertyReferenceIDs")]
        [XmlArrayItem("PropertyReferenceID")]
        public int[] PropertyReferenceIDs { get; set; }

        [XmlArray("Resorts")]
        [XmlArrayItem("ResortID")]
        public int[] Resorts { get; set; }

        public string ArrivalDate { get; set; } = string.Empty;

        public int Duration { get; set; }

        [XmlArray("RoomRequests")]
        [XmlArrayItem("RoomRequest")]
        public List<RoomRequest> RoomRequests { get; set; } = new();

        public string RegionID { get; set; } = string.Empty;

        public bool ShouldSerializeRegionID() => !string.IsNullOrEmpty(RegionID);

        public int MealBasisID { get; set; }

        public string MinStarRating { get; set; } = string.Empty;

        public bool ShouldSerializeMinStarRating() => !string.IsNullOrEmpty(MinStarRating);

        [XmlArray("ProductAttributes")]
        [XmlArrayItem("ProductAttributeID")]
        public int[] ProductAttributes { get; set; }
    }
}
