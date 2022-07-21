namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class CancelPenalty
    {
        [XmlElement("PenaltyDescription")]
        public PenaltyDescription PenaltyDescription { get; set; } = new();
    }
}