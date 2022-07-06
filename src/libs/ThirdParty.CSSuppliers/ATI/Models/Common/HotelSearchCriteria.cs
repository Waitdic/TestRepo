namespace ThirdParty.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class HotelSearchCriteria
    {
        [XmlElement("Criterion")]
        public Criterion[] Criterion { get; set; } = Array.Empty<Criterion>();
    }
}
