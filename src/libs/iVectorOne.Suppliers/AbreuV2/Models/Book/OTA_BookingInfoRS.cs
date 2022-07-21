namespace iVectorOne.Suppliers.AbreuV2.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OTA_BookingInfoRS
    {
        public ResGlobalInfo ResGlobalInfo { get; set; } = new();

        [XmlArray("HotelResList")]
        [XmlArrayItem("HotelRes")]
        public List<HotelReserv> HotelResList { get; set; } = new();

        [XmlElement("Success")]
        public string Success { get; set; } = string.Empty;


        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public List<Error> Errors { get; set; } = new();
    }
}
