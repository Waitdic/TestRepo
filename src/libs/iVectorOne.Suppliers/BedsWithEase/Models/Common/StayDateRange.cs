namespace iVectorOne.Suppliers.BedsWithEase.Models.Common
{
    using System.Xml.Serialization;

    [XmlRoot("StayDateRange")]
    public class StayDateRange
    {
        [XmlElement("Start")]
        public string? Start;

        [XmlElement("End")]
        public string? End;
    }
}
