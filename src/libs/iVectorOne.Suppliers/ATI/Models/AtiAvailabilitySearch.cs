namespace iVectorOne.Suppliers.ATI.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    //[XmlType(Namespace = SoapNamespaces.Ns)]
    public class AtiAvailabilitySearch : SoapContent
    {
        [XmlArray("RoomStays")]
        [XmlArrayItem("RoomStay")]
        public RoomStay[] RoomStays { get; set; } = Array.Empty<RoomStay>();
    }
}