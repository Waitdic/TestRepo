using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.TeamAmerica.Models
{
    public class CancellationPolicy
    {
        [XmlElement("NumberDaysPrior")]
        public int NumberDaysPrior { get; set; }

        [XmlElement("PenaltyType")]
        public string PenaltyType { get; set; }

        [XmlElement("PenaltyAmount")]
        public string PenaltyAmount { get; set; }
    }
}
