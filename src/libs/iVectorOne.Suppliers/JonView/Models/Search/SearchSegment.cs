namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    public class SearchSegment 
    {
        [XmlElement("prodtypecode")]
        public string ProdTypeCode { get; set; } = string.Empty;

        [XmlElement("searchtype")]
        public string SearchType { get; set; } = string.Empty;

        [XmlElement("citycode")]
        public string CityCode { get; set; } = string.Empty;

        [XmlElement("startdate")]
        public string StartDate { get; set; } = string.Empty;
        
        [XmlElement("duration")]
        public int Duration { get; set; }

        [XmlElement("status")]
        public string Status { get; set; } = string.Empty;

        [XmlElement("displayname")]
        public string DisplayName { get; set; } = string.Empty;

        [XmlElement("displaynamedetails")]
        public string DisplayNameDetails { get; set; } = string.Empty;

        [XmlElement("displayroomconf")]
        public string DisplayRoomConf { get; set; } = string.Empty;

        [XmlElement("displayprice")]
        public string DisplayPrice { get; set; } = string.Empty;

        [XmlElement("displaysuppliercd")]
        public string DisplaySupplierCd { get; set; } = string.Empty;

        [XmlElement("displayavail")]
        public string DisplayAvail { get; set; } = string.Empty;

        [XmlElement("displaypolicy")]
        public string DisplayPolicy { get; set; } = string.Empty;

        [XmlElement("displayrestriction")]
        public string DisplayRestriction { get; set; } = string.Empty;

        [XmlElement("displaydynamicrates")]
        public string DisplayDynamicRates { get; set; } = string.Empty;

        [XmlElement("adults")]
        public int Adults { get; set; }

        [XmlElement("children")]
        public int Children { get; set; }

        [XmlElement("childrenage")]
        public string ChildrenAge { get; set; } = string.Empty;

        [XmlElement("rooms")]
        public int Rooms { get; set; }
    }
}
