namespace iVectorOne.CSSuppliers.Hotelston.Models.Common
{
    using System.Xml.Serialization;

    public class CancellationRule
    {
        [XmlElement("cancellationDeadline")]
        public CancellationDeadline CancellationDeadline { get; set; } = new();

        [XmlElement("cancellationPenalty")]
        public CancellationPenalty CancellationPenalty { get; set; } = new();
    }
}