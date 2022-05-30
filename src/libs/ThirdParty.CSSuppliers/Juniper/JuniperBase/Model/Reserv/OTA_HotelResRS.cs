namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OTA_HotelResRS
    {
        [XmlElement("Success")]
        public string sSuccess { get; set; } = Constant.NotEmptyStringToken;

        public bool Success { get => string.IsNullOrEmpty(sSuccess); }

        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public List<BookHotelReservation> HotelReservations { get; set; } = new();

        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public List<ErrorType> Errors { get; set; } = new();
    }

    public class ErrorType
    {
        [XmlAttribute("ShortText")]
        public string ShortText { get; set; } = string.Empty;
    }

    public class BookHotelReservation
    {
        [XmlAttribute("ResStatus")]
        public string ResStatus { get; set; } = string.Empty;

        [XmlElement("UniqueID")]
        public UniqueId UniqueId { get; set; } = new();
    }
}
