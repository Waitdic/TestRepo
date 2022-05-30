using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Model.JuniperBase
{

    public class OTA_HotelAvailServiceResponse
    {
        public OTA_HotelAvailServiceResponse() { }

        [XmlElement("OTA_HotelAvailRS")]
        public List<HotelAvailResponse> HotelAvailResponse { get; set; } = new();
    }


    public class HotelAvailResponse
    {
        [XmlElement("Success")]
        public string sSuccess { get; set; } = Constant.NotEmptyStringToken;

        public bool Success
        {
            get => string.IsNullOrEmpty(sSuccess);
        }

        [XmlAttribute("SequenceNmbr")]
        public string SequenceNmbr { get; set; }

        [XmlArray("RoomStays")]
        [XmlArrayItem("RoomStay")]
        public List<RoomStay> RoomStays { get; set; } = new();
    }
}
