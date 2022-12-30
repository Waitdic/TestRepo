namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class StayDateRange
    {
        [XmlAttribute] 
        public string Start { get; set; } = string.Empty;

        [XmlAttribute]
        public string End { get; set; } = string.Empty;
    }
}
