namespace ThirdParty.CSSuppliers.Serhs.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("response")]
    public class SerhsAvailabilityResponse
    {
        [XmlElement("period")]
        public Period Period = new();

        [XmlArray("accommodations")]
        [XmlArrayItem("accommodation")]
        public List<Accommodation> Accommodations { get; set; } = new();
    }
}
