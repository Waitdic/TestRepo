namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class OccupantList
    {
        [XmlElement("PassengerReference")]
        public ReferenceBase PassengerReference { get; set; } = new();
    }
}
