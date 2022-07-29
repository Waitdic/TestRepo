namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class PackageReservation
    {
        [XmlArray("ItineraryItems")]
        [XmlArrayItem("ItineraryItem")]
        public ItineraryItem[] ItineraryItems { get; set; } = Array.Empty<ItineraryItem>();
    }
}
