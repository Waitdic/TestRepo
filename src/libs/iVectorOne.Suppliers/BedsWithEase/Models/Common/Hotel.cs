namespace iVectorOne.CSSuppliers.BedsWithEase.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Hotel
    {
        [XmlArray("RoomStays")]
        [XmlArrayItem("RoomStay")]
        public List<RoomStay> RoomStays { get; set; } = new();

        public HotelInfo HotelInfo { get; set; } = new();

        public string OperatorCode { get; set; } = string.Empty;

        public string OriginalOperatorCode { get; set; } = string.Empty;


        public string SupplierPriorityCodeDescription { get; set; } = string.Empty;


    }
}
