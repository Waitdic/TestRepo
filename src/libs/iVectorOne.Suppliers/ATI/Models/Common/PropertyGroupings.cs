namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class PropertyGroupings
    {
        [XmlElement("Property")]
        public Property[] Properties { get; set; } = Array.Empty<Property>();
    }
}
