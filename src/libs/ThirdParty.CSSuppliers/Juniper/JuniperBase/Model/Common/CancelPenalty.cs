namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class CancelPenalty
    {
        [XmlElement("PenaltyDescription")]
        public PenaltyDescription PenaltyDescription { get; set; } = new();
    }
}
