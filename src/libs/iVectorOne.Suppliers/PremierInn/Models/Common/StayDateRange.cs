namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class StayDateRange
    {
        [XmlAttribute]
        public DateTime Start { get; set; }

        [XmlAttribute]
        public DateTime End { get; set; }
    }
}
