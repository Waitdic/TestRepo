namespace iVectorOne.Suppliers.BedsWithEase.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class HotelSearchCriterion
    {
        [XmlArray("Locations")]
        [XmlArrayItem("Location")]
        public Location[] Locations { get; set; } = Array.Empty<Location>();
    }
}
