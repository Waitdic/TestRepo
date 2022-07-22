namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PreBookRsRuleMessage
    {
        [XmlArray("BookingRules")]
        [XmlArrayItem("BookingRule")]
        public List<BookingRule> BookingRules { get; set; } = new();
    }
}