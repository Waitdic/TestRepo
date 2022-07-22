namespace iVectorOne.CSSuppliers.TBOHolidays.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class HotelCancelPolicies
    {
        [XmlAttribute]
        public PolicyFormat PolicyFormat { get; set; }

        [XmlElement("LastCancellationDeadline", Type = typeof(LastCancellationDeadline))]
        [XmlElement("TextPolicy", Type = typeof(TextPolicy))]
        [XmlElement("CancelPolicy", Type = typeof(CancelPolicy))]
        [XmlElement("DefaultPolicy", Type = typeof(DefaultPolicy))]
        public BasePolicy[] CancelPolicies { get; set; } = Array.Empty<BasePolicy>();
    }
}