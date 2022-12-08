namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Extra
    {
        [XmlArray("DISCOUNTS")]
        [XmlArrayItem("DISCOUNT")]
        public Discount[] Discount { get; set; } = Array.Empty<Discount>();

        [XmlArray("SUPPLEMENTS")]
        [XmlArrayItem("SUPPLEMENT")]
        public Supplement[] Supplements { get; set; } = Array.Empty<Supplement>();
    }
}
