namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class RepresentativeParties
    {
        [XmlElement("occupantList")]
        public OccupantList OccupantList { get; set; } = new();
    }
}
