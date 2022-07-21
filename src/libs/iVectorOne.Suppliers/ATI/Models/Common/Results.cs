namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    [XmlRoot("Results")]
    public class Results
    {
        public Envelope<AtiAvailabilitySearch> Envelope { get; set; } = new();

        public PropertyGroupings PropertyGroupings { get; set; } = new();

        public OccupancyInfo OccupancyInfo { get; set; } = new();
    }
}
