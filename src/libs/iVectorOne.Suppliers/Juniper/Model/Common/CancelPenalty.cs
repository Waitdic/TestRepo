namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class CancelPenalty
    {
        [XmlElement("PenaltyDescription")]
        public PenaltyDescription PenaltyDescription { get; set; } = new();
    }
}