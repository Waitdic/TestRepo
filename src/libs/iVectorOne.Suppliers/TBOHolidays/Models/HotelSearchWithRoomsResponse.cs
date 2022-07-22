
namespace iVectorOne.CSSuppliers.TBOHolidays.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class HotelSearchWithRoomsResponse : SoapContent
    {
        [XmlArray("HotelResultList")]
        [XmlArrayItem("HotelResult")]
        public HotelResult[] HotelResultList { get; set; } = Array.Empty<HotelResult>();

        public string SessionId { get; set; } = string.Empty;
    }
}
