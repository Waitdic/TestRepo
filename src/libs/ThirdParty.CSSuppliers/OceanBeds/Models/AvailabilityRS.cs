namespace ThirdParty.CSSuppliers.OceanBeds.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("PropertyAvailabilityRS", Namespace = "http://oceanbeds.com/2014/10")]
    public class AvailabilityRS
    {
        [XmlArray("Response")]
        [XmlArrayItem("Property")]
        public Property[] Response { get; set; } = Array.Empty<Property>();

        public Status Status { get; set; } = new();
    }
}
