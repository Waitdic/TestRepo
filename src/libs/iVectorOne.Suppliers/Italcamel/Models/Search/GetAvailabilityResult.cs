namespace iVectorOne.Suppliers.Italcamel.Models.Search
{
    using System;
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Common;

    public class GetAvailabilityResult
    {
        [XmlArray("ACCOMMODATIONS")]
        [XmlArrayItem("ACCOMMODATION")]
        public Accommodation[] Accommodations { get; set; } = Array.Empty<Accommodation>();
    }
}
