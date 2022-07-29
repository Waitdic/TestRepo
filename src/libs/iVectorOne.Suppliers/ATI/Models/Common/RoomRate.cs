namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomRate
    {
        [XmlArray("Rates")]
        [XmlArrayItem("Rate")]
        public Rate[] Rates { get; set; } = Array.Empty<Rate>();

        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;
    }
}
