namespace iVectorOne.Suppliers.TeamAmerica.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PriceSearch : SoapContent
    {
        [XmlElement("UserName")]
        public string UserName { get; set; } = string.Empty;

        [XmlElement("Password")]
        public string Password { get; set; } = string.Empty;

        [XmlElement("CityCode")]
        public string CityCode { get; set; } = string.Empty;

        [XmlElement("ProductCode")]
        public string ProductCode { get; set; } = string.Empty;

        [XmlElement("Type")]
        public string RequestType { get; set; } = Constant.SearchTypeHotel;

        [XmlElement("Occupancy")]
        public string Occupancy { get; set; } = string.Empty;

        [XmlElement("ArrivalDate")]
        public string ArrivalDate { get; set; } = string.Empty;

        [XmlElement("NumberOfNights")]
        public int NumberOfNights { get; set; }

        [XmlElement("NumberOfRooms")]
        public int NumberOfRooms { get; set; }

        [XmlElement("DisplayClosedOut")]
        public string DisplayClosedOut { get; set; } = Constant.TokenNo;

        [XmlElement("DisplayOnRequest")]
        public string DisplayOnRequest { get; set; } = Constant.TokenNo;

        [XmlArray("VendorIDs")]
        [XmlArrayItem("VendorID")]
        public List<string> VendorIds { get; set; } = new();
    }
}
